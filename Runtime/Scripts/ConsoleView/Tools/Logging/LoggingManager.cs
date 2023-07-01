using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class LoggingManager : CompositeManager, IToolManager
    {
        public Observable<int> OnErrorsCountChanged => _logCatchController.OnErrorCountChanged;
        public int ErrorsCount => _logCatchController.GetLogsAmount((int)ELogType.Error);

        [SerializeField] private LogDetailsView LogDetailsView;
        [SerializeField] private SplitViewManager SplitViewManager;
        [SerializeField] private ScrollContentController ScrollContentController;
        [SerializeField] private LogTypeTogglingController LogTypeTogglingController;
        private LogCatchController _logCatchController = new();

        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(_logCatchController);
            InstallChild(ScrollContentController);
            InstallChild(LogTypeTogglingController);
            InstallChild(LogDetailsView);
            InstallChild(SplitViewManager, bindingMode: BindingMode.NonInjectable);
        }
    }
}