// -----------------------------------------------------------------------
// <copyright file="Camera.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Numerics;

namespace WkndRay
{
  public class Camera
  {
    public Camera(
      Vector3 lookFrom,
      Vector3 lookAt,
      Vector3 up,
      float verticalFov,
      float aspect,
      float aperture,
      float focusDistance)
    {
      LensRadius = aperture / 2.0f;
      float theta = verticalFov * MathF.PI / 180.0f;
      float halfHeight = MathF.Tan(theta / 2.0f);
      float halfWidth = aspect * halfHeight;
      Origin = lookFrom;
      W = (lookFrom - lookAt).ToUnitVector();
      U = Vector3.Cross(up, W).ToUnitVector();
      V = Vector3.Cross(W, U);
      LowerLeftCorner = Origin - (halfWidth * focusDistance * U) - (halfHeight * focusDistance * V) - (focusDistance * W);
      Horizontal = 2.0f * halfWidth * focusDistance * U;
      Vertical = 2.0f * halfHeight * focusDistance * V;
    }

    public Vector3 Origin { get; }
    public Vector3 LowerLeftCorner { get; }
    public Vector3 Horizontal { get; }
    public Vector3 Vertical { get; }
    public Vector3 U { get; }
    public Vector3 V { get; }
    public Vector3 W { get; }
    public float LensRadius { get; }

    public Ray GetRay(float s, float t)
    {
      return new Ray(Origin, LowerLeftCorner + (s * Horizontal) + (t * Vertical) - Origin);

      // this is the lens managed code, but getrandominunitsphere is slow due to while loop.
      // var rd = LensRadius * Vector3Extensions.GetRandomInUnitSphere();
      // var offset = (U * rd.X) + (V * rd.Y);
      // return new Ray(Origin + offset, LowerLeftCorner + (s * Horizontal) + (t * Vertical) - Origin - offset);
    }
  }
}
