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
    public HitRecord? Hit(Ray ray, float tMin, float tMax)
    {
      HitRecord? hitRecord = null;

      float closestSoFar = tMax;
      foreach (IHitable item in this)
      {
        var hr = item.Hit(ray, tMin, closestSoFar);
        if (hr == null)
        {
          continue;
        }

        closestSoFar = hr.T;
        hitRecord = hr;
      }

      return hitRecord;
    }

    /// <inheritdoc />
    public AABB? GetBoundingBox(float t0, float t1)
    {
      if (Count < 1)
      {
        return null;
      }


      var tempBox = new AABB();
      var outputBox = new AABB();
      bool firstBox = true;

      foreach (var obj in this)
      {
        tempBox = obj.GetBoundingBox(t0, t1);
        if (tempBox == null)
        {
          return null;
        }

        outputBox = firstBox ? tempBox : outputBox.GetSurroundingBox(tempBox);
        firstBox = false;
      }

      return outputBox;
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
