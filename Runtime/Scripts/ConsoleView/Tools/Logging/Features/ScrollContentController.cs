using System.Collections.Generic;
using CompositeArchitecture;
using UnityEngine;
using UnityEngine.UI;

namespace CompositeConsole
{
    public class ScrollContentController : MonoCompositeElement
    {
        [SerializeField] private RectTransform ScrollContent;
        [SerializeField] private RectTransform RealContent;
        [SerializeField] private ScrollRect ScrollRect;
        
        [SerializeField] private CompositePool<LogElementView> LogViewPool;
        
        private List<LogElementView> LogElementViews => LogViewPool.SpawnedItems;
        private LogCatchController _logCatchController;
        private LogDetailsView _logDetailsView;
        
        private float _logViewHeight = -1;

        private float ScrollHeight => ScrollRect.normalizedPosition.y;

        private float _previousScrollHeight = -1;
        private float _contentHeight = -1;
        private int _logsCount = -1;

        private LogCatchController.LogInfo _selectedLogInfo;
        
        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(LogViewPool);
        }

        protected override void OnInject()
        {
            Resolve(out _logCatchController);
            Resolve(out _logDetailsView);
        }

        protected override void OnInitialize()
        {
            _logViewHeight = LogViewPool.Prefab.GetComponent<RectTransform>().sizeDelta.y;
        }

        protected override void OnActivate()
        {
            RefreshScrollContent();
        }

        protected override void OnRefresh()
        {
            var scrollHeight = ScrollHeight;
            ScrollContent.sizeDelta = new Vector2(ScrollContent.sizeDelta.x, _logCatchController.LogInfos.Count * _logViewHeight);
            
            if (Mathf.Abs(scrollHeight) < 0.0001f)
            {
                ScrollRect.normalizedPosition = new Vector2(0f, 0f);
            }
            
            if (CheckViewNeedsChange())
            {
                RefreshScrollContent();
            }
        }

        private bool CheckViewNeedsChange()
        {
            var needChange = false;
            
            var contentHeight = RealContent.rect.height;
            needChange |= Mathf.Approximately(contentHeight, _contentHeight) == false;
            _contentHeight = contentHeight;

            needChange |= Mathf.Approximately(_previousScrollHeight, ScrollHeight) == false;
            _previousScrollHeight = ScrollHeight;

            needChange |= _logsCount != _logCatchController.LogInfos.Count;
            _logsCount = _logCatchController.LogInfos.Count;

            return needChange;
        }

        private void RefreshScrollContent()
        {
            var logsInContent = _contentHeight / _logViewHeight;
            var logsToSpawn = Mathf.CeilToInt(logsInContent) + 1;
            var logPosition = Mathf.Max(0, (_logCatchController.LogInfos.Count - logsInContent) * (1 - ScrollHeight));

            var shift = logPosition % 1;

            while (LogElementViews.Count < logsToSpawn)
            {
                var item = LogViewPool.Spawn(activate: false);
                Subscribe(item.OnClicked, ChangeSelection);
            }

            var initialIndex = Mathf.FloorToInt(logPosition);
            var index = 0;
            for (index = 0; index < logsToSpawn && initialIndex + index < _logCatchController.LogInfos.Count; index++)
            {
                var logInfo = _logCatchController.LogInfos[initialIndex + index];
                var logView = LogElementViews[index];
                logView.RectTransform.anchoredPosition = new Vector2(0, -((index - shift) * _logViewHeight));
                logView.SetupLogInfo(logInfo);
                logView.ToggleActive(true);
                logView.transform.SetSiblingIndex(index);
                logView.ToggleSelected(_selectedLogInfo == logInfo);
            }

            for (; index < LogElementViews.Count; index++)
            {
                LogElementViews[index].ToggleActive(false);
            }
        }

        private void ChangeSelection (LogElementView logElementView) => ChangeSelection(logElementView.LogInfo);
        
        private void ChangeSelection(LogCatchController.LogInfo logInfo)
        {
            _selectedLogInfo = logInfo;
            foreach (var view in LogElementViews)
            {
                if (view.State.IsActive)
                {
                    view.ToggleSelected(_selectedLogInfo == view.LogInfo);
                }
            }
            
            _logDetailsView.SetLogDetails(_selectedLogInfo);
        }
    }
}