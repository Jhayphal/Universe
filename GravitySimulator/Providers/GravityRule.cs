using System.Diagnostics;
using System.Drawing;
using Universe.Simulator;

namespace Universe.Providers;

[DebuggerDisplay("Light: {Light}, SourceGroupCount: {SourceGroup.Length}, TargetGroupCount: {TargetGroup.Length}, GravityForce: {Force}, GravitateDistance: {AreaOfInfluence}")]
internal class GravityRule : IGravityRule
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
}