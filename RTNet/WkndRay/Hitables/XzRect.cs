﻿// -----------------------------------------------------------------------
// <copyright file="XyRect.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using WkndRay.Materials;

namespace WkndRay.Hitables
{
  public class XzRect : AbstractHitable
  {
    public XzRect(float x0, float x1, float z0, float z1, float k, IMaterial material)
    {
      X0 = x0;
      X1 = x1;
      Z0 = z0;
      Z1 = z1;
      K = k;
      Material = material;
    }

    public float X0 { get; }
    public float X1 { get; }
    public float Z0 { get; }
    public float Z1 { get; }
    public float K { get; }
    public IMaterial Material { get; }

    public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hr)
    {
      float t = (K - ray.Origin.Y) / ray.Direction.Y;
      if (t < tMin || t > tMax)
      {
        return false;
      }

      float x = ray.Origin.X + (t * ray.Direction.X);
      float z = ray.Origin.Z + (t * ray.Direction.Z);
      if (x < X0 || x > X1 || z < Z0 || z > Z1)
      {
        return false;
      }

      hr.T = t;
      hr.P = ray.GetPointAtParameter(t);
      hr.Normal = Vector3.UnitY;
      hr.UvCoords = new Vector2((x - X0) / (X1 - X0), (z - Z0) / (Z1 - Z0));
      hr.Material = Material;
      return true;
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
      box = new AABB(new Vector3(X0, K - 0.001f, Z0), new Vector3(X1, K + 0.0001f, Z1));
      return true;
    }

    public override float GetPdfValue(Vector3 origin, Vector3 v)
    {
      HitRecord hr = new HitRecord();
      if (!Hit(new Ray(origin, v), 0.001f, float.MaxValue, ref hr))
      {
        return 0.0f;
      }

      float area = (X1 - X0) * (Z1 - Z0);
      float distanceSquared = hr.T * hr.T * v.LengthSquared();
      float cosine = MathF.Abs(Vector3.Dot(v, hr.Normal) / v.Length());
      return distanceSquared / (cosine * area);
    }

    public override Vector3 Random(Vector3 origin)
    {
      var randomPoint = new Vector3(X0 + (RandomService.NextSingle() * (X1 - X0)), K, Z0 + (RandomService.NextSingle() * (Z1 - Z0)));
      return randomPoint - origin;
    }
  }
}
