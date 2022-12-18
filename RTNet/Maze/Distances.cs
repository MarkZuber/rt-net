namespace MazeNet
{
    public class Distances
    {
        private readonly Dictionary<Cell, Int32> _cells;
        private readonly Cell _root;

        public Distances(Cell root)
        {
            _root = root;
            _cells = new Dictionary<Cell, int>();
            _cells[_root] = 0;
        }

        public IEnumerable<Cell> Cells => _cells.Keys;

        public Int32? GetDistance(Cell cell)
        {
            int val = 0;
            if (_cells.TryGetValue(cell, out val))
            {
                return val;
            }
            return null;
        }
        public void SetDistance(Cell cell, Int32 distance) => _cells[cell] = distance;

        public static Distances CalculateDistances(Cell cell)
        {
            var distances = new Distances(cell);
            var frontier = new List<Cell>() { cell };

            while (frontier.Any())
            {
                var newFrontier = new List<Cell>();

                foreach (var c in frontier)
                {
                    foreach (var linked in c.Links)
                    {
                        if (distances.GetDistance(linked) != null)
                        {
                            continue;
                        }
                        distances.SetDistance(linked, distances.GetDistance(c).GetValueOrDefault() + 1);
                        newFrontier.Add(linked);
                    }
                }
                frontier = newFrontier;
            }

            return distances;
        }
    }
}