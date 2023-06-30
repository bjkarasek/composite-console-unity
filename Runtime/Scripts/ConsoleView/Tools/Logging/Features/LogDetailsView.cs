using System;
using System.Collections.Generic;
using System.Linq;
using CompositeArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CompositeConsole
{
    public class LogDetailsView : MonoCompositeElement
    {
        [SerializeField] private TextMeshProUGUI LogTitle;
        [SerializeField] private TextMeshProUGUI LogStacktrace;
        [SerializeField] private RectTransform Content;
        [SerializeField] private ScrollRect ScrollRect;
        [SerializeField] private Button CopyToClipboardButton;
        [SerializeField] private TextMeshProUGUI CopyInfoText;
        [SerializeField] private VerticalLayoutGroup LayoutGroup;

        [SerializeField] private List<RectTransform> ContentElementsExcludingText;

        private RectTransform _titleRT;
        private RectTransform _stacktraceRT;

        private LogCatchController.LogInfo _logInfo;

        private float? _resetCopyInfoTextTime;

        private int _needRefresh = 0;
        
        protected override void OnInitialize()
        {
            Content.gameObject.SetActive(false);
            LogTitle.SetText("");
            LogStacktrace.SetText("");

            _titleRT = LogTitle.rectTransform;
            _stacktraceRT = LogStacktrace.rectTransform;
        }

        protected override void OnActivate()
        {
            CopyToClipboardButton.onClick.AddListener(CopyToClipboard);
        }

        private void CopyToClipboard()
        {
            if (_logInfo != null)
            {
                GUIUtility.systemCopyBuffer = $"{_logInfo.Condition}\n{_logInfo.Stacktrace}";
                CopyInfoText.SetText($"Copied!");
                CopyInfoText.color = Color.green;
                _resetCopyInfoTextTime = 1f;
            }
        }

        protected override void OnRefresh()
        {
            if (_resetCopyInfoTextTime.HasValue)
                _resetCopyInfoTextTime = _resetCopyInfoTextTime.Value - Time.deltaTime;
            if (_resetCopyInfoTextTime is <= 0)
            {
                _resetCopyInfoTextTime = null;
                CopyInfoText.SetText($"Copy to clipboard");
                CopyInfoText.color = Color.white;
            }

            RefreshRectSize();
        }

        public void SetLogDetails(LogCatchController.LogInfo logInfo)
        {
            _logInfo = logInfo;
            if (logInfo != null)
            {
                Content.gameObject.SetActive(true);
                LogTitle.SetText(logInfo.Condition);
                LogStacktrace.SetText(logInfo.Stacktrace);
                LogTitle.color = GetColor(logInfo.Type);
                RefreshRectSize();
                _needRefresh = 2;
            }
            else
            {
                Clear();
            }
        }

        private Color GetColor(LogType logInfoType)
        {
            switch (logInfoType)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    return Color.red;
                case LogType.Warning:
                    return Color.yellow;
                case LogType.Log:
                    return Color.white;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logInfoType), logInfoType, null);
            }
        }

        private void Clear()
        {
            Content.gameObject.SetActive(false);
            LogTitle.SetText("");
            LogStacktrace.SetText("");
            RefreshRectSize();
            _needRefresh = 2;
        }
        
        private void RefreshRectSize()
        {
            var height = ContentElementsExcludingText.Sum(rt => rt.sizeDelta.y);
            var titleHeight = LogTitle.textBounds.max.y - LogTitle.textBounds.min.y;
            var stacktraceHeight = LogStacktrace.textBounds.max.y - LogStacktrace.textBounds.min.y;

            _titleRT.sizeDelta = new Vector2(_titleRT.sizeDelta.x, titleHeight);
            _stacktraceRT.sizeDelta = new Vector2(_stacktraceRT.sizeDelta.x, stacktraceHeight);
            var marginHeight = 30;
            var spacingHeight = LayoutGroup.spacing * (2 + ContentElementsExcludingText.Count);
            Content.sizeDelta = new Vector2(Content.sizeDelta.x, height + titleHeight + stacktraceHeight + spacingHeight + marginHeight);
        }

        protected override void OnDeactivate()
        {
            CopyToClipboardButton.onClick.RemoveAllListeners();
        }
    }
}