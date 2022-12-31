// -----------------------------------------------------------------------
// <copyright file="IExecutor.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using RTNet.ImgCore;

namespace WkndRay.Executors
{
  public interface IExecutor
  {
    ImageBuffer Execute(uint width, uint height);
  }
}