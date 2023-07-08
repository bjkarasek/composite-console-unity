using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using CompositeArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CompositeConsole
{
    public class MethodParameterBoolView : MonoCompositeElement, IParameterHolder
    {
        public object Value => Toggle.isOn;

        [SerializeField] private TextMeshProUGUI ParameterName;
        [SerializeField] private Toggle Toggle;

        private Type _parameterType;

        private string _lastValidValue;
        
        public static bool IsHandled(Type parameterType)
        {
            return parameterType == typeof(bool);
        }

        public void Setup(ParameterInfo parameterInfo)
        {
            _parameterType = parameterInfo.ParameterType;
            
            ParameterName.SetText($"{parameterInfo.Name}");
			
            if (parameterInfo.HasDefaultValue)
            {
                Toggle.isOn = (bool)parameterInfo.DefaultValue;
            }
            else
            {
                Toggle.isOn = false;
            }
        }
    }
}