// -----------------------------------------------------------------------
// <copyright file="ImageTextureScene.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using WkndRay.Hitables;
using WkndRay.Materials;
using WkndRay.Textures;

namespace WkndRay.Scenes
{
  public class CornellBoxWithSmokeScene : IScene
  {
    private readonly IHitable _light;

    public CornellBoxWithSmokeScene()
    {
      var light = new DiffuseLight(new ColorTexture(7.0f, 7.0f, 7.0f));
      _light = new XzRect(113.0f, 443.0f, 127.0f, 432.0f, 554.0f, light);
    }

    /// <inheritdoc />
    public Camera GetCamera(uint imageWidth, uint imageHeight)
    {
      var lookFrom = new Vector3(278.0f, 278.0f, -800.0f);
      var lookAt = new Vector3(278.0f, 278.0f, 0.0f);
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
      var red = new LambertianMaterial(new ColorTexture(0.65f, 0.05f, 0.05f));
      var white = new LambertianMaterial(new ColorTexture(0.73f, 0.73f, 0.73f));
      var green = new LambertianMaterial(new ColorTexture(0.12f, 0.45f, 0.15f));

      var b1 = new Translate(
        new RotateY(new Box(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(165.0f, 165.0f, 165.0f), white), -18.0f),
        new Vector3(130.0f, 0.0f, 65.0f));
      var b2 = new Translate(
        new RotateY(new Box(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(165.0f, 330.0f, 165.0f), white), 15.0f),
        new Vector3(265.0f, 0.0f, 295.0f));

      var list = new HitableList()
      {
        new FlipNormals(new YzRect(0.0f, 555.0f, 0.0f, 555.0f, 555.0f, green)),
        new YzRect(0.0f, 555.0f, 0.0f, 555.0f, 0.0f, red),
        new FlipNormals(_light),
        new FlipNormals(new XzRect(0.0f, 555.0f, 0.0f, 555.0f, 555.0f, white)),
        new XzRect(0.0f, 555.0f, 0.0f, 555.0f, 0.0f, white),
        new FlipNormals(new XyRect(0.0f, 555.0f, 0.0f, 555.0f, 555.0f, white)),

        new ConstantMedium(b1, 0.01f, new ColorTexture(1.0f, 1.0f, 1.0f)),
        new ConstantMedium(b2, 0.01f, new ColorTexture(0.0f, 0.0f, 0.0f))
      };

      return new BvhNode(list, 0.0f, 1.0f);
    }

    /// <inheritdoc />
    public IHitable GetLightHitable()
    {
      return _light;
    }

    /// <inheritdoc />
    public Func<Ray, Vector4> GetBackgroundFunc()
    {
      return ray => Vector4.Zero;
    }
  }
}
