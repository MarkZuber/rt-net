using System;
using System.Numerics;
using RTNet.ImgCore;

namespace WkndRay.Textures
{
  public class ColorTexture : ITexture
  {
    public ColorTexture(float r, float g, float b) : this(new Vector4(r, g, b, 1.0f))
    {
    }

    public ColorTexture(Vector4 color)
    {
      Color = color;
    }

    public Vector4 Color { get; }

    public Vector4 GetValue(Vector2 uvCoords, Vector3 p)
    {
      return Color;
    }
  }
}
