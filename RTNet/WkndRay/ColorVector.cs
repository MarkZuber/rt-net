// // -----------------------------------------------------------------------
// // <copyright file="Vector4.cs" company="ZubeNET">
// //   Copyright...
// // </copyright>
// // -----------------------------------------------------------------------

// using System;
// using System.Numerics;
// using SixLabors.ImageSharp.PixelFormats;

// namespace WkndRay
// {
//     public struct Vector4 
//     {
//         private readonly Vector3 _vector3;

//         /// <inheritdoc />
//         public Vector4(float r, float g, float b)
//         {
//             _vector3 = new Vector3(float.IsNaN(r) ? 0.0f : r, float.IsNaN(g) ? 0.0f : g, float.IsNaN(b) ? 0.0f : b);
//         }

//         private Vector4(Vector3 vec)
//         {
//             _vector3 = vec;
//         }

//         public float R => _vector3.X;
//         public float G => _vector3.Y;
//         public float B => _vector3.Z;

//         public static Vector4 Zero => new Vector4(Vector3.Zero);
//         public static Vector4 One => new Vector4(Vector3.One);

//         public static Vector4 FromBytes(byte r, byte g, byte b)
//         {
//             return new Vector4(ByteToColor(r), ByteToColor(g), ByteToColor(b));
//         }

//         public Vector4 DeNan()
//         {
//             return new Vector4(
//                 float.IsNaN(R) ? 0.0f : R,
//                 float.IsNaN(G) ? 0.0f : G,
//                 float.IsNaN(B) ? 0.0f : B);
//         }

//         public Vector4 ClampColor()
//         {
//             return new Vector4(Vector3.Clamp(_vector3, Vector3.Zero, Vector3.One));
//         }

//         public Rgba32 ToRgba32()
//         {
//             var v2 = ClampColor();
//             return new Rgba32(ColorToByte(v2.R), ColorToByte(v2.G), ColorToByte(v2.B));
//         }

//         private static byte ColorToByte(float c)
//         {
//             try
//             {
//                 return Convert.ToByte(c * 255.0f);
//             }
//             catch (OverflowException ex)
//             {
//                 Console.WriteLine(ex);
//                 throw;
//             }
//         }

//         private static float ByteToColor(byte c)
//         {
//             return Convert.ToSingle(c) / 255.0f;
//         }

//         public override string ToString()
//         {
//             return string.Format($"({R},{G},{B})");
//         }

//         public static float CosVectors(Vector3 v1, Vector3 v2)
//         {
//             return Vector3.Dot(v1, v2) / MathF.Sqrt(v1.LengthSquared() * v2.LengthSquared());
//         }

//         public static Vector4 operator +(Vector4 a, Vector4 b)
//         {
//             return new Vector4(a._vector3 + b._vector3);
//         }

//         public static Vector4 operator -(Vector4 a, Vector4 b)
//         {
//             return new Vector4(a._vector3 - b._vector3);
//         }

//         public static Vector4 operator *(Vector4 a, Vector4 b)
//         {
//             return new Vector4(a._vector3 * b._vector3);
//         }

//         public static Vector4 operator *(Vector4 a, float scalar)
//         {
//             return new Vector4(a._vector3 * scalar);
//         }

//         public static Vector4 operator *(float scalar, Vector4 a)
//         {
//             return new Vector4(a._vector3 * scalar);
//         }

//         public static Vector4 operator /(Vector4 a, float scalar)
//         {
//             return new Vector4(a._vector3 / scalar);
//         }

//         public Vector4 ApplyGamma2()
//         {
//             return new Vector4(Vector3.SquareRoot(_vector3));
//             // return new Vector4(MathF.Sqrt(R), MathF.Sqrt(G), MathF.Sqrt(B));
//         }
//     }
// }
