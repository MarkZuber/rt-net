// -----------------------------------------------------------------------
// <copyright file="IPixelBuffer.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Numerics;

namespace Zube.ImGuiNet
{
  public interface IPixelBuffer : IDisposable
  {
    UInt32 Width { get; }
    UInt32 Height { get; }

    /// <summary>
    ///   Our Y axis is UP (right handed coordinate system)
    ///   X is right, and positive Z is out of the screen towards
    ///   the viewer.  So our calculated Y pixels are
    ///   the opposite direction of the Y in the image buffer.
    ///   If IsYUp is true then we'll invert Y when setting it into
    ///   the image.
    /// </summary>
    bool IsYUp { get; }

    void SetPixel(UInt32 x, UInt32 y, Vector4 color);
    void SetPixel(UInt32 x, UInt32 y, byte r, byte g, byte b);

    Vector4 GetPixel(UInt32 x, UInt32 y);

    void SaveToFileAsPng(string filePath);
    void SetPixelRowColors(UInt32 y, IEnumerable<Vector4> rowPixels);
    void SetPixelRowBytes(uint y, byte[] pixelData);
  }
}