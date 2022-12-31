// // -----------------------------------------------------------------------
// // <copyright file="PixelBuffer.cs" company="ZubeNET">
// //   Copyright...
// // </copyright>
// // -----------------------------------------------------------------------

// using System.Collections.Generic;
// using System.IO;
// using SixLabors.ImageSharp;
// using SixLabors.ImageSharp.PixelFormats;

// namespace WkndRay
// {
//   public class PixelBuffer : IPixelBuffer
//   {
//     private readonly Image<Rgba32> _image;
//     private readonly object _lock = new object();

//     public PixelBuffer(int width, int height, bool isYUp = true)
//     {
//       _image = new Image<Rgba32>(width, height);
//       IsYUp = isYUp;
//     }

//     private PixelBuffer(Image<Rgba32> image, bool isYUp = true)
//     {
//       _image = image;
//       IsYUp = isYUp;
//     }

//     public static PixelBuffer FromFile(string inputFilePath)
//     {
//       using (var stream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
//       {
//         return new PixelBuffer(Image.Load<Rgba32>(stream));
//       }
//     }

//     public int Width => _image.Width;
//     public int Height => _image.Height;

//     /// <summary>
//     ///   Our Y axis is UP (right handed coordinate system)
//     ///   X is right, and positive Z is out of the screen towards
//     ///   the viewer.  So our calculated Y pixels are
//     ///   the opposite direction of the Y in the image buffer.
//     ///   If IsYUp is true then we'll invert Y when setting it into
//     ///   the image.
//     /// </summary>
//     public bool IsYUp { get; }

//     public void SetPixelColor(int x, int y, Vector4 color)
//     {
//       SetPixelColor(x, y, color.ToRgba32());
//     }

//     public void SetPixelColor(int x, int y, byte r, byte g, byte b)
//     {
//       SetPixelColor(x, y, new Rgba32(r, g, b));
//     }

//     public Vector4 GetPixelColor(int x, int y)
//     {
//       lock (_lock)
//       {
//         return _image[x, y].ToVector4();
//       }
//     }

//     public void SaveAsFile(string outputFilePath)
//     {
//       lock (_lock)
//       {
//         _image.Save(outputFilePath);
//       }
//     }

//     public void SetPixelRowColors(int y, IEnumerable<Vector4> rowPixels)
//     {
//       int actualY = CalculateActualY(y);
//       lock (_lock)
//       {
//         int x = 0;
//         foreach (Vector4 color in rowPixels)
//         {
//           _image[x, actualY] = color.ToRgba32();
//           x++;
//         }
//       }
//     }

//     private int CalculateActualY(int y)
//     {
//       return IsYUp ? (Height - 1 - y) : y;
//     }

//     private void SetPixelColor(int x, int y, Rgba32 rgba)
//     {
//       lock (_lock)
//       {
//         _image[x, CalculateActualY(y)] = rgba;
//       }
//     }
//   }
// }