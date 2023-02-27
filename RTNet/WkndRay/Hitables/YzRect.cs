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

    public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hr)
    {
      float t = (K - ray.Origin.X) / ray.Direction.X;
      if (t < tMin || t > tMax)
      {
        return false;
      }

      float y = ray.Origin.Y + (t * ray.Direction.Y);
      float z = ray.Origin.Z + (t * ray.Direction.Z);
      if (y < Y0 || y > Y1 || z < Z0 || z > Z1)
      {
        return false;
      }

      hr = new HitRecord(
        t,
        ray.GetPointAtParameter(t),
        Vector3.UnitX,
        new Vector2((y - Y0) / (Y1 - Y0), (z - Z0) / (Z1 - Z0)),
        Material);

      return true;
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
      box = new AABB(new Vector3(K - 0.001f, Y0, Z0), new Vector3(K + 0.0001f, Y1, Z1));
      return true;
    }

    //   public override float GetPdfValue(Vector3 origin, Vector3 v)
    //   {
    //     HitRecord hr = new HitRecord();
    //     if (!Hit(new Ray(origin, v), 0.001f, float.MaxValue, ref hr))
    //     {
    //       return 0.0f;
    //     }

    //     float area = (Y1 - Y0) * (Z1 - Z0);
    //     float distanceSquared = hr.T * hr.T * v.LengthSquared();
    //     float cosine = MathF.Abs(Vector3.Dot(v, hr.Normal) / v.Length());
    //     return distanceSquared / (cosine * area);
    //   }

    //   public override Vector3 Random(Vector3 origin)
    //   {
    //     var randomPoint = new Vector3(Y0 + (RandomService.NextSingle() * (Y1 - Y0)), K, Z0 + (RandomService.NextSingle() * (Z1 - Z0)));
    //     return randomPoint - origin;
    //   }
  }
}
