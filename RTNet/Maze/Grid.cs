using System.Numerics;
using System.Text;
using Zube.ImGuiNet;

namespace MazeNet
{
  public class Grid
  {
    private Random rand = new Random();
    private List<List<Cell>> _cells = new List<List<Cell>>();
    private Distances? _distances;

    protected void Initialize(int rows, int columns)
    {
      Rows = rows;
      Columns = columns;
      PrepareGrid();
      ConfigureCells();
    }

    protected virtual void PrepareGrid()
    {
      _cells = new List<List<Cell>>(Rows);
      for (int r = 0; r < Rows; r++)
      {
        _cells.Add(new List<Cell>(Columns));
        for (int c = 0; c < Columns; c++)
        {
          _cells[r].Add(new Cell(r, c));
        }
      }
    }

    protected virtual void ConfigureCells()
    {
      foreach (var cell in GetCells())
      {
        int row = cell.Row;
        int col = cell.Col;
        cell.North = GetCell(row - 1, col);
        cell.South = GetCell(row + 1, col);
        cell.East = GetCell(row, col + 1);
        cell.West = GetCell(row, col - 1);
      }
    }

    public static Grid CreateGrid<T>(int rows, int columns) where T : Grid, new()
    {
      Grid g = new T();
      g.Initialize(rows, columns);
      return g;
    }

    public virtual Cell? GetCell(int row, int col)
    {
      if (row < 0 || row >= Rows || col < 0 || col >= Columns)
      {
        return null;
      }
      return _cells[row][col];
    }

    public IEnumerable<Cell> GetCells()
    {
      foreach (var row in _cells)
      {
        foreach (var cell in row)
        {
          yield return cell;
        }
      }
    }

    public IEnumerable<List<Cell>> GetRows()
    {
      foreach (var row in _cells)
      {
        yield return row;
      }
    }

    public Cell GetRandomCell()
    {
      int row = rand.Next() % Rows;
      int column = rand.Next() % _cells[row].Count;
      return GetCell(row, column)!;
    }

    public int Size => Rows * Columns;
    public int Rows { get; private set; }
    public int Columns { get; private set; }

    public int NextRand => rand.Next();


    protected virtual string ContentsOf(Cell cell)
    {
      if (_distances != null)
      {
        var distance = _distances.GetDistance(cell).GetValueOrDefault();
        return String.Format($" {distance.ToString("x2")} ");
      }
      return "    ";
    }

    public void CalculateDistancesFrom(int row, int col)
    {
      var cell = GetCell(row, col);
      if (cell != null)
      {
        _distances = Distances.CalculateDistances(cell);
      }
    }

    public override string ToString()
    {
      var sb = new StringBuilder();

      sb.Append("+");
      for (int i = 0; i < Columns; i++)
      {
        sb.Append("----+");
      }
      sb.AppendLine();

      foreach (var row in GetRows())
      {
        var top = new StringBuilder("|");
        var bottom = new StringBuilder("+");

        foreach (var cell in row)
        {
          var useCell = cell != null ? cell : new Cell(-1, -1);
          top.Append(ContentsOf(useCell));
          top.Append(useCell.IsLinked(useCell.East) ? " " : "|");

          bottom.Append(useCell.IsLinked(useCell.South) ? "    " : "----");
          bottom.Append("+");

        }
        top.AppendLine();
        bottom.AppendLine();
        sb.Append(top);
        sb.Append(bottom);
      }
      return sb.ToString();
    }

    public PixelBuffer GetPixelBuffer(UInt32 pixelWidth, UInt32 pixelHeight)
    {
      using (var _ = new LogTimer("GetTexture()"))
      {
        var pixelBuffer = new PixelBuffer(pixelWidth, pixelHeight);

        // for (UInt32 y = 0; y < 255; y++)
        // {
        //   for (UInt32 x = 0; x < 255; x++)
        //   {
        //     pixelBuffer.SetPixel(x, y, new Vector4(
        //       ((float)(255.0 - (float)y)) / 255.0f,
        //       ((float)y / 255.0f) / 255.0f,
        //       ((float)(255.0f - (float)x)) / 255.0f,
        //       0.9f));
        //   }
        // }

        // for (UInt32 y = 0; y < 255; y++)
        // {
        //   for (UInt32 x = 400; x < 655; x++)
        //   {
        //     pixelBuffer.SetPixel(x, y, new Vector4(
        //       ((float)(255.0 - (float)y)) / 255.0f,
        //       ((float)y / 255.0f) / 255.0f,
        //       ((float)(255.0f - (float)(x - 400))) / 255.0f,
        //       0.2f));
        //   }
        // }

        for (UInt32 y = 0; y < 255; y++)
        {
          for (UInt32 x = 0; x < 255; x++)
          {
            pixelBuffer.SetPixel(x, y, new Vector3((float)y / 255.0f, 0.0f, 0.0f));
            pixelBuffer.SetPixel(x + 300, y, new Vector3(0.0f, (float)y / 255.0f, 0.0f));
            pixelBuffer.SetPixel(x + 600, y, new Vector3(0.0f, 0.0f, (float)y / 255.0f));
          }
        }

        UInt32 maxCellSizeWidth = Convert.ToUInt32(pixelWidth / Columns);
        UInt32 maxCellSizeHeight = Convert.ToUInt32(pixelHeight / Rows);

        // TODO: use largestvalue to determine rendering/centering within the 
        // available image size so we don't distort.

        // Minus 20 to get a bit of a border.
        UInt32 cellSize = Math.Min(maxCellSizeWidth, maxCellSizeHeight) - 20;

        UInt32 gridPixelWidth = Convert.ToUInt32(cellSize * Columns);
        UInt32 gridPixelHeight = Convert.ToUInt32(cellSize * Rows);

        UInt32 horizBorder = (pixelWidth - gridPixelWidth) / 2;
        UInt32 vertBorder = (pixelHeight - gridPixelHeight) / 2;

        var foreColor = new Vector3(1.0f, 0, 0);

        // using (var l = new LogTimer("Fill buffer"))
        // {
        //   imageBuffer.Fill(backColor);
        // }
        using (var l = new LogTimer("GetCells"))
        {
          foreach (var cell in GetCells())
          {
            UInt32 x1 = Convert.ToUInt32((cell.Col * cellSize) + horizBorder);
            UInt32 y1 = Convert.ToUInt32((cell.Row * cellSize) + vertBorder);
            UInt32 x2 = Convert.ToUInt32(((cell.Col + 1) * cellSize) + horizBorder);
            UInt32 y2 = Convert.ToUInt32(((cell.Row + 1) * cellSize) + vertBorder);

            if (cell.North == null)
            {
              pixelBuffer.DrawLineWithThickness(x1, y1, x2, y1, foreColor);
            }
            if (cell.West == null)
            {
              pixelBuffer.DrawLineWithThickness(x1, y1, x1, y2, foreColor);
            }
            if (!cell.IsLinked(cell.East))
            {
              pixelBuffer.DrawLineWithThickness(x2, y1, x2, y2, foreColor);
            }
            if (!cell.IsLinked(cell.South))
            {
              pixelBuffer.DrawLineWithThickness(x1, y2, x2, y2, foreColor);
            }
          }
        }

        return pixelBuffer;
      }
    }
  }
}