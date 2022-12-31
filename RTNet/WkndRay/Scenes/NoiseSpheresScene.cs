// -----------------------------------------------------------------------
// <copyright file="NoiseSpheresScene.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using WkndRay.Materials;
using WkndRay.Textures;

namespace WkndRay.Scenes
{
  public class NoiseSpheresScene : IScene
  {
    /// <inheritdoc />
    public Camera GetCamera(uint imageWidth, uint imageHeight)
    {
      var lookFrom = new Vector3(13.0f, 2.0f, 3.0f);
      var lookAt = Vector3.Zero;
      float distToFocus = 10.0f;
      float aperture = 0.0f;
      return new Camera(
        lookFrom,
        lookAt,
        Vector3.UnitY,
        20.0f,
        Convert.ToSingle(imageWidth) / Convert.ToSingle(imageHeight),
        aperture,
        distToFocus);
    }

    /// <inheritdoc />
    public IHitable GetWorld()
    {
      //var perlinTexture = new NoiseTexture(true, 3.0f);
      var list = new HitableList()
            {
                new Sphere(
                    new Vector3(0.0f, -1000.0f, 0.0f),
                    1000.0f,
                    new LambertianMaterial(new VectorNoiseTexture(VectorNoiseMode.Soft, 3.0f))),
                new Sphere(
                    new Vector3(0.0f, 2.0f, 0.0f),
                    2.0f,
                    new LambertianMaterial(new VectorNoiseTexture(VectorNoiseMode.Marble, 3.0f)))
            };

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
        var unitDirection = ray.Direction.ToUnitVector();
        float t = 0.5f * (unitDirection.Y + 1.0f);
        return ((1.0f - t) * Vector4.One) + (t * new Vector4(0.5f, 0.7f, 1.0f, 1.0f));
      };
    }
  }
}
