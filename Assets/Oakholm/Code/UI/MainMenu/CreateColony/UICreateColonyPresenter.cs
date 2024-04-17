using JetBrains.Annotations;

namespace Oakholm.UI {

	[UsedImplicitly]
	public class UICreateColonyPresenter : UIPresenter<UICreateColonyView> {

		public UICreateColonyPresenter(UICreateColonyView view) : base(view) {
		}

		public override void OnCreate() {
			View.OnBackButtonClicked += OnBackButtonClicked;
			View.OnCreateColonyButtonClicked += OnCreatePlanetButtonClicked;
		}

		public override void OnClose() {
			View.OnBackButtonClicked -= OnBackButtonClicked;
			View.OnCreateColonyButtonClicked -= OnCreatePlanetButtonClicked;
		}

		private void OnBackButtonClicked() {
			Managers.GetManager<UIManager>().CloseView(this);
		}

		private void OnCreatePlanetButtonClicked() {
			Managers.GetManager<StateManager>().SetState(EState.Loading);
		}
	}
}
