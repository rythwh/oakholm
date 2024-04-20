using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Oakholm.UI {
	public class UIDebugSliderElementComponent : UIElementComponent {

		[SerializeField] private TMP_Text titleText;
		[SerializeField] private Button resetButton;
		[SerializeField] private Slider valueSlider;
		[SerializeField] private Button leftShiftButton;
		[SerializeField] private TMP_Text valueText;
		[SerializeField] private Button rightShiftButton;

		public event Action OnResetButtonClicked;
		public event Action<float> OnSliderChanged;
		public event Action<ShiftDirection> OnShiftButtonClicked;

		protected override void OnCreate() {
			resetButton.onClick.AddListener(ResetButtonClicked);
			valueSlider.onValueChanged.AddListener(SliderValueChanged);
			leftShiftButton.onClick.AddListener(() => ShiftButtonClicked(ShiftDirection.Left));
			rightShiftButton.onClick.AddListener(() => ShiftButtonClicked(ShiftDirection.Right));
		}

		public override void OnClose() {
			resetButton.onClick.RemoveListener(ResetButtonClicked);
			valueSlider.onValueChanged.RemoveListener(SliderValueChanged);
			leftShiftButton.onClick.RemoveListener(() => ShiftButtonClicked(ShiftDirection.Left));
			rightShiftButton.onClick.RemoveListener(() => ShiftButtonClicked(ShiftDirection.Right));
		}

		public void SetTitleText(string title) {
			titleText.SetText(title);
		}

		public void SetValueText(string value) {
			valueText.SetText(value);
		}

		public void SetSliderValues(float value, float min, float max) {
			SetSliderMinMax(min, max);
			valueSlider.SetValueWithoutNotify(value);
		}

		public void SetSliderMinMax(float min, float max) {
			valueSlider.minValue = min;
			valueSlider.maxValue = max;
		}

		private void ResetButtonClicked() {
			OnResetButtonClicked?.Invoke();
		}

		private void SliderValueChanged(float sliderValue) {
			OnSliderChanged?.Invoke(sliderValue);
		}

		private void ShiftButtonClicked(ShiftDirection shiftDirection) {
			OnShiftButtonClicked?.Invoke(shiftDirection);
		}
	}

	public enum ShiftDirection {
		Left,
		Right
	}
}
