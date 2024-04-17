using JetBrains.Annotations;

namespace Oakholm.UI {

	[UsedImplicitly]
	public class UILoadingScreenPresenter : UIPresenter<UILoadingScreenView> {

		public UILoadingScreenPresenter(UILoadingScreenView view) : base(view) {
		}

		public override void OnCreate() {
		}

		public override void OnClose() {
		}
	}
}
