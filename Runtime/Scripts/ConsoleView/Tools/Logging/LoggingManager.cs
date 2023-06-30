using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class LoggingManager : CompositeManager, IToolManager
    {
        public Observable<int> OnErrorsCountChanged => _logCatchController.OnErrorCountChanged;
        public int ErrorsCount { get; private set; }

        [SerializeField] private LogDetailsView LogDetailsView;
        [SerializeField] private SplitViewManager SplitViewManager;
        [SerializeField] private ScrollContentController ScrollContentController;
        private LogCatchController _logCatchController = new();

        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(_logCatchController);
            InstallChild(ScrollContentController);
            InstallChild(LogDetailsView);
            InstallChild(SplitViewManager, bindingMode: BindingMode.NonInjectable);
        }
    }
}