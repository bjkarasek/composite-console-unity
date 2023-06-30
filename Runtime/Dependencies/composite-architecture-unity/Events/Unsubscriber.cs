using System;
using System.Collections.Generic;

namespace CompositeArchitecture
{
    public class Unsubscriber<T> : IDisposable
    {
        private readonly List<Action<T>> _observers;
        private readonly Action<T> _observer;

        public Unsubscriber(List<Action<T>> observers, Action<T> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            _observers.Remove(_observer);
        }
    }

    public class Unsubscriber : IDisposable
    {
        private readonly List<Action> _observers;
        private readonly Action _observer;

        public Unsubscriber(List<Action> observers, Action observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            _observers.Remove(_observer);
        }
    }
}