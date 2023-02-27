// -----------------------------------------------------------------------
// <copyright file="IsotropicMaterial.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;
using Zube.ImGuiNet;
using WkndRay.Textures;

namespace WkndRay.Materials
{
  public class IsotropicMaterial : AbstractMaterial
  {
    public IsotropicMaterial(ITexture albedo)
    {
      Albedo = albedo;
    }

    public ITexture Albedo { get; }

    /// <inheritdoc />
    public override ScatterResult Scatter(Ray rayIn, HitRecord hitRecord)
    {
      var scattered = new Ray(hitRecord.P, Vector3Extensions.GetRandomInUnitSphere());
      var attenuation = Albedo.GetValue(hitRecord.UvCoords, hitRecord.P);
      return new ScatterResult(true, attenuation, scattered, null);
    }
  }
}
