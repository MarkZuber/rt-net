using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace WkndRay
{
  public class PixelData
  {
    public PixelData(uint x, uint y)
    {
      X = x;
      Y = y;
    }

    public uint X { get; }
    public uint Y { get; }
    public Vector4 Color { get; set; }

    public long PixelColorMilliseconds { get; private set; }
    public void SetPixelColorMilliseconds(long ms) => PixelColorMilliseconds = ms;

    public long AverageSampleMilliseconds { get; set; }

    public int MaxDepthReached { get; private set; } = 0;

    internal void SetDepth(int depth)
    {
      if (depth > MaxDepthReached)
      {
        MaxDepthReached = depth;
      }
    }
  }
}
