using System.Drawing;
using OpenTK.Mathematics;

namespace Universe;

public sealed class LifeParticle : IElementaryParticle, IVisualParticle, IEquatable<LifeParticle>
{
  private static int _lastId;

  private readonly int hashCode;

  public Vector2 Position { get; set; }
  
  public Vector2 Acceleration { get; set; }

  public Color Light { get; }
  
  public float Radius { get; }

  public LifeParticle(Vector2 position, float radius, Vector2 acceleration, Color light)
  {
    Position = position;
    Radius = radius;
    Acceleration = acceleration;
    Light = light;

    hashCode = ++_lastId;
  }

  public void SetPosition(Vector2 position) => Position = position;

  public void SetAcceleration(Vector2 acceleration) => Acceleration = acceleration;

  public IElementaryParticle Clone() => new LifeParticle(Position, Radius, Acceleration, Light);

  public override int GetHashCode() => hashCode;

  public bool Equals(LifeParticle other)
  {
    if (other == null)
      return false;

    return other.GetHashCode() == GetHashCode();
  }

  public override bool Equals(object obj) => Equals(obj as LifeParticle);

  public override string ToString() => Light.ToString();
}