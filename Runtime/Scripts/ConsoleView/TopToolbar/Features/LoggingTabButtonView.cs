using TMPro;
using UnityEngine;

namespace CompositeConsole
{
    public class LoggingTabButtonView : TabButtonView
    {
        [SerializeField] private TextMeshProUGUI ErrorsCountText;
        [SerializeField] private GameObject ErrorsPopupParent;

        private LoggingManager _loggingManager;
        
        protected override void OnInject()
        {
            Resolve(out _loggingManager);
        }

        protected override void OnActivate()
        {
            Subscribe(_loggingManager.OnErrorsCountChanged, RefreshErrorsCount);
            RefreshErrorsCount(_loggingManager.ErrorsCount);
        }

        private void RefreshErrorsCount(int errorsCount)
        {
            ErrorsCountText.SetText(errorsCount.ToString());
            ErrorsPopupParent.SetActive(errorsCount > 0);
        }
    }
}