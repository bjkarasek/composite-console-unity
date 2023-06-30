using System.Collections.Generic;

namespace CompositeArchitecture
{
    public class SubElementsController
    {
        private readonly List<CompositeContainer> _compositeElements = new();

        private class CompositeContainer
        {
            public ICompositeElement CompositeElement;
            public LifecycleFlags Flags;
            public bool IsRemoved;
        }
        
        public void AddCompositeElement(ICompositeElement compositeElement, LifecycleFlags flags = LifecycleFlags.All)
        {
            _compositeElements.Add(new CompositeContainer()
            {
                CompositeElement = compositeElement,
                Flags = flags,
                IsRemoved = false,
            });
        }
        
        public void RemoveCompositeElement(ICompositeElement compositeElement)
        {
            for (var i = 0; i < _compositeElements.Count; i++)
            {
                if (_compositeElements[i].CompositeElement == compositeElement)
                {
                    _compositeElements[i].IsRemoved = true;
                    return;
                }
            }
        }
        
        public void Inject()
        {
            foreach (var compositeContainer in _compositeElements)
            {
                if (compositeContainer.IsRemoved == false && compositeContainer.Flags.HasFlag(LifecycleFlags.Inject))
                {
                    compositeContainer.CompositeElement.Inject();
                }
            }
            
            RemoveUninstalledElements();
        }
        
        public void Initialize()
        {
            foreach (var compositeContainer in _compositeElements)
            {
                if (compositeContainer.IsRemoved == false && compositeContainer.Flags.HasFlag(LifecycleFlags.Initialize))
                {
                    compositeContainer.CompositeElement.Initialize();
                }
            }
            
            RemoveUninstalledElements();
        }

        public void Activate()
        {
            foreach (var compositeContainer in _compositeElements)
            {
                if (compositeContainer.IsRemoved == false && compositeContainer.Flags.HasFlag(LifecycleFlags.Activate))
                {
                    compositeContainer.CompositeElement.Activate();
                }
            }
            
            RemoveUninstalledElements();
        }
        
        public void EarlyRefresh()
        {
            foreach (var compositeContainer in _compositeElements)
            {
                if (compositeContainer.IsRemoved == false && compositeContainer.Flags.HasFlag(LifecycleFlags.EarlyRefresh) && 
                    compositeContainer.CompositeElement.State.IsActive)
                {
                    compositeContainer.CompositeElement.EarlyRefresh();
                }
            }

            RemoveUninstalledElements();
        }

        public void Refresh()
        {
            foreach (var compositeContainer in _compositeElements)
            {
                if (compositeContainer.IsRemoved == false && compositeContainer.Flags.HasFlag(LifecycleFlags.Refresh) && 
                    compositeContainer.CompositeElement.State.IsActive)
                {
                    compositeContainer.CompositeElement.Refresh();
                }
            }

            RemoveUninstalledElements();
        }
        
        public void LateRefresh()
        {
            foreach (var compositeContainer in _compositeElements)
            {
                if (compositeContainer.IsRemoved == false && compositeContainer.Flags.HasFlag(LifecycleFlags.LateRefresh) && 
                    compositeContainer.CompositeElement.State.IsActive)
                {
                    compositeContainer.CompositeElement.LateRefresh();
                }
            }

            RemoveUninstalledElements();
        }
        
        public void FixedRefresh()
        {
            foreach (var compositeContainer in _compositeElements)
            {
                if (compositeContainer.IsRemoved == false && compositeContainer.Flags.HasFlag(LifecycleFlags.FixedRefresh) && 
                    compositeContainer.CompositeElement.State.IsActive)
                {
                    compositeContainer.CompositeElement.FixedRefresh();
                }
            }

            RemoveUninstalledElements();
        }

        public void Deactivate()
        {
            foreach (var compositeContainer in _compositeElements)
            {
                if (compositeContainer.IsRemoved == false && compositeContainer.Flags.HasFlag(LifecycleFlags.Deactivate) && 
                    compositeContainer.CompositeElement.State.IsActive)
                {
                    compositeContainer.CompositeElement.Deactivate();
                }
            }

            RemoveUninstalledElements();
        }

        public void Deinitialize()
        {
            foreach (var compositeContainer in _compositeElements)
            {
                if (compositeContainer.IsRemoved == false && compositeContainer.Flags.HasFlag(LifecycleFlags.Deinitialize))
                {
                    compositeContainer.CompositeElement.Deinitialize();
                }
            }

            _compositeElements.Clear();
        }

        private void RemoveUninstalledElements()
        {
            for (var i = _compositeElements.Count - 1; i >= 0; i--)
            {
                if (_compositeElements[i].IsRemoved)
                {
                    _compositeElements.RemoveAt(i);
                }
            }
        }
    }
}