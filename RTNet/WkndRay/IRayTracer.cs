// -----------------------------------------------------------------------
// <copyright file="IRayTracer.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

namespace WkndRay
{
  public interface IRayTracer
  {
    PixelData GetPixelColor(uint x, uint y);
  }
}
