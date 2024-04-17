using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Oakholm.UI {
	public class UIConfig<TView, TPresenter> : IUIConfig
		where TView : IUIView
		where TPresenter : IUIPresenter
	{
		public IUIView ViewComponent { get; private set; }
		public GameObject ViewGameObject { get; private set; }
		public IUIPresenter Presenter { get; private set; }

		protected virtual string AddressableKey() {
			return $"UI/{GetType().Name}";
		}

		public async UniTask Open(Transform parent) {
			GameObject viewPrefab = await GetViewPrefab();
			ViewGameObject = Object.Instantiate(viewPrefab, parent, false);
			ViewComponent = ViewGameObject.GetComponent<TView>();

			Presenter = CreatePresenter((TView)ViewComponent);
		}

		private async UniTask<GameObject> GetViewPrefab() {
			return await Addressables.LoadAssetAsync<GameObject>(AddressableKey());
		}

		private TPresenter CreatePresenter(TView view) {
			return (TPresenter)Activator.CreateInstance(typeof(TPresenter), new object[] {
				view
			});
		}
	}
}
