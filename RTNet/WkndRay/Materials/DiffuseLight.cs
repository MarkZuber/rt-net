// -----------------------------------------------------------------------
// <copyright file="DiffuseLight.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;
using Zube.ImGuiNet;
using WkndRay.Textures;

namespace WkndRay.Materials
{
  public class DiffuseLight : AbstractMaterial
  {
    public DiffuseLight(ITexture texture)
    {
      Texture = texture;
    }

    public ITexture Texture { get; }

    public override ScatterResult Scatter(Ray rayIn, HitRecord hitRecord)
    {
      return ScatterResult.False();
    }

    public override Vector4 Emitted(Ray rayIn, HitRecord hitRecord, Vector2 uvCoords, Vector3 p)
    {
      return Vector3.Dot(hitRecord.Normal, rayIn.Direction) < 0.0f ? Texture.GetValue(uvCoords, p) : Texture.GetValue(uvCoords, p);
    }
  }
}
