// -----------------------------------------------------------------------
// <copyright file="IHitable.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;

namespace WkndRay
{
  public interface IHitable
  {
    bool Hit(Ray ray, float tMin, float tMax, ref HitRecord rec);
    bool BoundingBox(float t0, float t1, out AABB box);

    float GetPdfValue(Vector3 origin, Vector3 v);
    Vector3 Random(Vector3 origin);
  }
}
