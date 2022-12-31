// -----------------------------------------------------------------------
// <copyright file="VectorPerlin.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;

namespace WkndRay
{
    public class VectorPerlin
    {
        private static readonly Vector3[] s_ranVector;
        private static readonly int[] s_permX;
        private static readonly int[] s_permY;
        private static readonly int[] s_permZ;

        static VectorPerlin()
        {
            s_ranVector = PerlinGenerate();
            s_permX = PerlinGenratePerm();
            s_permY = PerlinGenratePerm();
            s_permZ = PerlinGenratePerm();
        }

        public float Noise(Vector3 p)
        {
            float u = p.X - MathF.Floor(p.X);
            float v = p.Y - MathF.Floor(p.Y);
            float w = p.Z - MathF.Floor(p.Z);
            int i = Convert.ToInt32(MathF.Floor(p.X));
            int j = Convert.ToInt32(MathF.Floor(p.Y));
            int k = Convert.ToInt32(MathF.Floor(p.Z));
            var c = new Vector3[2, 2, 2];
            for (int di = 0; di < 2; di++)
            {
                for (int dj = 0; dj < 2; dj++)
                {
                    for (int dk = 0; dk < 2; dk++)
                    {
                        c[di, dj, dk] = s_ranVector[s_permX[(i + di) & 255] ^ s_permY[(j + dj) & 255] ^ s_permZ[(k + dk) & 255]];
                    }
                }
            }

            return PerlinInterpolate(c, u, v, w);
        }

        public float Turbulence(Vector3 p, int depth = 7)
        {
            float accum = 0.0f;
            Vector3 tempP = p;
            float weight = 1.0f;
            for (int i = 0; i < depth; i++)
            {
                accum += weight * Noise(tempP);
                weight *= 0.5f;
                tempP *= 2.0f;
            }

            return MathF.Abs(accum);
        }

        private float PerlinInterpolate(Vector3[,,] c, float u, float v, float w)
        {
            float uu = u * u * (3.0f - (2.0f * u));
            float vv = v * v * (3.0f - (2.0f * v));
            float ww = w * w * (3.0f - (2.0f * w));
            float accum = 0.0f;
            for (int i = 0; i < 2; i++)
            {
                float dubi = Convert.ToSingle(i);
                for (int j = 0; j < 2; j++)
                {
                    float dubj = Convert.ToSingle(j);
                    for (int k = 0; k < 2; k++)
                    {
                        float dubk = Convert.ToSingle(k);
                        Vector3 weightVec = new Vector3(u - dubi, v - dubj, w - dubk);
                        accum += ((dubi * uu) + ((1.0f - dubi) * (1.0f - uu))) *
                                 ((dubj * vv) + ((1.0f - dubj) * (1.0f - vv))) *
                                 ((dubk * ww) + ((1.0f - dubk) * (1.0f - ww))) *
                                 Vector3.Dot(c[i, j, k], weightVec);
                    }
                }
            }

            return accum;
        }

        private static Vector3[] PerlinGenerate()
        {
            var p = new Vector3[256];
            for (int i = 0; i < 256; i++)
            {
                p[i] = new Vector3(
                  -1.0f + (2.0f * RandomService.Nextfloat()),
                  -1.0f + (2.0f * RandomService.Nextfloat()),
                  -1.0f + (2.0f * RandomService.Nextfloat())).ToUnitVector();
            }

            return p;
        }

        private static int[] PerlinGenratePerm()
        {
            var p = new int[256];
            for (int i = 0; i < 256; ++i)
            {
                p[i] = i;
            }

            Permute(p);
            return p;
        }

        private static void Permute(int[] p)
        {
            for (int i = p.Length - 1; i > 0; i--)
            {
                int target = Convert.ToInt32(RandomService.Nextfloat() * Convert.ToSingle(i + 1));
                int tmp = p[i];
                p[i] = p[target];
                p[target] = tmp;
            }
        }
    }
}
