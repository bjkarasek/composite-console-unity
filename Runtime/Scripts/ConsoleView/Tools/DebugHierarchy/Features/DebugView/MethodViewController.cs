using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private TextMeshProUGUI InfoText;

        [SerializeField] private CompositeSpawner<MethodParameterInputFieldView> ParameterViewSpawner;
        [SerializeField] private CompositeSpawner<MethodParameterEnumView> EnumViewSpawner;
        [SerializeField] private CompositeSpawner<MethodParameterBoolView> BoolViewSpawner;

        private List<IParameterHolder> _parameterHolders = new();
        
        private MonoBehaviour _classInstance;
        private MethodInfo _methodInfo;
        private ParameterInfo[] _parameterInfos;

        private float ParameterHeight = 25;
        private const float OtherElementsHeight = 35;
        private const float Margin = 30;
        private float ParametersContainerHeight => ParameterHeight * _parameterInfos.Length + InfoTextHeight;
        private float InfoTextHeight => InfoText.gameObject.activeSelf ? Mathf.Max(0, InfoText.textBounds.max.y - InfoText.textBounds.min.y + 10) : 0;
        
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
            InstallChild(BoolViewSpawner);
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
            return MethodParameterInputFieldView.IsHandled(parameterType) ||
                   MethodParameterEnumView.IsHandled(parameterType) ||
                   MethodParameterBoolView.IsHandled(parameterType);
        }

        private void SetupParameters()
        {
            ParametersContainer.sizeDelta = new Vector2(ParametersContainer.sizeDelta.x, ParameterHeight * ParametersContainerHeight);
            Container.sizeDelta = new Vector2(Container.sizeDelta.x, ParametersContainerHeight + OtherElementsHeight + Margin);
            
            var infoText = GetAttributeText(_methodInfo);
            if (infoText != "")
            {
                InfoText.gameObject.SetActive(true);
                InfoText.SetText(infoText);
            }
            else
            {
                InfoText.gameObject.SetActive(false);
            }
            
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
                else if (MethodParameterBoolView.IsHandled(parameter.ParameterType))
                {
                    var parameterView = BoolViewSpawner.Spawn();
                    parameterView.Setup(parameter);
                    _parameterHolders.Add(parameterView);
                }
            }
        }

        protected override void OnRefresh()
        {
            if (InfoText.gameObject.activeSelf)
            {
                var stacktraceHeight = Mathf.Max(0, InfoText.textBounds.max.y - InfoText.textBounds.min.y + 10);
                InfoText.rectTransform.sizeDelta = new Vector2(InfoText.rectTransform.sizeDelta.x, stacktraceHeight);
            }
            
            ParametersContainer.sizeDelta = new Vector2(ParametersContainer.sizeDelta.x, ParameterHeight * ParametersContainerHeight);
            Container.sizeDelta = new Vector2(Container.sizeDelta.x, ParametersContainerHeight + OtherElementsHeight + Margin);
        }

        private static string GetAttributeText(MemberInfo member)
        {
            const bool includeInherited = false;
            var attribute = (DebugMethodAttribute)member
                .GetCustomAttributes(typeof(DebugMethodAttribute), includeInherited).FirstOrDefault();
            Debug.Assert(attribute != null, $"{nameof(DebugMethodAttribute)} should not be null!");

            return attribute.Info;
        }

        protected override void OnDeactivate()
        {
            InvokeButton.onClick.RemoveListener(InvokeMethod);
        }
    }
}