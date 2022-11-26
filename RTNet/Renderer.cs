using RTNet.ImgCore;
using Veldrid;

namespace RTNet
{
  public class Renderer
  {
    private Random _random = new Random();

    private int[] _imageData;

    private ImageBuffer _finalImageBuffer;

    public Renderer(ImGuiController controller, UInt32 renderWidth, UInt32 renderHeight)
    {
      RenderWidth = renderWidth;
      RenderHeight = renderHeight;
      _imageData = new int[renderWidth * renderHeight];
      _finalImageBuffer = new ImageBuffer(controller, renderWidth, renderHeight);
    }

    public UInt32 RenderWidth { get; private set; }
    public UInt32 RenderHeight { get; private set; }

    public IntPtr GetFinalImagePtr()
    {
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
      _finalImageBuffer.Resize(RenderWidth, RenderHeight);
    }

    private void PerPixel(UInt32 x, UInt32 y)
    {
      _imageData[(y * RenderWidth) + x] = new Color(_random.Next()).RGBA;
    }

    public void Render()
    {
      for (UInt32 y = 0; y < RenderHeight; y++)
      {
        for (UInt32 x = 0; x < RenderWidth; x++)
        {
          PerPixel(x, y);
        }
      }
    }
  }
}