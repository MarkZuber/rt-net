namespace MazeNet.Algorithms
{
    public class Sidewinder : GridExecutor
    {
        public Sidewinder(Grid grid) : base(grid)
        {
        }

        public override IEnumerable<bool> ExecuteStep()
        {
            foreach (var row in Grid.GetRows())
            {
                var run = new List<Cell>();
                foreach (var cell in row)
                {
                    run.Add(cell);
                    bool atEasternBoundary = cell.East == null;
                    bool atNorthernBounday = cell.North == null;
                    bool shouldCloseOut = atEasternBoundary || (!atNorthernBounday && (Grid.NextRand % 2) == 0);

                    if (shouldCloseOut)
                    {
                        // TODO: make this an extension method of list to
                        // sample an item
                        var runRandIndex = (Grid.NextRand % run.Count);
                        var member = run[runRandIndex];
                        if (member.North != null)
                        {
                            member.Link(member.North);
                        }
                        run.Clear();
                    }
                    else
                    {
                        cell.Link(cell.East);
                    }

                    yield return true;
                }
            }
        }
    }
}