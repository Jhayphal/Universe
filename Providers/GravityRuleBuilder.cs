using OpenTK.Mathematics;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Universe.Providers;

internal sealed class GravityRuleBuilder
{
  private const int Radius = 6;

  private readonly Dictionary<Color, IElementaryParticle[]> particles = new();
  private readonly Dictionary<Color, List<GravityRule>> rules = new();

  public Dictionary<Color, ReadOnlyCollection<GravityRule>> GetRules()
    => rules.Select(x => new
    {
      x.Key, 
      Value = new ReadOnlyCollection<GravityRule>(x.Value)
    }).ToDictionary(x => x.Key, x => x.Value);

  public void MakeRule(Color source, Color target, float gravityForce, float gravitateDistance)
  {
    if (!rules.TryGetValue(source, out var items))
    {
      rules.Add(source, items = new List<GravityRule>());
    }

    items.Add(new GravityRule
    (
      light: source,
      sourceGroup: particles[source],
      targetGroup: particles[target],
      force: gravityForce,
      areaOfInfluence: gravitateDistance
    ));
  }

  public void CreateParticles(Color color, int count, Vector2i area)
  {
    var items = new IElementaryParticle[count];
    
    for (var i = 0; i < count; ++i)
    {
      var position = Generator.MakePosition(area);
      var acceleration = Generator.MakeAcceleration(-1f, 1f);
      items[i] = new LifeParticle(position, Radius, acceleration, color);
    }

    particles.Add(color, items);
  }
}