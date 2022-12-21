using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Universe;

internal class Program
{
  private static void Main()
  {
    var areaSize = Monitors.GetPrimaryMonitor().ClientArea.Size;

    var nativeWindowSettings = new NativeWindowSettings()
    {
      Title = $"{nameof(Universe)}, F5 - Update, F11 - Fullscreen",
      Size = (areaSize.X >> 1, areaSize.Y >> 1),
      Location = (areaSize.X >> 2, areaSize.Y >> 4),
      AspectRatio = (1, 1),
      Flags = ContextFlags.ForwardCompatible,
      WindowState = WindowState.Normal
    };
    
    using var window = new Window(GameWindowSettings.Default, nativeWindowSettings);
    window.CursorState = CursorState.Hidden;
    window.Run();
  }
}