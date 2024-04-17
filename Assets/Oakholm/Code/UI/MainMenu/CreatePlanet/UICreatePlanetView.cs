using System;
using UnityEngine;
using UnityEngine.UI;

namespace Oakholm.UI {
	public class UICreatePlanetView : UIView {

		[SerializeField] private Button backButton;
		[SerializeField] private Button createPlanetButton;

		public event Action OnBackButtonClicked;
		public event Action OnCreatePlanetButtonClicked;

		public override void OnOpen() {
			backButton.onClick.AddListener(() => OnBackButtonClicked?.Invoke());
			createPlanetButton.onClick.AddListener(() => OnCreatePlanetButtonClicked?.Invoke());
		}

		public override void OnClose() {
			backButton.onClick.RemoveListener(() => OnBackButtonClicked?.Invoke());
			createPlanetButton.onClick.RemoveListener(() => OnCreatePlanetButtonClicked?.Invoke());
		}
	}
}
