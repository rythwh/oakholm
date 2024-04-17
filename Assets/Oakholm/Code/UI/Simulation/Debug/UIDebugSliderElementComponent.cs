using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Oakholm.UI {
	public class UIDebugSliderElementComponent : UIElementComponent {

		[SerializeField] private TMP_Text titleText;
		[SerializeField] private Slider valueSlider;
		[SerializeField] private Button leftShiftButton;
		[SerializeField] private TMP_Text valueText;
		[SerializeField] private Button rightShiftButton;

		public event Action<float> OnSliderChanged;
		public event Action<int> OnShiftButtonClicked;

		protected override void OnCreate() {
			valueSlider.onValueChanged.AddListener(SliderValueChanged);
			leftShiftButton.onClick.AddListener(() => ShiftButtonClicked(1));
			rightShiftButton.onClick.AddListener(() => ShiftButtonClicked(-1));
		}

		public override void OnClose() {
			valueSlider.onValueChanged.RemoveListener(SliderValueChanged);
			leftShiftButton.onClick.RemoveListener(() => ShiftButtonClicked(1));
			rightShiftButton.onClick.RemoveListener(() => ShiftButtonClicked(-1));
		}

		public void SetTitleText(string title) {
			titleText.SetText(title);
		}

		public void SetValueText(string value) {
			valueText.SetText(value);
		}

		public void SliderValueChanged(float sliderValue) {
			OnSliderChanged?.Invoke(sliderValue);
		}

		public void ShiftButtonClicked(int shiftDirection) {
			OnShiftButtonClicked?.Invoke(shiftDirection);
		}
	}
}
