using UnityEngine;

namespace Oakholm {
	public class TileView : MonoBehaviour {
		[SerializeField] private SpriteRenderer spriteRenderer;
		public SpriteRenderer SpriteRenderer => spriteRenderer;

		public void SetColour((float value, Height obj) height) {
			spriteRenderer.color = height.obj.DebugColour * height.value;
		}
	}
}
