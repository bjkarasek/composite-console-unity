using System;
using System.ComponentModel;
using System.Reflection;
using CompositeArchitecture;
using TMPro;
using UnityEngine;

namespace CompositeConsole
{
    public class MethodParameterInputFieldView : MonoCompositeElement, IParameterHolder
    {
        public object Value => GetValue();

        [SerializeField] private TextMeshProUGUI ParameterName;
        [SerializeField] private TMP_InputField InputField;

        private Type _parameterType;

        private string _lastValidValue;
        
        public static bool IsHandled(Type parameterType)
        {
            return (parameterType.IsValueType && parameterType.IsPrimitive && parameterType != typeof(bool)) || parameterType == typeof(string);
        }
        
        public void Setup(ParameterInfo parameterInfo)
        {
            _parameterType = parameterInfo.ParameterType;
            
            ParameterName.SetText($"{parameterInfo.Name}");
            InputField.placeholder.GetComponent<TextMeshProUGUI>().text = $"{parameterInfo.ParameterType.Name}";

            if (parameterInfo.HasDefaultValue)
            {
                InputField.text = parameterInfo.DefaultValue.ToString();
            }
        }

        protected override void OnActivate()
        {
            InputField.contentType = GetContentType(_parameterType);
            InputField.onEndEdit.AddListener(FixValue);
        }

        private void FixValue(string _)
        {
            FixValue();
        }

        private object GetValue()
        {
            TryConvert(InputField.text, _parameterType, out var result);
            return result;
        }

        private bool TryConvert(string input, Type type, out object result)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(type);
                result = converter.ConvertFromString(input);
                return true;
            }
            catch (Exception)
            {
                result = type.IsValueType ? Activator.CreateInstance(type) : "";
                return false;
            }
        }
        
        private void FixValue()
        {
            if (TryConvert(InputField.text, _parameterType, out var result))
            {
                InputField.text = result.ToString();
                _lastValidValue = InputField.text;
            }
            else
            {
                InputField.text = _lastValidValue;
            }
        }
        
        private static TMP_InputField.ContentType GetContentType(Type type)
        {   
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return TMP_InputField.ContentType.IntegerNumber;
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return TMP_InputField.ContentType.DecimalNumber;
                default:
                    return TMP_InputField.ContentType.Alphanumeric;
            }
        }

        protected override void OnDeactivate()
        {
            InputField.onValueChanged.RemoveListener(FixValue);
        }
    }
}