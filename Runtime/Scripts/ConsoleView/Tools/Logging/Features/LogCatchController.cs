using System.Collections.Generic;
using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class LogCatchController : CompositeElement
    {
        public Observable<int> OnErrorCountChanged = new();
        public Observable OnLogsChanged = new();
        
        public int ErrorsCount { get; private set; }
        public int CurrentMask = 0;
        public List<LogInfo> LogInfos => _logInfos[CurrentMask];
        
        private List<LogInfo>[] _logInfos;

        private const int MaxMask = 8;

        protected override void OnInstall(DependencyInjectionContainer diContainer)
        {
            _logInfos = new List<LogInfo>[MaxMask];
            for (var i = 0; i < _logInfos.Length; i++)
            {
                _logInfos[i] = new List<LogInfo>();
            }
        }

        protected override void OnInitialize()
        {
            Application.logMessageReceivedThreaded += LogMessageReceivedThreaded;
        }

        public int GetLogsAmount(int shift)
        {
            var value = 1 << shift;
            return _logInfos[value].Count;
        }
        
        public void ChangeMask(int mask)
        {
            CurrentMask = mask;
            if (State.IsActive)
            {
                OnLogsChanged.Invoke();
            }
        }

        private void LogMessageReceivedThreaded(string condition, string stacktrace, LogType type)
        {
            if (type is LogType.Error or LogType.Exception or LogType.Assert)
            {
                ErrorsCount++;
                OnErrorCountChanged.Invoke(ErrorsCount);
            }

            var logInfo = new LogInfo()
            {
                Condition = condition,
                Stacktrace = stacktrace,
                Type = type
            };
            
            for (var i = 0; i < MaxMask; i++)
            {
                if (IsInMask(type, i))
                {
                    _logInfos[i].Add(logInfo);
                }
            }
        }

        private bool IsInMask(LogType logType, int mask)
        {
            var value = 1 << GetMaskShift(logType);
            return (mask & value) > 0;
        }

        private int GetMaskShift(LogType logType)
        {
            return logType switch
            {
                LogType.Error => 2,
                LogType.Exception => 2,
                LogType.Assert => 2,
                LogType.Warning => 1,
                LogType.Log => 0,
            };
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