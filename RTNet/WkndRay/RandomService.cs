// -----------------------------------------------------------------------
// <copyright file="RandomService.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;

namespace WkndRay
{
    public class RandomService
    {
        private static readonly SafeRandom s_random = new SafeRandom();

        public static float Nextfloat()
        {
            return Convert.ToSingle(s_random.NextDouble());
        }

        public static Vector3 GetRandomCosineDirection()
        {
            float r1 = Nextfloat();
            float r2 = Nextfloat();
            float z = MathF.Sqrt(1.0f - r2);
            float phi = 2.0f * MathF.PI * r1;
            float x = MathF.Cos(phi) * 2.0f * MathF.Sqrt(r2);
            float y = MathF.Sin(phi) * 2.0f * MathF.Sqrt(r2);
            return new Vector3(x, y, z);
        }

        public static Vector3 RandomToSphere(float radius, float distanceSquared)
        {
            float r1 = Nextfloat();
            float r2 = Nextfloat();
            float z = 1.0f + (r2 * (MathF.Sqrt(1.0f - (radius * radius / distanceSquared)) - 1.0f));
            float phi = 2.0f * MathF.PI * r1;
            float x = MathF.Cos(phi) * MathF.Sqrt(1.0f - (z * z));
            float y = MathF.Sin(phi) * MathF.Sqrt(1.0f - (z * z));
            return new Vector3(x, y, z);
        }
    }
}
