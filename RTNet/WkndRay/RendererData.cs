using System;
using System.Collections.Generic;
using System.Text;

namespace WkndRay
{
  public class RendererData
  {
    private readonly PixelData[,] _pixelData;
    private readonly object _lock = new object();

    public RendererData(uint width, uint height)
    {
      Width = width;
      Height = height;

      _pixelData = new PixelData[width, height];

      _totalPixelColorMilliseconds = new Lazy<long>(() =>
      {
        long total = 0;

        for (int x = 0; x < Width; x++)
        {
          for (int y = 0; y < Height; y++)
          {
            total += GetPixelData(x, y).PixelColorMilliseconds;
          }
        }

        return total;
      });
    }

    public uint Width { get; }
    public uint Height { get; }

    private readonly Lazy<long> _totalPixelColorMilliseconds;

    public long GetTotalPixelColorMilliseconds()
    {
      return _totalPixelColorMilliseconds.Value;
    }

    public long GetNumPixels()
    {
      return Width * Height;
    }

    public long GetAveragePixelColorMilliseconds()
    {
      return GetTotalPixelColorMilliseconds() / GetNumPixels();
    }

    public void SetPixelData(PixelData data)
    {
      lock (_lock)
      {
        _pixelData[data.X, data.Y] = data;
      }
    }

    public PixelData GetPixelData(int x, int y)
    {
      lock (_lock)
      {
        return _pixelData[x, y];
      }
    }
  }
}
