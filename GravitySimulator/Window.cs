using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

using GL = OpenTK.Graphics.OpenGL4.GL;

namespace Universe;

internal sealed class Window : GameWindow
{
  private UniverseView view = new();

  public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : base(gameWindowSettings, nativeWindowSettings)
  {
  }

  private readonly Chronograph FrameChronograph = new("Frame: ");

  protected override void OnLoad()
  {
    base.OnLoad();
    view.OnLoad(Size);
  }

  protected override void OnRenderFrame(FrameEventArgs e)
  {
    FrameChronograph.Start();

    base.OnRenderFrame(e);
    view.OnRenderFrame(Size);

    SwapBuffers();

    FrameChronograph.Stop();
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
    else if (input.IsKeyDown(Keys.F11))
    {
      WindowState = WindowState != WindowState.Fullscreen
        ? WindowState.Fullscreen
        : WindowState.Normal;
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