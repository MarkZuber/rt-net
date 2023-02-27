using Zube.ImGuiNet;

namespace MazeNet
{
  public abstract class GridExecutor
  {
    protected GridExecutor(Grid grid)
    {
      Grid = grid;
    }

    public Grid Grid { get; }

    public void Execute()
    {
      foreach (var _ in ExecuteStep())
        ;
    }

    public abstract IEnumerable<bool> ExecuteStep();

    public virtual PixelBuffer GetPixelBuffer(UInt32 pixelWidth, UInt32 pixelHeight)
    {
      return Grid.GetPixelBuffer(pixelWidth, pixelHeight);
    }

    public override string ToString()
    {
      return Grid.ToString();
    }
  }
}