// -----------------------------------------------------------------------
// <copyright file="BvhNode.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WkndRay.Hitables;

namespace WkndRay
{
    public class BvhNode : AbstractHitable
    {
        public BvhNode(IEnumerable<IHitable> hitables, float time0, float time1)
        {
            List<IHitable> list = hitables.ToList();
            int axis = Convert.ToInt32(3.0f * RandomService.Nextfloat());
            if (axis == 0)
            {
                list.Sort(new HitableXCompare());
            }
            else if (axis == 1)
            {
                list.Sort(new HitableYCompare());
            }
            else
            {
                list.Sort(new HitableZCompare());
            }

            if (list.Count == 1)
            {
                Left = list[0];
                Right = list[0];
            }
            else if (list.Count == 2)
            {
                Left = list[0];
                Right = list[1];
            }
            else
            {
                Left = new BvhNode(list.Take(list.Count / 2), time0, time1);
                Right = new BvhNode(list.Skip(list.Count / 2), time0, time1);
            }

            var boxLeft = Left.GetBoundingBox(time0, time1);
            var boxRight = Right.GetBoundingBox(time0, time1);

            if (boxLeft == null || boxRight == null)
            {
                throw new InvalidOperationException("no bounding box");
            }

            Box = boxLeft.GetSurroundingBox(boxRight);
        }

        public IHitable Left { get; }
        public IHitable Right { get; }
        public AABB Box { get; }

        public override HitRecord Hit(Ray ray, float tMin, float tMax)
        {
            if (!Box.Hit(ray, tMin, tMax))
            {
                return null;
            }

            HitRecord hrLeft = Left.Hit(ray, tMin, tMax);
            HitRecord hrRight = Right.Hit(ray, tMin, tMax);
            if (hrLeft != null && hrRight != null)
            {
                return (hrLeft.T < hrRight.T) ? hrLeft : hrRight;
            }
            else if (hrLeft != null)
            {
                return hrLeft;
            }
            else if (hrRight != null)
            {
                return hrRight;
            }

            return null;
        }

        /// <inheritdoc />
        public override AABB GetBoundingBox(float t0, float t1)
        {
            return Box;
        }

        private abstract class HitableBoxCompare : IComparer<IHitable>
        {
            public int Compare(IHitable x, IHitable y)
            {
                if (x == null || y == null)
                {
                    throw new ArgumentNullException();
                }

                // we ignore time ranges with the comparators
                var xbb = x.GetBoundingBox(0.0f, 0.0f);
                var ybb = y.GetBoundingBox(0.0f, 0.0f);

                if (xbb == null || ybb == null)
                {
                    throw new InvalidOperationException("no bounding box");
                }

                return BoxCompare(xbb, ybb);
            }

            protected abstract int BoxCompare(AABB a, AABB b);
        }

        private class HitableXCompare : HitableBoxCompare
        {
            protected override int BoxCompare(AABB a, AABB b)
            {
                return (a.Min.X - b.Min.X < 0.0f) ? -1 : 1;
            }
        }

        private class HitableYCompare : HitableBoxCompare
        {
            protected override int BoxCompare(AABB a, AABB b)
            {
                return (a.Min.Y - b.Min.Y < 0.0f) ? -1 : 1;
            }
        }

        private class HitableZCompare : HitableBoxCompare
        {
            protected override int BoxCompare(AABB a, AABB b)
            {
                return (a.Min.Z - b.Min.Z < 0.0f) ? -1 : 1;
            }
        }
    }
}