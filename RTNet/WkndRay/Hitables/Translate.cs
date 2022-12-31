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

        public override HitRecord Hit(Ray ray, float tMin, float tMax)
        {
            var movedRay = new Ray(ray.Origin - Displacement, ray.Direction);
            var hitRecord = Hitable.Hit(movedRay, tMin, tMax);
            if (hitRecord == null)
            {
                return null;
            }

            return new HitRecord(
              hitRecord.T,
              hitRecord.P + Displacement,
              hitRecord.Normal,
              hitRecord.UvCoords,
              hitRecord.Material);
        }

        public override AABB GetBoundingBox(float t0, float t1)
        {
            var box = Hitable.GetBoundingBox(t0, t1);
            if (box == null)
            {
                return null;
            }

            box = new AABB(box.Min + Displacement, box.Max + Displacement);
            return box;

        }
    }
}
