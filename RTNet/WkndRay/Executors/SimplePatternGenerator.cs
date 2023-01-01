// -----------------------------------------------------------------------
// <copyright file="SimplePatternGenerator.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;
using RTNet.ImgCore;

namespace WkndRay.Executors
{
  public class SimplePatternGenerator : IExecutor
  {
    public PixelBuffer Execute(uint width, uint height)
    {
      var pixelBuffer = new PixelBuffer(width, height);

      for (uint j = height - 1; j >= 0; j--)
      {
        for (uint i = 0; i < width; i++)
        {
          var color = new Vector4(
            Convert.ToSingle(i) / Convert.ToSingle(width),
            Convert.ToSingle(j) / Convert.ToSingle(height),
            0.2f,
            1.0f);
          pixelBuffer.SetPixel(i, j, color);
        }
      }

      return pixelBuffer;
    }
  }
}
