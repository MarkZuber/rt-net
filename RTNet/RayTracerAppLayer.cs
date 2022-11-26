using System.Numerics;
using ImGuiNET;
using RTNet.ImgCore;
using Veldrid;
using Veldrid.ImageSharp;

namespace RTNet
{
  public class RayTracerAppLayer : IAppLayer
  {
    private UInt32 _viewportWidth;
    private UInt32 _viewportHeight;
    private double _lastRenderTime;
    private Renderer _renderer;
    private Camera _camera;

    public RayTracerAppLayer()
    {
      _renderer = new Renderer(600, 600);
      _camera = new Camera(45.0f, 0.1f, 100.0f);
    }


    public void Initialize(ImGuiController controller)
    {
      _renderer.Initialize(controller);
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

      _renderer.Resize(_viewportWidth, _viewportHeight);
      // _camera.OnResize(_viewportWidth, _viewportHeight);

      _renderer.Render();

      var end = DateTime.UtcNow;
      _lastRenderTime = (end - start).TotalMilliseconds;
    }

    public void OnUIRender()
    {
      ImGui.Begin("Settings");
      ImGui.Text(String.Format("Last Render: {0,3}ms", _lastRenderTime));
      if (ImGui.Button("Render"))
      {
        Render();
      }
      ImGui.End();

      ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0.0f);

      ImGui.Begin("Viewport");

      _viewportWidth = (UInt32)ImGui.GetContentRegionAvail().X;
      _viewportHeight = (UInt32)ImGui.GetContentRegionAvail().Y;

      var finalImgPtr = _renderer.GetFinalImagePtr();
      if (finalImgPtr != IntPtr.Zero)
      {
        ImGui.Image(finalImgPtr, new Vector2(_renderer.RenderWidth, _renderer.RenderHeight));
      }

      ImGui.End();
      // Not sure why this crashes the app...
      // ImGui.PopStyleVar();

      Render();

      float framerate = ImGui.GetIO().Framerate;
      ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");
    }

    public void OnUpdate(float ts)
    {
    }
  }
}