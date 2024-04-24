using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Oakholm {

	[UsedImplicitly]
	public class LoadingManager : Manager {

		private readonly StateManager stateManager;

		private readonly Queue<UniTask> loadingTaskHandles = new();

		public LoadingManager(StateManager stateManager) {
			this.stateManager = stateManager;
			stateManager.OnPostStateChanged += OnPostStateChanged;
		}

		public override void OnClose() {
			stateManager.OnPostStateChanged -= OnPostStateChanged;
		}

		public void AddLoadingTaskHandle(UniTask task) {
			loadingTaskHandles.Enqueue(task);
		}

		private void OnPostStateChanged(EState state) {
			if (state == EState.Loading) {
				ProcessLoadingTasks().Forget();
			}
		}

		private async UniTask ProcessLoadingTasks() {
			await UniTask.WhenAll(loadingTaskHandles);
			stateManager.SetState(EState.Simulation);
		}
	}
}
