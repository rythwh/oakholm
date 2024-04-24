using Unity.Mathematics;
using UnityEngine;

namespace Oakholm {
	public class TileView : MonoBehaviour {

		[SerializeField] private SpriteRenderer spriteRenderer;

		public void Initialize(Tile tile) {
			Set(tile);
		}

		public void Set(Tile tile) {
			int2 position = tile.Position;
			transform.position = new Vector2(position.x, position.y);

			gameObject.SetActive(true);

			spriteRenderer.color = tile.Height.obj.DebugColour * tile.Height.value;

			#if UNITY_EDITOR
			gameObject.name = $"Tile_{position}";
			gameObject.name += $"_H:{tile.Height}";
			#endif
		}
	}
}
