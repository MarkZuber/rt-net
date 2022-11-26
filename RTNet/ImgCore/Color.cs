namespace RTNet.ImgCore
{
  public struct Color
  {
    public Color(byte r, byte g, byte b)
    {
      RGBA = ((int)r << 24) | ((int)g << 16) | ((int)b << 8) | 0x00;
    }

    public Color(int rgba)
    {
      RGBA = rgba;
    }

    public int RGBA { get; private set; }
  }
}