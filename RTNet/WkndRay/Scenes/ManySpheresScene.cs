// -----------------------------------------------------------------------
// <copyright file="ManySpheresScene.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using WkndRay.Materials;
using WkndRay.Textures;

namespace WkndRay.Scenes
{
  public class ManySpheresScene : IScene
  {
    public Camera GetCamera(uint imageWidth, uint imageHeight)
    {
      float aperture = 0.01f;
      var lookFrom = new Vector3(24.0f, 2.0f, 6.0f);
      var lookAt = Vector3.UnitY;
      float distanceToFocus = (lookFrom - lookAt).Length();
      return new Camera(
        lookFrom,
        lookAt,
        Vector3.UnitY,
        15.0f,
        Convert.ToSingle(imageWidth) / Convert.ToSingle(imageHeight),
        aperture,
        distanceToFocus);
    }

    public IHitable GetWorld()
    {
      var list = new HitableList();

      var checkerTexture = new CheckerTexture(
        new ColorTexture(0.2f, 0.3f, 0.1f),
        new ColorTexture(0.9f, 0.9f, 0.9f),
        Vector3.One * 10.0f);

      // original color of large sphere...
      // var colorTexture = new ColorTexture(0.5f, 0.5f, 0.5f);

      list.Add(new Sphere(new Vector3(0.0f, -1000.0f, 0.0f), 1000.0f, new LambertianMaterial(checkerTexture)));
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

      return new BvhNode(list, 0.0f, 1.0f);
    }

    /// <inheritdoc />
    public IHitable GetLightHitable()
    {
      return new HitableList();
    }

    /// <inheritdoc />
    public Func<Ray, Vector4> GetBackgroundFunc()
    {
      return ray =>
      {
        return new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
        //var unitDirection = ray.Direction.ToUnitVector();
        //float t = 0.5f * (unitDirection.Y + 1.0f);
        //return ((1.0f - t) * Vector4.One) + (t * new Vector4(0.5f, 0.7f, 1.0f));
      };
    }
  }
}
