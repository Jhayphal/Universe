using Universe.Helpers;
using OpenTK.Compute.OpenCL;

namespace Universe.Simulator
{
  sealed class GpuGravitySimulator : IGravitySimulator, IDisposable
  {
    private const string GravityProgramSource = "Gravitate.cl";
    private const string GravityKernel = "gravitate";

    private readonly int platformId;
    private readonly int deviceId;
    private readonly DeviceType deviceType;

    private GpuBag bag;

    private GpuProgram program;
    private CLBuffer vertices;
    private CLBuffer accelerations;
    private CLBuffer maps;
    private CLBuffer props;

    public GpuGravitySimulator(int platformId = 1, int deviceId = 0, DeviceType deviceType = DeviceType.Gpu)
    {
      this.platformId = platformId;
      this.deviceId = deviceId;
      this.deviceType = deviceType;
    }

    public void Dispose()
    {
      CLHelper.FreeBuffers(vertices, accelerations, maps, props);

      program?.Dispose();
    }

    public void Setup(GpuBag bag)
    {
      this.bag = bag;

      var kernelSource = ResourceHelper.ReadResourceAsText(GravityProgramSource);
      program = new GpuProgram(kernelSource, GravityKernel, platformId, deviceId, deviceType);

      vertices = CLHelper.CreateBuffer(
        program.Context,
        program.CommandQueue,
        MemoryFlags.ReadWrite,
        this.bag.Vertices);

      accelerations = CLHelper.CreateBuffer(
        program.Context,
        program.CommandQueue,
        MemoryFlags.ReadWrite,
        this.bag.Accelerations);

      maps = CLHelper.CreateBuffer(
        program.Context,
        program.CommandQueue,
        MemoryFlags.ReadOnly,
        this.bag.Maps.SelectMany(x => x.ToArray()).ToArray());

      props = CLHelper.CreateBuffer(
        program.Context,
        program.CommandQueue,
        MemoryFlags.ReadOnly,
        new float[] { this.bag.Area.X, this.bag.Area.Y, this.bag.Attenuation });
    }

    public void Gravitate()
    {
      if (program == null)
      {
        throw new InvalidOperationException(nameof(program));
      }

      int index = 0;
      SetArg(vertices, index++);
      SetArg(accelerations, index++);
      SetArg(maps, index++);
      SetArg(props, index++);

      NDRangeKernel(bag.Maps.Length);

      CLHelper.ReadBuffer(program.CommandQueue, vertices, bag.Vertices);
      CLHelper.ReadBuffer(program.CommandQueue, accelerations, bag.Accelerations);

      CLHelper.Flush(program.CommandQueue);
      CL.Finish(program.CommandQueue);
    }

    private void NDRangeKernel(params int[] dimensions) => CLHelper.NDRangeKernel(program.CommandQueue, program.Kernel, dimensions);

    private void SetArg(CLBuffer buffer, int index) => CLHelper.SetArg(program.Kernel, buffer, index);
  }
}