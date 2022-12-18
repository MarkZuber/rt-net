namespace MazeNet.Algorithms
{
    public class BinaryTree : GridExecutor
    {
        public BinaryTree(Grid grid) : base(grid)
        {
        }

        public override IEnumerable<bool> ExecuteStep()
        {
            foreach (var cell in Grid.GetCells())
            {
                var neighbors = new List<Cell>();
                if (cell.North != null)
                { neighbors.Add(cell.North); }
                if (cell.East != null)
                { neighbors.Add(cell.East); }

                if (neighbors.Count > 0)
                {
                    int index = Grid.NextRand % neighbors.Count;
                    var neighbor = neighbors[index];
                    if (neighbor != null)
                    {
                        cell.Link(neighbor);
                    }
                }
                yield return true;
            }
        }
    }
}