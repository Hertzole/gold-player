using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Hertzole.GoldPlayer.Editor
{
	[Serializable]
	internal struct MinMaxData
	{
		public float min;
		public float max;

		public MinMaxData(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}

	internal sealed class MinMaxField : BaseField<MinMaxData>
	{
		private readonly FloatField minField;
		private readonly FloatField maxField;

		public MinMaxField(string label, float min, float max) : base(label, new VisualElement())
		{
			value = new MinMaxData(min, max);
			VisualElement input = this.Q<VisualElement>(className: inputUssClassName);
			input.style.flexDirection = FlexDirection.Row;
			input.style.paddingRight = 2;

			minField = CreateFloatField("Min", min);
			minField.style.marginLeft = 0;
			minField.style.marginRight = 2;
			minField.RegisterValueChangedCallback(evt =>
			{
				MinMaxData val = value;
				val.min = evt.newValue;
				value = val;
			});

			input.Add(minField);

			maxField = CreateFloatField("Max", max);
			maxField.style.marginLeft = 2;
			maxField.RegisterValueChangedCallback(evt =>
			{
				MinMaxData val = value;
				val.max = evt.newValue;
				value = val;
			});

			input.Add(maxField);
		}

		public MinMaxField(string label, SerializedProperty min, SerializedProperty max) : this(label, min.floatValue, max.floatValue)
		{
			minField.BindProperty(min);
			maxField.BindProperty(max);
		}

		private FloatField CreateFloatField(string fieldLabel, float fieldValue)
		{
			FloatField field = new FloatField(fieldLabel)
			{
				value = fieldValue,
				style =
				{
					width = new StyleLength(new Length(50, LengthUnit.Percent)),
					marginTop = 0,
					marginBottom = 0
				}
			};

			field.labelElement.style.minWidth = 30;
			field.labelElement.style.marginLeft = 0;
			return field;
		}
	}
}