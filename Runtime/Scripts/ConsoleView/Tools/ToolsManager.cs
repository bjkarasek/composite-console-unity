using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class ToolsManager : CompositeManager
    {
        public Observable<EToolType> OnToolChanged = new();
        
        public EToolType CurrentToolType
        {
            get => (EToolType)PlayerPrefs.GetInt(ConsoleCurrentVisibleToolPrefKey);
            private set => PlayerPrefs.SetInt(ConsoleCurrentVisibleToolPrefKey, (int)value);
        }
        
        [SerializeField] private DebugHierarchyManager DebugHierarchyManager;
        [SerializeField] private LoggingManager LoggingManager;

        private IToolManager CurrentTool => GetToolManager(CurrentToolType);
        
        private const string ConsoleCurrentVisibleToolPrefKey = "CONSOLE_CurrentVisibleTool";

        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(DebugHierarchyManager, LifecycleFlags.WithoutActivate);
            InstallChild(LoggingManager, LifecycleFlags.WithoutActivate);
        }

        protected override void OnActivate()
        {
            CurrentTool.Activate();
        }

        public void SwitchTool(EToolType toolType)
        {
            if (CurrentToolType != toolType)
            {
                CurrentTool.Deactivate();
                CurrentToolType = toolType;
                CurrentTool.Activate();
                
                OnToolChanged.Invoke(CurrentToolType);
            }
        }

        private IToolManager GetToolManager(EToolType toolType)
        {
            return toolType switch
            {
                EToolType.DebugHierarchy => DebugHierarchyManager,
                EToolType.Logging => LoggingManager,
                _ => null
            };
        }
    }
}