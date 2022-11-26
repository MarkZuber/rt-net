using System.Numerics;

namespace RTNet.ImgCore
{
  public struct Color
  {
    public Color(byte r, byte g, byte b)
    {
      RGBA = ((int)r << 24) | ((int)g << 16) | ((int)b << 8) | 0x00;
    }
    public Color(byte r, byte g, byte b, byte a)
    {
      RGBA = ((int)r << 24) | ((int)g << 16) | ((int)b << 8) | (int)a;
    }

    public Color(int rgba)
    {
      RGBA = (int)((UInt32)rgba & 0xffffff00);
    }

    public static int ToRGBA(Vector4 color)
    {
      var r = (int)(((float)color.X) * 255.0f);
      var g = (int)(((float)color.Y) * 255.0f);
      var b = (int)(((float)color.Z) * 255.0f);
      var a = (int)(((float)color.W) * 255.0f);

      return r << 24 | g << 16 | b << 8 | a;
    }

    public Color(Vector4 color)
    {
      // var clamped = Vector4.Clamp(color, new Vector4(0.0f), new Vector4(1.0f));

      var r = (int)(((float)color.X) * 255.0f);
      var g = (int)(((float)color.Y) * 255.0f);
      var b = (int)(((float)color.Z) * 255.0f);
      var a = (int)(((float)color.W) * 255.0f);

      RGBA = r << 24 | g << 16 | b << 8 | a;
    }

    public int RGBA { get; private set; }
  }
}