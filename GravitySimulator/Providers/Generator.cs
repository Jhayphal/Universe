using OpenTK.Mathematics;
using System.Drawing;

namespace Universe;

internal static class Generator
{
  public static readonly Random Current;

  static Generator()
  {
    int seed = DateTime.Now.TimeOfDay.TotalSeconds.GetHashCode();
    Console.WriteLine(seed);

    Current = new Random(seed);
  }

  public static float MakeFloat(float maxValue) => Current.Next((int)maxValue);

  public static float MakeFloat() => Current.NextSingle();

  public static Color MakeColor()
    => Color.FromArgb(
      100,
      Current.Next(180, byte.MaxValue), 
      Current.Next(180, byte.MaxValue), 
      Current.Next(180, byte.MaxValue));

  public static bool MakeChance() => Current.NextDouble() - 0.5d > 0.0001d;

  public static Vector2 MakeAcceleration(float min, float max)
  {
    var top = max - min;
    return new Vector2(Current.NextSingle() * top + min, Current.NextSingle() * top + min);
  }

  public static Vector2 MakePosition(Vector2i area) => new (MakeFloat(area.X), MakeFloat(area.Y));
}