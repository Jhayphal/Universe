using System.Drawing;
using System.Diagnostics;
using OpenTK.Mathematics;

namespace Universe.Providers;

internal sealed class ParticleProvider : IParticleProvider
{
  public IDictionary<Color, IReadOnlyCollection<GravityRule>> GetRules(Vector2i area)
    => GetRandomSample(area);

  public IDictionary<Color, IReadOnlyCollection<GravityRule>> GetRandomSample(Vector2i area)
  {
    var colorsCount = Generator.Current.Next(
      Settings.ColorsCount.Start.Value, 
      Settings.ColorsCount.End.Value);
    var maxParticlesPerColor = Settings.MaximumParticlesCount / colorsCount;

    var colors = new HashSet<Color>();
    while (colors.Count < colorsCount)
    {
      colors.Add(Generator.MakeColor());
    }

    return GetRandomRules(
      area, 
      Settings.MinimumParticlesPerColor..maxParticlesPerColor, 
      Settings.AreaOfInfluenceRange, 
      colors.ToArray());
  }

  public IDictionary<Color, IReadOnlyCollection<GravityRule>> GetSample1(Vector2i area)
  {
    var count = Settings.MaximumParticlesCount;
    var green = Color.Green;
    var red = Color.Red;
    var yellow = Color.Yellow;

    var builder = new GravityRuleBuilder();

    builder.CreateParticles(green, count, area);
    builder.CreateParticles(red, count, area);
    builder.CreateParticles(yellow, count, area);

    builder.MakeRule(green, green, 0.32f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(green, red, 0.17f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(red, green, 0.34f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(green, yellow, 0.34f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(green, green, 0.20f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(red, red, 0.1f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(yellow, yellow, -0.55f, Settings.DefaultAreaOfInfluence);

    return builder.GetRules();
  }

  private IDictionary<Color, IReadOnlyCollection<GravityRule>> GetSample2(Vector2i area)
  {
    var count = Settings.MaximumParticlesCount;
    var green = Color.Green;
    var red = Color.Red;
    var white = Color.White;
    var blue = Color.Blue;

    var builder = new GravityRuleBuilder();

    builder.CreateParticles(green, count, area);
    builder.CreateParticles(red, count, area);
    builder.CreateParticles(white, count, area);
    builder.CreateParticles(blue, count, area);

    builder.MakeRule(green, green, -0.105f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(green, red, -0.20f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(red, green, -0.235f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(green, white, -0.195f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(white, green, 0.295f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(green, blue, -0.165f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(blue, green, -0.135f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(red, red, 0.075f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(red, white, -0.195f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(white, red, 0.285f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(red, blue, 0.40f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(blue, red, -0.54f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(white, white, -0.0599999f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(white, blue, -0.245f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(blue, white, -0.54f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(blue, blue, 0.375f, Settings.DefaultAreaOfInfluence);

    return builder.GetRules();
  }

  public IDictionary<Color, IReadOnlyCollection<GravityRule>> GetSample3(Vector2i area)
  {
    var count = Settings.MaximumParticlesCount;
    var green = Color.Green;
    var red = Color.Red;
    var yellow = Color.Yellow;

    var builder = new GravityRuleBuilder();

    builder.CreateParticles(green, count, area);
    builder.CreateParticles(red, count, area);
    builder.CreateParticles(yellow, count, area);

    builder.MakeRule(green, green, 0.32f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(green, red, 0.17f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(red, green, 0.34f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(green, yellow, 0.34f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(green, green, 0.20f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(red, red, 0.1f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(yellow, yellow, -0.15f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(yellow, red, 0.15f, Settings.DefaultAreaOfInfluence);

    return builder.GetRules();
  }

  public IDictionary<Color, IReadOnlyCollection<GravityRule>> GetSample4(Vector2i area)
  {
    var count = Settings.MaximumParticlesCount;
    var green = Color.Green;
    var red = Color.Red;
    var builder = new GravityRuleBuilder();

    builder.CreateParticles(green, count, area);
    builder.CreateParticles(red, count << 1, area);

    builder.MakeRule(green, green, -0.35f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(green, red, 0.38f, Settings.DefaultAreaOfInfluence);
    builder.MakeRule(red, green, 0.18f, Settings.DefaultAreaOfInfluence);

    return builder.GetRules();
  }

  public IDictionary<Color, IReadOnlyCollection<GravityRule>> GetRandomRules(
    Vector2i area,
    Range countRange,
    Range gravitateDistanceRange,
    params Color[] colors)
  {
    var builder = new GravityRuleBuilder();

    foreach (var color in colors)
    {
      int count = Generator.Current.Next(countRange.Start.Value, countRange.End.Value);

      Debug.WriteLine($"Color: {color}, Count: {count}");

      builder.CreateParticles(color, count, area);
    }

    foreach (var colorA in colors)
    {
      foreach (var colorB in colors)
      {
        var gravityForce = Generator.Current.NextSingle() * 2f - 1f;
        var areaOfInfluence =
          Generator.Current.Next(gravitateDistanceRange.Start.Value, gravitateDistanceRange.End.Value);
        Debug.WriteLine(
          $"A: {colorA}, B: {colorB}, Force: {gravityForce}, Distance: {areaOfInfluence}");

        builder.MakeRule(
          colorA,
          colorB,
          gravityForce,
          areaOfInfluence);
      }
    }

    return builder.GetRules();
  }
}