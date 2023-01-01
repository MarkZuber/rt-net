using System.Numerics;
using ImGuiNET;
using RTNet.ImgCore;
using Veldrid;
using Veldrid.ImageSharp;

namespace RTNet.ChernoRay
{
  public class ChernoRayAppLayer : IAppLayer
  {
    private UInt32 _viewportWidth;
    private UInt32 _viewportHeight;
    private double _lastRenderTime;
    private ChernoRenderer _renderer;
    private Camera _camera;
    private Scene _scene;

    public ChernoRayAppLayer()
    {
      _renderer = new ChernoRenderer(600, 600);
      _camera = new Camera(45.0f, 0.1f, 100.0f);
      _scene = new Scene();

      var pinkSphere = new Material { Albedo = new Vector3(1.0f, 1.0f, 1.0f), Roughness = 0.0f };
      _scene.Materials.Add(pinkSphere);

      var blueSphere = new Material { Albedo = new Vector3(0.2f, 0.3f, 1.0f), Roughness = 0.1f };
      _scene.Materials.Add(blueSphere);

      _scene.Spheres.Add(new Sphere() { Position = new Vector3(0.0f, 0.0f, 0.0f), Radius = 1.0f, MaterialIndex = 0 });
      _scene.Spheres.Add(new Sphere() { Position = new Vector3(0.0f, -101.0f, 0.0f), Radius = 100.0f, MaterialIndex = 1 });
    }

    private void Render()
    {
      var start = DateTime.UtcNow;

      _renderer.Resize(_viewportWidth, _viewportHeight);
      _camera.OnResize(_viewportWidth, _viewportHeight);
      _renderer.Render(_camera, _scene);

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
        ImGui.Image(finalImgPtr, new Vector2(_renderer.Width, _renderer.Height));
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
      if (_camera.OnUpdate(ts))
      {
        _renderer.ResetFrameIndex();
      }
    }
  }
}