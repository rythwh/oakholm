using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Oakholm.UI {

	[UsedImplicitly]
	public class UIManager : Manager {

		private readonly StateManager stateManager;
		private readonly LoadingManager loadingManager;

		private readonly Canvas canvas;
		private readonly Transform canvasTransform;
		private readonly List<IUIGroup> parentGroups = new List<IUIGroup>();

		public event Action<IUIPresenter> OnActiveViewChanged;

		public UIManager(
			SceneReferenceProvider sceneReferenceProvider,
			StateManager stateManager,
			LoadingManager loadingManager
		) {
			this.stateManager = stateManager;
			this.loadingManager = loadingManager;

			canvas = sceneReferenceProvider.Canvas;
			canvasTransform = canvas.transform;

			stateManager.OnStateChanged += OnStateChanged;
		}

		private void OnStateChanged((EState oldState, EState newState) states) {
			foreach (IUIGroup group in parentGroups) {
				group.Close();
			}
			parentGroups.Clear();

			switch (states.newState) {
				case EState.Boot:
					break;
				case EState.MainMenu:
					OpenViewAsync<UIMainMenu>(null).Forget();
					break;
				case EState.Loading:
					loadingManager.AddLoadingTaskHandle(OpenViewAsync<UILoadingScreen>(null));
					break;
				case EState.Simulation:
					OpenViewAsync<UISimulation>(null).Forget();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(states.newState), states.newState, null);
			}
		}

		public async UniTask OpenViewAsync<TUIConfig>(IUIPresenter parent) where TUIConfig : IUIConfig, new() {

			TUIConfig config = new TUIConfig();
			await config.Open(canvasTransform);

			UIGroup<IUIConfig> group = new UIGroup<IUIConfig>(config, FindGroup(parent));

			if (parent == null) {
				parentGroups.Add(group);
			}

			OnActiveViewChanged?.Invoke(config.Presenter);
		}

		private IUIGroup FindGroup(IUIPresenter presenter) {
			if (presenter == null) {
				return null;
			}

			foreach (IUIGroup group in parentGroups) {
				IUIGroup foundGroup = group.FindGroup(presenter);
				if (foundGroup == null) {
					continue;
				}
				if (foundGroup.GetPresenter().Equals(presenter)) {
					return foundGroup;
				}
			}

			return null;
		}

		public void CloseView<TPresenter>(TPresenter presenter) where TPresenter : IUIPresenter {
			IUIGroup group = FindGroup(presenter);
			if (group.GetParent() == null) {
				parentGroups.Remove(group);
				OnActiveViewChanged?.Invoke(null);
			} else {
				OnActiveViewChanged?.Invoke(group.GetParent().GetPresenter());
			}
			group.Close();
		}

		public void CloseViewFromTopParent<TPresenter>(TPresenter presenter) where TPresenter : IUIPresenter {
			IUIGroup group = FindGroup(presenter);
			IUIGroup parent = group;
			while (parent != null) {
				group = parent;
				parent = parent.GetParent();
			}
			if (group == null) {
				throw new Exception($"Group is null when trying to close {nameof(presenter)}");
			}
			group.Close();
			OnActiveViewChanged?.Invoke(null);
		}
	}
}
