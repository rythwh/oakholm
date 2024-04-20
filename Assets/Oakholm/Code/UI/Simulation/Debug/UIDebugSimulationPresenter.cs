using JetBrains.Annotations;

namespace Oakholm.UI {

	[UsedImplicitly]
	public class UIDebugSimulationPresenter : UIPresenter<UIDebugSimulationView> {

		public UIDebugSimulationPresenter(UIDebugSimulationView view) : base(view) {

		}

		public override void OnCreate() {

			View.OnDebugValueChanged += OnDebugValueChanged;

			foreach (Noise noise in MapGenerator.GetNoises()) {
				View.AddNoiseDebugPanel(noise);
			}

			foreach (Height height in MapGenerator.GetHeights()) {
				View.AddHeightDebugPanel(height);
			}
		}

		private void OnDebugValueChanged() {
			Managers.GetManager<MapManager>().Map.RebuildAllChunks();
		}

		public override void OnClose() {
			View.OnDebugValueChanged -= OnDebugValueChanged;
		}
	}
}
