// -----------------------------------------------------------------------
// <copyright file="ManySpheresGenerator.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using RTNet.ImgCore;
using WkndRay.Materials;
using WkndRay.Textures;

namespace WkndRay.Executors
{
  public class ManySpheresGenerator : IExecutor
  {
    private readonly int _numSamples;

    public ManySpheresGenerator(int numSamples)
    {
      _numSamples = numSamples;
    }

    public ImageBuffer Execute(uint width, uint height)
    {
      var pixelBuffer = new ImageBuffer(width, height);
      float aperture = 0.01f;
      var lookFrom = new Vector3(24.0f, 2.0f, 6.0f);
      var lookAt = Vector3.UnitY;
      float distanceToFocus = (lookFrom - lookAt).Length();
      var camera = new Camera(
        lookFrom,
        lookAt,
        Vector3.UnitY,
        15.0f,
        Convert.ToSingle(width) / Convert.ToSingle(height),
        aperture,
        distanceToFocus);

      var world = CreateRandomScene();

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

    private IHitable CreateRandomScene()
    {
      var list = new HitableList
            {
                new Sphere(new Vector3(0.0f, -1000.0f, 0.0f), 1000.0f, new LambertianMaterial(new ColorTexture(0.5f, 0.5f, 0.5f)))
            };
      for (int a = -11; a < 11; a++)
      {
        for (int b = -11; b < 11; b++)
        {
          float chooseMat = RandomService.Nextfloat();
          var center = new Vector3(
            Convert.ToSingle(a) * RandomService.Nextfloat(),
            0.2f,
            Convert.ToSingle(b) + (0.9f * RandomService.Nextfloat()));
          if ((center - new Vector3(4.0f, 0.2f, 0.0f)).Length() > 0.9)
          {
            if (chooseMat < 0.8)
            {
              // diffuse
              list.Add(
                new Sphere(
                  center,
                  0.2f,
                  new LambertianMaterial(
                    new ColorTexture(
                      RandomService.Nextfloat() * RandomService.Nextfloat(),
                      RandomService.Nextfloat() * RandomService.Nextfloat(),
                      RandomService.Nextfloat() * RandomService.Nextfloat()))));
            }
            else if (chooseMat < 0.95)
            {
              // metal
              list.Add(
                new Sphere(
                  center,
                  0.2f,
                  new MetalMaterial(
                    new Vector4(
                      0.5f * (1.0f + RandomService.Nextfloat()),
                      0.5f * (1.0f + RandomService.Nextfloat()),
                      0.5f * (1.0f + RandomService.Nextfloat()),
                      1.0f),
                    0.5f * RandomService.Nextfloat())));
            }
            else
            {
              // glass
              list.Add(new Sphere(center, 0.2f, new DialectricMaterial(1.5f)));
            }
          }
        }
      }

      list.Add(new Sphere(new Vector3(0.0f, 1.0f, 0.0f), 1.0f, new DialectricMaterial(1.5f)));
      list.Add(new Sphere(new Vector3(-4.0f, 1.0f, 0.0f), 1.0f, new LambertianMaterial(new ColorTexture(0.4f, 0.2f, 0.1f))));
      list.Add(new Sphere(new Vector3(4.0f, 1.0f, 0.0f), 1.0f, new MetalMaterial(new Vector4(0.7f, 0.6f, 0.5f, 1.0f), 0.0f)));

      return list;
    }

    private float GetRandom()
    {
      return RandomService.Nextfloat();
    }

    private Vector4 GetRayColor(Ray ray, IHitable world, int depth)
    {
      // the 0.001 corrects for the "shadow acne"
      HitRecord hr = world.Hit(ray, 0.001f, float.MaxValue);
      if (hr != null)
      {
        if (depth < 50)
        {
          var scatterResult = hr.Material.Scatter(ray, hr);
          if (scatterResult.IsScattered)
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
