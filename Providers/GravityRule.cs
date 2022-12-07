using Universe.Simulator;
using System.Drawing;

namespace Universe.Providers;

public class GravityRule : IGravityRule
{
  public GravityRule(Color light, IElementaryParticle[] sourceGroup, IElementaryParticle[] targetGroup, float force,
    float areaOfInfluence)
  {
    Light = light;
    SourceGroup = sourceGroup;
    TargetGroup = targetGroup;
    Force = force;
    AreaOfInfluence = areaOfInfluence;
  }

  public Color Light { get; }

  public IElementaryParticle[] SourceGroup { get; }

  public IElementaryParticle[] TargetGroup { get; }

  public float Force { get; }

  public float AreaOfInfluence { get; } = Settings.DefaultAreaOfInfluence;

  public override bool Equals(object obj) => Light.Equals((obj as GravityRule)?.Light);

  public override int GetHashCode() => Light.GetHashCode();

  public override string ToString()
    =>
      $"Light: {Light}, SourceGroupCount: {SourceGroup.Length}, TargetGroupCount: {TargetGroup.Length}, GravityForce: {Force}, GravitateDistance: {AreaOfInfluence}";
}