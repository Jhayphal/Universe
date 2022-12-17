using Universe.Providers;
using OpenTK.Mathematics;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Universe.Simulator;

internal interface IGravityRulesAdapter
{
  int ParticlesCount { get; }
  
  float[] Vertices { get; }

  void FillUp(Dictionary<Color, ReadOnlyCollection<GravityRule>> rules, Vector2i area);
  
  void Setup(IGravitySimulator executor);
}