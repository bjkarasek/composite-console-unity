using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CompositeConsole
{
    public class HierarchyNode
    {
        public string Name => Scene.HasValue ? Scene.Value.name : GetBehaviourName();

        public GameObject GameObject;
        public Scene? Scene;
        public List<MonoBehaviour> Behaviours = new();
        
        public HierarchyNode Parent;
        public List<HierarchyNode> Children = new();

        public DebugHierarchyElementView View;

        private MonoBehaviour RootBehaviour => Behaviours[0];
        
        private string GetBehaviourName()
        {
            return RootBehaviour is IDebugBehaviourNamed behaviourNamed
                ? behaviourNamed.DebugHierarchyName
                : GameObject.name;
        }
        
        public int CalculateDepth()
        {
            return Parent == null ? 0 : 1 + Parent.CalculateDepth();
        }
        
        public override string ToString()
        {
            var log = $"";
            for (var i = 0; i < CalculateDepth(); i++)
            {
                log += "    ";
            }
            if (Scene.HasValue)
            {
                log += $"[SCENE] {Name}\n";
            }
            else
            {
                log += $"[GAME OBJECT] {Name}\n";
            }

            foreach (var child in Children)
            {
                log += child.ToString();
            }

            return log;
        }
    }
}