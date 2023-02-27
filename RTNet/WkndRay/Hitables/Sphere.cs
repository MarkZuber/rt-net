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

    public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hr)
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
          hr.T = temp;
          hr.P = ray.GetPointAtParameter(temp);
          hr.Normal = (hr.P - Center) / Radius;
          hr.Material = Material;
          return true;
        }

        temp = (-b + MathF.Sqrt((b * b) - (a * c))) / a;
        if (temp < tMax && temp > tMin)
        {
          hr.T = temp;
          hr.P = ray.GetPointAtParameter(temp);
          hr.Normal = (hr.P - Center) / Radius;
          hr.Material = Material;
          return true;
        }
      }

      return false;
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
      box = new AABB(Center - new Vector3(Radius, Radius, Radius), Center + new Vector3(Radius, Radius, Radius));
      return true;
    }

    public override float GetPdfValue(Vector3 origin, Vector3 v)
    {
      HitRecord hr = new HitRecord();
      if (!Hit(new Ray(origin, v), 0.001f, float.MaxValue, ref hr))
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
