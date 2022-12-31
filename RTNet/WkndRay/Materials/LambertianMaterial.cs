// -----------------------------------------------------------------------
// <copyright file="LambertianMaterial.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using RTNet.ImgCore;
using WkndRay.Pdfs;
using WkndRay.Textures;

namespace WkndRay.Materials
{
  public class LambertianMaterial : AbstractMaterial
  {
    public LambertianMaterial(ITexture albedo)
    {
      Albedo = albedo;
    }

    public ITexture Albedo { get; }

    /// <inheritdoc />
    public override ScatterResult Scatter(Ray rayIn, HitRecord hitRecord)
    {
      //var uvw = OrthoNormalBase.FromW(hitRecord.Normal);
      //Vector3 direction = uvw.Local(RandomService.GetRandomCosineDirection());

      //var scatteredRay = new Ray(hitRecord.P, direction.ToUnitVector());
      var attenuation = Albedo.GetValue(hitRecord.UvCoords, hitRecord.P);
      //var pdf = uvw.W.Dot(scatteredRay.Direction) / MathF.PI;
      return new ScatterResult(true, attenuation, null, new CosinePdf(hitRecord.Normal));
    }

    public override float ScatteringPdf(Ray rayIn, HitRecord hitRecord, Ray scattered)
    {
      float cosine = Vector3.Dot(hitRecord.Normal, scattered.Direction.ToUnitVector());
      if (cosine < 0.0f)
      {
        cosine = 0.0f;
      }

      return cosine / MathF.PI;
    }
  }
}
