using System.Drawing;
using OpenTK.Mathematics;

namespace Universe.Providers;

internal interface IParticleProvider
{
  IDictionary<Color, IReadOnlyCollection<GravityRule>> GetRules(Vector2i area);
}