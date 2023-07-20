using System;
using System.Collections.Generic;
using CompositeArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CompositeConsole
{
    public class LogElementView : MonoCompositeElement, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Observable<LogElementView> OnClicked = new();

        public LogCatchController.LogInfo LogInfo;
        
        [SerializeField] public RectTransform RectTransform;
        
        [SerializeField] private Image BackgroundImage;
        [SerializeField] private Image IconImage;
        [SerializeField] private TextMeshProUGUI FrameText;
        [SerializeField] private TextMeshProUGUI TimeText;
        [SerializeField] private TextMeshProUGUI LogTypeText;
        [SerializeField] private TextMeshProUGUI LogText;
        [SerializeField] private Color SelectedBackgroundColor;
        [SerializeField] private Color SelectedTextColor;

        [SerializeField] private List<LogSetup> LogSprites;

        private LogSetup _logSetup;
        private bool _isHovered;
        private bool _isSelected;

        public void SetupLogInfo(LogCatchController.LogInfo logInfo)
        {
            LogInfo = logInfo;
            _logSetup = GetSetup(logInfo.Type);
            IconImage.sprite = _logSetup.Sprite;
            IconImage.color = _logSetup.IconColor;
            var typeText = logInfo.Type.ToString();

            var frameString = logInfo.Frame == -1 ? "-" : logInfo.Frame.ToString();
            if (FrameText.text != frameString)
            {
                FrameText.SetText(frameString);
            }
            
            var myDate = new DateTime(logInfo.TimeTicks);
            var timeText = myDate.ToString("HH:mm:ss");

            if (TimeText.text != timeText)
            {
                TimeText.SetText(timeText);
            }
            
            if (LogTypeText.text != typeText)
            {
                LogTypeText.SetText(typeText);
            }

            if (LogText.text != logInfo.Condition)
            {
                LogText.SetText(logInfo.Condition);
            }

            RefreshColors();
        }

        private LogSetup GetSetup(LogType logType)
        {
            foreach (var logSprite in LogSprites)
            {
                if (logSprite.LogType == logType)
                {
                    return logSprite;
                }
            }

            return null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked.Invoke(this);
        }

        public void ToggleSelected(bool isSelected)
        {
            _isSelected = isSelected;
            RefreshColors();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            RefreshColors();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            RefreshColors();
        }

        private void RefreshColors()
        {
            if (_isSelected)
            {
                BackgroundImage.color = SelectedBackgroundColor;
                FrameText.color = SelectedTextColor;
                TimeText.color = SelectedTextColor;
                LogTypeText.color = SelectedTextColor;
                LogText.color = SelectedTextColor;
            }
            else if (_isHovered)
            {
                BackgroundImage.color = _logSetup.HoveredBackgroundColor;
                FrameText.color = _logSetup.TextColor;
                TimeText.color = _logSetup.TextColor;
                LogTypeText.color = _logSetup.TextColor;
                LogText.color = _logSetup.TextColor;
            }
            else
            {
                if (BackgroundImage == null)
                {
                    Debug.Log($"{name}", this);
                }
                if (_logSetup == null)
                {
                    Debug.Log($"{name}", this);
                }
                BackgroundImage.color = _logSetup.BackgroundColor;
                FrameText.color = _logSetup.TextColor;
                TimeText.color = _logSetup.TextColor;
                LogTypeText.color = _logSetup.TextColor;
                LogText.color = _logSetup.TextColor;
            }
        }
        
        [Serializable]
        private class LogSetup
        {
            public LogType LogType;
            public Sprite Sprite;
            public Color IconColor;
            public Color BackgroundColor;
            public Color TextColor;
            public Color HoveredBackgroundColor;
        }
    }
}