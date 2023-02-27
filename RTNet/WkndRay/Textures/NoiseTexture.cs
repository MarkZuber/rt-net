// -----------------------------------------------------------------------
// <copyright file="NoiseTexture.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;
using Zube.ImGuiNet;

namespace WkndRay.Textures
{
  public class NoiseTexture : ITexture
  {
    private readonly Perlin _noise = new Perlin();

    public NoiseTexture(bool interpolate, float scale)
    {
      Interpolate = interpolate;
      Scale = scale;
    }

    public bool Interpolate { get; }
    public float Scale { get; }

    public Vector4 GetValue(Vector2 uvCoords, Vector3 p)
    {
      return Vector4.One * _noise.Noise(Scale * p, Interpolate);
    }
  }
}
