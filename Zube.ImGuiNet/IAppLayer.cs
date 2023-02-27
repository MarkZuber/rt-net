// -----------------------------------------------------------------------
// <copyright file="ImageTexture.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

namespace Zube.ImGuiNet
{
  public interface IAppLayer
  {
    void OnUpdate(float ts);
    void OnUIRender();
  }
}