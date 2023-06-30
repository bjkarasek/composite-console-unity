using System;
using System.Collections.Generic;
using System.Reflection;
using CompositeArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CompositeConsole
{
    public class MethodViewController : MonoCompositeElement
    {
        public float ElementHeight => ParametersContainerHeight + OtherElementsHeight + Margin;
        
        [SerializeField] private TextMeshProUGUI MethodNameText;
        [SerializeField] private Button InvokeButton;
        [SerializeField] private RectTransform Container;
        [SerializeField] private RectTransform ParametersContainer;
        
        [SerializeField] private CompositeSpawner<MethodParameterInputFieldView> ParameterViewSpawner;
        [SerializeField] private CompositeSpawner<MethodParameterEnumView> EnumViewSpawner;

        private List<IParameterHolder> _parameterHolders = new();
        
        private MonoBehaviour _classInstance;
        private MethodInfo _methodInfo;
        private ParameterInfo[] _parameterInfos;

        private float ParameterHeight = 25;
        private const float OtherElementsHeight = 50;
        private const float Margin = 15;
        private float ParametersContainerHeight => ParameterHeight * _parameterInfos.Length;
        
        public void BeforeInstall(MonoBehaviour classInstance, MethodInfo methodInfo, ParameterInfo[] parameterInfos)
        {
            _classInstance = classInstance;
            _methodInfo = methodInfo;
            _parameterInfos = parameterInfos;
        }

        protected override void OnInstall(DependencyInjectionContainer container)
        {
            InstallChild(ParameterViewSpawner);
            InstallChild(EnumViewSpawner);
        }

        protected override void OnActivate()
        {
            InvokeButton.onClick.AddListener(InvokeMethod);

            Container.sizeDelta = new Vector2(Container.sizeDelta.x, ParametersContainerHeight + OtherElementsHeight + Margin);
            MethodNameText.SetText($"Method name: {_methodInfo.Name}");
            SetupParameters();
        }
        
        private void InvokeMethod()
        {
            var parametersArray = new object[_parameterHolders.Count];
            for (var i = 0; i < _parameterHolders.Count; i++)
            {
                parametersArray[i] = _parameterHolders[i].Value;
            }

            _methodInfo.Invoke(_classInstance, parametersArray);
        }

        public bool IsParameterTypeValid (Type parameterType)
        {
            return (MethodParameterInputFieldView.IsHandled(parameterType)) ||
                (MethodParameterEnumView.IsHandled(parameterType));
        }

        private void SetupParameters()
        {
            ParametersContainer.sizeDelta = new Vector2(ParametersContainer.sizeDelta.x, 25 * ParametersContainerHeight);
            
            foreach (var parameter in _parameterInfos)
            {
                if (MethodParameterInputFieldView.IsHandled(parameter.ParameterType))
                {
                    var parameterView = ParameterViewSpawner.Spawn();
                    parameterView.Setup(parameter);
                    _parameterHolders.Add(parameterView);
                }
                else if (MethodParameterEnumView.IsHandled(parameter.ParameterType))
                {
                    var parameterView = EnumViewSpawner.Spawn();
                    parameterView.Setup(parameter);
                    _parameterHolders.Add(parameterView);
                }
            }
        }

        protected override void OnDeactivate()
        {
            InvokeButton.onClick.RemoveListener(InvokeMethod);
        }
    }
}