using System.Drawing;
using OpenTK.Mathematics;
using Universe.Providers;

namespace Universe.Simulator;

internal interface IGravityRulesAdapter
{
  int ParticlesCount { get; }
  
  float[] Vertices { get; }

  void FillUp(IDictionary<Color, IReadOnlyCollection<GravityRule>> rules, Vector2i area);
  
  void Setup(IGravitySimulator executor);
}