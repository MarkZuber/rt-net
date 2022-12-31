// -----------------------------------------------------------------------
// <copyright file="Renderer.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using RTNet.ImgCore;
using WkndRay.Scenes;

namespace WkndRay
{
  public class PerLineRenderer : IRenderer
  {
    private Vector4[] _accumulationData = new Vector4[0];
    private UInt64 _frameIndex = 1;

    public event EventHandler<RenderProgressEventArgs> Progress;

    public RendererData Render(IPixelBuffer pixelArray, IScene scene, RenderConfig renderConfig)
    {
      return Render(pixelArray, scene.GetCamera(pixelArray.Width, pixelArray.Height), scene.GetWorld(), scene.GetLightHitable(), renderConfig, scene.GetBackgroundFunc());
    }

    public RendererData Render(IPixelBuffer pixelArray, Camera camera, IHitable world, IHitable lightHitable, RenderConfig renderConfig, Func<Ray, Vector4> backgroundFunc)
    {
      Progress?.Invoke(this, new RenderProgressEventArgs(0.0f));

      _accumulationData = new Vector4[pixelArray.Width * pixelArray.Height];

      return RenderSingleThreaded(pixelArray, camera, world, lightHitable, renderConfig, backgroundFunc);
    }

    public RendererData RenderMulti(IPixelBuffer pixelArray, Camera camera, IHitable world, IHitable lightHitable, RenderConfig renderConfig, Func<Ray, Vector4> backgroundFunc)
    {
      var rendererData = new RendererData(pixelArray.Width, pixelArray.Height);
      var rayTracer = new RayTracer(camera, world, lightHitable, renderConfig, pixelArray.Width, pixelArray.Height, backgroundFunc);

      ThreadPool.SetMinThreads(6, 6);

      var tasks = new List<Task>();

      for (uint y = 0; y < pixelArray.Height; y++)
      {
        tasks.Add(Task.Run(() =>
        {
          for (uint x = 0; x < pixelArray.Width; x++)
          {
            pixelArray.SetPixel(x, y, rayTracer.GetPixelColor(x, y).Color);
          }
        }));
      }

      Task.WaitAll(tasks.ToArray());

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
          // This doesn't work since the monte carlo renderer comes up with so many black colors
          // from not hitting a light source that the entire scene goes black.

          // var color = rayTracer.GetPixelColor(x, y).Color;
          // _accumulationData[y * pixelArray.Width + x] += color;
          // var accumulatedColor = _accumulationData[y * pixelArray.Width + x];
          // accumulatedColor /= (float)_frameIndex;
          // accumulatedColor.W = 1.0f;
          // pixelArray.SetPixel(x, y, accumulatedColor);

          pixelArray.SetPixel(x, y, rayTracer.GetPixelColor(x, y).Color);
        }
      }
      _frameIndex++;

      return rendererData;
    }

    private RendererData RenderMultiThreaded(IPixelBuffer pixelArray, Camera camera, IHitable world, IHitable lightHitable, RenderConfig renderConfig, Func<Ray, Vector4> backgroundFunc)
    {
      var rendererData = new RendererData(pixelArray.Width, pixelArray.Height);

      var rayTracer = new RayTracer(camera, world, lightHitable, renderConfig, pixelArray.Width, pixelArray.Height, backgroundFunc);
      ThreadPool.SetMinThreads(renderConfig.NumThreads * 3, renderConfig.NumThreads * 3);

      var queueDataAvailableEvent = new AutoResetEvent(false);
      var rowQueue = new ConcurrentQueue<uint>();
      var resultQueue = new ConcurrentQueue<RenderLineResult>();

      for (uint y = 0; y < pixelArray.Height; y++)
      {
        rowQueue.Enqueue(y);
      }

      var tasks = new List<Task>();

      try
      {
        for (var thid = 0; thid < renderConfig.NumThreads; thid++)
        {
          tasks.Add(
            Task.Run(() => RenderFunc(rayTracer, pixelArray.Width, rowQueue, resultQueue, queueDataAvailableEvent)));
        }

        tasks.Add(Task.Run(() => ResultFunc(pixelArray, rendererData, resultQueue, queueDataAvailableEvent)));

        Task.WaitAll(tasks.ToArray());
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }

      return rendererData;
    }

    private static void RenderFunc(
      IRayTracer rayTracer,
      uint pixelWidth,
      ConcurrentQueue<uint> rowQueue,
      ConcurrentQueue<RenderLineResult> resultQueue,
      AutoResetEvent queueDataAvailableEvent)
    {
      try
      {
        while (rowQueue.TryDequeue(out uint y))
        {
          var rowPixels = new List<PixelData>();
          for (uint x = 0; x < pixelWidth; x++)
          {
            rowPixels.Add(rayTracer.GetPixelColor(x, y));
          }

          resultQueue.Enqueue(new RenderLineResult(y, rowPixels));
          queueDataAvailableEvent.Set();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    private void ResultFunc(
      IPixelBuffer pixelBuffer,
      RendererData rendererData,
      ConcurrentQueue<RenderLineResult> resultQueue,
      AutoResetEvent queueDataAvailableEvent)
    {
      var incompleteRows = new HashSet<uint>();
      for (uint y = 0; y < pixelBuffer.Height; y++)
      {
        incompleteRows.Add(y);
      }

      while (incompleteRows.Count > 0)
      {
        queueDataAvailableEvent.WaitOne(TimeSpan.FromMilliseconds(1000));

        while (resultQueue.TryDequeue(out var renderLineResult))
        {
          foreach (var data in renderLineResult.RowPixels)
          {
            rendererData.SetPixelData(data);
          }
          // assert pixelArray.Width == renderLineResult.Count
          pixelBuffer.SetPixelRowColors(renderLineResult.Y, renderLineResult.RowPixels.Select(x => x.Color));
          incompleteRows.Remove(renderLineResult.Y);

          var totalRows = Convert.ToSingle(pixelBuffer.Height);
          var completeRows = Convert.ToSingle(pixelBuffer.Height - incompleteRows.Count);
          var percentComplete = completeRows / totalRows * 100.0f;
          Console.WriteLine($"Percent Complete: {percentComplete:F}%");
          Progress?.Invoke(this, new RenderProgressEventArgs(percentComplete));
        }
      }
    }

    private class RenderLineResult
    {
      public RenderLineResult(uint y, List<PixelData> rowPixels)
      {
        Y = y;
        RowPixels = rowPixels;
      }

      public List<PixelData> RowPixels { get; }
      public uint Y { get; }
    }
  }
}
