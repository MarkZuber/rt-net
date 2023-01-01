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
  public class CornellBoxScene : IScene
  {
    private readonly IHitable _light;
    private readonly IHitable _glassSphere;

    public CornellBoxScene()
    {
      // var light = new DiffuseLight(new ColorTexture(15.0f, 15.0f, 15.0f));
      var light = new DiffuseLight(new ColorTexture(1.0f, 1.0f, 1.0f));
      var glass = new DialectricMaterial(1.5f);

      var red = new LambertianMaterial(new ColorTexture(0.65f, 0.05f, 0.05f));

      // _light = new XzRect(213.0f, 343.0f, 227.0f, 332.0f, 554.0f, light);
      _light = new XzRect(113.0f, 443.0f, 127.0f, 432.0f, 554.0f, light);
      _glassSphere = new Sphere(new Vector3(190.0f, 90.0f, 190.0f), 90.0f, glass);
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
      var blue = new LambertianMaterial(new ColorTexture(0.12f, 0.12f, 0.45f));
      // var aluminum = new MetalMaterial(new Vector4(0.8f, 0.85f, 0.88f, 255.0f), 0.0f);

      var list = new HitableList()
            {
                new FlipNormals(new YzRect(0.0f, 555.0f, 0.0f, 555.0f, 555.0f, green)),
                new YzRect(0.0f, 555.0f, 0.0f, 555.0f, 0.0f, white),
                new FlipNormals(_light),
                new FlipNormals(new XzRect(0.0f, 555.0f, 0.0f, 555.0f, 555.0f, white)),
                new XzRect(0.0f, 555.0f, 0.0f, 555.0f, 0.0f, red),
                new FlipNormals(new XyRect(0.0f, 555.0f, 0.0f, 555.0f, 555.0f, blue)),

                // // new Translate(new RotateY(new Box(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(165.0f, 165.0f, 165.0f), white), -18.0f), new Vector3(130.0f, 0.0f, 65.0f)), 
                new Translate(new RotateY(new Box(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(165.0f, 330.0f, 165.0f), white), 15.0f), new Vector3(265.0f, 0.0f, 295.0f)),
                _glassSphere,
            };

      return new BvhNode(list, 0.0f, 1.0f);
    }

    /// <inheritdoc />
    public IHitable GetLightHitable()
    {
      return new HitableList
            {
                _light,
                _glassSphere
            };
    }

    /// <inheritdoc />
    public Func<Ray, Vector4> GetBackgroundFunc()
    {
      return ray => new Vector4(0.12f, 0.34f, 0.56f, 1.0f); // Vector4.1; // * 0.1;
    }
  }
}
