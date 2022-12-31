using System.Numerics;
using Veldrid;

namespace RTNet.ImgCore
{
  public class ImageBuffer : IPixelBuffer // : IDisposable
  {
    private Texture? _texture;
    private Texture? _stagingTexture;
    private IntPtr _textureId;
    private byte[] _imageData;

    public ImageBuffer(UInt32 width, UInt32 height)
    {
      Width = width;
      Height = height;
      _imageData = new byte[width * height * PixelFormatSize];
    }

    private UInt32 PixelFormatSize => 4;
    private PixelFormat Format = PixelFormat.R8_G8_B8_A8_UNorm;

    public UInt32 Width { get; private set; }
    public UInt32 Height { get; private set; }

    public bool IsYUp => throw new NotImplementedException();

    unsafe public IntPtr CaptureImageBufferPointer()
    {
      if (_textureId == IntPtr.Zero)
      {
        Allocate();
      }

      var controller = Application.GetController();
      if (_stagingTexture == null)
      {
        _stagingTexture = controller.Graphics.ResourceFactory.CreateTexture(
          TextureDescription.Texture2D(Width, Height, 1, 1, Format, TextureUsage.Staging));
      }

      fixed (byte* texDataPtr = &_imageData[0])
      {
        controller.Graphics.UpdateTexture(_stagingTexture, (IntPtr)texDataPtr, Width * Height * PixelFormatSize, 0, 0, 0, Width, Height, 1, 0, 0);
      }

      CommandList cl = controller.Graphics.ResourceFactory.CreateCommandList();
      cl.Begin();
      cl.CopyTexture(_stagingTexture, _texture);
      cl.End();
      controller.Graphics.SubmitCommands(cl);

      return _textureId;
    }

    public void SetPixel(UInt32 x, UInt32 y, Vector4 color)
    {
      uint actualY = Height - 1 - y;

      UInt32 offset = actualY * Width * PixelFormatSize + x * PixelFormatSize;
      _imageData[offset] = Convert.ToByte(Math.Clamp(((float)color.X), 0.0, 1.0) * 255.0f);
      _imageData[offset + 1] = Convert.ToByte(Math.Clamp(((float)color.Y), 0.0, 1.0) * 255.0f);
      _imageData[offset + 2] = Convert.ToByte(Math.Clamp(((float)color.Z), 0.0, 1.0) * 255.0f);
      _imageData[offset + 3] = Convert.ToByte(Math.Clamp(((float)color.W), 0.0, 1.0) * 255.0f);
    }

    public void SetPixel(UInt32 x, UInt32 y, Vector3 color)
    {
      SetPixel(x, y, new Vector4(color.X, color.Y, color.Z, 1.0f));
    }

    public void SetPixel(uint x, uint y, byte r, byte g, byte b)
    {
      throw new NotImplementedException();
    }

    public void SetPixelRowColors(uint y, IEnumerable<Vector4> rowPixels)
    {
      uint x = 0;
      foreach (var color in rowPixels)
      {
        SetPixel(x, y, color);
        x++;
      }
    }


    public Vector4 GetPixel(UInt32 x, UInt32 y)
    {
      uint actualY = Height - 1 - y;
      UInt32 offset = actualY * Width * PixelFormatSize + x * PixelFormatSize;
      float r = (float)_imageData[offset] / 255.0f;
      float g = (float)_imageData[offset + 1] / 255.0f;
      float b = (float)_imageData[offset + 2] / 255.0f;
      float a = (float)_imageData[offset + 3] / 255.0f;
      return new Vector4(r, g, b, a);
    }

    public void Fill(byte val)
    {
      Array.Fill(_imageData, val);
    }

    public void DrawLineWithThickness(UInt32 x0, UInt32 y0, UInt32 x1, UInt32 y1, Vector3 color)
    {
      DrawLine(x0, y0, x1, y1, color);
      DrawLine(x0 - 1, y0 - 1, x1 - 1, y1 - 1, color);
      DrawLine(x0 + 1, y0 + 1, x1 + 1, y1 + 1, color);
    }

    public void DrawLine(UInt32 x0, UInt32 y0, UInt32 x1, UInt32 y1, Vector3 color)
    {
      int dx = (int)x1 - (int)x0;
      int dy = (int)y1 - (int)y0;
      int absdx = Math.Abs(dx);
      int absdy = Math.Abs(dy);

      UInt32 x = x0;
      UInt32 y = y0;

      SetPixel(x, y, color);

      // slope < 1
      if (absdx > absdy)
      {
        int d = 2 * absdy - absdx;

        for (int i = 0; i < absdx; i++)
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
        int d = 2 * absdx - absdy;

        for (int i = 0; i < absdy; i++)
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

    private void Allocate()
    {
      var controller = Application.GetController();
      _texture = controller.Graphics.ResourceFactory.CreateTexture(
        TextureDescription.Texture2D(Width, Height, 1, 1, Format, TextureUsage.Sampled, 0));
      _textureId = controller.GetOrCreateImGuiBinding(controller.Graphics.ResourceFactory, _texture);

      // textureView?
    }

    private void Release()
    {
      if (_texture != null)
      {
        _texture.Dispose();
        _texture = null;
      }
      if (_stagingTexture != null)
      {
        _stagingTexture.Dispose();
        _stagingTexture = null;
      }
      _textureId = IntPtr.Zero;
    }

    public void Resize(UInt32 width, UInt32 height)
    {
      if (Width == width && Height == height)
      {
        return;
      }
      Width = width;
      Height = height;
      _imageData = new byte[Width * Height * PixelFormatSize];
      Release();
      Allocate();
    }


    public void SaveAsFile(string outputFilePath)
    {
      throw new NotImplementedException();
    }
  }
}