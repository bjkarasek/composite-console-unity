using System.Collections.Generic;
using System.Linq;
using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class CustomObjectRegistryController : CompositeElement
    {
        private readonly Dictionary<IDebugBehaviour, HashSet<object>> _customDebugObjects = new();

        public List<object> GetCustomObjects(IDebugBehaviour parent)
        {
            if (_customDebugObjects.TryGetValue(parent, out var objects))
            {
                return objects.ToList();
            }

            return new List<object>();
        }
        
        public void RegisterDebugObject(IDebugBehaviour parent, object obj)
        {
            _customDebugObjects.TryAdd(parent, new HashSet<object>());
            if (_customDebugObjects[parent].Add(obj) == false)
            {
                Debug.LogError($"Failed to register {obj} to {parent}! It has already been added!");
            }
        }

        public void UnregisterDebugObject(IDebugBehaviour parent, object obj)
        {
            if (_customDebugObjects[parent].Remove(obj) == false)
            {
                Debug.LogError($"Failed to unregister {obj} from {parent}! It's not in the registry!");
            }
        }
    }
}