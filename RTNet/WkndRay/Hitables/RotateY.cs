using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WkndRay.Hitables
{
    public class RotateY : AbstractHitable
    {
        public RotateY(IHitable hitable, float angle)
        {
            Hitable = hitable;
            Angle = angle;

            float radians = MathF.PI / 180.0f * angle;
            SinTheta = MathF.Sin(radians);
            CosTheta = MathF.Cos(radians);
            var box = Hitable.GetBoundingBox(0.0f, 1.0f);
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue).ToSingleArray();
            var max = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue).ToSingleArray();

            for (int i = 0; i < 2; i++)
            {
                float dubi = Convert.ToSingle(i);
                for (int j = 0; j < 2; j++)
                {
                    float dubj = Convert.ToSingle(j);
                    for (int k = 0; k < 2; k++)
                    {
                        float dubk = Convert.ToSingle(k);
                        float x = (dubi * box.Max.X) + ((1.0f - dubi) * box.Min.X);
                        float y = (dubj * box.Max.Y) + ((1.0f - dubj) * box.Min.Y);
                        float z = (dubk * box.Max.Z) + ((1.0f - dubk) * box.Min.Z);
                        float newx = (CosTheta * x) + (SinTheta * z);
                        float newz = (-SinTheta * x) + (CosTheta * z);
                        var tester = new Vector3(newx, y, newz).ToSingleArray();
                        for (int c = 0; c < 3; c++)
                        {
                            if (tester[c] > max[c])
                            {
                                max[c] = tester[c];
                            }

                            if (tester[c] < min[c])
                            {
                                min[c] = tester[c];
                            }
                        }
                    }
                }
            }

            BoundingBox = new AABB(new Vector3(min[0], min[1], min[2]), new Vector3(max[0], max[1], max[2]));
        }

        public IHitable Hitable { get; }
        public float Angle { get; }
        public float SinTheta { get; }
        public float CosTheta { get; }
        public AABB BoundingBox { get; }

        public override HitRecord Hit(Ray ray, float tMin, float tMax)
        {
            var origin = ray.Origin.ToSingleArray();
            var dir = ray.Direction.ToSingleArray();
            origin[0] = (CosTheta * ray.Origin.X) - (SinTheta * ray.Origin.Z);
            origin[2] = (SinTheta * ray.Origin.X) + (CosTheta * ray.Origin.Z);
            dir[0] = (CosTheta * ray.Direction.X) - (SinTheta * ray.Direction.Z);
            dir[2] = (SinTheta * ray.Direction.X) + (CosTheta * ray.Direction.Z);
            var rotatedRay = new Ray(new Vector3(origin[0], origin[1], origin[2]), new Vector3(dir[0], dir[1], dir[2]));
            var hitRecord = Hitable.Hit(rotatedRay, tMin, tMax);
            if (hitRecord == null)
            {
                return null;
            }

            var p = hitRecord.P.ToSingleArray();
            var normal = hitRecord.Normal.ToSingleArray();
            p[0] = (CosTheta * hitRecord.P.X) + (SinTheta * hitRecord.P.Z);
            p[2] = (-SinTheta * hitRecord.P.X) + (CosTheta * hitRecord.P.Z);
            normal[0] = (CosTheta * hitRecord.Normal.X) + (SinTheta * hitRecord.Normal.Z);
            normal[2] = (-SinTheta * hitRecord.Normal.X) + (CosTheta * hitRecord.Normal.Z);
            return new HitRecord(hitRecord.T, new Vector3(p[0], p[1], p[2]), new Vector3(normal[0], normal[1], normal[2]), hitRecord.UvCoords, hitRecord.Material);
        }

        public override AABB GetBoundingBox(float t0, float t1)
        {
            return BoundingBox;
        }
    }
}
