using CompositeArchitecture;
using UnityEngine;

namespace CompositeConsole
{
    public class LogTypeTogglingController : MonoCompositeElement
    {
        [SerializeField] private LogTypeToggleView LogToggleView;
        [SerializeField] private LogTypeToggleView WarningToggleView;
        [SerializeField] private LogTypeToggleView ErrorToggleView;

        private LogCatchController _logCatchController;
        
        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(LogToggleView, bindingMode: BindingMode.NonInjectable);
            InstallChild(WarningToggleView, bindingMode: BindingMode.NonInjectable);
            InstallChild(ErrorToggleView, bindingMode: BindingMode.NonInjectable);
        }

        protected override void OnInject()
        {
            Resolve(out _logCatchController);
        }

        protected override void OnActivate()
        {
            Subscribe(LogToggleView.OnToggled, Toggle);
            Subscribe(WarningToggleView.OnToggled,Toggle);
            Subscribe(ErrorToggleView.OnToggled, Toggle);

            _logCatchController.ChangeMask(CalculateMask());
        }

        private int CalculateMask()
        {
            var mask = 0;
            mask += LogToggleView.IsToggled ? 1 << 0 : 0;
            mask += WarningToggleView.IsToggled ? 1 << 1 : 0;
            mask += ErrorToggleView.IsToggled ? 1 << 2 : 0;
            return mask;
        }

        private void Toggle()
        {
            _logCatchController.ChangeMask(CalculateMask());
        }
    }
}