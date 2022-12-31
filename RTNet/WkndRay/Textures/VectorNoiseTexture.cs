// -----------------------------------------------------------------------
// <copyright file="VectorNoiseTexture.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using RTNet.ImgCore;

namespace WkndRay.Textures
{
  public enum VectorNoiseMode
  {
    DarkNoise,
    DarkTurbulence,
    Soft,
    Marble,
  }

  public class VectorNoiseTexture : ITexture
  {
    private readonly VectorPerlin _noise = new VectorPerlin();

    public VectorNoiseTexture(VectorNoiseMode mode, float scale)
    {
      Mode = mode;
      Scale = scale;
    }

    public float Scale { get; }
    public VectorNoiseMode Mode { get; }

    public Vector4 GetValue(Vector2 uvCoords, Vector3 p)
    {
      switch (Mode)
      {
        case VectorNoiseMode.Soft:
          return Vector4.One * 0.5f * (1.0f + _noise.Turbulence(Scale * p));  // 2
        case VectorNoiseMode.DarkNoise:
          return Vector4.One * _noise.Noise(Scale * p);  // 1
        case VectorNoiseMode.DarkTurbulence:
          return Vector4.One * _noise.Turbulence(Scale * p); // 4
        case VectorNoiseMode.Marble:
          return Vector4.One * 0.5f * (1.0f + MathF.Sin((Scale * p.Z) + (10.0f * _noise.Turbulence(p)))); // 3
        default:
          throw new InvalidOperationException("unknown VectorNoiseMode");
      }
    }
  }
}
