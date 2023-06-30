using UnityEngine;

namespace CompositeArchitecture
{
    public class InstallationController
    {
        private ICompositeElementState State => _compositeElement.State;

        private SubElementsController _subElementsController;
        private ICompositeElement _compositeElement;
        private DependencyInjectionContainer _diContainer;
        
        public InstallationController(SubElementsController subElementsController, ICompositeElement compositeElement,
            DependencyInjectionContainer diContainer)
        {
            _subElementsController = subElementsController;
            _compositeElement = compositeElement;
            _diContainer = diContainer;
        }
        
        public void InstallChild<T>(T subElement, LifecycleFlags lifecycleFlags = LifecycleFlags.All, 
            BindingMode bindingMode = BindingMode.Injectable, bool fromSubContainer = false)
            where T : ICompositeElement
        {
            if (subElement == null)
            {
                Debug.LogError($"<b><color=#e60000>{_compositeElement.GetType()}</color></b>: " +
                               $"Trying to bind null value of type <b><color=#e60000>{typeof(T)}</color></b>!");
                return;
            }

            if (lifecycleFlags != LifecycleFlags.Empty)
            {
                _subElementsController.AddCompositeElement(subElement, lifecycleFlags);
            }

            var container = fromSubContainer ? new DependencyInjectionContainer(_diContainer) : _diContainer;

            switch (bindingMode)
            {
                case BindingMode.Injectable:
                    container.Bind(subElement, false);
                    break;
                case BindingMode.InjectableWithInterfaces:
                    container.Bind(subElement, true);
                    break;
            }

            subElement.Install(container);

            if (State.IsInjected) subElement.Inject();
            if (State.IsInitialized) subElement.Initialize();
            if (State.IsActive) subElement.Activate();
        }

        public void BindChild<T>(T obj, BindingMode bindingMode = BindingMode.Injectable)
        {
            switch (bindingMode)
            {
                case BindingMode.Injectable:
                    _diContainer.Bind(obj, false);
                    break;
                case BindingMode.InjectableWithInterfaces:
                    _diContainer.Bind(obj, true);
                    break;
            }
        }

        public void UninstallSubElement<T>(T subElement, bool dontDestroy = false)
            where T : ICompositeElement
        {
            if (subElement.State.IsActive) subElement.Deactivate();
            if (subElement.State.IsDeinitialized == false) subElement.Deinitialize();
            _subElementsController.RemoveCompositeElement(subElement);
            if (dontDestroy == false && subElement is Behaviour behaviour)
            {
                Object.Destroy(behaviour.gameObject);
            }
        }
    }
}