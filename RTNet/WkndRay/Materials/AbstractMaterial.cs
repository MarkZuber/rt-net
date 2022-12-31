// -----------------------------------------------------------------------
// <copyright file="AbstractMaterial.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;

namespace WkndRay.Materials
{
  public abstract class AbstractMaterial : IMaterial
  {
    public abstract ScatterResult Scatter(Ray rayIn, HitRecord hitRecord);

    public virtual float ScatteringPdf(Ray rayIn, HitRecord hitRecord, Ray scattered)
    {
      return 0.0f;
    }

    public virtual Vector4 Emitted(Ray rayIn, HitRecord hitRecord, Vector2 uvCoords, Vector3 p)
    {
      return Vector4.Zero;
    }
  }
}
