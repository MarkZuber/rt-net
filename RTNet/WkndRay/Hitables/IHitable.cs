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
        HitRecord Hit(Ray ray, float tMin, float tMax);
        AABB GetBoundingBox(float t0, float t1);

        float GetPdfValue(Vector3 origin, Vector3 v);
        Vector3 Random(Vector3 origin);
    }
}
