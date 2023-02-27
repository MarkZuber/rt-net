// -----------------------------------------------------------------------
// <copyright file="AbstractHitable.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;

namespace WkndRay.Hitables
{
  public abstract class AbstractHitable : IHitable
  {
    public abstract bool Hit(Ray ray, float tMin, float tMax, ref HitRecord rec);

    public abstract bool BoundingBox(float t0, float t1, out AABB box);

    public virtual float GetPdfValue(Vector3 origin, Vector3 v)
    {
      return 1.0f;
    }

    public virtual Vector3 Random(Vector3 origin)
    {
      return Vector3.UnitX;
    }
  }
}
