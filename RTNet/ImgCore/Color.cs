using System.Numerics;

namespace RTNet.ImgCore
{
  public struct Color
  {
    private float _r;
    private float _g;
    private float _b;
    private float _a;

    public Color(byte r, byte g, byte b)
    {
      _r = (float)r / 255.0f;
      _g = (float)g / 255.0f;
      _b = (float)b / 255.0f;
      _a = 1.0f;
    }

    public Color(byte r, byte g, byte b, byte a)
    {
      _r = (float)r / 255.0f;
      _g = (float)g / 255.0f;
      _b = (float)b / 255.0f;
      _a = (float)a / 255.0f; ;
    }

    public Color(float r, float g, float b)
    {
      _r = r;
      _g = g;
      _b = b;
      _a = 1.0f;
    }

    public Color(float allColors)
    {
      _r = allColors;
      _g = allColors;
      _b = allColors;
      _a = 1.0f;
    }

    public Color(float r, float g, float b, float a)
    {
      _r = r;
      _g = g;
      _b = b;
      _a = a;
    }

    public UInt32 ToRGBA()
    {
      var r = Convert.ToUInt32(_r * 255.0f);
      var g = Convert.ToUInt32(_g * 255.0f);
      var b = Convert.ToUInt32(_b * 255.0f);
      var a = Convert.ToUInt32(_a * 255.0f);

      return (a << 24) | (b << 16) | (g << 8) | r;
    }

    public void Clamp()
    {
      var clamped = Vector4.Clamp(new Vector4(_r, _g, _b, _a), new Vector4(0.0f), new Vector4(1.0f));
      _r = clamped.X;
      _g = clamped.Y;
      _b = clamped.Z;
      _a = clamped.W;
    }

    public byte[] ToBytes()
    {
      var bytes = new byte[4];
      bytes[0] = Convert.ToByte(_r * 255.0f);
      bytes[1] = Convert.ToByte(_g * 255.0f);
      bytes[2] = Convert.ToByte(_b * 255.0f);
      bytes[3] = Convert.ToByte(_a * 255.0f);
      return bytes;
    }
  }
}