using System.Numerics;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace RTNet.ImgCore
{

  public class Application : IDisposable
  {
    private Sdl2Window _window;
    private GraphicsDevice _gd;
    private CommandList _cl;
    private ImGuiController _controller;

    private List<IAppLayer> _appLayers = new List<IAppLayer>();

    private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);

    private bool disposedValue;

    static void SetThing(out float i, float val) { i = val; }

    private ApplicationSpec _appSpec;

    public Application(ApplicationSpec appSpec)
    {
      _appSpec = appSpec;
    }

    public void AddAppLayer(IAppLayer appLayer)
    {
      _appLayers.Add(appLayer);
    }

    public void Run()
    {
      VeldridStartup.CreateWindowAndGraphicsDevice(
          new WindowCreateInfo(50, 50, (int)_appSpec.Width, (int)_appSpec.Height, WindowState.Normal, "ImGui.NET Sample Program"),
          new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
          out _window,
          out _gd);

      _window.Resized += () =>
        {
          _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
          _controller.WindowResized(_window.Width, _window.Height);
        };

      _cl = _gd.ResourceFactory.CreateCommandList();
      _controller = new ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);
      Random random = new Random();

      foreach (var appLayer in _appLayers)
      {
        appLayer.Initialize(_controller);
      }

      // Main application loop
      while (_window.Exists)
      {
        InputSnapshot snapshot = _window.PumpEvents();
        if (!_window.Exists) { break; }
        _controller.Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

        foreach (var appLayer in _appLayers)
        {
          appLayer.OnUIRender();
        }

        _cl.Begin();
        _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
        _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
        _controller.Render(_gd, _cl);
        _cl.End();
        _gd.SubmitCommands(_cl);
        _gd.SwapBuffers(_gd.MainSwapchain);
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          // Clean up Veldrid resources
          _gd.WaitForIdle();
          _controller.Dispose();
          _cl.Dispose();
          _gd.Dispose();
        }

        disposedValue = true;
      }
    }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
  }
}