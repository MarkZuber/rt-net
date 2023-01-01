using System.Diagnostics;
using System.Numerics;
using ImGuiNET;
using RTNet.ImgCore;
using Veldrid;
using Veldrid.ImageSharp;
using WkndRay;
using WkndRay.Scenes;

namespace WkndRay
{
  public enum RenderingState
  {
    Stopped,
    Running,
    Stopping
  }

  public class WkndRayAppLayer : IAppLayer
  {
    private UInt32 _viewportWidth;
    private UInt32 _viewportHeight;
    private double _lastRenderTime;
    private Renderer _renderer;
    private CancellationTokenSource _rendererCancellationTokenSource = new CancellationTokenSource();
    private IScene _scene;
    private PixelBuffer? _pixelBuffer;
    private int _desiredImageWidth = 600;
    private int _desiredImageHeight = 600;
    private int _desiredRayDepth = 50;
    private int _desiredSamples = 50;
    private bool _desiredTwoPhase = true;
    private RenderingState _renderingState = RenderingState.Stopped;
    private Task _renderTask = Task.CompletedTask;

    public WkndRayAppLayer()
    {
      _renderer = new Renderer();
      // _scene = new CornellBoxScene();
      _scene = new ManySpheresScene();
    }

    private void Render()
    {
      var start = DateTime.UtcNow;

      int numThreads = Environment.ProcessorCount;

      var renderConfig = new RenderConfig(numThreads, _desiredRayDepth, _desiredSamples)
      {
        TwoPhase = _desiredTwoPhase
      };

      _pixelBuffer = new PixelBuffer(Convert.ToUInt32(_desiredImageWidth), Convert.ToUInt32(_desiredImageHeight), true);

      // TODO: the cancel operation isn't immediate.  We need a way to wait for the renderer to exit
      // before proceeding.

      _rendererCancellationTokenSource.Dispose();
      _rendererCancellationTokenSource = new CancellationTokenSource();
      _renderer = new Renderer();

      _renderTask = Task.Run(() =>
      {
        var rendererData = _renderer.Render(_rendererCancellationTokenSource.Token, _pixelBuffer!, _scene, renderConfig);
        _pixelBuffer.SaveToFileAsPng("/Users/zube/trialrun.png");
      });
      var end = DateTime.UtcNow;
      _lastRenderTime = (end - start).TotalMilliseconds;
    }

    private void StopRenderer()
    {
      _rendererCancellationTokenSource.Cancel();
    }

    public void OnUIRender()
    {
      ImGui.Begin("Settings");
      ImGui.Text(String.Format("Render: {0,3}ms", _renderer.ElapsedMilliseconds));

      switch (_renderingState)
      {
        case RenderingState.Stopped:
          if (ImGui.Button("Render"))
          {
            _renderingState = RenderingState.Running;
            Render();
          }
          break;
        case RenderingState.Stopping:
          ImGui.Text("Stopping...");
          if (_renderTask.IsCompleted)
          {
            _renderingState = RenderingState.Stopped;
          }
          break;
        case RenderingState.Running:
          if (ImGui.Button("Stop"))
          {
            _renderingState = RenderingState.Stopping;
            StopRenderer();
          }
          if (_renderTask.IsCompleted)
          {
            _renderingState = RenderingState.Stopped;
          }
          break;
      }

      ImGui.DragInt("Image Width", ref _desiredImageWidth, 5.0f, 25, 1000);
      ImGui.DragInt("Image Height", ref _desiredImageHeight, 5.0f, 25, 1000);
      ImGui.DragInt("Ray Depth", ref _desiredRayDepth, 5.0f, 1, 100);
      ImGui.DragInt("Samples", ref _desiredSamples, 5.0f, 1, 10000);
      ImGui.Checkbox("Two Phase", ref _desiredTwoPhase);
      ImGui.End();

      ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0.0f);

      ImGui.Begin("Viewport");

      _viewportWidth = (UInt32)ImGui.GetContentRegionAvail().X;
      _viewportHeight = (UInt32)ImGui.GetContentRegionAvail().Y;

      if (_pixelBuffer != null)
      {
        var finalImgPtr = _pixelBuffer.CaptureImageBufferPointer();
        if (finalImgPtr != IntPtr.Zero)
        {
          ImGui.Image(finalImgPtr, new Vector2(_pixelBuffer.Width, _pixelBuffer.Height));
        }
      }

      ImGui.End();
      // Not sure why this crashes the app...
      // ImGui.PopStyleVar();

      float framerate = ImGui.GetIO().Framerate;
      ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");
    }

    public void OnUpdate(float ts)
    {
    }
  }
}