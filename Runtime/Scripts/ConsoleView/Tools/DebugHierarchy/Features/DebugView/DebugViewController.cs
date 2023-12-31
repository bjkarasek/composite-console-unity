﻿using System.Linq;
using System.Reflection;
using CompositeArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CompositeConsole
{
    public class DebugViewController : MonoCompositeElement
    {
        [SerializeField] private TextMeshProUGUI ObjectNameText;
        [SerializeField] private VerticalLayoutGroup VerticalLayoutGroup;
        [SerializeField] private RectTransform ContentRT;
        
        [SerializeField] private CompositeSpawner<MethodViewController> MethodViewSpawner;
        
        private HierarchyNode _representedElement;

        private const float ContentMargin = 15f;
        
        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(MethodViewSpawner);
        }

        protected override void OnActivate()
        {
            RefreshDisplay();
        }

        public void Setup(HierarchyNode selectedElement)
        {
            if (_representedElement != selectedElement)
            {
                _representedElement = selectedElement;
                RefreshDisplay();
            }
        }

        private void RefreshDisplay()
        {
            ResetDisplay();

            if (_representedElement is { Exists: true })
            {
                ObjectNameText.SetText($"{_representedElement.Name}");
                SetupMethods();
            }
        }

        private void SetupMethods()
        {
            var contentHeight = ObjectNameText.rectTransform.sizeDelta.y;

            foreach (var behaviour in _representedElement.Behaviours)
            {
                SetupMethodsInternal(behaviour);
            }
            
            foreach (var obj in _representedElement.CustomDebugObjects)
            {
                SetupMethodsInternal(obj);
            }
            
            ContentRT.sizeDelta = new Vector2(ContentRT.sizeDelta.x, contentHeight + ContentMargin);

            void SetupMethodsInternal(object obj)
            {
                var methodInfos = obj.GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var method in methodInfos)
                {
                    if (MethodsHasAttribute(method))
                    {
                        var parameters = method.GetParameters();
                        if (HasValidParameters(parameters))
                        {
                            var methodView = MethodViewSpawner.Spawn(onBeforeInstall:
                                viewController => viewController.BeforeInstall(obj, method, parameters));
                            contentHeight += methodView.ElementHeight + VerticalLayoutGroup.spacing;
                        }
                        else
                        {
                            Debug.LogWarning($"Method {method.Name} has invalid attributes!");
                        }
                    }
                }
            }
            
            bool HasValidParameters(ParameterInfo [] parameters)
            {
                var areValid = true;
                
                foreach (var parameter in parameters)
                {
                    var isValid = MethodViewSpawner.Prefab.IsParameterTypeValid(parameter.ParameterType);
                    areValid &= isValid;
                }
                
                return areValid;
            }
        }
        
        protected override void OnRefresh()
        {
            var contentHeight = 0f;
            foreach (var method in MethodViewSpawner.Elements)
            {
                contentHeight += method.ElementHeight + VerticalLayoutGroup.spacing;
            }
            
            ContentRT.sizeDelta = new Vector2(ContentRT.sizeDelta.x, contentHeight + ContentMargin);
        }

        private static bool MethodsHasAttribute(MemberInfo member)
        {
            const bool includeInherited = false;
            return member.GetCustomAttributes(typeof(DebugMethodAttribute), includeInherited).Any();
        }

        protected override void OnDeactivate()
        {
            ResetDisplay();
        }

        public void ResetDisplay()
        {
            while (MethodViewSpawner.Elements.Count > 0)
            {
                MethodViewSpawner.Despawn(MethodViewSpawner.Elements[^1]);
            }
            ObjectNameText.SetText("No object selected");
            ContentRT.sizeDelta = new Vector2(ContentRT.sizeDelta.x, 0);
        }
    }
}