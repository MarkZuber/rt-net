using System;
using System.Diagnostics;

namespace WkndRay
{
    public class BlockTimer : IDisposable
    {
        private readonly Stopwatch _sw;
        private readonly Action<long> _resultSetter;

        public BlockTimer(Action<long> resultSetter)
        {
            _resultSetter = resultSetter;
            _sw = Stopwatch.StartNew();
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _sw.Stop();
                    _resultSetter(_sw.ElapsedMilliseconds);
                }

                _disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
