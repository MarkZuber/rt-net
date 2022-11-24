using System.Numerics;
using ImGuiNET;
using RTNet.ImgCore;
using Veldrid;

namespace RTNet
{
  public class ImageBuffer
  {
    private GraphicsDevice _gd;
    private UInt32 _width;
    private UInt32 _height;
    private Texture _texture;

    public ImageBuffer(GraphicsDevice gd, UInt32 width, UInt32 height)
    {
      _gd = gd;
      _width = width;
      _height = height;
      _texture = CreateTex(width, height);
    }

    private Texture CreateTex(UInt32 width, UInt32 height)
    {
      return _gd.ResourceFactory.CreateTexture(
        TextureDescription.Texture2D(
          width, height, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled
        )
      );
    }

    public void SetData()
    {
      int bytesPerPixel = 4;

      var l = new List<int>();
      var la = l.ToArray<int>();

      var pixdata = new int[10, 10];

      unsafe
      {
        fixed (int* pArray = pixdata)
        {
          IntPtr pixels = new IntPtr((void*)pArray);

          _gd.UpdateTexture(
              _texture,
              pixels,
              (uint)(bytesPerPixel * _width * _height),
              0,
              0,
              0,
              (uint)_width,
              (uint)_height,
              1,
              0,
              0);
        }
      }
    }

    public void Resize(UInt32 width, UInt32 height)
    {
      if (_width == width && _height == height)
      {
        return;
      }
      _width = width;
      _height = height;
      _texture = CreateTex(_width, _height);
    }
  }

  public class GraphicsOps
  {
    private GraphicsDevice _gd;
    public GraphicsOps(GraphicsDevice gd)
    {
      _gd = gd;
    }

    // public Texture CreateTexture(UInt32 width, UInt32 height)
    // {
    //   return texture;
    // }
  }

  public class RayTracerAppLayer : IAppLayer
  {
    private UInt32 _viewportWidth;
    private UInt32 _viewportHeight;
    private double _lastRenderTime;
    private GraphicsDevice _gd;
    private Renderer _renderer;

    public void SetGraphicsDevice(GraphicsDevice gd)
    {
      _gd = gd;
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

      // _renderer.OnResize(_viewportWidth, _viewportHeight);
      // _camera.OnResize(_viewportWidth, _viewportHeight);
      // _renderer.Render(_scene, _camera);

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

      // var image = _renderer.GetFinalImage();
      // ImGui.Image(
      //   image.getDescriptorSet(),
      //   new Vector2(image.GetWidth(), image.GetHeight()),
      //   new Vector2(0, 1), new Vector2(1, 0));


      ImGui.End();
      // ImGui.PopStyleVar();

      // TODO: do we want/need this here since we have the "Render" button above?
      Render();

      float framerate = ImGui.GetIO().Framerate;
      ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");
    }

    public void OnUpdate(float ts)
    {
    }
  }
}