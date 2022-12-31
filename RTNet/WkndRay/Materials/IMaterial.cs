// -----------------------------------------------------------------------
// <copyright file="IMaterial.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;

namespace WkndRay.Materials
{
  public interface IMaterial
  {
    ScatterResult Scatter(Ray rayIn, HitRecord hitRecord);
    float ScatteringPdf(Ray rayIn, HitRecord hitRecord, Ray scattered);
    Vector4 Emitted(Ray rayIn, HitRecord hitRecord, Vector2 uvCoords, Vector3 p);
  }
}
