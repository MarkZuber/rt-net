using System.Numerics;
using ImGuiNET;
using RTNet.ImgCore;

namespace RTNet
{
  public class RayTracerAppLayer : IAppLayer
  {
    private static float _f = 0.0f;
    private static int _counter = 0;
    private static int _dragInt = 0;
    private static bool _showAnotherWindow = false;
    private static bool _showMemoryEditor = false;

    public void OnAttach()
    {
    }

    public void OnDetach()
    {
    }

    public void OnUIRender()
    {
      ImGui.Text("Hello, world!");                                        // Display some text (you can use a format string too)
      ImGui.SliderFloat("float", ref _f, 0, 1, _f.ToString("0.000"));  // Edit 1 float using a slider from 0.0f to 1.0f    
                                                                       //ImGui.ColorEdit3("clear color", ref _clearColor);                   // Edit 3 floats representing a color

      ImGui.Text($"Mouse position: {ImGui.GetMousePos()}");

      // ImGui.Checkbox("ImGui Demo Window", ref _showImGuiDemoWindow);                 // Edit bools storing our windows open/close state
      ImGui.Checkbox("Another Window", ref _showAnotherWindow);
      ImGui.Checkbox("Memory Editor", ref _showMemoryEditor);
      if (ImGui.Button("Button"))                                         // Buttons return true when clicked (NB: most widgets return true when edited/activated)
        _counter++;
      ImGui.SameLine(0, -1);
      ImGui.Text($"counter = {_counter}");

      ImGui.DragInt("Draggable Int", ref _dragInt);

      float framerate = ImGui.GetIO().Framerate;
      ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");

    }

    public void OnUpdate(float ts)
    {
    }
  }
}