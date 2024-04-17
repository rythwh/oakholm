using UnityEngine;

namespace Oakholm {
	public class GameManager : MonoBehaviour {

		[Header("Don't Destroy on Load")]
		[SerializeField] private GameObject[] dontDestroyOnLoads;

		[Header("Scene Reference Provider")]
		[SerializeField] private SceneReferenceProvider sceneReferenceProvider;

		public void OnEnable() {
			foreach (GameObject dontDestroyOnLoad in dontDestroyOnLoads) {
				DontDestroyOnLoad(dontDestroyOnLoad);
			}

			Managers.CreateManagers(sceneReferenceProvider);
		}

		public void Update() {
			Managers.UpdateManagers();
		}

		public void OnDisable() {
			Managers.RemoveManagers();
		}

		public SceneReferenceProvider GetSceneReference() {
			return sceneReferenceProvider;
		}
	}
}
