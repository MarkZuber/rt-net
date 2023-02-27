// -----------------------------------------------------------------------
// <copyright file="ImageTexture.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Zube.ImGuiNet
{
  public class Application
  {
    private List<IAppLayer> _appLayers = new List<IAppLayer>();

    private static Vector3 clearColor = new Vector3(0.45f, 0.55f, 0.6f);

    private ApplicationSpec _appSpec;

    Sdl2Window _window;
    CommandList _cl;
    GraphicsDevice _gd;

    public Application(ApplicationSpec appSpec)
    {
      _appSpec = appSpec;

      VeldridStartup.CreateWindowAndGraphicsDevice(
        new WindowCreateInfo(50, 50, (int)_appSpec.Width, (int)_appSpec.Height, WindowState.Normal, _appSpec.Name),
        new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
        out _window,
        out _gd);

      _cl = _gd.ResourceFactory.CreateCommandList();
      _controller = new ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);

      _window.Resized += () =>
        {
          _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
          _controller.WindowResized(_window.Width, _window.Height);
        };
    }

    public void AddAppLayer(IAppLayer appLayer)
    {
      _appLayers.Add(appLayer);
    }

    private static ImGuiController? _controller;
    public static ImGuiController GetController()
    {
      return _controller ?? throw new Exception("Controller Not Initialized");
    }

    public void Run()
    {
      try
      {
        // Main application loop
        while (_window.Exists)
        {
          InputSnapshot snapshot = _window.PumpEvents();
          if (!_window.Exists) { break; }
          GetController().Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

          foreach (var appLayer in _appLayers)
          {
            appLayer.OnUpdate(0.1f); // todo: this 0.1f value is blatantly wrong
            appLayer.OnUIRender();
          }

          _cl.Begin();
          _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
          _cl.ClearColorTarget(0, new RgbaFloat(clearColor.X, clearColor.Y, clearColor.Z, 1f));
          GetController().Render(_gd, _cl);
          _cl.End();
          _gd.SubmitCommands(_cl);
          _gd.SwapBuffers(_gd.MainSwapchain);
        }
      }
      finally
      {
      }
    }
  }
}