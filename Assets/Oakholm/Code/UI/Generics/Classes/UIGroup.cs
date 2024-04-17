using System.Collections.Generic;
using UnityEngine;

namespace Oakholm.UI {
	public class UIGroup<TConfig> : IUIGroup where TConfig : IUIConfig {

		private readonly TConfig config;

		private readonly IUIGroup parent;
		private readonly List<IUIGroup> children = new List<IUIGroup>();

		public UIGroup(TConfig config, IUIGroup parent) {
			this.config = config;

			this.parent = parent;

			OnConstructed();
		}

		private void OnConstructed() {
			parent?.AddChild(this);

			config.ViewComponent.OnOpen();
			config.Presenter.OnCreate();
		}

		public IUIView GetView() {
			return config.ViewComponent;
		}

		public IUIPresenter GetPresenter() {
			return config.Presenter;
		}

		public IUIGroup GetParent() {
			return parent;
		}

		public void AddChild(IUIGroup child) {
			children.Add(child);
		}

		public void RemoveChild(IUIGroup child) {
			children.Remove(child);
		}

		public void Close() {
			foreach (IUIGroup child in children) {
				child.Close();
				break;
			}

			config.ViewComponent.OnClose();
			config.Presenter.OnClose();

			Object.Destroy(config.ViewGameObject);

			parent?.RemoveChild(this);
		}

		public IUIGroup FindGroup(IUIPresenter presenterToFind) {

			if (presenterToFind == null) {
				return null;
			}

			if (config.Presenter.Equals(presenterToFind)) {
				return this;
			}

			foreach (IUIGroup child in children) {
				IUIGroup foundGroup = child.FindGroup(presenterToFind);
				if (foundGroup.GetPresenter().Equals(presenterToFind)) {
					return foundGroup;
				}
			}

			return null;
		}

		public IUIGroup FindGroup(IUIView viewToFind) {

			if (viewToFind == null) {
				return null;
			}

			if (config.ViewComponent.Equals(viewToFind)) {
				return this;
			}

			foreach (IUIGroup child in children) {
				IUIGroup foundGroup = child.FindGroup(viewToFind);
				if (foundGroup.GetView().Equals(viewToFind)) {
					return foundGroup;
				}
			}

			return null;
		}
	}
}
