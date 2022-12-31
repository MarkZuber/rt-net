using System;
using System.Numerics;
using RTNet.ImgCore;
using WkndRay.Scenes;

namespace WkndRay
{
  public interface IRenderer
  {
    event EventHandler<RenderProgressEventArgs> Progress;
    RendererData Render(IPixelBuffer pixelArray, IScene scene, RenderConfig renderConfig);
    RendererData Render(IPixelBuffer pixelArray, Camera camera, IHitable world, IHitable lightHitable, RenderConfig renderConfig, Func<Ray, Vector4> backgroundFunc);
  }
}
