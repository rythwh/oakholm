using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Oakholm.UI {
	public interface IUIConfig {
		public IUIView ViewComponent { get; }
		public GameObject ViewGameObject { get; }
		public IUIPresenter Presenter { get; }
		public UniTask Open(Transform parent);
	}
}
