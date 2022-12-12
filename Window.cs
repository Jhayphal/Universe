#if DEBUG
#define LOG_TIME
#endif

using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using GL = OpenTK.Graphics.OpenGL4.GL;
using System.Diagnostics;

namespace Universe;

public sealed class Window : GameWindow
{
  private UniverseView view = new();

  public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : base(gameWindowSettings, nativeWindowSettings)
  {
  }

  protected override void OnLoad()
  {
    base.OnLoad();

    view.OnLoad(Size);
  }

  protected override void OnRenderFrame(FrameEventArgs e)
  {
#if LOG_TIME
    var frameStopwatch = Stopwatch.StartNew();
#endif
    base.OnRenderFrame(e);

    view.OnRenderFrame(Size);

    SwapBuffers();

#if LOG_TIME
    frameStopwatch.Stop();
    Debug.WriteLine("Frame: " + frameStopwatch.Elapsed.TotalMilliseconds);

    frameStopwatch.Reset();
    frameStopwatch.Start();
#endif
  }

  protected override void OnUpdateFrame(FrameEventArgs e)
  {
    base.OnUpdateFrame(e);

    var input = KeyboardState;

    if (input.IsKeyDown(Keys.Escape))
    {
      Close();
    }
    else if (input.IsKeyDown(Keys.F5))
    {
      view.Dispose();
      view = new();
      view.OnLoad(Size);
    }
  }

  protected override void OnResize(ResizeEventArgs e)
  {
    base.OnResize(e);

    GL.Viewport(0, 0, Size.X, Size.Y);
  }

  protected override void OnUnload()
  {
    view.Dispose();
    base.OnUnload();
  }
}