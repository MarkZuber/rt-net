// -----------------------------------------------------------------------
// <copyright file="MixturePdf.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;

namespace WkndRay.Pdfs
{
    public class MixturePdf : IPdf
    {
        public MixturePdf(IPdf p0, IPdf p1)
        {
            P0 = p0;
            P1 = p1;
        }

        public IPdf P0 { get; }
        public IPdf P1 { get; }

        public float GetValue(Vector3 direction)
        {
            return (0.5f * P0.GetValue(direction)) + (0.5f * P1.GetValue(direction));
        }

        public Vector3 Generate()
        {
            return RandomService.Nextfloat() < 0.5f ? P0.Generate() : P1.Generate();
        }
    }
}
