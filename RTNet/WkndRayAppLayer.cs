using System.Numerics;
using ImGuiNET;
using RTNet.ImgCore;
using Veldrid;
using Veldrid.ImageSharp;
using WkndRay;
using WkndRay.Scenes;

namespace RTNet
{
  public class WkndRayAppLayer : IAppLayer
  {
    private UInt32 _viewportWidth;
    private UInt32 _viewportHeight;
    private double _lastRenderTime;
    private IRenderer _renderer;
    private IScene _scene;
    private ImageBuffer? _imageBuffer;

    private bool _renderStarted;

    public WkndRayAppLayer()
    {
      _renderer = new PerLineRenderer();
      _scene = new CornellBoxScene();
    }

    public void OnAttach()
    {
    }

    public void OnDetach()
    {
    }

    private void Render()
    {
      var start = DateTime.UtcNow;

      int numThreads = Environment.ProcessorCount;
      const int RayTraceDepth = 10;
      const int NumSamples = 50;

      var renderConfig = new RenderConfig(numThreads, RayTraceDepth, NumSamples)
      {
        TwoPhase = false
      };

      if (_imageBuffer == null)
      {
        _imageBuffer = new ImageBuffer(600, 600);
      }

      // _renderer.Progress += (_, args) => { Dispatcher.Invoke(() => { RenderProgress.Value = args.PercentComplete; }); };
      Task.Run(() =>
      {
        var rendererData = _renderer.Render(_imageBuffer!, _scene, renderConfig);
      });
      var end = DateTime.UtcNow;
      _lastRenderTime = (end - start).TotalMilliseconds;
    }

    public void OnUIRender()
    {
      ImGui.Begin("Settings");
      ImGui.Text(String.Format("Last Render: {0,3}ms", _lastRenderTime));
      if (ImGui.Button("Render"))
      {
        // Render();
      }
      ImGui.End();

      if (!_renderStarted)
      {
        Render();
        _renderStarted = true;
      }

      ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0.0f);

      ImGui.Begin("Viewport");

      _viewportWidth = (UInt32)ImGui.GetContentRegionAvail().X;
      _viewportHeight = (UInt32)ImGui.GetContentRegionAvail().Y;

      var finalImgPtr = _imageBuffer!.CaptureImageBufferPointer();
      if (finalImgPtr != IntPtr.Zero)
      {
        ImGui.Image(finalImgPtr, new Vector2(_imageBuffer.Width, _imageBuffer.Height));
      }

      ImGui.End();
      // Not sure why this crashes the app...
      // ImGui.PopStyleVar();

      float framerate = ImGui.GetIO().Framerate;
      ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");
    }

    public void OnUpdate(float ts)
    {
      //   if (_camera.OnUpdate(ts))
      //   {
      //     _renderer.ResetFrameIndex();
      //   }
    }
  }
}