using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Oakholm {

	[UsedImplicitly]
	public class LoadingManager : Manager {

		private readonly StateManager stateManager;

		private readonly List<UniTask> loadingTaskHandles = new List<UniTask>();
		private bool stateEqualsLoading = false;

		public LoadingManager(StateManager stateManager) {
			this.stateManager = stateManager;
			stateManager.OnStateChanged += OnStateChanged;
		}

		private void OnStateChanged((EState oldState, EState newState) states) {
			stateEqualsLoading = states.newState == EState.Loading;
		}

		public void AddLoadingTaskHandle(UniTask task) {
			loadingTaskHandles.Add(task);
		}

		public override void OnUpdate() {
			if (!stateEqualsLoading) {
				return;
			}
			CheckTaskStatuses();
			if (loadingTaskHandles.Count == 0) {
				stateManager.SetState(EState.Simulation);
			}
		}

		private void CheckTaskStatuses() {
			for (int taskIndex = loadingTaskHandles.Count - 1; taskIndex >= 0; taskIndex--) {
				UniTask task = loadingTaskHandles[taskIndex];
				if (task.Status.IsCompleted()) {
					loadingTaskHandles.Remove(task);
				}
			}
		}
	}
}
