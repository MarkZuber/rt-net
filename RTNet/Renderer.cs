using RTNet.ImgCore;
using Veldrid;
using System.Numerics;

namespace RTNet
{
  public class Renderer
  {
    private Random _random = new Random();

    private int[] _imageData;

    private ImageBuffer? _finalImageBuffer;

    public Renderer(UInt32 renderWidth, UInt32 renderHeight)
    {
      RenderWidth = renderWidth;
      RenderHeight = renderHeight;
      _imageData = new int[renderWidth * renderHeight];
    }

    public void Initialize(ImGuiController controller)
    {
      _finalImageBuffer = new ImageBuffer(controller, RenderWidth, RenderHeight);
    }

    public UInt32 RenderWidth { get; private set; }
    public UInt32 RenderHeight { get; private set; }

    public IntPtr GetFinalImagePtr()
    {
      if (_finalImageBuffer == null)
      {
        return IntPtr.Zero;
      }

      _finalImageBuffer.SetData(_imageData);
      return _finalImageBuffer.TextureId;
    }

    public void Resize(UInt32 renderWidth, UInt32 renderHeight)
    {
      if (renderWidth == RenderWidth && renderHeight == RenderHeight)
      {
        return;
      }

      RenderWidth = renderWidth;
      RenderHeight = renderHeight;

      _imageData = new int[RenderWidth * RenderHeight];
      _finalImageBuffer?.Resize(RenderWidth, RenderHeight);
    }

    private Vector4 PerPixel(UInt32 x, UInt32 y)
    {
      return new Vector4(Convert.ToSingle(_random.NextDouble()), Convert.ToSingle(_random.NextDouble()), Convert.ToSingle(_random.NextDouble()), 0.0f);
    }

    private Vector4 PerPixelNA(UInt32 x, UInt32 y)
    {
      return new Vector4(0.0f);
    }

    public void Render()
    {
      for (UInt32 y = 0; y < RenderHeight; y++)
      {
        for (UInt32 x = 0; x < RenderWidth; x++)
        {
          _imageData[(y * RenderWidth) + x] = Color.ToRGBA(PerPixel(x, y));
        }
      }
    }
  }
}