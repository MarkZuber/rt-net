// -----------------------------------------------------------------------
// <copyright file="BetterCameraGenerator.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using Zube.ImGuiNet;
using WkndRay.Materials;
using WkndRay.Textures;

namespace WkndRay.Executors
{
  public class BetterCameraGenerator : IExecutor
  {
    private readonly int _numSamples;

    public BetterCameraGenerator(int numSamples)
    {
      _numSamples = numSamples;
    }

    public PixelBuffer Execute(uint width, uint height)
    {
      var pixelBuffer = new PixelBuffer(width, height);
      float aperture = 2.0f;
      var lookFrom = new Vector3(3.0f, 3.0f, 2.0f);
      var lookAt = new Vector3(0.0f, 0.0f, -1.0f);
      float distanceToFocus = (lookFrom - lookAt).Length(); // .Magnitude();
      var camera = new Camera(
          lookFrom,
          lookAt,
          new Vector3(0.0f, 1.0f, 0.0f),
          30.0f,
          Convert.ToSingle(width) / Convert.ToSingle(height),
          aperture,
          distanceToFocus);

      var hitables = new HitableList
            {
                new Sphere(new Vector3(0.0f, 0.0f, -1.0f), 0.5f, new LambertianMaterial(new ColorTexture(0.1f, 0.2f, 0.5f))),
                new Sphere(new Vector3(0.0f, -100.5f, -1.0f), 100.0f, new LambertianMaterial(new ColorTexture(0.8f, 0.8f, 0.0f))),
                new Sphere(new Vector3(1.0f, 0.0f, -1.0f), 0.5f, new MetalMaterial(new Vector4(0.8f, 0.6f, 0.2f, 1.0f), 0.3f)),
                new Sphere(new Vector3(-1.0f, 0.0f, -1.0f), 0.5f, new DialectricMaterial(1.5f)),
                new Sphere(new Vector3(-1.0f, 0.0f, -1.0f), -0.45f, new DialectricMaterial(1.5f)),
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
            float u = Convert.ToSingle(i + RandomService.NextSingle()) / Convert.ToSingle(width);
            float v = Convert.ToSingle(j + RandomService.NextSingle()) / Convert.ToSingle(height);
            var r = camera.GetRay(u, v);

            color += GetRayColor(r, world, 0);
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

    private Vector4 GetRayColor(Ray ray, IHitable world, int depth)
    {
      // the 0.001 corrects for the "shadow acne"
      HitRecord hr = new HitRecord();
      if (world.Hit(ray, 0.01f, float.MaxValue, ref hr))
      {
        if (depth < 50 && hr.Material != null)
        {
          var scatterResult = hr.Material.Scatter(ray, hr);
          if (scatterResult.IsScattered && scatterResult.SpecularRay != null)
          {
            return scatterResult.Attenuation * GetRayColor(scatterResult.SpecularRay, world, depth + 1);
          }
        }

        return Vector4.Zero;
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
