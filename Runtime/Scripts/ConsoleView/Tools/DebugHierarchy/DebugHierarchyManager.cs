using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class DebugHierarchyManager : CompositeManager, IToolManager
    {
        [SerializeField] private HierarchyController HierarchyController;
        [SerializeField] private SplitViewManager SplitViewManager;

        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(HierarchyController);
            InstallChild(SplitViewManager, bindingMode: BindingMode.NonInjectable);
        }
    }
}