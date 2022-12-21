using Universe.Providers;
using Universe.Simulator;
using Universe.Shaders;

using OpenTK.Mathematics;

using GL = OpenTK.Graphics.OpenGL4.GL;
using BufferTarget = OpenTK.Graphics.OpenGL4.BufferTarget;
using BufferUsageHint = OpenTK.Graphics.OpenGL4.BufferUsageHint;
using ClearBufferMask = OpenTK.Graphics.OpenGL4.ClearBufferMask;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using VertexAttribPointerType = OpenTK.Graphics.OpenGL4.VertexAttribPointerType;

namespace Universe;

internal sealed class UniverseView : IDisposable
{
  private const string VertexShaderResource = "Shaders.shader.vert";
  private const string FragmentShaderResource = "Shaders.shader.frag";

  private const int Dimensions = 3;
  private const int ColorBytes = 3;

  private float[] vertices;
  private int vertexBufferObject;
  private int vertexArrayObject;
  private int particlesCount;

  private Shader shader;
  private bool disposed;
  private bool loaded;

  private readonly IGravitySimulator Simulator = new GpuGravitySimulator(Settings.PlatformId, Settings.DeviceId, Settings.Type);
  private readonly Chronograph GravitateChronograph = new("Gravitate: ");

  public void Dispose()
  {
    disposed = true;

    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    GL.BindVertexArray(0);
    GL.UseProgram(0);

    GL.DeleteBuffer(vertexBufferObject);
    GL.DeleteVertexArray(vertexArrayObject);

    GL.DeleteProgram(shader.Handle);

    if (Simulator is IDisposable disposable)
    {
      disposable.Dispose();
    }
  }

  public void OnLoad(Vector2i size)
  {
    if (disposed)
    {
      return;
    }

    SetupGl();

    PrepareSimulation(size);
    PrepareGlBuffer();

    SetVertexAttribPointer(index: 0, offset: 0);
    SetVertexAttribPointer(index: 1, offset: Dimensions * sizeof(float));

    LoadShader();

    loaded = true;
  }

  public void OnRenderFrame(Vector2i size)
  {
    if (disposed || !loaded)
    {
      return;
    }

    SetShader(size);
    Draw();
    Gravitate();
  }

  private static void SetupGl()
  {
    GL.ClearColor(Settings.Background);
    GL.PointSize(Settings.PointSize);

    GL.Enable(OpenTK.Graphics.OpenGL4.EnableCap.Blend);
    GL.BlendFunc(OpenTK.Graphics.OpenGL4.BlendingFactor.SrcAlpha, OpenTK.Graphics.OpenGL4.BlendingFactor.OneMinusSrcAlpha);
  }

  private static void SetVertexAttribPointer(int index, int offset)
  {
    GL.VertexAttribPointer(index, Dimensions, VertexAttribPointerType.Float, false, (Dimensions + ColorBytes) * sizeof(float), offset);
    GL.EnableVertexAttribArray(index);
  }

  private void PrepareSimulation(Vector2i size)
  {
    IParticleProvider provider = new ParticleProvider();
    var rules = provider.GetRules(size);
    IGravityRulesAdapter adapter = new GpuGravityRulesAdapter();
    adapter.FillUp(rules, size);
    adapter.Setup(Simulator);

    vertices = adapter.Vertices;
    particlesCount = adapter.ParticlesCount;
  }

  private void PrepareGlBuffer()
  {
    vertexBufferObject = GL.GenBuffer();

    GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StreamDraw);

    vertexArrayObject = GL.GenVertexArray();
    GL.BindVertexArray(vertexArrayObject);
  }

  private void LoadShader()
  {
    shader = new Shader(VertexShaderResource, FragmentShaderResource);
    shader.Use();
  }

  private void SetShader(Vector2i size)
  {
    shader.Use();
    shader.SetFloat("scaleX", size.Y / (float)size.X);
    shader.SetMatrix4("transform", Matrix4.Identity * Matrix4.CreateScale(size.X / (float)size.Y));
  }

  private void Draw()
  {
    GL.Clear(ClearBufferMask.ColorBufferBit);
    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StreamDraw);
    GL.DrawArrays(PrimitiveType.Points, 0, particlesCount);
  }

  private void Gravitate()
  {
    GravitateChronograph.Start();
    Simulator.Gravitate();
    GravitateChronograph.Stop();
  }
}
