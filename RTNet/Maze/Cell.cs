namespace MazeNet
{
    public class Cell
    {
        private readonly Dictionary<Cell, bool> _links = new Dictionary<Cell, bool>();

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public int Row { get; }
        public int Col { get; }

        public Cell? North { get; set; }
        public Cell? South { get; set; }
        public Cell? East { get; set; }
        public Cell? West { get; set; }

        public void Link(Cell? other, bool bidi = true)
        {
            if (other == null)
            {
                return;
            }
            _links[other] = true;
            if (bidi)
            {
                other.Link(this, false);
            }
        }

        public void Unlink(Cell? other, bool bidi = true)
        {
            if (other == null)
            {
                return;
            }
            _links.Remove(other);
            if (bidi)
            {
                other.Unlink(this, false);
            }
        }

        public IEnumerable<Cell> Links => _links.Keys;

        public bool IsLinked(Cell? cell)
        {
            if (cell == null)
            {
                return false;
            }
            return _links.ContainsKey(cell);
        }

        public IEnumerable<Cell> Neighbors
        {
            get
            {
                var neighbors = new List<Cell>();
                if (North != null)
                { neighbors.Append(North); }
                if (South != null)
                { neighbors.Append(South); }
                if (East != null)
                { neighbors.Append(East); }
                if (West != null)
                { neighbors.Append(West); }
                return neighbors;
            }
        }
    }
}