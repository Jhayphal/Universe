using Universe.Helpers;
using OpenTK.Compute.OpenCL;

namespace Universe.Simulator;

internal class GpuProgram : IDisposable
{
  private bool disposedValue;

  public readonly CLContext Context;
  public readonly CLCommandQueue CommandQueue;
  public readonly CLProgram Program;
  public readonly CLKernel Kernel;

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

  protected virtual void Dispose(bool disposing)
  {
    if (!disposedValue)
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

      disposedValue = true;
    }
  }

  ~GpuProgram()
  {
    Dispose(disposing: false);
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}
