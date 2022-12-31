// -----------------------------------------------------------------------
// <copyright file="Rgba32Extensions.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using SixLabors.ImageSharp.PixelFormats;

namespace WkndRay
{
  public static class Rgba32Extensions
  {
    public static Vector4 ToVector4(this Rgba32 rgba32)
    {
      return new Vector4(ByteToColor(rgba32.R), ByteToColor(rgba32.G), ByteToColor(rgba32.B), 1.0f);
    }

    private static float ByteToColor(byte c)
    {
      return Convert.ToSingle(c) / 255.0f;
    }
  }
}