using OpenTK.Compute.OpenCL;
using System.Drawing;

namespace Universe;

public static class Settings
{
  #region Particles
  
  public const int MaximumParticlesCount = 5000;

  public const int MinimumParticlesPerColor = 10;

  public static readonly Range ColorsCount = 2..10;

  #endregion Particles

  #region Gravity

  public const float Attenuation = 0.00085f;

  public static readonly Range AreaOfInfluenceRange = 30..600;

  public const int DefaultAreaOfInfluence = 80;

  #endregion Gravity

  #region View

  public static readonly Color Background = Color.Black;

  public const float PointSize = 8f;

  #endregion View

  #region Device

  public const int PlatformId = 0;

  public const int DeviceId = 0;

  public const DeviceType Type = DeviceType.Gpu;

  #endregion Device
}