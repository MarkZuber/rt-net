//// -----------------------------------------------------------------------
//// <copyright file="PosVector.cs" company="ZubeNET">
////   Copyright...
//// </copyright>
//// -----------------------------------------------------------------------

//using System;

//namespace WkndRay
//{
//    public class PosVector
//    {
//        public PosVector(float x, float y, float z)
//        {
//            X = x;
//            Y = y;
//            Z = z;
//        }

//        public PosVector(float[] xyz)
//        {
//            if (xyz.Length != 3)
//            {
//                throw new ArgumentException("xyz array must be of length 3");
//            }

//            X = xyz[0];
//            Y = xyz[1];
//            Z = xyz[2];
//        }

//        public float X { get; }
//        public float Y { get; }
//        public float Z { get; }

//        public static PosVector Zero => new PosVector(0.0f, 0.0f, 0.0f);
//        public static PosVector One => new PosVector(1.0f, 1.0f, 1.0f);
//        public static PosVector UnitX => new PosVector(1.0f, 0.0f, 0.0f);
//        public static PosVector UnitY => new PosVector(0.0f, 1.0f, 0.0f);
//        public static PosVector UnitZ => new PosVector(0.0f, 0.0f, 1.0f);

//        public float[] ToSingleArray()
//        {
//            return new float[] { X, Y, Z };
//        }

//        public override string ToString()
//        {
//            return string.Format($"({X},{Y},{Z})");
//        }

//        private static float ClampValue(float val, float min, float max)
//        {
//            if (val < min)
//            {
//                return min;
//            }

//            return val > max ? max : val;
//        }

//        public PosVector Clamp(PosVector min, PosVector max)
//        {
//            return new PosVector(ClampValue(X, min.X, max.X), ClampValue(Y, min.Y, max.Y), ClampValue(Z, min.Z, max.Z));
//        }

//        public static float CosVectors(PosVector v1, PosVector v2)
//        {
//            return v1.Dot(v2) / MathF.Sqrt(v1.MagnitudeSquared() * v2.MagnitudeSquared());
//        }

//        public float MagnitudeSquared()
//        {
//            return Dot(this);
//        }

//        public float Magnitude()
//        {
//            return MathF.Sqrt(MagnitudeSquared());
//        }

//        public PosVector Normalize()
//        {
//            return this / Magnitude();
//        }

//        public Vector4 ToVector4()
//        {
//            return new Vector4(X, Y, Z);
//        }

//        public PosVector ToUnitVector()
//        {
//            float k = 1.0f / Magnitude();
//            return new PosVector(X * k, Y * k, Z * k);
//        }

//        public static PosVector operator +(PosVector a, PosVector b)
//        {
//            return new PosVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
//        }

//        public static PosVector operator -(PosVector a, PosVector b)
//        {
//            return new PosVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
//        }

//        public static PosVector operator -(PosVector a)
//        {
//            return new PosVector(-a.X, -a.Y, -a.Z);
//        }

//        public static PosVector operator *(PosVector a, float scalar)
//        {
//            return new PosVector(a.X * scalar, a.Y * scalar, a.Z * scalar);
//        }

//        public static PosVector operator *(float scalar, PosVector a)
//        {
//            return new PosVector(a.X * scalar, a.Y * scalar, a.Z * scalar);
//        }

//        public static PosVector operator /(PosVector a, float scalar)
//        {
//            return new PosVector(a.X / scalar, a.Y / scalar, a.Z / scalar);
//        }

//        public PosVector AddScaled(PosVector b, float scale)
//        {
//            return new PosVector(X + (scale * b.X), Y + (scale * b.Y), Z + (scale * b.Z));
//        }

//        public PosVector Cross(PosVector b)
//        {
//            return new PosVector((Y * b.Z) - (Z * b.Y), (Z * b.X) - (X * b.Z), (X * b.Y) - (Y * b.X));
//        }

//        public float Dot(PosVector b)
//        {
//            return (X * b.X) + (Y * b.Y) + (Z * b.Z);
//        }

//        public static PosVector GetRandomInUnitSphere()
//        {
//            PosVector pv;
//            do
//            {
//                pv = (2.0f * new PosVector(RandomService.Nextfloat(), RandomService.Nextfloat(), RandomService.Nextfloat())) -
//                     PosVector.One;
//            }
//            while (pv.MagnitudeSquared() >= 1.0f);

//            return pv;
//        }

//        public PosVector Reflect(PosVector other)
//        {
//            return this - (2 * Dot(other) * other);
//        }

//        public PosVector Refract(PosVector normal, float niOverNt)
//        {
//            var unitVector = ToUnitVector();
//            float dt = unitVector.Dot(normal);
//            float discriminant = 1.0f - (niOverNt * niOverNt * (1.0f - (dt * dt)));
//            if (discriminant > 0.0f)
//            {
//                return (niOverNt * (unitVector - (normal * dt))) - (normal * MathF.Sqrt(discriminant));
//            }

//            return null;
//        }
//    }
//}
