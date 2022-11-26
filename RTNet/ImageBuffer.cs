using Veldrid;

namespace RTNet
{
  public class ImageBuffer
  {
    private ImGuiController _controller;
    private Texture _texture;

    public ImageBuffer(ImGuiController controller, UInt32 width, UInt32 height)
    {
      Width = width;
      Height = height;
      _controller = controller;
      (_texture, TextureId) = CreateTex(Width, Height);
    }

    public UInt32 Width { get; private set; }
    public UInt32 Height { get; private set; }

    public IntPtr TextureId { get; private set; }

    public void SetData(int[] pixelData)
    {
      _controller.Graphics.UpdateTexture(_texture, pixelData, 0, 0, 0, Width, Height, 1, 0, 0);
    }

    private Tuple<Texture, IntPtr> CreateTex(UInt32 width, UInt32 height)
    {
      var texture = _controller.Graphics.ResourceFactory.CreateTexture(
        TextureDescription.Texture2D(
          width, height, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled
        )
      );

      var textureId = _controller.GetOrCreateImGuiBinding(_controller.Graphics.ResourceFactory, texture);

      return new Tuple<Texture, IntPtr>(texture, textureId);
    }

    public void Resize(UInt32 width, UInt32 height)
    {
      if (Width == width && Height == height)
      {
        return;
      }
      Width = width;
      Height = height;
      (_texture, TextureId) = CreateTex(Width, Height);
    }
  }
}