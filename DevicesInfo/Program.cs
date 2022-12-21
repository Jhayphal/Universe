using OpenTK.Compute.OpenCL;
using System.Text;

string BytesAsString(byte[] bytes) => Encoding.UTF8.GetString(bytes);

void Print(string message, int indent = 0) => Console.WriteLine(new string('\t', indent) + message);

bool OkResult(CLResultCode code) => code == CLResultCode.Success;

var result = CL.GetPlatformIds(out var platforms);
if (!OkResult(result))
{
  Print(result.ToString());
  return;
}

var platformId = 0;
foreach (var platform in platforms)
{
  result = CL.GetPlatformInfo(platform, PlatformInfo.Name, out var info);
  if (!OkResult(result))
  {
    continue;
  }

  Print($"[{platformId++}] {BytesAsString(info)}:");

  var deviceId = 0;
  foreach (DeviceType type in Enum.GetValues(typeof(DeviceType)))
  {
    result = CL.GetDeviceIds(platform, type, out var devices);
    if (!OkResult(result))
    {
      continue;
    }

    Print($"[{deviceId++}] {type}:", indent: 1);

    foreach (var device in devices)
    {
      result = CL.GetDeviceInfo(device, DeviceInfo.Name, out info);
      if (!OkResult(result))
      {
        continue;
      }

      Print(BytesAsString(info), indent: 2);
    }
  }
}