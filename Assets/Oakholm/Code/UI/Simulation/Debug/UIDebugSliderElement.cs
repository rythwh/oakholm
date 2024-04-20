using System;
using System.Globalization;
using UnityEngine;

namespace Oakholm.UI {
	public class UIDebugSliderElement : UIElement<UIDebugSliderElementComponent> {

		private readonly Func<float> getter;
		private readonly Action<float> setter;

		private readonly float originalValue;

		private (float min, float max) sliderRange;

		private Func<float, float> ShiftRightFunction => value => value / Mathf.Pow(10, 1);
		private Func<float, float> ShiftLeftFunction => value => value * Mathf.Pow(10, 1);

		public event Action OnValueChanged;

		public UIDebugSliderElement(
			Transform parent,
			string title,
			Func<float> getter,
			Action<float> setter,
			(float min, float max) sliderRange
		) : base(parent) {
			this.getter = getter;
			this.setter = setter;
			this.sliderRange = sliderRange;

			float value = getter.Invoke();
			originalValue = value;

			Component.SetTitleText(title);
			SetInitialValues(value);

			Component.OnResetButtonClicked += OnResetButtonClicked;
			Component.OnSliderChanged += OnSliderChanged;
			Component.OnShiftButtonClicked += OnShiftButtonClicked;
			OnValueChanged += UpdateValueText;
		}

		private void SetInitialValues(float value) {
			Component.SetValueText(value.ToString(CultureInfo.InvariantCulture));
			Component.SetSliderValues(value, ShiftRightFunction(value), ShiftLeftFunction(value));
		}

		public override void OnClose() {
			Component.OnSliderChanged -= OnSliderChanged;
			Component.OnShiftButtonClicked -= OnShiftButtonClicked;
			OnValueChanged -= UpdateValueText;
		}

		private void OnResetButtonClicked() {
			setter.Invoke(originalValue);
			SetInitialValues(originalValue);
			OnValueChanged?.Invoke();
		}

		private void OnSliderChanged(float sliderValue) {
			setter.Invoke(sliderValue);
			OnValueChanged?.Invoke();
		}

		private void OnShiftButtonClicked(ShiftDirection shiftDirection) {
			switch (shiftDirection) {
				case ShiftDirection.Left:
					setter.Invoke(ShiftLeftFunction(getter.Invoke()));
					break;
				case ShiftDirection.Right:
					setter.Invoke(ShiftRightFunction(getter.Invoke()));
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(shiftDirection), shiftDirection, null);
			}
			float value = getter.Invoke();
			Component.SetSliderMinMax(ShiftRightFunction(value), ShiftLeftFunction(value));
			OnValueChanged?.Invoke();
		}

		private void UpdateValueText() {
			Component.SetValueText(getter.Invoke().ToString(CultureInfo.InvariantCulture));
		}
	}
}
