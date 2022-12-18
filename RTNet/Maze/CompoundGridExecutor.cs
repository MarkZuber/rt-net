namespace MazeNet
{
    public class CompoundGridExecutor : GridExecutor
    {
        private readonly List<GridExecutor> _executors;

        public CompoundGridExecutor(Grid grid, IEnumerable<GridExecutor> executors) : base(grid)
        {
            _executors = new List<GridExecutor>(executors);
        }

        public void Add(GridExecutor executor)
        {
            _executors.Add(executor);
        }

        public override IEnumerable<bool> ExecuteStep()
        {
            foreach (var executor in Concatenate(_executors))
            {
                foreach (var s in executor.ExecuteStep())
                {
                    yield return s;
                }
            }
        }

        public static IEnumerable<T> Concatenate<T>(params IEnumerable<T>[] lists)
        {
            return lists.SelectMany(x => x);
        }
    }
}