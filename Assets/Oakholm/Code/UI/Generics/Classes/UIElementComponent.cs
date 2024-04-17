using UnityEngine;

namespace Oakholm.UI {
	public abstract class UIElementComponent : MonoBehaviour {
		private void OnEnable() {
			OnCreate();
		}

		private void OnDisable() {
			OnClose();
		}

		protected abstract void OnCreate();
		public abstract void OnClose();
	}
}
