using System;
using UnityEngine;

namespace CompositeArchitecture
{
    public abstract class MonoCompositeElement : MonoBehaviour, ICompositeElement
    {
        public ICompositeElementState State => _state;

        private CompositeElementState _state;
        private InstallationController _installationController;
        private DependencyInjectionContainer _diContainer;
        private SubElementsController _subElementsController;
        private ObservableBinder _observableBinder;

#region Dependency Injection
        protected void Resolve<T>(out T dependency)
        {
            _diContainer.Resolve(out dependency, this);
        }
#endregion

#region Installation
        protected void InstallChild<T>(T subElement, LifecycleFlags lifecycleFlags = LifecycleFlags.All, BindingMode bindingMode = BindingMode.Injectable,
            bool fromSubContainer = false) where T : ICompositeElement
        {
            _installationController.InstallChild(subElement, lifecycleFlags, bindingMode, fromSubContainer);
        }

        protected void BindChild<T>(T obj, BindingMode bindingMode = BindingMode.Injectable)
        {
            _installationController.BindChild(obj, bindingMode);
        }

        protected void UninstallSubElement<T>(T subElement, bool dontDestroy = false) where T : ICompositeElement
        {
            _installationController.UninstallSubElement(subElement, dontDestroy);
        }
#endregion

#region Virtual lifecycle methods
        protected virtual void OnInstall(DependencyInjectionContainer container) {}
        protected virtual void OnInject() {}
        protected virtual void OnInitialize() {}
        protected virtual void OnActivate() {}
        protected virtual void OnEarlyRefresh() {}
        protected virtual void OnRefresh() {}
        protected virtual void OnLateRefresh() {}
        protected virtual void OnFixedRefresh() {}
        protected virtual void OnDeactivate() {}
        protected virtual void OnDeinitialize() {}
#endregion

#region Public lifecycle methods
        public void Install(DependencyInjectionContainer diContainer)
        {
            _diContainer = diContainer;
            _state = new CompositeElementState();
            _subElementsController = new SubElementsController();
            _installationController = new InstallationController(_subElementsController, this, _diContainer);
            _observableBinder = new ObservableBinder();
            OnInstall(diContainer.Parent);
            _state.IsInstalled = true;
        }

        public void Inject()
        {
            OnInject();
            _subElementsController.Inject();
            _state.IsInjected = true;
        }

        public void Initialize()
        {
            gameObject.SetActive(false);
            OnInitialize();
            _subElementsController.Initialize();
            _state.IsInitialized = true;
        }

        public void Activate()
        {
            OnActivate();
            _subElementsController.Activate();
            gameObject.SetActive(true);
            _state.IsActive = true;
        }

        public void ToggleActive(bool setActive)
        {
            if (_state.IsActive && setActive == false)
                Deactivate();
            else if (_state.IsActive == false && setActive)
                Activate();
        }

        public void EarlyRefresh()
        {
            OnEarlyRefresh();
            _subElementsController.EarlyRefresh();
        }

        public void Refresh()
        {
            OnRefresh();
            _subElementsController.Refresh();
        }

        public void LateRefresh()
        {
            OnLateRefresh();
            _subElementsController.LateRefresh();
        }

        public void FixedRefresh()
        {
            OnFixedRefresh();
            _subElementsController.FixedRefresh();
        }

        public void Deactivate()
        {
            _subElementsController.Deactivate();
            OnDeactivate();
            gameObject.SetActive(false);
            _state.IsActive = false;
        }

        public void Deinitialize()
        {
            _subElementsController.Deinitialize();
            OnDeinitialize();
            _state.IsDeinitialized = false;
        }
#endregion

#region Event subscription methods

        protected void Subscribe(Observable e, Action action)
        {
            _observableBinder.Subscribe(e, action);
        }

        protected void Subscribe<T>(Observable<T> e, Action<T> action)
        {
            _observableBinder.Subscribe(e, action);
        }

        public void UnsubscribeAll()
        {
            _observableBinder.UnsubscribeAll();
        }

#endregion

#region Transform cache
        private Transform _cachedTransform;
        public Transform Transform => _cachedTransform ??= transform;
#endregion
    }
}