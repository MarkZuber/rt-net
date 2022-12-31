using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using RTNet.ImgCore;

namespace WkndRay.Textures
{
  public class CheckerTexture : ITexture
  {
    public CheckerTexture(ITexture t1, ITexture t2, Vector3 scale)
    {
      T1 = t1;
      T2 = t2;
      Scale = scale;
    }

    public ITexture T1 { get; }
    public ITexture T2 { get; }
    public Vector3 Scale { get; }

    /// <inheritdoc />
    public Vector4 GetValue(Vector2 uvCoords, Vector3 p)
    {
      float sines = MathF.Sin(Scale.X * p.X) * MathF.Sin(Scale.Y * p.Y) * MathF.Sin(Scale.Z * p.Z);
      return (sines < 0.0f) ? T1.GetValue(uvCoords, p) : T2.GetValue(uvCoords, p);
    }
  }
}
