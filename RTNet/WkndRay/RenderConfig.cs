// -----------------------------------------------------------------------
// <copyright file="RenderConfig.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

namespace WkndRay
{
    public class RenderConfig
    {
        public RenderConfig(int numThreads, int rayTraceDepth, int numSamples)
        {
            NumThreads = numThreads;
            RayTraceDepth = rayTraceDepth;
            NumSamples = numSamples;
        }

        public int NumThreads { get; }
        public int RayTraceDepth { get; }
        public int NumSamples { get; }

        public bool TwoPhase { get; set; }
    }
}