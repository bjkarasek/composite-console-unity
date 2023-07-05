using System;
using System.Collections.Generic;
using System.Linq;
using CompositeArchitecture;
using UnityEngine;
using UnityEngine.Profiling;

namespace CompositeConsole
{
    public class HierarchyController : MonoCompositeElement
    {
        [SerializeField] private CompositeSpawner<DebugHierarchyElementView> HierarchyElementCompositeSpawner;
        [SerializeField] private DebugViewController DebugViewController;
        private HierarchyFindingController _hierarchyFindingController = new();
        
        [SerializeField] private RectTransform ContentRT;

        private DebugHierarchyElementView _selectedElement = null;
        private Dictionary<HierarchyNode, DebugHierarchyElementView> _registeredObjects = new();
        private Dictionary<DebugHierarchyElementView, HierarchyNode> _spawnedHierarchyElements = new();
        private Dictionary<DebugHierarchyElementView, IDisposable> _subs = new();

        private GameObject _lastSelectedElement;

        private const float ElementSize = 20;

        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(HierarchyElementCompositeSpawner);
            InstallChild(DebugViewController);
            InstallChild(_hierarchyFindingController);
        }

        protected override void OnActivate()
        {
            DespawnAll();
            var hierarchyRoots = _hierarchyFindingController.BuildHierarchy();
            SpawnHierarchy(hierarchyRoots);
            UpdateContentHeight();
            DebugViewController.ResetDisplay();

            if (_lastSelectedElement != null && _spawnedHierarchyElements.Count > 0)
            {
                var hierarchyNode = _spawnedHierarchyElements.
                    FirstOrDefault(e => e.Value.GameObject == _lastSelectedElement).Key;
                
                if (_selectedElement == null)
                {
                    hierarchyNode = _spawnedHierarchyElements.First().Key;
                }
                
                ChangeSelection(hierarchyNode);
            }
        }

        private void SpawnHierarchy(List<HierarchyNode> hierarchyRoots)
        {
            foreach (var rootNode in hierarchyRoots)
            {
                SpawnElement(rootNode);
            }
        }
        
        private void SpawnElement(HierarchyNode representedElement)
        {
            var hierarchyElement = HierarchyElementCompositeSpawner.Spawn(onBeforeInstall: view => view.BeforeInstallSetup(representedElement));
            hierarchyElement.RepresentedObject = representedElement;
            _registeredObjects.Add(representedElement, hierarchyElement);
            RegisterElement(hierarchyElement, representedElement);
            
            SpawnHierarchy(representedElement.Children);
        }

        private void RegisterElement(DebugHierarchyElementView element, HierarchyNode representedObject)
        {
            _subs.Add(element, element.OnClicked.Subscribe(ChangeSelection));
            _spawnedHierarchyElements.Add(element, representedObject);
            if (representedObject != null)
            {
                element.Text.SetText($"{representedObject.Name}");
            }

            UpdateContentHeight();
        }

        private void ChangeSelection(DebugHierarchyElementView debugHierarchyElementView)
        {
            if (_selectedElement != null)
            {
                _selectedElement.Unselect();
            }

            _selectedElement = debugHierarchyElementView;
            _lastSelectedElement = _spawnedHierarchyElements[_selectedElement].GameObject;
            DebugViewController.Setup(_spawnedHierarchyElements[_selectedElement]);
            debugHierarchyElementView.Select();
        }

        private void UpdateContentHeight()
        {
            ContentRT.sizeDelta = new Vector2(ContentRT.sizeDelta.x, ElementSize * _spawnedHierarchyElements.Count);
        }

        protected override void OnDeinitialize()
        {
            DespawnAll();
        }

        private void DespawnAll()
        {
            while (HierarchyElementCompositeSpawner.Elements.Count > 0)
            {
                DespawnElement(HierarchyElementCompositeSpawner.Elements[^1]);
            }
        }

        private void DespawnElement(DebugHierarchyElementView elementToDespawn)
        {
            _registeredObjects.Remove(_spawnedHierarchyElements[elementToDespawn]);
            _subs[elementToDespawn].Dispose();
            _subs.Remove(elementToDespawn);
            _spawnedHierarchyElements.Remove(elementToDespawn);
            HierarchyElementCompositeSpawner.Despawn(elementToDespawn);
            
            UpdateContentHeight();
        }
    }
}