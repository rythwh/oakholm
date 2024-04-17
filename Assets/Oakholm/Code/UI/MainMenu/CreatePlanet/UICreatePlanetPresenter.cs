using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Oakholm.UI {

	[UsedImplicitly]
	public class UICreatePlanetPresenter : UIPresenter<UICreatePlanetView> {

		public UICreatePlanetPresenter(UICreatePlanetView view) : base(view) {
		}

		public override void OnCreate() {
			View.OnBackButtonClicked += OnBackButtonClicked;
			View.OnCreatePlanetButtonClicked += OnCreatePlanetButtonClicked;
		}

		public override void OnClose() {
			View.OnBackButtonClicked -= OnBackButtonClicked;
			View.OnCreatePlanetButtonClicked -= OnCreatePlanetButtonClicked;
		}

		private void OnBackButtonClicked() {
			Managers.GetManager<UIManager>().CloseView(this);
		}

		private void OnCreatePlanetButtonClicked() {
			Managers.GetManager<UIManager>().OpenViewAsync<UICreateColony>(this).Forget();
		}
	}
}
