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

    public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hr)
    {
      float t = (K - ray.Origin.Z) / ray.Direction.Z;
      if (t < tMin || t > tMax)
      {
        return false;
      }

      float x = ray.Origin.X + (t * ray.Direction.X);
      float y = ray.Origin.Y + (t * ray.Direction.Y);
      if (x < X0 || x > X1 || y < Y0 || y > Y1)
      {
        return false;
      }

      hr.T = t;
      hr.P = ray.GetPointAtParameter(t);
      hr.Normal = Vector3.UnitZ;
      hr.UvCoords = new Vector2((x - X0) / (X1 - X0), (y - Y0) / (Y1 - Y0));
      hr.Material = Material;

      return true;
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
      box = new AABB(new Vector3(X0, Y0, K - 0.001f), new Vector3(X1, Y1, K + 0.0001f));
      return true;
    }

    // public override float GetPdfValue(Vector3 origin, Vector3 v)
    // {
    //   HitRecord hr = new HitRecord();
    //   if (!Hit(new Ray(origin, v), 0.001f, float.MaxValue, ref hr))
    //   {
    //     return 0.0f;
    //   }

    //   float area = (X1 - X0) * (Y1 - Y0);
    //   float distanceSquared = hr.T * hr.T * v.LengthSquared();
    //   float cosine = MathF.Abs(Vector3.Dot(v, hr.Normal) / v.Length());
    //   return distanceSquared / (cosine * area);
    // }

    // public override Vector3 Random(Vector3 origin)
    // {
    //   var randomPoint = new Vector3(X0 + (RandomService.NextSingle() * (X1 - X0)), K, Y0 + (RandomService.NextSingle() * (Y1 - Y0)));
    //   return randomPoint - origin;
    // }
  }
}
