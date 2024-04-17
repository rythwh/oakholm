using UnityEngine;

namespace Oakholm.UI {
	public abstract class UIView : MonoBehaviour, IUIView {
		public abstract void OnOpen();
		public abstract void OnClose();
	}
}
