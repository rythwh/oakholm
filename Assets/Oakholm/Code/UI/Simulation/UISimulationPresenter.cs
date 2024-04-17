using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Oakholm.UI {

	[UsedImplicitly]
	public class UISimulationPresenter : UIPresenter<UISimulationView> {

		public UISimulationPresenter(UISimulationView view) : base(view) {
		}

		public override void OnCreate() {
			Managers.GetManager<UIManager>().OpenViewAsync<UIDebugSimulation>(this).Forget();
		}

		public override void OnClose() {

		}
	}
}
