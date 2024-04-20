using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Oakholm.UI {
	public abstract class UIElement<TComponent> where TComponent : UIElementComponent {

		protected readonly TComponent Component;

		protected UIElement(Transform parent) {
			string addressableKey = $"UI/Component/{GetType().Name.Split('`')[0]}";
			GameObject addressablePrefab = Addressables.LoadAssetAsync<GameObject>(addressableKey).WaitForCompletion();
			Component = Object.Instantiate(addressablePrefab, parent, false).GetComponent<TComponent>();
		}

		public virtual void OnClose() {
			Component.OnClose();
		}
	}
}
