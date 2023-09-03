using System.Collections.Generic;
using CompositeArchitecture;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CompositeConsole
{
    public class HierarchyFindingController : CompositeElement
    {
        private Dictionary<MonoBehaviour, HierarchyNode> _monoBehaviourDebugObjects = new();
        private Dictionary<GameObject, HierarchyNode> _debugGameObjects = new();
        private List<MonoBehaviour> _subBehaviours = new();
        
        private CustomObjectRegistryController _customObjectRegistryController;

        protected override void OnInject()
        {
            Resolve(out _customObjectRegistryController);
        }

        public List<HierarchyNode> BuildHierarchy()
        {
            var result = new List<HierarchyNode>();

            var allMonoBehaviours = Object.FindObjectsOfType<MonoBehaviour>(true);
            _monoBehaviourDebugObjects.Clear();
            _debugGameObjects.Clear();
            _subBehaviours.Clear();
            
            foreach (var monoBehaviour in allMonoBehaviours)
            {
                var go = monoBehaviour.gameObject;
                if (monoBehaviour is IDebugBehaviour debugBehaviour)
                {
                    _debugGameObjects.TryAdd(go, new HierarchyNode());
                    var node = _debugGameObjects[go];
                    _monoBehaviourDebugObjects.TryAdd(monoBehaviour, node);
                    node.GameObject = go;
                    node.Behaviours.Add(monoBehaviour);
                    node.CustomDebugObjects = _customObjectRegistryController.GetCustomObjects(debugBehaviour);
                }
                else if (monoBehaviour is IDebugSubBehaviour)
                {
                    _subBehaviours.Add(monoBehaviour);
                }
            }

            foreach (var (go, node) in _debugGameObjects)
            {
                var parent = go.transform.parent;
                HierarchyNode parentNode = null;
                while (parent != null)
                {
                    var parentGo = parent.gameObject;
                    if (_debugGameObjects.TryGetValue(parentGo, out parentNode)) break;
                    parent = parent.parent;
                }

                if (parentNode != null)
                {
                    parentNode.Children.Add(node);
                    node.Parent = parentNode;
                }
                else
                {
                    result.Add(node);
                }
            }

            foreach (var subBehaviour in _subBehaviours)
            {
                var parent = subBehaviour.transform.parent;
                HierarchyNode parentNode = null;
                while (parent != null)
                {
                    var parentGo = parent.gameObject;
                    if (_debugGameObjects.TryGetValue(parentGo, out parentNode)) break;
                    parent = parent.parent;
                }

                if (parentNode != null)
                {
                    parentNode.Behaviours.Add(subBehaviour);
                }
                else
                {
                    Debug.LogWarning($"{subBehaviour.name} is {nameof(IDebugSubBehaviour)} but has no parent!");
                }
            }

            return result;
        }
    }
}