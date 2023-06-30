using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class ConsoleViewManager : CompositeManager
    {
        [SerializeField] private TopToolbarManager TopToolbarManager;
        [SerializeField] private ResizeController ResizeController;
        [SerializeField] private ToolsManager ToolsManager;
        
        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(TopToolbarManager);
            InstallChild(ResizeController);
            InstallChild(ToolsManager);
        }
    }
}