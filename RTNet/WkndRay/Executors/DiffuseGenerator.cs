// -----------------------------------------------------------------------
// <copyright file="DiffuseGenerator.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using RTNet.ImgCore;

namespace WkndRay.Executors
{
  public class DiffuseGenerator : IExecutor
  {
    private readonly int _numSamples;

    public DiffuseGenerator(int numSamples)
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
            float u = Convert.ToSingle(i + RandomService.Nextfloat()) / Convert.ToSingle(width);
            float v = Convert.ToSingle(j + RandomService.Nextfloat()) / Convert.ToSingle(height);
            var r = camera.GetRay(u, v);

            color += GetRayColor(r, world);
          }

          color /= Convert.ToSingle(_numSamples);
          color = color.ApplyGamma2();

          pixelBuffer.SetPixel(i, j, color);
        }

        Console.Write(".");
      }

      Console.WriteLine();
      return pixelBuffer;
    }

    private Vector4 GetRayColor(Ray ray, IHitable world)
    {
      // the 0.001 corrects for the "shadow acne"
      HitRecord hr = world.Hit(ray, 0.001f, float.MaxValue);
      if (hr != null)
      {
        var target = hr.P + hr.Normal + Vector3Extensions.GetRandomInUnitSphere();
        return 0.5f * GetRayColor(new Ray(hr.P, target - hr.P), world);
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
