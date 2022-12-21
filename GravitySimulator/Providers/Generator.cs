using System.Diagnostics;
using System.Drawing;
using OpenTK.Mathematics;

namespace Universe;

internal static class Generator
{
  public static readonly Random Current;

  static Generator()
  {
    var now = DateTime.Now;
    int seed = now.Millisecond * now.Second * now.Minute * now.Hour;

    Debug.WriteLine(seed);

    Current = new Random(seed);
  }

  public static float MakeFloat(float minValue, float maxValue)
    => MakeFloat(maxValue) + minValue;

  public static float MakeFloat(float maxValue)
    => Current.NextSingle() * maxValue;

  public static Color MakeColor()
    => Color.FromArgb(Current.Next(100, byte.MaxValue), Current.Next(100, byte.MaxValue), Current.Next(100, byte.MaxValue));

  public static Vector2 MakeAcceleration(float min, float max)
    => new Vector2(MakeFloat(min, max), MakeFloat(min, max));

  public static Vector2 MakePosition(Vector2i area)
    => new (MakeFloat(area.X), MakeFloat(area.Y));
}