using System.Drawing;
using OpenTK.Mathematics;

namespace Universe;

public sealed class ElementaryParticle : IElementaryParticle, IVisualParticle, IEquatable<ElementaryParticle>
{
  private static int _lastId;

  private readonly int hashCode;
  
  public ElementaryParticle(Vector2 position, Vector2 acceleration, Color light)
  {
    Position = position;
    Acceleration = acceleration;
    Light = light;

    hashCode = ++_lastId;
  }

  public Color Light { get; }

  public Vector2 Position { get; set; }

  public Vector2 Acceleration { get; set; }

  public void SetPosition(Vector2 position)
    => Position = position;

  public void SetAcceleration(Vector2 acceleration)
    => Acceleration = acceleration;

  public bool Equals(ElementaryParticle other)
  {
    if (other == null)
      return false;

    return other.GetHashCode() == GetHashCode();
  }

  public override int GetHashCode()
    => hashCode;

  public override bool Equals(object obj)
    => Equals(obj as ElementaryParticle);

  public override string ToString()
    => Light.ToString();
}