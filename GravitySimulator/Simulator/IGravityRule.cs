namespace Universe.Simulator;

internal interface IGravityRule
{
  IElementaryParticle[] SourceGroup { get; }

  IElementaryParticle[] TargetGroup { get; }

  float Force { get; }

  float AreaOfInfluence { get; }
}