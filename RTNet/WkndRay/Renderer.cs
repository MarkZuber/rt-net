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
using Zube.ImGuiNet;
using WkndRay.Scenes;

namespace WkndRay
{
  public class Renderer
  {
    // public event EventHandler<RenderProgressEventArgs> Progress;

    private Stopwatch _renderStopwatch = new Stopwatch();

    public RendererData Render(CancellationToken cancellationToken, IPixelBuffer pixelArray, IScene scene, RenderConfig renderConfig)
    {
      return Render(cancellationToken, pixelArray, scene.GetCamera(pixelArray.Width, pixelArray.Height), scene.GetWorld(), scene.GetLightHitable(), renderConfig, scene.GetBackgroundFunc());
    }

    public long ElapsedMilliseconds => _renderStopwatch.ElapsedMilliseconds;


    public RendererData Render(CancellationToken cancellationToken, IPixelBuffer pixelArray, Camera camera, IHitable world, IHitable lightHitable, RenderConfig renderConfig, Func<Ray, Vector4> backgroundFunc)
    {
      // Progress?.Invoke(this, new RenderProgressEventArgs(0.0f));

      if (renderConfig.TwoPhase)
      {
        RenderMulti(cancellationToken, pixelArray, camera, world, lightHitable, new RenderConfig(renderConfig.NumThreads, 5, 1), backgroundFunc);
      }
      return RenderMulti(cancellationToken, pixelArray, camera, world, lightHitable, renderConfig, backgroundFunc);
    }

    public RendererData RenderMulti(CancellationToken cancellationToken, IPixelBuffer pixelArray, Camera camera, IHitable world, IHitable lightHitable, RenderConfig renderConfig, Func<Ray, Vector4> backgroundFunc)
    {
      _renderStopwatch.Reset();
      _renderStopwatch.Start();

      var parallelOptions = new ParallelOptions();
      parallelOptions.CancellationToken = cancellationToken;
      parallelOptions.MaxDegreeOfParallelism = Environment.ProcessorCount;
      var rendererData = new RendererData(pixelArray.Width, pixelArray.Height);
      var rayTracer = new RayTracer(camera, world, lightHitable, renderConfig, pixelArray.Width, pixelArray.Height, backgroundFunc);

      try
      {

        try
        {
          Parallel.ForEach(Enumerable.Range(0, Convert.ToInt32(pixelArray.Height)), parallelOptions, y =>
          {
            for (uint x = 0; x < pixelArray.Width; x++)
              pixelArray.SetPixel(x, Convert.ToUInt32(y), rayTracer.GetPixelColor(x, Convert.ToUInt32(y)).Color);
          });
        }
        catch (OperationCanceledException)
        {
        }

      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
      }
      finally
      {
        _renderStopwatch.Stop();
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
