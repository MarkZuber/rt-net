// -----------------------------------------------------------------------
// <copyright file="Ray.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;

namespace WkndRay
{
    public class Ray
    {
        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Vector3 Origin { get; }
        public Vector3 Direction { get; }

        public Vector3 GetPointAtParameter(float t)
        {
            return Origin + (t * Direction);
        }
    }
}
