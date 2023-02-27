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

    public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hr)
    {
      var hitrec = new HitRecord();
      if (Hitable.Hit(ray, tMin, tMax, ref hitrec))
      {
        // invert the normal...
        hr.T = hitrec.T;
        hr.P = hitrec.P;
        hr.Normal = -hitrec.Normal;
        hr.UvCoords = hitrec.UvCoords;
        hr.Material = hitrec.Material;
        return true;
      }
      else
      {
        return false;
      }
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
      return Hitable.BoundingBox(t0, t1, out box);
    }
  }
}