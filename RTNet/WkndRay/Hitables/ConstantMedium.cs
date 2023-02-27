// -----------------------------------------------------------------------
// <copyright file="ConstantMedium.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using RTNet.ImgCore;
using WkndRay.Materials;
using WkndRay.Textures;

namespace WkndRay.Hitables
{
  public class ConstantMedium : AbstractHitable
  {
    public ConstantMedium(IHitable boundary, float density, ITexture a)
    {
      Boundary = boundary;
      Density = density;
      PhaseFunction = new IsotropicMaterial(a);
    }

    public IHitable Boundary { get; }
    public float Density { get; }
    public IMaterial PhaseFunction { get; }

    public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hr)
    {
      HitRecord hitRecord1 = new HitRecord();
      if (!Boundary.Hit(ray, -float.MaxValue, float.MaxValue, ref hitRecord1))
      {
        return false;
      }

      HitRecord? hitRecord2 = new HitRecord();
      if (!Boundary.Hit(ray, hitRecord1.T + 0.0001f, float.MaxValue, ref hitRecord2))
      {
        return false;
      }

      float rec1T = hitRecord1.T;
      float rec2T = hitRecord2.T;

      if (rec1T < tMin)
      {
        rec1T = tMin;
      }

      if (rec2T > tMax)
      {
        rec2T = tMax;
      }

      if (rec1T >= rec2T)
      {
        return false;
      }

      if (rec1T < 0.0f)
      {
        rec1T = 0.0f;
      }

      float distanceInsideBoundary = ((rec2T - rec1T) * ray.Direction).Length();
      float hitDistance = -(1.0f / Density) * MathF.Log(RandomService.NextSingle());
      if (hitDistance < distanceInsideBoundary)
      {
        float recT = rec1T + (hitDistance / ray.Direction.Length());

        hr.T = recT;
        hr.P = ray.GetPointAtParameter(recT);
        hr.Normal = Vector3.UnitX;
        hr.UvCoords = new Vector2(0.0f, 0.0f); // don't need u/v since PhaseFunction is a calculation
        hr.Material = PhaseFunction;

        return true;
      }

      return false;
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
      return Boundary.BoundingBox(t0, t1, out box);
    }
  }
}
