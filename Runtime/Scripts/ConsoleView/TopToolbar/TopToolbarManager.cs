using System.Collections.Generic;
using CompositeArchitecture;
using UnityEngine;
using UnityEngine.UI;

namespace CompositeConsole
{
    public class TopToolbarManager : CompositeManager
    {
        [SerializeField] private MinimizeMaximizeController MinimizeMaximizeController;

        [SerializeField] private LoggingTabButtonView LoggingTabButtonView;
        [SerializeField] private TabButtonView DebugHierarchyButton;
        
        private ToolsManager _toolsManager;

        private List<(TabButtonView, EToolType)> _tabButtons;
        
        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(MinimizeMaximizeController);
            InstallChild(LoggingTabButtonView);
            
            _tabButtons = new List<(TabButtonView, EToolType)>
            {
                (LoggingTabButtonView, EToolType.Logging),
                (DebugHierarchyButton, EToolType.DebugHierarchy),
            };
        }

        protected override void OnInject()
        {
            Resolve(out _toolsManager);
        }

        protected override void OnActivate()
        {
            Subscribe(_toolsManager.OnToolChanged, RefreshCurrentToolButton);
            RefreshCurrentToolButton(_toolsManager.CurrentToolType);
                
            LoggingTabButtonView.Button.onClick.AddListener(() => _toolsManager.SwitchTool(EToolType.Logging));
            DebugHierarchyButton.Button.onClick.AddListener(() => _toolsManager.SwitchTool(EToolType.DebugHierarchy));
        }

        private void RefreshCurrentToolButton(EToolType currentToolType)
        {
            foreach (var (buttonView, toolType) in _tabButtons)
            {
                buttonView.ToggleSelected(toolType == currentToolType);
            }
        }

        protected override void OnDeactivate()
        {
            LoggingTabButtonView.Button.onClick.RemoveAllListeners();
            DebugHierarchyButton.Button.onClick.RemoveAllListeners();
        }
    }
}