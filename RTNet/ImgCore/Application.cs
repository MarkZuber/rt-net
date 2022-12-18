using System.Numerics;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace RTNet.ImgCore
{
  public class Application
  {
    private List<IAppLayer> _appLayers = new List<IAppLayer>();

    private static Vector3 clearColor = new Vector3(0.45f, 0.55f, 0.6f);

    private ApplicationSpec _appSpec;

    public Application(ApplicationSpec appSpec)
    {
      _appSpec = appSpec;
    }

    public void AddAppLayer(IAppLayer appLayer)
    {
      _appLayers.Add(appLayer);
    }

    public void Run(AppInfo appInfo)
    {
      Sdl2Window window;
      GraphicsDevice gd;
      CommandList cl;
      ImGuiController controller;

      try
      {
        VeldridStartup.CreateWindowAndGraphicsDevice(
            new WindowCreateInfo(50, 50, (int)_appSpec.Width, (int)_appSpec.Height, WindowState.Normal, _appSpec.Name),
            new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
            out window,
            out gd);

        cl = gd.ResourceFactory.CreateCommandList();
        controller = new ImGuiController(gd, gd.MainSwapchain.Framebuffer.OutputDescription, window.Width, window.Height);
        Random random = new Random();

        window.Resized += () =>
          {
            gd.MainSwapchain.Resize((uint)window.Width, (uint)window.Height);
            controller.WindowResized(window.Width, window.Height);
          };

        appInfo.Initialize(controller);

        // Main application loop
        while (window.Exists)
        {
          InputSnapshot snapshot = window.PumpEvents();
          if (!window.Exists) { break; }
          controller.Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

          foreach (var appLayer in _appLayers)
          {
            appLayer.OnUpdate(0.1f); // todo: this 0.1f value is blatantly wrong
            appLayer.OnUIRender();
          }

          cl.Begin();
          cl.SetFramebuffer(gd.MainSwapchain.Framebuffer);
          cl.ClearColorTarget(0, new RgbaFloat(clearColor.X, clearColor.Y, clearColor.Z, 1f));
          controller.Render(gd, cl);
          cl.End();
          gd.SubmitCommands(cl);
          gd.SwapBuffers(gd.MainSwapchain);
        }
      }
      finally
      {
      }
    }
  }
}