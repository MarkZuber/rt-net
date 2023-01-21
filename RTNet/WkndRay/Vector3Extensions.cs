using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WkndRay
{
  public static class Vector3Extensions
  {
    public static Vector3 ToUnitVector(this Vector3 vec)
    {
      return vec * (1.0f / vec.Length());
    }

    public static Vector4 ToVector4(this Vector3 vec)
    {
      return new Vector4(vec.X, vec.Y, vec.Z, 1.0f);
    }

    public static float[] ToSingleArray(this Vector3 vec)
    {
      return new float[] { vec.X, vec.Y, vec.Z };
    }

    public static Vector3 GetRandomInUnitSphere()
    {
      Vector3 pv;
      do
      {
        pv = (2.0f * new Vector3(RandomService.NextSingle(), RandomService.NextSingle(), RandomService.NextSingle())) -
             Vector3.One;
      }
      while (pv.LengthSquared() >= 1.0f);

      return pv;
    }

    public static Vector3 Reflect(this Vector3 vec, Vector3 other)
    {
      return vec - (2.0f * Vector3.Dot(vec, other) * other);
    }

    public static Vector3 Refract(this Vector3 vec, Vector3 normal, float niOverNt)
    {
      var unitVector = vec.ToUnitVector();
      float dt = Vector3.Dot(unitVector, normal);
      float discriminant = 1.0f - (niOverNt * niOverNt * (1.0f - (dt * dt)));
      if (discriminant > 0.0f)
      {
        return (niOverNt * (unitVector - (normal * dt))) - (normal * MathF.Sqrt(discriminant));
      }

      return Vector3.Zero;
    }
  }
}
