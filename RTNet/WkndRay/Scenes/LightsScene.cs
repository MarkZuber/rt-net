// -----------------------------------------------------------------------
// <copyright file="ImageTextureScene.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using Zube.ImGuiNet;
using WkndRay.Hitables;
using WkndRay.Materials;
using WkndRay.Textures;

namespace WkndRay.Scenes
{
  public class LightsScene : IScene
  {
    private readonly string _globeImagePath;

    public LightsScene(string globeImagePath)
    {
      _globeImagePath = globeImagePath;
    }

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
        40.0f,
        Convert.ToSingle(imageWidth) / Convert.ToSingle(imageHeight),
        aperture,
        distToFocus);
    }

    /// <inheritdoc />
    public IHitable GetWorld()
    {
      var globe = ImageHelper.FromFile(_globeImagePath);

      var list = new HitableList()
      {
        new Sphere(
          new Vector3(0.0f, -1000.0f, 0.0f),
          1000.0f,
          new LambertianMaterial(new VectorNoiseTexture(VectorNoiseMode.Marble, 3.0f))),
        new Sphere(
          new Vector3(0.0f, 2.0f, 0.0f),
          2.0f,
          new LambertianMaterial(new ImageTexture(globe))),
        new Sphere(new Vector3(0.0f, 7.0f, 0.0f), 2.0f, new DiffuseLight(new ColorTexture(4.0f, 4.0f, 4.0f))),
        new XyRect(3.0f, 5.0f, 1.0f, 3.0f, -2.0f, new DiffuseLight(new ColorTexture(4.0f, 4.0f, 4.0f)))
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
      return ray => Vector4.One * 0.05f;
    }
  }
}
