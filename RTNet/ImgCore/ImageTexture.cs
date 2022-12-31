// -----------------------------------------------------------------------
// <copyright file="ImageTexture.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using RTNet.ImgCore;

namespace WkndRay.Textures
{
  public class ImageTexture : ITexture
  {
    private readonly IPixelBuffer _pixelBuffer;

    public ImageTexture(IPixelBuffer pixelBuffer)
    {
      _pixelBuffer = pixelBuffer;
    }

    public uint Width => _pixelBuffer.Width;
    public uint Height => _pixelBuffer.Height;

    public Vector4 GetValue(Vector2 uvCoords, Vector3 p)
    {
      uint i = Convert.ToUInt32(uvCoords.X * Convert.ToSingle(Width));
      uint j = Convert.ToUInt32(((1.0f - uvCoords.Y) * Convert.ToSingle(Height)) - 0.001);
      if (i < 0)
      {
        i = 0;
      }

      if (j < 0)
      {
        j = 0;
      }

      if (i > Width - 1)
      {
        i = Width - 1;
      }

      if (j > Height - 1)
      {
        j = Height - 1;
      }

      return _pixelBuffer.GetPixel(i, j);
    }
  }
}
