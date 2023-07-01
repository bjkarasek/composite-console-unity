using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class ShowLogsOnErrorController : CompositeElement
    {
        private ConsoleViewManager _consoleViewManager;

        protected override void OnInject()
        {
            Resolve(out _consoleViewManager);
        }

        protected override void OnActivate()
        {
            Application.logMessageReceivedThreaded += ShowLogsOnError;
        }

        private void ShowLogsOnError(string condition, string stacktrace, LogType type)
        {
            if (type is LogType.Error or LogType.Exception or LogType.Assert && _consoleViewManager.State.IsActive == false)
            {
                _consoleViewManager.Activate();
                _consoleViewManager.ToolsManager.SwitchTool(EToolType.Logging);
            }
        }

        protected override void OnDeactivate()
        {
            Application.logMessageReceivedThreaded -= ShowLogsOnError;
        }
    }
}