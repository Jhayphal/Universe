using System.Drawing;

namespace Universe;

public interface IVisualParticle
{
  Color Light { get; }

  float Radius { get; }
}