#if DEBUG
#define LOG_TIME
#endif

using Universe.Providers;
using BufferTarget = OpenTK.Graphics.OpenGL4.BufferTarget;
using BufferUsageHint = OpenTK.Graphics.OpenGL4.BufferUsageHint;
using ClearBufferMask = OpenTK.Graphics.OpenGL4.ClearBufferMask;
using GL = OpenTK.Graphics.OpenGL4.GL;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using VertexAttribPointerType = OpenTK.Graphics.OpenGL4.VertexAttribPointerType;
using Universe.Simulator;
using Universe.Shaders;
using OpenTK.Mathematics;

namespace Universe;

internal sealed class UniverseView : IDisposable
{
  private readonly IGravitySimulator simulator = new GpuGravitySimulator(Settings.PlatformId, Settings.DeviceId, Settings.Type);

  private const int Dimensions = 3;
  private const int ColorBytes = 3;

  private float[] vertices;
  private int vertexBufferObject;
  private int vertexArrayObject;
  private int particlesCount;

  private Shader shader;
  private bool disposed;
  private bool loaded;

  public void OnLoad(Vector2i size)
  {
    if (disposed)
    {
      return;
    }

    GL.ClearColor(Settings.Background);
    GL.PointSize(Settings.PointSize);
    GL.Enable(OpenTK.Graphics.OpenGL4.EnableCap.Blend);

    IParticleProvider provider = new ParticleProvider();
    var rules = provider.GetRules(size);
    IGravityRulesAdapter adapter = new GpuGravityRulesAdapter();
    adapter.FillUp(rules, size);
    adapter.Setup(simulator);

    vertices = adapter.Vertices;
    particlesCount = adapter.ParticlesCount;

    vertexBufferObject = GL.GenBuffer();

    GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StreamDraw);

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

    shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
    shader.Use();

    loaded = true;
  }

  public void OnRenderFrame(Vector2i size)
  {
    if (disposed || !loaded)
    {
      return;
    }

#if LOG_TIME
    var frameStopwatch = Stopwatch.StartNew();
#endif
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
    shader.Use();
    shader.SetFloat("scaleX", size.Y / (float)size.X);
    var scale = Matrix4.Identity * Matrix4.CreateScale(size.X / (float)size.Y);
    shader.SetMatrix4("transform", scale);

    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StreamDraw);

    GL.DrawArrays(PrimitiveType.Points, 0, particlesCount);

#if LOG_TIME
    frameStopwatch.Stop();
    Debug.WriteLine("Frame: " + frameStopwatch.Elapsed.TotalMilliseconds);

    frameStopwatch.Reset();
    frameStopwatch.Start();
#endif
  }

  public void Dispose()
  {
    disposed = true;

    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    GL.BindVertexArray(0);
    GL.UseProgram(0);

    GL.DeleteBuffer(vertexBufferObject);
    GL.DeleteVertexArray(vertexArrayObject);

    GL.DeleteProgram(shader.Handle);

    if (simulator is IDisposable disposable)
    {
      disposable.Dispose();
    }
  }
}
