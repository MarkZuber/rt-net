using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WkndRay.Hitables
{
  public class Translate : AbstractHitable
  {
    public Translate(IHitable hitable, Vector3 displacement)
    {
      Hitable = hitable;
      Displacement = displacement;
    }

    public IHitable Hitable { get; }
    public Vector3 Displacement { get; }

    public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hr)
    {
      var movedRay = new Ray(ray.Origin - Displacement, ray.Direction);
      var hitRecord = new HitRecord();
      if (!Hitable.Hit(movedRay, tMin, tMax, ref hitRecord))
      {
        return false;
      }

      hr.T = hitRecord.T;
      hr.P = hitRecord.P + Displacement;
      hr.Normal = hitRecord.Normal;
      hr.UvCoords = hitRecord.UvCoords;
      hr.Material = hitRecord.Material;

      return true;
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
      if (!Hitable.BoundingBox(t0, t1, out box))
      {
        return false;
      }

      box = new AABB(box.Min + Displacement, box.Max + Displacement);
      return true;

    }
  }
}
