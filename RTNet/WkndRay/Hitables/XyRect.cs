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
  public class XyRect : AbstractHitable
  {
    public XyRect(float x0, float x1, float y0, float y1, float k, IMaterial material)
    {
      X0 = x0;
      X1 = x1;
      Y0 = y0;
      Y1 = y1;
      K = k;
      Material = material;
    }

    public float X0 { get; }
    public float X1 { get; }
    public float Y0 { get; }
    public float Y1 { get; }
    public float K { get; }
    public IMaterial Material { get; }

    public override HitRecord Hit(Ray ray, float tMin, float tMax)
    {
      float t = (K - ray.Origin.Z) / ray.Direction.Z;
      if (t < tMin || t > tMax)
      {
        return null;
      }

      float x = ray.Origin.X + (t * ray.Direction.X);
      float y = ray.Origin.Y + (t * ray.Direction.Y);
      if (x < X0 || x > X1 || y < Y0 || y > Y1)
      {
        return null;
      }

      return new HitRecord(
        t,
        ray.GetPointAtParameter(t),
        Vector3.UnitZ,
        new Vector2((x - X0) / (X1 - X0), (y - Y0) / (Y1 - Y0)),
        Material);
    }

    public override AABB GetBoundingBox(float t0, float t1)
    {
      return new AABB(new Vector3(X0, Y0, K - 0.001f), new Vector3(X1, Y1, K + 0.0001f));
    }
  }
}
