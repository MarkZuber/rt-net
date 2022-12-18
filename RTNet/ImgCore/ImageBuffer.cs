using System.Numerics;
using Veldrid;

namespace RTNet.ImgCore
{
  public class ImageBuffer
  {
    private IAppInfo _appInfo;
    private Texture? _texture;
    private IntPtr _textureId;
    private int[] _imageData;

    public ImageBuffer(IAppInfo appInfo, UInt32 width, UInt32 height)
    {
      Width = width;
      Height = height;
      _appInfo = appInfo;
      _imageData = new int[width * height];
    }

    public UInt32 Width { get; private set; }
    public UInt32 Height { get; private set; }

    public IntPtr CaptureImageBufferPointer()
    {
      if (_textureId == IntPtr.Zero)
      {
        (_texture, _textureId) = CreateTex(Width, Height);
      }
      _appInfo.GetController().Graphics.UpdateTexture(_texture, _imageData, 0, 0, 0, Width, Height, 1, 0, 0);
      return _textureId;
    }

    public void SetPixel(UInt32 x, UInt32 y, Vector4 color)
    {
      _imageData[(y * Width) + x] = Color.ToRGBA(color);
    }

    public void Fill(Vector4 color)
    {
      Array.Fill(_imageData, Color.ToRGBA(color));
    }

    public void DrawLineWithThickness(UInt32 x0, UInt32 y0, UInt32 x1, UInt32 y1, Vector4 color)
    {
      DrawLine(x0, y0, x1, y1, color);
      DrawLine(x0 - 1, y0 - 1, x1 - 1, y1 - 1, color);
      DrawLine(x0 + 1, y0 + 1, x1 + 1, y1 + 1, color);
    }

    public void DrawLine(UInt32 x0, UInt32 y0, UInt32 x1, UInt32 y1, Vector4 color)
    {
      UInt32 dx = x1 - x0;
      UInt32 dy = y1 - y0;
      UInt32 absdx = Convert.ToUInt32(Math.Abs(dx));
      UInt32 absdy = Convert.ToUInt32(Math.Abs(dy));

      UInt32 x = x0;
      UInt32 y = y0;

      SetPixel(x, y, color);

      // slope < 1
      if (absdx > absdy)
      {
        UInt32 d = 2 * absdy - absdx;

        for (UInt32 i = 0; i < absdx; i++)
        {
          x = dx < 0 ? x - 1 : x + 1;
          if (d < 0)
          {
            d = d + 2 * absdy;
          }
          else
          {
            y = dy < 0 ? y - 1 : y + 1;
            d = d + (2 * absdy - 2 * absdx);
          }
          SetPixel(x, y, color);
        }
      }
      else
      { // case when slope is greater than or equals to 1
        UInt32 d = 2 * absdx - absdy;

        for (UInt32 i = 0; i < absdy; i++)
        {
          y = dy < 0 ? y - 1 : y + 1;
          if (d < 0)
            d = d + 2 * absdx;
          else
          {
            x = dx < 0 ? x - 1 : x + 1;
            d = d + (2 * absdx) - (2 * absdy);
          }
          SetPixel(x, y, color);
        }
      }
    }

    private Tuple<Texture, IntPtr> CreateTex(UInt32 width, UInt32 height)
    {
      var controller = _appInfo.GetController();
      var texture = controller.Graphics.ResourceFactory.CreateTexture(
        TextureDescription.Texture2D(
          width, height, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled
        )
      );

      var textureId = controller.GetOrCreateImGuiBinding(controller.Graphics.ResourceFactory, texture);
      return new Tuple<Texture, IntPtr>(texture, textureId);
    }

    public void Resize(UInt32 width, UInt32 height)
    {
      if (Width == width && Height == height)
      {
        return;
      }
      Width = width;
      Height = height;
      _imageData = new int[Width * Height];
      (_texture, _textureId) = CreateTex(Width, Height);
    }
  }
}