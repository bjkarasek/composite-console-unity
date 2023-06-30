namespace CompositeArchitecture
{
    public interface ILifecycle
    {
        void Install(DependencyInjectionContainer diContainer);
        void Inject();
        void Initialize();
        void Activate();
        void ToggleActive(bool setActive);
        void EarlyRefresh();
        void Refresh();
        void LateRefresh();
        void FixedRefresh();
        void Deactivate();
        void Deinitialize();
    }
}