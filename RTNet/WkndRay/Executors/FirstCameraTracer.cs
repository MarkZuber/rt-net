// -----------------------------------------------------------------------
// <copyright file="FirstCameraTracer.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using RTNet.ImgCore;

namespace WkndRay.Executors
{
  public class FirstCameraTracer : IExecutor
  {
    private readonly int _numSamples;
    private readonly Random _random = new Random();

    public FirstCameraTracer(int numSamples)
    {
      _numSamples = numSamples;
    }

    public PixelBuffer Execute(uint width, uint height)
    {
      var pixelBuffer = new PixelBuffer(width, height);
      var lowerLeftCorner = new Vector3(-2.0f, -1.0f, -1.0f);
      var horizontal = new Vector3(4.0f, 0.0f, 0.0f);
      var vertical = new Vector3(0.0f, 2.0f, 0.0f);
      var origin = Vector3.Zero;

      var camera = new BasicCamera(origin, lowerLeftCorner, horizontal, vertical);

      var hitables = new HitableList
            {
                new Sphere(new Vector3(0.0f, 0.0f, -1.0f), 0.5f),
                new Sphere(new Vector3(0.0f, -100.5f, -1.0f), 100.0f)
            };

      var world = new HitableList
            {
                hitables
            };

      for (uint j = height - 1; j >= 0; j--)
      {
        for (uint i = 0; i < width; i++)
        {
          Vector4 color = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
          for (int sample = 0; sample < _numSamples; sample++)
          {
            float u = Convert.ToSingle(i + GetRandom()) / Convert.ToSingle(width);
            float v = Convert.ToSingle(j + GetRandom()) / Convert.ToSingle(height);
            var r = camera.GetRay(u, v);

            color += GetRayColor(r, world);
          }

          color /= Convert.ToSingle(_numSamples);

          pixelBuffer.SetPixel(i, j, color);
        }

        Console.Write(".");
      }

      Console.WriteLine();
      return pixelBuffer;
    }

    private float GetRandom()
    {
      return Convert.ToSingle(_random.NextDouble());
    }

    private Vector4 GetRayColor(Ray ray, IHitable world)
    {
      HitRecord? hr = world.Hit(ray, 0.0f, float.MaxValue);
      if (hr != null)
      {
        return (0.5f * new Vector3(hr.Normal.X + 1.0f, hr.Normal.Y + 1.0f, hr.Normal.Z + 1.0f)).ToVector4();
      }
      else
      {
        var unitDirection = ray.Direction.ToUnitVector();
        float t = 0.5f * (unitDirection.Y + 1.0f);
        return (((1.0f - t) * Vector3.One) + (t * new Vector3(0.5f, 0.7f, 1.0f))).ToVector4();
      }
    }
  }
}
