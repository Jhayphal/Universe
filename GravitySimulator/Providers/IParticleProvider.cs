using OpenTK.Mathematics;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Universe.Providers;

public interface IParticleProvider
{
  Dictionary<Color, ReadOnlyCollection<GravityRule>> GetRules(Vector2i area);
}