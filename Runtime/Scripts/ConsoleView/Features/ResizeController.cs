using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class ResizeController : MonoCompositeElement
    {
        [SerializeField] private RectTransform RectTransform;
        [SerializeField] private ResizeManualController ResizeManualController;

        private string ConsoleViewHeightPrefKey = "CONSOLE_WindowHeightPrefKey";
        private string ConsoleViewIsMaximizedPrefKey = "CONSOLE_WindowIsMaximizedPrefKey";

        private Canvas _canvas;
        private RectTransform _canvasRT;
        private float Height
        {
            get => PlayerPrefs.GetFloat(ConsoleViewHeightPrefKey, 0.5f);
            set => PlayerPrefs.SetFloat(ConsoleViewHeightPrefKey, value);
        }

        private bool IsMaximized
        {
            get => PlayerPrefs.GetInt(ConsoleViewIsMaximizedPrefKey, 0) == 1;
            set => PlayerPrefs.SetInt(ConsoleViewIsMaximizedPrefKey, value ? 1 : 0);
        }
        
        private float MinimumHeight = 54;
        private float MaximumHeight = 10;

        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(ResizeManualController, bindingMode: BindingMode.NonInjectable);
        }

        protected override void OnInject()
        {
            Resolve(out _canvas);
            _canvasRT = _canvas.GetComponent<RectTransform>();
        }

        protected override void OnActivate()
        {
            ResizeManualController.SetInitialPosition(new Vector2(0, Height * Screen.height));
            Subscribe(ResizeManualController.OnResizing, Resize);
            Resize(new Vector2(0, Height * Screen.height));

            if (IsMaximized)
            {
                Maximize();
            }
            else
            {
                Contract();
            }
        }

        protected override void OnRefresh()
        {
            if (IsMaximized == false)
            {
                Resize(new Vector2(0, Height * Screen.height));
            }
        }

        public void Maximize()
        {
            IsMaximized = true;
            RectTransform.anchorMin = new Vector2(0, 0);
            RectTransform.anchorMax = new Vector2(1, 1);
            if (ResizeManualController.State.IsActive)
            {
                ResizeManualController.Deactivate();
            }
        }

        public void Contract()
        {
            IsMaximized = false;
            RectTransform.anchorMin = new Vector2(0, Height);
            RectTransform.anchorMax = new Vector2(1, 1);
            if (ResizeManualController.State.IsActive == false)
            {
                ResizeManualController.Activate();
            }
        }
        
        private void Resize(Vector2 mousePosition)
        {
            var mouseHeight = Mathf.Clamp(mousePosition.y, MaximumHeight, Screen.height - MinimumHeight);
            var height = mouseHeight / Screen.height;
            ChangeHeight(height);
        }
        
        private void ChangeHeight(float height)
        {
            Height = height;
            Contract();
        }
    }
}