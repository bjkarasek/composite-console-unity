using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CompositeConsole
{
    public class HierarchyNode
    {
        public string Name => Scene.HasValue ? Scene.Value.name : Behaviours[0].gameObject.name;

        public GameObject GameObject;
        public Scene? Scene;
        public List<MonoBehaviour> Behaviours = new();
        
        public HierarchyNode Parent;
        public List<HierarchyNode> Children = new();

        public DebugHierarchyElementView View;

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