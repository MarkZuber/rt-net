// -----------------------------------------------------------------------
// <copyright file="HitableList.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Numerics;

namespace WkndRay
{
  public class HitableList : List<IHitable>,
                             IHitable
  {
    public bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hr)
    {
      HitRecord tempRec = new HitRecord();
      bool hitAnything = false;

      float closestSoFar = tMax;
      foreach (IHitable item in this)
      {
        if (!item.Hit(ray, tMin, closestSoFar, ref tempRec))
        {
          continue;
        }
        hitAnything = true;

        closestSoFar = tempRec.T;
        hr = tempRec;
      }

      return hitAnything;
    }

    /// <inheritdoc />
    public bool BoundingBox(float t0, float t1, out AABB box)
    {
      if (Count < 1)
      {
        box = new AABB();
        return false;
      }


      bool firstTrue = this[0].BoundingBox(t0, t1, out AABB tempBox);
      if (!firstTrue)
      {
        box = new AABB();
        return false;
      }

      box = tempBox;

      for (var i = 1; i < this.Count; i++)
      {
        if (this[i].BoundingBox(t0, t1, out tempBox))
        {
          box = box.SurroundingBox(tempBox);
        }
        else
        {
          return false;
        }
      }

      return true;
    }

    public float GetPdfValue(Vector3 origin, Vector3 v)
    {
      float weight = 1.0f / Count;
      float sum = 0.0f;
      foreach (var hitable in this)
      {
        sum += weight * hitable.GetPdfValue(origin, v);
      }
      return sum;
    }

    public Vector3 Random(Vector3 origin)
    {
      if (Count == 0)
      {
        return new Vector3();
      }
      int index = RandomService.IntInRange(0, Count - 1);
      return this[index].Random(origin);
    }
  }
}
