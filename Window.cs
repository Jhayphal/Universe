#if DEBUG
#define LOG_TIME
#endif

using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using Universe.Providers;
using BufferTarget = OpenTK.Graphics.OpenGL4.BufferTarget;
using BufferUsageHint = OpenTK.Graphics.OpenGL4.BufferUsageHint;
using ClearBufferMask = OpenTK.Graphics.OpenGL4.ClearBufferMask;
using GL = OpenTK.Graphics.OpenGL4.GL;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using VertexAttribPointerType = OpenTK.Graphics.OpenGL4.VertexAttribPointerType;
using System.Diagnostics;
using Universe.Simulator;
using Universe.Shaders;

namespace Universe;

public sealed class Window : GameWindow
{
  private readonly IGravitySimulator simulator = new GpuGravitySimulator(platformId: 0);

  private const int Dimensions = 3;
  private const int ColorBytes = 3;

  private float[] vertices;
  private int vertexBufferObject;
  private int vertexArrayObject;
  private int particlesCount;

  private Shader _shader;

  public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : base(gameWindowSettings, nativeWindowSettings)
  {
  }

  protected override void OnLoad()
  {
    base.OnLoad();

    IParticleProvider provider = new ParticleProvider();
    var rules = provider.GetRules(Size);
    IGravityRulesAdapter adapter = new GpuGravityRulesAdapter();
    adapter.FillUp(rules, Size);
    adapter.Setup(simulator);
    
    vertices = adapter.Vertices;
    particlesCount = adapter.ParticlesCount;

    GL.ClearColor(Settings.Background);
    GL.PointSize(Settings.PointSize);
    GL.Enable(OpenTK.Graphics.OpenGL4.EnableCap.Blend);

    vertexBufferObject = GL.GenBuffer();

    GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

    vertexArrayObject = GL.GenVertexArray();
    GL.BindVertexArray(vertexArrayObject);

    GL.VertexAttribPointer(
      0, 
      Dimensions, 
      VertexAttribPointerType.Float, 
      false,
      (Dimensions + ColorBytes) * sizeof(float), 
      0);
    GL.EnableVertexAttribArray(0);

    GL.VertexAttribPointer(
      1, 
      ColorBytes, 
      VertexAttribPointerType.Float, 
      false,
      (Dimensions + ColorBytes) * sizeof(float),
      Dimensions * sizeof(float));
    GL.EnableVertexAttribArray(1);

    _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
    _shader.Use();
  }

  protected override void OnRenderFrame(FrameEventArgs e)
  {
#if LOG_TIME
    var frameStopwatch = Stopwatch.StartNew();
#endif
    base.OnRenderFrame(e);

#if LOG_TIME
    var gravitateStopwatch = Stopwatch.StartNew();
#endif

    simulator.Gravitate();

#if LOG_TIME
    gravitateStopwatch.Stop();
    Debug.WriteLine("Gravitate: " + gravitateStopwatch.Elapsed.TotalMilliseconds);

    gravitateStopwatch.Reset();
    gravitateStopwatch.Start();
#endif

    GL.Clear(ClearBufferMask.ColorBufferBit);
    
    _shader.Use();

    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

    GL.DrawArrays(PrimitiveType.Points, 0, particlesCount);

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
  }

  protected override void OnResize(ResizeEventArgs e)
  {
    base.OnResize(e);

    GL.Viewport(0, 0, Size.X, Size.Y);
  }

  protected override void OnUnload()
  {
    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    GL.BindVertexArray(0);
    GL.UseProgram(0);

    GL.DeleteBuffer(vertexBufferObject);
    GL.DeleteVertexArray(vertexArrayObject);

    GL.DeleteProgram(_shader.Handle);

    if (simulator is IDisposable disposable)
    {
      disposable.Dispose();
    }

    base.OnUnload();
  }
}