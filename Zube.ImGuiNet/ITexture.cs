// -----------------------------------------------------------------------
// <copyright file="ImageTexture.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;

namespace Zube.ImGuiNet
{
  public interface ITexture
  {
    Vector4 GetValue(Vector2 uvCoords, Vector3 p);
  }
}
