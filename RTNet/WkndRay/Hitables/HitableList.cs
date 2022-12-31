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
        public HitRecord Hit(Ray ray, float tMin, float tMax)
        {
            HitRecord hitRecord = null;

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
        public AABB GetBoundingBox(float t0, float t1)
        {
            if (Count < 1)
            {
                return null;
            }

            AABB box = this[0].GetBoundingBox(t0, t1);
            if (box == null)
            {
                return null;
            }

            for (int i = 1; i < Count; i++)
            {
                var tempBox = this[i].GetBoundingBox(t0, t1);
                if (tempBox == null)
                {
                    return null;
                }

                box = box.GetSurroundingBox(tempBox);
            }

            return box;
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
            int index = Convert.ToInt32(MathF.Floor(RandomService.Nextfloat() * Convert.ToSingle(Count)));
            if (index < Count)
            {
                return this[index].Random(origin);
            }
            else
            {
                return origin;
            }
        }
    }
}
