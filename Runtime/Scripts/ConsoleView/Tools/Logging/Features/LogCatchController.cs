using System.Collections.Generic;
using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class LogCatchController : CompositeElement
    {
        public Observable<int> OnErrorCountChanged = new();
        public int ErrorsCount { get; private set; }
        public List<LogInfo> LogInfos = new();
        
        protected override void OnInitialize()
        {
            Application.logMessageReceivedThreaded += LogMessageReceivedThreaded;
        }

        private void LogMessageReceivedThreaded(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                ErrorsCount++;
                OnErrorCountChanged.Invoke(ErrorsCount);
            }
            
            LogInfos.Add(new LogInfo()
            {
                Condition = condition,
                Stacktrace = stacktrace,
                Type = type
            });
        }

        protected override void OnDeinitialize()
        {
            Application.logMessageReceivedThreaded -= LogMessageReceivedThreaded;
        }

        public class LogInfo
        {
            public string Condition;
            public string Stacktrace;
            public LogType Type;
        }
    }
}