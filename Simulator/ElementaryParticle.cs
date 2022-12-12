using System.Drawing;
using OpenTK.Mathematics;

namespace Universe;

public sealed class ElementaryParticle : IElementaryParticle, IVisualParticle, IEquatable<ElementaryParticle>
{
  private static int _lastId;

  private readonly int hashCode;

  public Vector2 Position { get; set; }
  
  public Vector2 Acceleration { get; set; }

  public Color Light { get; }
  
  public float Radius { get; }

  public ElementaryParticle(Vector2 position, float radius, Vector2 acceleration, Color light)
  {
    Position = position;
    Radius = radius;
    Acceleration = acceleration;
    Light = light;

    hashCode = ++_lastId;
  }

  public void SetPosition(Vector2 position) => Position = position;

  public void SetAcceleration(Vector2 acceleration) => Acceleration = acceleration;

  public IElementaryParticle Clone() => new ElementaryParticle(Position, Radius, Acceleration, Light);

  public override int GetHashCode() => hashCode;

  public bool Equals(ElementaryParticle other)
  {
    if (other == null)
      return false;

    return other.GetHashCode() == GetHashCode();
  }

  public override bool Equals(object obj) => Equals(obj as ElementaryParticle);

  public override string ToString() => Light.ToString();
}