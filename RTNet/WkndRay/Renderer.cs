// -----------------------------------------------------------------------
// <copyright file="Renderer.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using RTNet.ImgCore;
using WkndRay.Scenes;

namespace WkndRay
{
  public class Renderer : IRenderer
  {
    // public event EventHandler<RenderProgressEventArgs> Progress;

    public RendererData Render(IPixelBuffer pixelArray, IScene scene, RenderConfig renderConfig)
    {
      return Render(pixelArray, scene.GetCamera(pixelArray.Width, pixelArray.Height), scene.GetWorld(), scene.GetLightHitable(), renderConfig, scene.GetBackgroundFunc());
    }

    public RendererData Render(IPixelBuffer pixelArray, Camera camera, IHitable world, IHitable lightHitable, RenderConfig renderConfig, Func<Ray, Vector4> backgroundFunc)
    {
      // Progress?.Invoke(this, new RenderProgressEventArgs(0.0f));

      // two pass
      RenderMulti(pixelArray, camera, world, lightHitable, new RenderConfig(renderConfig.NumThreads, 5, 1), backgroundFunc);
      return RenderMulti(pixelArray, camera, world, lightHitable, renderConfig, backgroundFunc);
    }

    public RendererData RenderMulti(IPixelBuffer pixelArray, Camera camera, IHitable world, IHitable lightHitable, RenderConfig renderConfig, Func<Ray, Vector4> backgroundFunc)
    {
      var rendererData = new RendererData(pixelArray.Width, pixelArray.Height);
      var rayTracer = new RayTracer(camera, world, lightHitable, renderConfig, pixelArray.Width, pixelArray.Height, backgroundFunc);

      try
      {
        ThreadPool.SetMinThreads(16, 16);
        Parallel.ForEach(Enumerable.Range(0, Convert.ToInt32(pixelArray.Height)), y =>
        {
          for (uint x = 0; x < pixelArray.Width; x++)
            pixelArray.SetPixel(x, Convert.ToUInt32(y), rayTracer.GetPixelColor(x, Convert.ToUInt32(y)).Color);
        });

        // Other possible approaches.  The one above seems to work just fine.

        // Parallel.ForEach(Enumerable.Range(0, Convert.ToInt32(pixelArray.Height)), y =>
        // {
        //   var rowPixels = new Vector4[pixelArray.Width];
        //   for (uint x = 0; x < pixelArray.Width; x++)
        //   {
        //     rowPixels[x] = rayTracer.GetPixelColor(x, Convert.ToUInt32(y)).Color;
        //   }
        //   pixelArray.SetPixelRowColors(Convert.ToUInt32(y), rowPixels);
        // });

        // Parallel.ForEach(Enumerable.Range(0, Convert.ToInt32(pixelArray.Height)), y =>
        // {
        //   var rowPixels = new byte[pixelArray.Width * PixelBuffer.PixelFormatSize];
        //   for (uint x = 0; x < pixelArray.Width; x++)
        //   {
        //     var color = rayTracer.GetPixelColor(x, Convert.ToUInt32(y)).Color;
        //     var bytes = PixelBuffer.GetByteArrayFromVector4(color);
        //     int offset = Convert.ToInt32(x * PixelBuffer.PixelFormatSize);
        //     Array.Copy(bytes, 0, rowPixels, offset, bytes.Length);
        //   }
        //   pixelArray.SetPixelRowBytes(Convert.ToUInt32(y), rowPixels);
        // });
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
      }

      return rendererData;
    }

    public RendererData RenderSingleThreaded(IPixelBuffer pixelArray, Camera camera, IHitable world, IHitable lightHitable, RenderConfig renderConfig, Func<Ray, Vector4> backgroundFunc)
    {
      var rendererData = new RendererData(pixelArray.Width, pixelArray.Height);
      var rayTracer = new RayTracer(camera, world, lightHitable, renderConfig, pixelArray.Width, pixelArray.Height, backgroundFunc);

      for (uint y = 0; y < pixelArray.Height; y++)
      {
        for (uint x = 0; x < pixelArray.Width; x++)
        {
          pixelArray.SetPixel(x, y, rayTracer.GetPixelColor(x, y).Color);
        }
      }

      return rendererData;
    }
  }
}
