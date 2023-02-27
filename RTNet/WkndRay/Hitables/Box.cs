// -----------------------------------------------------------------------
// <copyright file="Box.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using WkndRay.Materials;

namespace WkndRay.Hitables
{
  public class Box : AbstractHitable
  {
    private readonly HitableList _list = new HitableList();

    public Box(Vector3 p0, Vector3 p1, IMaterial material)
    {
      PosMin = p0;
      PosMax = p1;

      _list.Add(new XyRect(p0.X, p1.X, p0.Y, p1.Y, p1.Z, material));
      _list.Add(new FlipNormals(new XyRect(p0.X, p1.X, p0.Y, p1.Y, p0.Z, material)));
      _list.Add(new XzRect(p0.X, p1.X, p0.Z, p1.Z, p1.Y, material));
      _list.Add(new FlipNormals(new XzRect(p0.X, p1.X, p0.Z, p1.Z, p0.Y, material)));
      _list.Add(new YzRect(p0.Y, p1.Y, p0.Z, p1.Z, p1.X, material));
      _list.Add(new FlipNormals(new YzRect(p0.Y, p1.Y, p0.Z, p1.Z, p0.X, material)));
    }

    public Vector3 PosMin { get; }
    public Vector3 PosMax { get; }

    public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hr)
    {
      return _list.Hit(ray, tMin, tMax, ref hr);
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
      box = new AABB(PosMin, PosMax);
      return true;
    }
  }
}
