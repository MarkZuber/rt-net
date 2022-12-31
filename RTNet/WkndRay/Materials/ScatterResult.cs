// -----------------------------------------------------------------------
// <copyright file="ScatterResult.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;
using WkndRay.Pdfs;

namespace WkndRay.Materials
{
  public class ScatterResult
  {
    public ScatterResult(bool isScattered, Vector4 attenuation, Ray specularRay, IPdf pdf)
    {
      IsScattered = isScattered;
      Attenuation = attenuation;
      SpecularRay = specularRay;
      Pdf = pdf;
    }

    public static ScatterResult False()
    {
      return new ScatterResult(false, Vector4.Zero, null, null);
    }

    public bool IsScattered { get; }
    public Ray SpecularRay { get; }
    public bool IsSpecular => SpecularRay != null;
    public Vector4 Attenuation { get; }
    public IPdf Pdf { get; }  // probability distribution function
  }
}
