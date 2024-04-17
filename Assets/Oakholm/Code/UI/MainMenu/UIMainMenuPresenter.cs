using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Oakholm.UI {

	[UsedImplicitly]
	public class UIMainMenuPresenter : UIPresenter<UIMainMenuView> {

		public UIMainMenuPresenter(UIMainMenuView view) : base(view) {
		}

		public override void OnCreate() {
			View.OnNewButtonClicked += OnNewButtonClicked;
			View.OnContinueButtonClicked += OnContinueButtonClicked;
			View.OnLoadButtonClicked += OnLoadButtonClicked;
			View.OnSettingsButtonClicked += OnSettingsButtonClicked;
			View.OnExitButtonClicked += OnExitButtonClicked;

			Managers.GetManager<UIManager>().OnActiveViewChanged += OnActiveViewChanged;
		}

		public override void OnClose() {
			View.OnNewButtonClicked -= OnNewButtonClicked;
			View.OnContinueButtonClicked -= OnContinueButtonClicked;
			View.OnLoadButtonClicked -= OnLoadButtonClicked;
			View.OnSettingsButtonClicked -= OnSettingsButtonClicked;
			View.OnExitButtonClicked -= OnExitButtonClicked;

			Managers.GetManager<UIManager>().OnActiveViewChanged -= OnActiveViewChanged;
		}

		private void OnActiveViewChanged(IUIPresenter presenter) {
			View.SetButtonContainerActive(presenter == this);
		}

		private void OnNewButtonClicked() {
			Managers.GetManager<UIManager>().OpenViewAsync<UICreatePlanet>(this).Forget();
		}

		private void OnContinueButtonClicked() {

		}

		private void OnLoadButtonClicked() {

		}

		private void OnSettingsButtonClicked() {

		}

		private void OnExitButtonClicked() {
			Application.Quit();
		}
	}
}
