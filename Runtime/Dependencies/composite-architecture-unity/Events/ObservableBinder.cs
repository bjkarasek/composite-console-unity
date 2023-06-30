using System;
using System.Collections.Generic;

namespace CompositeArchitecture
{
    public class ObservableBinder
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public void Subscribe(Observable e, Action action)
        {
            _disposables.Add(e.Subscribe(action));
        }

        public void Subscribe<T>(Observable<T> e, Action<T> action)
        {
            _disposables.Add(e.Subscribe(action));
        }

        public void UnsubscribeAll()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
            _disposables.Clear();
        }
    }
}