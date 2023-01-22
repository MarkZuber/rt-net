// -----------------------------------------------------------------------
// <copyright file="Sphere.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using WkndRay.Hitables;
using WkndRay.Materials;

namespace WkndRay
{
  public class Sphere : AbstractHitable
  {
    public Sphere(Vector3 center, float radius, IMaterial? material = null)
    {
      Center = center;
      Radius = radius;
      Material = material;
    }

    public Vector3 Center { get; }
    public float Radius { get; }
    public IMaterial? Material { get; }

    public override HitRecord? Hit(Ray ray, float tMin, float tMax)
    {
      var oc = ray.Origin - Center;
      float a = Vector3.Dot(ray.Direction, ray.Direction);
      float b = Vector3.Dot(oc, ray.Direction);
      float c = Vector3.Dot(oc, oc) - (Radius * Radius);
      float discriminant = (b * b) - (a * c);
      if (discriminant > 0.0f)
      {
        float temp = (-b - MathF.Sqrt((b * b) - (a * c))) / a;
        if (temp < tMax & temp > tMin)
        {
          var p = ray.GetPointAtParameter(temp);
          return new HitRecord(temp, p, (p - Center) / Radius, GetSphereUv(p), Material);
        }

        temp = (-b + MathF.Sqrt((b * b) - (a * c))) / a;
        if (temp < tMax && temp > tMin)
        {
          var p = ray.GetPointAtParameter(temp);
          return new HitRecord(temp, p, (p - Center) / Radius, GetSphereUv(p), Material);
        }
      }

      return null;
    }

    public override AABB? GetBoundingBox(float t0, float t1)
    {
      return new AABB(Center - new Vector3(Radius, Radius, Radius), Center + new Vector3(Radius, Radius, Radius));
    }

    public override float GetPdfValue(Vector3 origin, Vector3 v)
    {
      HitRecord? hr = Hit(new Ray(origin, v), 0.001f, float.MaxValue);
      if (hr == null)
      {
        return 0.0f;
      }

      float cosThetaMax = MathF.Sqrt(1.0f - (Radius * Radius / (Center - origin).LengthSquared()));
      float solidAngle = 2.0f * MathF.PI * (1.0f - cosThetaMax);
      return 1.0f / solidAngle;
    }

    public override Vector3 Random(Vector3 origin)
    {
      Vector3 direction = Center - origin;
      float distanceSquared = direction.LengthSquared();
      OrthoNormalBase uvw = OrthoNormalBase.FromW(direction);
      return uvw.Local(RandomService.RandomToSphere(Radius, distanceSquared));
    }

    private Vector2 GetSphereUv(Vector3 p)
    {
      var punit = p.ToUnitVector();
      float phi = MathF.Atan2(punit.Z, punit.X);
      float theta = MathF.Asin(punit.Y);
      float u = 1.0f - ((phi + MathF.PI) / (2.0f * MathF.PI));
      float v = (theta + (MathF.PI / 2.0f)) / MathF.PI;
      return new Vector2(u, v);
    }
  }
}
