using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RTNet.ImgCore
{
  public interface ITexture
  {
    Vector4 GetValue(Vector2 uvCoords, Vector3 p);
  }
}
