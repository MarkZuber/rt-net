using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WkndRay
{
    public class OrthoNormalBase
    {
        public static OrthoNormalBase FromW(Vector3 n)
        {
            Vector3 w = n.ToUnitVector();
            Vector3 a = MathF.Abs(w.X) > 0.9 ? Vector3.UnitY : Vector3.UnitX;
            Vector3 v = (Vector3.Cross(w, a)).ToUnitVector();
            Vector3 u = Vector3.Cross(w, v);
            return new OrthoNormalBase(u, v, w);
        }

        public Vector3 Local(float a, float b, float c)
        {
            return (a * U) + (b * V) + (c * W);
        }

        public Vector3 Local(Vector3 a)
        {
            return (a.X * U) + (a.Y * V) + (a.Z * W);
        }

        private OrthoNormalBase(Vector3 u, Vector3 v, Vector3 w)
        {
            U = u;
            V = v;
            W = w;
        }

        public Vector3 U { get; }
        public Vector3 V { get; }
        public Vector3 W { get; }
    }
}
