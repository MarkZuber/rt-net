// -----------------------------------------------------------------------
// <copyright file="Renderer.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using RTNet.ImgCore;
using WkndRay.Scenes;

namespace WkndRay
{
  public class PerPixelRenderer : IRenderer
  {
    // public event EventHandler<RenderProgressEventArgs> Progress;

    public RendererData Render(IPixelBuffer pixelArray, IScene scene, RenderConfig renderConfig)
    {
      return Render(
          pixelArray,
          scene.GetCamera(pixelArray.Width, pixelArray.Height),
          scene.GetWorld(),
          scene.GetLightHitable(),
          renderConfig,
          scene.GetBackgroundFunc());
    }

    public RendererData Render(
        IPixelBuffer pixelArray,
        Camera camera,
        IHitable world,
        IHitable lightHitable,
        RenderConfig renderConfig,
        Func<Ray, Vector4> backgroundFunc)
    {
      // Progress?.Invoke(this, new RenderProgressEventArgs(0.0f));

      if (renderConfig.TwoPhase)
      {
        // do a much lighter weight first pass first.
        RenderMultiThreaded(
          pixelArray,
          camera,
          world,
          lightHitable,
          new RenderConfig(renderConfig.NumThreads, 5, 1),
          backgroundFunc);
      }

      return RenderMultiThreaded(pixelArray, camera, world, lightHitable, renderConfig, backgroundFunc);
    }

    private RendererData RenderMultiThreaded(
        IPixelBuffer pixelArray,
        Camera camera,
        IHitable world,
        IHitable lightHitable,
        RenderConfig renderConfig,
        Func<Ray, Vector4> backgroundFunc)
    {
      var rendererData = new RendererData(pixelArray.Width, pixelArray.Height);

      var rayTracer = new RayTracer(camera, world, lightHitable, renderConfig, pixelArray.Width, pixelArray.Height, backgroundFunc);
      ThreadPool.SetMinThreads(renderConfig.NumThreads * 3, renderConfig.NumThreads * 3);

      var queueDataAvailableEvent = new AutoResetEvent(false);
      var inputQueue = new ConcurrentQueue<RenderInput>();
      var resultQueue = new ConcurrentQueue<RenderResult>();

      for (uint y = pixelArray.Height - 1; y >= 0; y--)
      {
        for (uint x = 0; x < pixelArray.Width; x++)
        {
          inputQueue.Enqueue(new RenderInput(x, y));
        }
      }

      var tasks = new List<Task>();

      try
      {
        for (var thid = 0; thid < renderConfig.NumThreads; thid++)
        {
          Console.WriteLine($"Creating task for thread id: {thid}");
          tasks.Add(
            Task.Run(() => RenderFunc(rayTracer, inputQueue, resultQueue, queueDataAvailableEvent)));
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
      ConcurrentQueue<RenderInput> inputQueue,
      ConcurrentQueue<RenderResult> resultQueue,
      AutoResetEvent queueDataAvailableEvent)
    {
      try
      {
        while (inputQueue.TryDequeue(out RenderInput? input))
        {
          var pixelData = rayTracer.GetPixelColor(input.X, input.Y);
          resultQueue.Enqueue(new RenderResult(input.X, input.Y, pixelData));
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
      ConcurrentQueue<RenderResult> resultQueue,
      AutoResetEvent queueDataAvailableEvent)
    {
      try
      {
        var incompletePixels = new HashSet<Tuple<uint, uint>>();
        for (uint y = 0; y < pixelBuffer.Height; y++)
        {
          for (uint x = 0; x < pixelBuffer.Width; x++)
          {
            incompletePixels.Add(Tuple.Create(x, y));
          }
        }

        var totalPixels = Convert.ToSingle(pixelBuffer.Height * pixelBuffer.Width);

        int previousPercent = 0;

        while (incompletePixels.Count > 0)
        {
          queueDataAvailableEvent.WaitOne(TimeSpan.FromMilliseconds(1000));

          while (resultQueue.TryDequeue(out var renderResult))
          {
            rendererData.SetPixelData(renderResult.PixelData);

            // assert pixelArray.Width == renderLineResult.Count
            pixelBuffer.SetPixel(renderResult.X, renderResult.Y, renderResult.PixelData.Color);
            incompletePixels.Remove(Tuple.Create(renderResult.X, renderResult.Y));

            var completePixels = Convert.ToSingle(totalPixels - incompletePixels.Count);
            var percentComplete = completePixels / totalPixels * 100.0f;
            int intPercent = Convert.ToInt32(percentComplete);
            if (intPercent > previousPercent)
            {
              previousPercent = intPercent;
              Console.WriteLine($"Percent Complete: {percentComplete:F}%");
              // Progress?.Invoke(this, new RenderProgressEventArgs(percentComplete));
            }
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    private class RenderInput
    {
      public RenderInput(uint x, uint y)
      {
        X = x;
        Y = y;
      }

      public uint X { get; }
      public uint Y { get; }
    }

    private class RenderResult
    {
      public RenderResult(uint x, uint y, PixelData pixelData)
      {
        X = x;
        Y = y;
        PixelData = pixelData;
      }

      public uint X { get; }
      public uint Y { get; }
      public PixelData PixelData { get; }
    }
  }
}
