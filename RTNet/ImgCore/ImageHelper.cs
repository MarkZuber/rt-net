using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RTNet.ImgCore
{
  public static class ImageHelper
  {
    public static ImageBuffer FromFile(string filePath)
    {
      using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
      {
        // TODO: there must be a way with Veldrid to get a veldrid texture from this that we can blit
        // Also, need to restructure ImageBuffer so that we can get the controller globally
        // and not have to pass it in.
        throw new NotImplementedException("FromFile is not implemented");
      }
    }
  }
}