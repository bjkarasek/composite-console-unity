using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class ConsoleViewManager : CompositeManager
    {
        [SerializeField] public ToolsManager ToolsManager;
        [SerializeField] private TopToolbarManager TopToolbarManager;
        [SerializeField] private ResizeController ResizeController;
        
        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(TopToolbarManager);
            InstallChild(ResizeController);
            InstallChild(ToolsManager);
        }
    }
}