using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class SplitViewManager : CompositeManager
    {
        [SerializeField] private RectTransform LeftRectTransform;
        [SerializeField] private RectTransform RightRectTransform;
        [SerializeField] private RectTransform SeparatorRectTransform;
        [SerializeField] private ResizeManualController ResizeManualController;
        [SerializeField] private int UniqueId;
        
        private string ConsoleViewHeightPrefKey => $"CONSOLE_WindowWidthPrefKey_{UniqueId}";
        private float Width
        {
            get => Mathf.Clamp(PlayerPrefs.GetFloat(ConsoleViewHeightPrefKey, 0.5f), 0.1f, 0.9f);
            set => PlayerPrefs.SetFloat(ConsoleViewHeightPrefKey, Mathf.Clamp(value, 0.1f, 0.9f));
        }

        
        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(ResizeManualController, bindingMode: BindingMode.NonInjectable);
        }

        protected override void OnActivate()
        {
            Subscribe(ResizeManualController.OnResizing, Resize);
            ChangeWidth(Width);
        }
        
        private void Resize(Vector2 mousePosition)
        {
            var mouseWidth = Mathf.Clamp(Input.mousePosition.x, 0, Screen.width);
            var width = mouseWidth / Screen.width;
            ChangeWidth(width);
        }
        
        private void ChangeWidth(float width)
        {
            Width = width;

            LeftRectTransform.anchorMin = new Vector2(0, 0);
            LeftRectTransform.anchorMax = new Vector2(Width, 1);
            
            RightRectTransform.anchorMin = new Vector2(Width, 0);
            RightRectTransform.anchorMax = new Vector2(1, 1);
            
            SeparatorRectTransform.anchorMin = new Vector2(Width, 0);
            SeparatorRectTransform.anchorMax = new Vector2(Width, 1);
        }
    }
}