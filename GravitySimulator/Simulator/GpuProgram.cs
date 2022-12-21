using OpenTK.Compute.OpenCL;
using Universe.Helpers;

namespace Universe.Simulator;

internal sealed class GpuProgram : IDisposable
{
  public GpuProgram(string sourceCode, string name, int platformId, int deviceId, DeviceType deviceType)
  {
    if (string.IsNullOrWhiteSpace(sourceCode))
      throw new ArgumentNullException(nameof(sourceCode));

    var devices = CLHelper.GetDeviceIds(platformId, deviceType);
    Context = CLHelper.CreateContext(devices);
    CommandQueue = CLHelper.CreateCommandQueue(Context, devices[deviceId]);
    Program = CLHelper.CreateProgramWithSource(Context, sourceCode);
    CLHelper.BuildProgram(Program, devices);
    Kernel = CLHelper.CreateKernel(Program, name);
  }

  public readonly CLContext Context;
  public readonly CLCommandQueue CommandQueue;
  public readonly CLProgram Program;
  public readonly CLKernel Kernel;

  public void Dispose()
  {
    if (Kernel.Handle != IntPtr.Zero)
    {
      var result = CL.ReleaseKernel(Kernel);
      CLHelper.CheckResult(nameof(CL.ReleaseKernel), result, @throw: false);
    }

    if (Program.Handle != IntPtr.Zero)
    {
      var result = CL.ReleaseProgram(Program);
      CLHelper.CheckResult(nameof(CL.ReleaseProgram), result, @throw: false);
    }

    if (CommandQueue.Handle != IntPtr.Zero)
    {
      var result = CL.ReleaseCommandQueue(CommandQueue);
      CLHelper.CheckResult(nameof(CL.ReleaseCommandQueue), result, @throw: false);
    }

    if (Context.Handle != IntPtr.Zero)
    {
      var result = CL.ReleaseContext(Context);
      CLHelper.CheckResult(nameof(CL.ReleaseContext), result, @throw: false);
    }
  }
}