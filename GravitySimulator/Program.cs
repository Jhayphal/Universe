using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Universe;

internal class Program
{
  private static void Main()
  {
    var area = Monitors.GetPrimaryMonitor().ClientArea;

    var nativeWindowSettings = new NativeWindowSettings()
    {
      Title = nameof(Universe),
      Flags = ContextFlags.ForwardCompatible,
      AspectRatio = (1, 1),
      WindowState = WindowState.Fullscreen
    };
    
    using var window = new Window(GameWindowSettings.Default, nativeWindowSettings);
    window.CursorState = CursorState.Hidden;
    window.Run();
  }
}