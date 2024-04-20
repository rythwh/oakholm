using System;
using UnityEngine;

namespace Oakholm.UI {
	public class UIDebugSimulationView : UIView {

		[SerializeField] private RectTransform debugPanelsParent;

		public event Action OnDebugValueChanged;

		public override void OnOpen() {

		}

		public override void OnClose() {

		}

		public void AddNoiseDebugPanel(Noise noise) {
			UIDebugSliderElement noiseMultiplierSlider = new UIDebugSliderElement(
				debugPanelsParent,
				$"{noise}",
				() => noise.Multiplier,
				newValue => noise.Multiplier = newValue,
				(noise.Multiplier / 10f, noise.Multiplier * 10f));
			noiseMultiplierSlider.OnValueChanged += () => OnDebugValueChanged?.Invoke();
			UIDebugSliderElement noiseWeightSlider = new UIDebugSliderElement(
				debugPanelsParent,
				$"{noise} Weight",
				() => noise.Weight,
				newValue => noise.Weight = newValue,
				(1, 100));
			noiseWeightSlider.OnValueChanged += () => OnDebugValueChanged?.Invoke();
		}

		public void AddHeightDebugPanel(Height height) {
			UIDebugSliderElement heightSliderMin = new UIDebugSliderElement(
				debugPanelsParent,
				$"{height} Min",
				() => height.Range.min,
				newValue => height.Range = (newValue, height.Range.max),
				(0, 1));
			heightSliderMin.OnValueChanged += () => OnDebugValueChanged?.Invoke();
			UIDebugSliderElement heightSliderMax = new UIDebugSliderElement(
				debugPanelsParent,
				$"{height} Max",
				() => height.Range.max,
				newValue => height.Range = (height.Range.min, newValue),
				(0, 1));
			heightSliderMax.OnValueChanged += () => OnDebugValueChanged?.Invoke();
		}
	}
}
