using System.Drawing;
using OpenTK.Mathematics;
using Universe.Providers;

using MathHelper = OpenTK.Mathematics.MathHelper;

namespace Universe.Simulator;

internal sealed class GpuGravityRulesAdapter : IGravityRulesAdapter
{
  // indexes      - [     0     ] [     1     ] ... [     N     ]
  // vertices     - [X Y Z R G B] [X Y Z R G B] ... [X Y Z R G B]
  // acceleration - [X Y Z 0 0 0] [X Y Z 0 0 0] ... [X Y Z 0 0 0]
  // map          - [SRC INDEX, DST INDEX, FORCE, AREA OF INFLUENCE]
  // attenuation  - [ATTENUATION]
  // area         - [WIDTH HEIGTH]
  private GpuBag bag;

  public float[] Vertices => bag?.Vertices;

  public int ParticlesCount { get; private set; }

  public void FillUp(IDictionary<Color, IReadOnlyCollection<GravityRule>> rules, Vector2i area)
  {
    var offset = 0;
    var maps = new List<GpuMap>();
    var particlesMap = new Dictionary<ElementaryParticle, int>();
    foreach (var ruleCollection in rules.Values)
    {
      foreach (var rule in ruleCollection)
      {
        var areaOfInfluence = MapDisplayRange(rule.AreaOfInfluence, area.Y);
        foreach (var sourceParticle in rule.SourceGroup.Cast<ElementaryParticle>())
        {
          if (!particlesMap.TryGetValue(sourceParticle, out var sourceIndex))
          {
            particlesMap.Add(sourceParticle, offset);
            sourceIndex = offset;
            ++offset;
          }

          foreach (var targetParticle in rule.TargetGroup.Cast<ElementaryParticle>())
          {
            if (!particlesMap.TryGetValue(targetParticle, out var targetIndex))
            {
              particlesMap.Add(targetParticle, offset);
              targetIndex = offset;
              ++offset;
            }

            var map = new GpuMap(sourceIndex, targetIndex, rule.Force, areaOfInfluence);
            maps.Add(map);
          }
        }
      }
    }

    var particles = particlesMap
      .OrderBy(x => x.Value)
      .Select(x => x.Key)
      .ToArray();

    ParticlesCount = particles.Length;

    List<float> vertices = new(Settings.MaximumParticlesCount);
    List<float> accelerations = new(Settings.MaximumParticlesCount);
    foreach (var particle in particles)
    {
      AddPoint(vertices, particle.Position, area);
      AddColor(vertices, particle.Light);

      AddAcceleration(accelerations, particle.Acceleration);
      AddColor(accelerations, Color.Black);
    }

    bag = new GpuBag(vertices.ToArray(), accelerations.ToArray(), maps.ToArray(), area, Settings.Attenuation);
  }

  public void Setup(IGravitySimulator executor)
    => ((GpuGravitySimulator)executor).Setup(bag);

  private static void AddPoint(List<float> vertices, Vector2 position, Vector2i area)
  {
    vertices.Add(MapDisplayRange(position.X, area.Y));
    vertices.Add(MapDisplayRange(position.Y, area.Y));
    vertices.Add(0f); // Z
  }

  private static void AddColor(List<float> vertices, Color color) // BGR
  {
    vertices.Add(color.B / (float)byte.MaxValue);
    vertices.Add(color.G / (float)byte.MaxValue);
    vertices.Add(color.R / (float)byte.MaxValue);
  }

  private static void AddAcceleration(List<float> accelerations, Vector2 acceleration)
  {
    accelerations.Add(acceleration.X);
    accelerations.Add(acceleration.Y);
    accelerations.Add(0); // Z
  }

  /// <summary>
  /// Map screen coordinates to GL coordinates.
  /// </summary>
  /// <param name="value">Coordinate.</param>
  /// <param name="valueMax">Maximum value.</param>
  /// <returns>Mapped to specific range (0-max) value.</returns>
  /// <remarks>Offset -1.0 will be applied in vert shader.</remarks>
  private static float MapDisplayRange(float value, float valueMax)
    => MathHelper.MapRange(value, 0f, valueMax, 0f, 2f);
}