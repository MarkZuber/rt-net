namespace MazeNet
{
    public class LogTimer : IDisposable
    {
        private string _message;
        private DateTime _startTime;
        public LogTimer(string message)
        {
            _message = message;
            _startTime = DateTime.Now;
        }

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    var span = DateTime.Now - _startTime;
                    Console.WriteLine($"{_message} took {span.TotalMilliseconds}ms");
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}