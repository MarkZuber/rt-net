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
        public abstract HitRecord Hit(Ray ray, float tMin, float tMax);

        public abstract AABB GetBoundingBox(float t0, float t1);

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
