using System;
using System.Collections.Generic;
using UnityEngine;

namespace CompositeArchitecture
{
    public class DependencyInjectionContainer
    {
        public readonly DependencyInjectionContainer Parent;
        
        private readonly Dictionary<Type, object> _objects = new();
        private readonly HashSet<Type> _ignoredTypes = new();

        public DependencyInjectionContainer(DependencyInjectionContainer parent = null)
        {
            Parent = parent;
        }

        public void Bind<T>(T obj, bool withInterfaces = true)
        {
            Bind(obj, typeof(T), withInterfaces);
        }

        public void Bind(object obj, Type type, bool withInterfaces = true)
        {
            if (obj == null) Debug.LogError($"Trying to Bind <color=#ff0000>{type}</color> but it's null!");

            if (_objects.ContainsKey(type) == false)
            {
                _objects.Add(type, obj);
            }
            else
            {
                Debug.LogError($"Type <color=#ff0000>{type}</color> is already bind!");
            }

            if (withInterfaces)
            {
                BindInterfaces(obj, type);
            }
        }

        private void BindInterfaces(object obj, Type originType)
        {
            foreach (var type in originType.GetInterfaces())
            {
                if (_objects.ContainsKey(type) == false && _ignoredTypes.Contains(type) == false)
                {
                    _objects.TryAdd(type, obj);
                }
                else
                {
                    _ignoredTypes.Add(type);
                }
            }
        }

        public void Resolve<T>(out T value, object debugObject = null)
        {
            var type = typeof(T);

            if (_ignoredTypes.Contains(type) == false && _objects.TryGetValue(type, out var o))
            {
                value = (T)o;
            }
            else if (Parent == null)
            {
                var log = $"Failed to resolve <b>{typeof(T).Name}</b>";

                if (debugObject != null)
                {
                    log += $" in <b>{debugObject.GetType().Name}</b>";
                }
                Debug.LogError(log);

                value = default;
            }
            else
            {
                Parent.Resolve(out value, debugObject);
            }
        }
    }
}