// -----------------------------------------------------------------------
// <copyright file="RenderProgressEventArgs.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace WkndRay
{
    public class RenderProgressEventArgs : EventArgs
    {
        public RenderProgressEventArgs(float percentComplete)
        {
            PercentComplete = percentComplete;
        }

        public float PercentComplete { get; }
    }
}