using CompositeArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CompositeConsole
{
    public class LogTypeToggleView : MonoCompositeElement
    {
        public Observable OnToggled = new();
        
        [SerializeField] private ELogType HandledLogType;
        [SerializeField] private Button Button;
        [SerializeField] private Image SelectedImage;
        [SerializeField] private TextMeshProUGUI LogsAmount;

        private string PrefKey => $"LogType{HandledLogType}PrefKey";

        private LogCatchController _logCatchController;

        private int _lastLogsAmount;
        
        public bool IsToggled
        {
            get => PlayerPrefs.GetInt(PrefKey, 1) == 1;
            private set => PlayerPrefs.SetInt(PrefKey, value ? 1 : 0);
        }

        protected override void OnInject()
        {
            Resolve(out _logCatchController);
        }

        protected override void OnActivate()
        {
            Button.onClick.AddListener(Toggle);
            SelectedImage.enabled = IsToggled;
        }

        protected override void OnRefresh()
        {
            var logsAmount = _logCatchController.GetLogsAmount((int)HandledLogType);
            if (_lastLogsAmount != logsAmount)
            {
                _lastLogsAmount = logsAmount;
                LogsAmount.SetText(logsAmount.ToString());
            }
        }

        private void Toggle()
        {
            IsToggled = !IsToggled;
            SelectedImage.enabled = IsToggled;
            OnToggled.Invoke();
        }
        
        protected override void OnDeactivate()
        {
            Button.onClick.RemoveAllListeners();
        }
    }
}