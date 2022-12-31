using System.Numerics;

namespace RTNet.ImgCore
{
  public static class Vector4Extensions
  {
    public static Vector4 DeNan(this Vector4 source)
    {
      return new Vector4(
          float.IsNaN(source.X) ? 0.0f : source.X,
          float.IsNaN(source.Y) ? 0.0f : source.Y,
          float.IsNaN(source.Z) ? 0.0f : source.Z,
          float.IsNaN(source.W) ? 0.0f : source.W);
    }

    public static Vector4 ApplyGamma2(this Vector4 source)
    {
      return Vector4.SquareRoot(source);
    }

    public static float U(this Vector4 source)
    {
      return source.X;
    }

    public static float V(this Vector4 source)
    {
      return source.Y;
    }
  }
}