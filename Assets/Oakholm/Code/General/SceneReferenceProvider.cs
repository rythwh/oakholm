using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Oakholm {
	public class SceneReferenceProvider : MonoBehaviour {

		[SerializeField] private new Camera camera;
		public Camera Camera => camera;

		[SerializeField] private Light2D sun;
		public Light2D Sun => sun;

		[SerializeField] private Canvas canvas;
		public Canvas Canvas => canvas;

		[SerializeField] private Grid tileGrid;
		public Grid TileGrid => tileGrid;
	}
}
