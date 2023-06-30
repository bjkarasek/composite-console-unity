using System;
using System.Collections.Generic;
using System.Reflection;
using CompositeArchitecture;
using TMPro;
using UnityEngine;

namespace CompositeConsole
{
	public class MethodParameterEnumView : MonoCompositeElement, IParameterHolder
	{
		public object Value => GetValue();

		[SerializeField] private TextMeshProUGUI ParameterName;
		[SerializeField] private TMP_Dropdown Dropdown;

		private Type _parameterType;

		private string _lastValidValue;
        
		public static bool IsHandled(Type parameterType)
		{
			return parameterType.IsEnum;
		}
        
		public void Setup(ParameterInfo parameterInfo)
		{
			_parameterType = parameterInfo.ParameterType;
            
			ParameterName.SetText($"{parameterInfo.Name}");
			Dropdown.ClearOptions();
			var options = Enum.GetValues(parameterInfo.ParameterType);
			var optionsList = new List<string>();

			foreach (var e in options)
			{
				optionsList.Add(e.ToString());
			}

			Dropdown.AddOptions(optionsList);
			
			if (parameterInfo.HasDefaultValue)
			{
				var defaultValueIndex = optionsList.FindIndex(s => s == parameterInfo.DefaultValue.ToString());
				Dropdown.value = defaultValueIndex;
			}
		}

		protected override void OnRefresh()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Dropdown.Hide();
			}
		}

		private object GetValue()
		{
			return Enum.Parse(_parameterType, Dropdown.value.ToString());
		}
	}
}
