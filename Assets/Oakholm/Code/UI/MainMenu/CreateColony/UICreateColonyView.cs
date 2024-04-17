using System;
using UnityEngine;
using UnityEngine.UI;

namespace Oakholm.UI {
	public class UICreateColonyView : UIView {

		[SerializeField] private Button backButton;
		[SerializeField] private Button createColonyButton;

		public event Action OnBackButtonClicked;
		public event Action OnCreateColonyButtonClicked;

		public override void OnOpen() {
			backButton.onClick.AddListener(() => OnBackButtonClicked?.Invoke());
			createColonyButton.onClick.AddListener(() => OnCreateColonyButtonClicked?.Invoke());
		}

		public override void OnClose() {
			backButton.onClick.RemoveListener(() => OnBackButtonClicked?.Invoke());
			createColonyButton.onClick.RemoveListener(() => OnCreateColonyButtonClicked?.Invoke());
		}
	}
}
