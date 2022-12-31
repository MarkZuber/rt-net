// -----------------------------------------------------------------------
// <copyright file="MetalMaterial.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;

namespace WkndRay.Materials
{
  public class MetalMaterial : AbstractMaterial
  {
    public MetalMaterial(Vector4 albedo, float fuzz)
    {
      Albedo = albedo;
      Fuzz = fuzz;
    }

    public Vector4 Albedo { get; }
    public float Fuzz { get; }

    /// <inheritdoc />
    public override ScatterResult Scatter(Ray rayIn, HitRecord hitRecord)
    {
      var reflected = rayIn.Direction.ToUnitVector().Reflect(hitRecord.Normal);
      var specularRay = new Ray(hitRecord.P, reflected + (Fuzz * Vector3Extensions.GetRandomInUnitSphere()));
      var attenuation = Albedo;
      return new ScatterResult(true, attenuation, specularRay, null);
    }
  }
}
