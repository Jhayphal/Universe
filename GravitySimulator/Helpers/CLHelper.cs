using System.Diagnostics;
using System.Data;
using OpenTK.Compute.OpenCL;

namespace Universe.Helpers;

internal static class CLHelper
{
  public static CLPlatform[] GetPlatformIds()
  {
    var result = CL.GetPlatformIds(out var platforms);
    CheckResult(nameof(CL.GetPlatformIds), result);
    CheckIfArrayEmpty(platforms, nameof(platforms));
    return platforms;
  }

  public static CLDevice[] GetDeviceIds(int platformId, DeviceType deviceType)
    => GetDeviceIds(GetPlatformIds()[platformId], deviceType);

  public static CLDevice[] GetDeviceIds(CLPlatform platform, DeviceType deviceType)
  {
    var result = CL.GetDeviceIds(platform, deviceType, out var devices);
    CheckResult(nameof(CL.GetDeviceIds), result);
    CheckIfArrayEmpty(devices, nameof(devices));
    return devices;
  }

  public static CLContext CreateContext(CLDevice[] devices)
  {
    var context = CL.CreateContext(
      IntPtr.Zero,
      (uint)devices.Length,
      devices,
      IntPtr.Zero,
      IntPtr.Zero,
      out var result);
    CheckResult(nameof(CL.CreateContext), result);
    return context;
  }

  public static CLCommandQueue CreateCommandQueue(CLContext context, CLDevice device)
  {
    var commandQueue = CL.CreateCommandQueueWithProperties(
      context,
      device,
      IntPtr.Zero,
      out var result);
    CheckResult(nameof(CL.CreateCommandQueueWithProperties), result);
    return commandQueue;
  }

  public static CLProgram CreateProgramWithSource(CLContext context, string sourceCode)
  {
    if (string.IsNullOrWhiteSpace(sourceCode))
      throw new ArgumentNullException(nameof(sourceCode));

    var program = CL.CreateProgramWithSource(context, sourceCode, out var result);
    CheckResult(nameof(CL.CreateProgramWithSource), result);
    return program;
  }

  public static void BuildProgram(CLProgram program, CLDevice[] devices)
  {
    var result = CL.BuildProgram(program, (uint)devices.Length, devices, null, IntPtr.Zero, IntPtr.Zero);
    CheckResult(nameof(CL.BuildProgram), result);
  }

  public static CLKernel CreateKernel(CLProgram program, string name)
  {
    var kernel = CL.CreateKernel(program, name, out var result);
    CheckResult(nameof(CL.CreateKernel), result);
    return kernel;
  }

  public static CLBuffer CreateBuffer(CLContext context, CLCommandQueue commandQueue, MemoryFlags flags, float[] array)
  {
    var memoryObject = CreateEmptyBuffer(context, array.Length * sizeof(float), flags);
    WriteBuffer(commandQueue, memoryObject, array);
    return memoryObject;
  }

  public static CLBuffer CreateEmptyBuffer(CLContext context, int length, MemoryFlags flags)
  {
    var memoryObject = CL.CreateBuffer(
      context,
      flags,
      new((uint)length),
      IntPtr.Zero,
      out var result);
    CheckResult(nameof(CL.CreateBuffer), result);
    return memoryObject;
  }

  public static void WriteBuffer<T>(CLCommandQueue commandQueue, CLBuffer buffer, T[] array) where T : unmanaged
  {
    var result = CL.EnqueueWriteBuffer(
      commandQueue,
      buffer,
      true,
      UIntPtr.Zero,
      array,
      null,
      out _);
    CheckResult(nameof(CL.EnqueueWriteBuffer), result);
  }

  public static T[] ReadBuffer<T>(CLCommandQueue commandQueue, CLBuffer buffer, T[] response) where T : unmanaged
  {
    var result = CL.EnqueueReadBuffer(
      commandQueue,
      buffer,
      true,
      UIntPtr.Zero,
      response,
      null, out _);
    CheckResult(nameof(CL.EnqueueReadBuffer), result);
    return response;
  }

  public static void FreeBuffer(CLBuffer buffer)
  {
    if (buffer.Handle != IntPtr.Zero)
    {
      CheckResult(nameof(CL.ReleaseMemoryObject), CL.ReleaseMemoryObject(buffer), @throw: false);
    }
  }

  public static void FreeBuffers(params CLBuffer[] buffers)
  {
    foreach (var buffer in buffers)
    {
      FreeBuffer(buffer);
    }
  }

  public static void SetArg(CLKernel kernel, CLBuffer buffer, int index)
    => CheckResult(nameof(CL.SetKernelArg), CL.SetKernelArg(kernel, (uint)index, buffer));

  public static void NDRangeKernel(CLCommandQueue commandQueue, CLKernel kernel, params int[] dimensions)
  {
    var result = CL.EnqueueNDRangeKernel(
      commandQueue,
      kernel,
      (uint)dimensions.Length,
      null,
      dimensions.Select(d => new UIntPtr((uint)d)).ToArray(),
      null,
      0,
      null, out _);
    CheckResult(nameof(CL.EnqueueNDRangeKernel), result);
  }

  public static void Flush(CLCommandQueue commandQueue)
  {
    var result = CL.Flush(commandQueue);
    CheckResult(nameof(CL.Flush), result);
  }

  public static void Finish(CLCommandQueue commandQueue)
  {
    var result = CL.Finish(commandQueue);
    CheckResult(nameof(CL.Finish), result);
  }

  public static void CheckResult(string name, CLResultCode result, bool @throw = true)
  {
    if (@throw && result != CLResultCode.Success)
    {
      Trace.WriteLine($"{name} is {result}");
      throw new InvalidOperationException();
    }
  }

  public static void CheckIfArrayEmpty(Array array, string name)
  {
    if (array.Length == 0)
    {
      throw new ArgumentNullException(name);
    }
  }
}