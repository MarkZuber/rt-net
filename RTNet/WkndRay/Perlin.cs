// -----------------------------------------------------------------------
// <copyright file="Perlin.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;

namespace WkndRay
{
    public class Perlin
    {
        private static readonly float[] s_ranfloat;
        private static readonly int[] s_permX;
        private static readonly int[] s_permY;
        private static readonly int[] s_permZ;

        static Perlin()
        {
            s_ranfloat = PerlinGenerate();
            s_permX = PerlinGenratePerm();
            s_permY = PerlinGenratePerm();
            s_permZ = PerlinGenratePerm();
        }

        public float Noise(Vector3 p, bool interpolate)
        {
            float u = p.X - MathF.Floor(p.X);
            float v = p.Y - MathF.Floor(p.Y);
            float w = p.Z - MathF.Floor(p.Z);

            if (interpolate)
            {
                int i = Convert.ToInt32(MathF.Floor(p.X));
                int j = Convert.ToInt32(MathF.Floor(p.Y));
                int k = Convert.ToInt32(MathF.Floor(p.Z));
                // Hermite cubic to round off the interpolation
                // to reduce Mach bands.
                u = u * u * (3.0f - (2.0f * u));
                v = v * v * (3.0f - (2.0f * v));
                w = w * w * (3.0f - (2.0f * w));

                var o = new float[2, 2, 2];
                for (int di = 0; di < 2; di++)
                {
                    for (int dj = 0; dj < 2; dj++)
                    {
                        for (int dk = 0; dk < 2; dk++)
                        {
                            o[di, dj, dk] = s_ranfloat[s_permX[(i + di) & 255] ^ s_permY[(j + dj) & 255] ^ s_permZ[(k + dk) & 255]];
                        }
                    }
                }

                return TrilinearInterpolate(o, u, v, w);
            }
            else
            {
                int i = Convert.ToInt32(4.0f * p.X) & 255;
                int j = Convert.ToInt32(4.0f * p.Y) & 255;
                int k = Convert.ToInt32(4.0f * p.Z) & 255;
                return s_ranfloat[s_permX[i] ^ s_permY[j] ^ s_permZ[k]];
            }
        }

        private float TrilinearInterpolate(float[,,] o, float u, float v, float w)
        {
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
                        accum += ((dubi * u) + ((1.0f - dubi) * (1.0f - u))) *
                                 ((dubj * v) + ((1.0f - dubj) * (1.0f - v))) *
                                 ((dubk * w) + ((1.0f - dubk) * (1.0f - w))) * o[i, j, k];
                    }
                }
            }

            return accum;
        }

        private static float[] PerlinGenerate()
        {
            var p = new float[256];
            for (int i = 0; i < 256; ++i)
            {
                p[i] = RandomService.Nextfloat();
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
