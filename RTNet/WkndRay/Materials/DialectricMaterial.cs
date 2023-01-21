// -----------------------------------------------------------------------
// <copyright file="DialectricMaterial.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;

namespace WkndRay.Materials
{
  public class DialectricMaterial : AbstractMaterial
  {
    public DialectricMaterial(float refractionIndex)
    {
      RefractionIndex = refractionIndex;
    }

    public float RefractionIndex { get; }

    /// <inheritdoc />
    public override ScatterResult Scatter(Ray rayIn, HitRecord hitRecord)
    {
      var reflected = rayIn.Direction.Reflect(hitRecord.Normal);
      var attenuation = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
      float niOverNt;
      Vector3 outwardNormal;
      float cosine;
      if (Vector3.Dot(rayIn.Direction, hitRecord.Normal) > 0.0f)
      {
        outwardNormal = -hitRecord.Normal;
        niOverNt = RefractionIndex;
        cosine = RefractionIndex * Vector3.Dot(rayIn.Direction, hitRecord.Normal) / rayIn.Direction.Length();
      }
      else
      {
        outwardNormal = hitRecord.Normal;
        niOverNt = 1.0f / RefractionIndex;
        cosine = -Vector3.Dot(rayIn.Direction, hitRecord.Normal) / rayIn.Direction.Length();
      }

      float reflectProbability;
      Ray scattered;
      var refracted = rayIn.Direction.Refract(outwardNormal, niOverNt);
      if (refracted != Vector3.Zero)
      {
        reflectProbability = CalculateSchlickApproximation(cosine, RefractionIndex);
      }
      else
      {
        scattered = new Ray(hitRecord.P, reflected);
        reflectProbability = 1.0f;
      }

      if (RandomService.NextSingle() < reflectProbability)
      {
        scattered = new Ray(hitRecord.P, reflected);
      }
      else
      {
        scattered = new Ray(hitRecord.P, refracted);
      }

      return new ScatterResult(true, attenuation, scattered, null);
    }

    private float CalculateSchlickApproximation(float cosine, float refractionIndex)
    {
      float r0 = (1.0f - refractionIndex) / (1.0f + refractionIndex);
      r0 *= r0;
      return r0 + ((1.0f - r0) * MathF.Pow(1.0f - cosine, 5.0f));
    }
  }
}
