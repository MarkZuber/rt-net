// -----------------------------------------------------------------------
// <copyright file="XyRect.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using WkndRay.Materials;

namespace WkndRay.Hitables
{
  public class YzRect : AbstractHitable
  {
    public YzRect(float y0, float y1, float z0, float z1, float k, IMaterial material)
    {
      Y0 = y0;
      Y1 = y1;
      Z0 = z0;
      Z1 = z1;
      K = k;
      Material = material;
    }

    public float Y0 { get; }
    public float Y1 { get; }
    public float Z0 { get; }
    public float Z1 { get; }
    public float K { get; }
    public IMaterial Material { get; }

    public override HitRecord Hit(Ray ray, float tMin, float tMax)
    {
      float t = (K - ray.Origin.X) / ray.Direction.X;
      if (t < tMin || t > tMax)
      {
        return null;
      }

      float y = ray.Origin.Y + (t * ray.Direction.Y);
      float z = ray.Origin.Z + (t * ray.Direction.Z);
      if (y < Y0 || y > Y1 || z < Z0 || z > Z1)
      {
        return null;
      }

      return new HitRecord(
        t,
        ray.GetPointAtParameter(t),
        Vector3.UnitX,
        new Vector2((y - Y0) / (Y1 - Y0), (z - Z0) / (Z1 - Z0)),
        Material);
    }

    public override AABB GetBoundingBox(float t0, float t1)
    {
      return new AABB(new Vector3(K - 0.001f, Y0, Z0), new Vector3(K + 0.0001f, Y1, Z1));
    }
  }
}
