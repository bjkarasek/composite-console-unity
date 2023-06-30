using System;
using System.Collections.Generic;

namespace CompositeArchitecture
{
    public class Observable
    {
        private readonly List<Action> _observers = new();
        private readonly List<Action> _temporaryObservers = new();

        public IDisposable Subscribe(Action observer)
        {
            _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        public void Unsubscribe(Action observer)
        {
            _observers.Remove(observer);
        }

        public void Invoke()
        {
            _temporaryObservers.Clear();
            foreach (var observer in _observers)
                _temporaryObservers.Add(observer);
            foreach (var observer in _temporaryObservers)
                observer.Invoke();
        }
    }

    public class Observable<T>
    {
        private readonly List<Action<T>> _observers = new();
        private readonly List<Action<T>> _temporaryObservers = new();

        public IDisposable Subscribe(Action<T> observer)
        {
            _observers.Add(observer);
            return new Unsubscriber<T>(_observers, observer);
        }

        public void Unsubscribe(Action<T> observer)
        {
            _observers.Remove(observer);
        }

        public void Invoke(T argument)
        {
            _temporaryObservers.Clear();
            foreach (var observer in _observers)
                _temporaryObservers.Add(observer);
            foreach (var observer in _temporaryObservers)
                observer.Invoke(argument);
        }
    }
}