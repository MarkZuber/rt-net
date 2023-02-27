// -----------------------------------------------------------------------
// <copyright file="BvhNode.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WkndRay.Hitables;

namespace WkndRay
{
  public class BvhNode : AbstractHitable
  {
    public void DebugPrint(int indent = 0)
    {
      string indentStr = new String(' ', indent * 4);
      Debug.WriteLine($"{indentStr}Box: {Box.Min} {Box.Max}");

      indent = indent + 1;
      indentStr = new String(' ', indent * 4);

      if (Left is BvhNode left)
      {
        Debug.WriteLine($"{indentStr}LEFT BOX");
        left.DebugPrint(indent + 1);
      }
      else
      {
        Debug.WriteLine($"{indentStr}Left -> {Left}");
      }

      if (Right is BvhNode right)
      {
        Debug.WriteLine($"{indentStr}RIGHT BOX");
        right.DebugPrint(indent + 1);
      }
      else
      {
        Debug.WriteLine($"{indentStr}Right -> {Right}");
      }
    }

    public BvhNode(List<IHitable> hitables, float time0, float time1)
    {
      int axis = RandomService.IntInRange(0, 2);

      switch (axis)
      {
        case 0:
          // sort x
          hitables.Sort(BvhNode.BoxXCompare);
          break;
        case 1:
          // sort y
          hitables.Sort(BvhNode.BoxYCompare);
          break;
        default:
          // sprt z
          hitables.Sort(BvhNode.BoxZCompare);
          break;
      }

      switch (hitables.Count)
      {
        case 1:
          Left = Right = hitables[0];
          break;
        case 2:
          Left = hitables[0];
          Right = hitables[1];
          break;
        default:
          Left = new BvhNode(hitables.Take(hitables.Count / 2).ToList(), time0, time1);
          Right = new BvhNode(hitables.Skip(hitables.Count / 2).ToList(), time0, time1);
          break;
      }

      if (!Left.BoundingBox(time0, time1, out AABB boxLeft) ||
          !Right.BoundingBox(time0, time1, out AABB boxRight))
      {
        throw new Exception("No bounding box in BVHNode");
      }

      Box = boxRight.SurroundingBox(boxLeft);
    }

    public IHitable Left { get; }
    public IHitable Right { get; }
    public AABB Box { get; }

    public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord rec)
    {
      if (Box.Hit(ray, tMin, tMax))
      {
        HitRecord leftRecord = new HitRecord();
        HitRecord rightRecord = new HitRecord();

        bool hitLeft = Left.Hit(ray, tMin, tMax, ref leftRecord);
        bool hitRight = Right.Hit(ray, tMin, tMax, ref rightRecord);
        if (hitLeft && hitRight)
        {
          rec = leftRecord.T < rightRecord.T ? leftRecord : rightRecord;
          return true;
        }

        if (hitLeft)
        {
          rec = leftRecord;
          return true;
        }

        if (hitRight)
        {
          rec = rightRecord;
          return true;
        }
      }
      return false;
    }

    /// <inheritdoc />
    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
      box = Box;
      return true;
    }

    private static int BoxXCompare(IHitable a, IHitable b)
    {
      if (!a.BoundingBox(0, 0, out AABB boxLeft) || !b.BoundingBox(0, 0, out AABB boxRight))
      {
        throw new Exception("No bounding box in BVHNode");
      }

      if (boxLeft.Min.X - boxRight.Min.X < 0)
      {
        return -1;
      }

      return 1;
    }

    private static int BoxYCompare(IHitable a, IHitable b)
    {
      if (!a.BoundingBox(0, 0, out AABB boxLeft) || !b.BoundingBox(0, 0, out AABB boxRight))
      {
        throw new Exception("No bounding box in BVHNode");
      }

      if (boxLeft.Min.Y - boxRight.Min.Y < 0)
      {
        return -1;
      }

      return 1;
    }

    private static int BoxZCompare(IHitable a, IHitable b)
    {
      if (!a.BoundingBox(0, 0, out AABB boxLeft) || !b.BoundingBox(0, 0, out AABB boxRight))
      {
        throw new Exception("No bounding box in BVHNode");
      }

      if (boxLeft.Min.Z - boxRight.Min.Z < 0)
      {
        return -1;
      }

      return 1;
    }
  }
}