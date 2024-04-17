using System;
using UnityEngine;
using UnityEngine.UI;

namespace Oakholm.UI {
	public class UIMainMenuView : UIView {

		[Header("Buttons")]
		[SerializeField] private GameObject buttonContainer;

		[SerializeField] private Button newButton;
		[SerializeField] private Button continueButton;
		[SerializeField] private Button loadButton;
		[SerializeField] private Button settingsButton;
		[SerializeField] private Button exitButton;

		public event Action OnNewButtonClicked;
		public event Action OnContinueButtonClicked;
		public event Action OnLoadButtonClicked;
		public event Action OnSettingsButtonClicked;
		public event Action OnExitButtonClicked;

		public override void OnOpen() {
			newButton.onClick.AddListener(() => OnNewButtonClicked?.Invoke());
		}

		public override void OnClose() {
			newButton.onClick.RemoveListener(() => OnNewButtonClicked?.Invoke());
		}

		public void SetButtonContainerActive(bool active) {
			buttonContainer.SetActive(active);
		}
	}
}
