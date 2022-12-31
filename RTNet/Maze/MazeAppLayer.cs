using System.Numerics;
using ImGuiNET;
using MazeNet;
using RTNet.ImgCore;
using Veldrid;
using Veldrid.ImageSharp;

namespace RTNet.Maze
{
  public class MazeAppLayer : IAppLayer
  {
    private UInt32 _viewportWidth;
    private UInt32 _viewportHeight;
    private double _lastRenderTime;
    private readonly TimeSpan _executeDelta = TimeSpan.FromMilliseconds(1);
    private bool _shouldAnimate = true;
    private int _stepsPerAnimation = 10;  // walk this many steps through the enumerator before rendering a new texture.
    private DateTime time = DateTime.Now;
    private IEnumerator<bool> _enum;
    private bool _hasConsoleWritten = false;
    private GridExecutor _executor;


    public MazeAppLayer()
    {
      int mazeRows = 10;
      int mazeColumns = 10;
      var grid = Grid.CreateGrid<Grid>(mazeRows, mazeColumns);
      _executor = new CompoundGridExecutor(grid,
          new List<GridExecutor>() { new MazeNet.Algorithms.BinaryTree(grid), new MazeNet.Algorithms.Sidewinder(grid) });
      _enum = _executor.ExecuteStep().GetEnumerator();
    }

    public void Initialize(ImGuiController controller)
    {
    }

    public void OnAttach()
    {
    }

    public void OnDetach()
    {
    }

    private void Render()
    {
      var start = DateTime.UtcNow;



      var end = DateTime.UtcNow;
      _lastRenderTime = (end - start).TotalMilliseconds;
    }

    public void OnUIRender()
    {
      ImGui.Begin("Settings");
      ImGui.Text(String.Format("Last Render: {0,3}ms", _lastRenderTime));
      if (ImGui.Button("Render"))
      {
        Render();
      }
      ImGui.End();

      ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0.0f);

      ImGui.Begin("Viewport");

      _viewportWidth = (UInt32)ImGui.GetContentRegionAvail().X;
      _viewportHeight = (UInt32)ImGui.GetContentRegionAvail().Y;


      if (_shouldAnimate && DateTime.Now > time + _executeDelta && _enum.MoveNext())
      {
        if (_stepsPerAnimation > 1)
        {
          for (int i = 0; i < _stepsPerAnimation - 1; ++i)
          {
            _enum.MoveNext();
          }
        }
        // _texture = _executor.GetTexture(this.ClientSize.X, this.ClientSize.Y);
        time = DateTime.Now;
      }

      if (!_enum.MoveNext() && !_hasConsoleWritten)
      {
        _executor.Grid.CalculateDistancesFrom(0, 0);
        Console.WriteLine(_executor.ToString());
        _hasConsoleWritten = true;
      }

      var finalImgPtr = _executor.GetImageBuffer(_viewportWidth, _viewportHeight).CaptureImageBufferPointer();
      if (finalImgPtr != IntPtr.Zero)
      {
        ImGui.Image(finalImgPtr, new Vector2(_viewportWidth, _viewportHeight));
      }

      ImGui.End();
      // Not sure why this crashes the app...
      // ImGui.PopStyleVar();

      Render();

      float framerate = ImGui.GetIO().Framerate;
      ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");
    }

    public void OnUpdate(float ts)
    {
    }
  }
}