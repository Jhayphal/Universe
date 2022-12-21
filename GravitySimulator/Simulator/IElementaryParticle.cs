using OpenTK.Mathematics;

namespace Universe;

internal interface IElementaryParticle
{
  Vector2 Position { get; }

  public Vector2 Acceleration { get; }

  void SetPosition(Vector2 position);

  void SetAcceleration(Vector2 acceleration);
}