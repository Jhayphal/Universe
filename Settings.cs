using System.Drawing;

namespace Universe;

public static class Settings
{
  public const int MaximumParticlesCount = 5000;

  public static int MinimumParticlesPerColor = 10;

  public static readonly Range ColorsCount = 2..10;

  public const float Attenuation = 0.00085f;

  public static readonly Range AreaOfInfluenceRange = 3..600;

  public const int DefaultAreaOfInfluence = 80;

  public static readonly Color Background = Color.Black;

  public const float PointSize = 6f;
}