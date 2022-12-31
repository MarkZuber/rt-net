// -----------------------------------------------------------------------
// <copyright file="BasicCamera.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;

namespace WkndRay
{
    public class BasicCamera
    {
        public BasicCamera(Vector3 origin, Vector3 lowerLeftCorner, Vector3 horizontal, Vector3 vertical)
        {
            Origin = origin;
            LowerLeftCorner = lowerLeftCorner;
            Horizontal = horizontal;
            Vertical = vertical;
        }

        public Vector3 Origin { get; }
        public Vector3 LowerLeftCorner { get; }
        public Vector3 Horizontal { get; }
        public Vector3 Vertical { get; }

        public Ray GetRay(float u, float v)
        {
            return new Ray(Origin, LowerLeftCorner + (u * Horizontal) + (v * Vertical) - Origin);
        }
    }
}
