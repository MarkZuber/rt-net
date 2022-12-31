// -----------------------------------------------------------------------
// <copyright file="IScene.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;

namespace WkndRay.Scenes
{
  public interface IScene
  {
    Camera GetCamera(uint imageWidth, uint imageHeight);
    IHitable GetWorld();
    IHitable GetLightHitable();
    Func<Ray, Vector4> GetBackgroundFunc();
  }
}