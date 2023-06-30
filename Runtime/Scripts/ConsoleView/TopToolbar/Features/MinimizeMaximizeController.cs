using CompositeArchitecture;
using UnityEngine;
using UnityEngine.UI;

namespace CompositeConsole
{
    public class MinimizeMaximizeController : MonoCompositeElement
    {
        [SerializeField] private Button MaximizeButton;
        [SerializeField] private Button MinimizeButton;

        private ResizeController _resizeController;
        
        protected override void OnInject()
        {
            Resolve(out _resizeController);
        }
        protected override void OnInitialize()
        {
            MaximizeButton.onClick.AddListener(MaximizeWindow);
            MinimizeButton.onClick.AddListener(ContractWindow);
        }

        private void MaximizeWindow()
        {
            _resizeController.Maximize();
            MaximizeButton.gameObject.SetActive(false);
            MinimizeButton.gameObject.SetActive(true);
        }

        private void ContractWindow()
        {
            _resizeController.Contract();
            MaximizeButton.gameObject.SetActive(true);
            MinimizeButton.gameObject.SetActive(false);
        }

        protected override void OnDeinitialize()
        {
            MaximizeButton.onClick.RemoveListener(MaximizeWindow);
            MinimizeButton.onClick.RemoveListener(ContractWindow);
        }
    }
}