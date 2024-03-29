﻿// -----------------------------------------------------------------------
// <copyright file="IExecutor.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using Zube.ImGuiNet;

namespace WkndRay.Executors
{
  public interface IExecutor
  {
    PixelBuffer Execute(uint width, uint height);
  }
}