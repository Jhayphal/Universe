using OpenTK.Mathematics;

namespace Universe.Simulator;

internal sealed class GpuBag
{
  public GpuBag(float[] vertices, float[] accelerations, GpuMap[] maps, Vector2i area, float attenuation)
  {
    Vertices = vertices;
    Accelerations = accelerations;
    Maps = maps;
    Area = area;
    Attenuation = attenuation;
  }

  public readonly float[] Vertices;
  public readonly float[] Accelerations;
  public readonly GpuMap[] Maps;
  public readonly Vector2i Area;
  public readonly float Attenuation;
}