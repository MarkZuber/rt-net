// -----------------------------------------------------------------------
// <copyright file="FlipNormals.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

namespace WkndRay.Hitables
{
    public class FlipNormals : AbstractHitable
    {
        public FlipNormals(IHitable hitable)
        {
            Hitable = hitable;
        }

        public IHitable Hitable { get; }

        public override HitRecord Hit(Ray ray, float tMin, float tMax)
        {
            var hitrec = Hitable.Hit(ray, tMin, tMax);
            if (hitrec != null)
            {
                // invert the normal...
                hitrec = new HitRecord(hitrec.T, hitrec.P, -hitrec.Normal, hitrec.UvCoords, hitrec.Material);
                return hitrec;
            }
            else
            {
                return null;
            }
        }

        public override AABB GetBoundingBox(float t0, float t1)
        {
            return Hitable.GetBoundingBox(t0, t1);
        }
    }
}