using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class ConsoleViewManager : CompositeManager
    {
        [SerializeField] private Canvas Canvas;
        [SerializeField] public ToolsManager ToolsManager;
        [SerializeField] private TopToolbarManager TopToolbarManager;
        [SerializeField] private ResizeController ResizeController;
        
        protected override void OnInstall(DependencyInjectionContainer container)
        {
            BindChild(Canvas);
            InstallChild(TopToolbarManager);
            InstallChild(ResizeController);
            InstallChild(ToolsManager);
        }

        protected override void OnActivate()
        {
            Canvas.ForceUpdateCanvases();
        }
    }
}