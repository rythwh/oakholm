using UnityEngine;

namespace Oakholm {
	public class Tile {

		public Vector3Int Position { get; private set; }
		private (float value, Height obj) Height { get; set; }

		private TileView view;
		private SpriteRenderer spriteRenderer;

		public Tile(Vector3Int position, TileView tilePrefab, Grid tileGrid) {
			Position = position;
			Create(tilePrefab, tileGrid);
		}

		private void Create(TileView tilePrefab, Grid tileGrid) {
			view = Object.Instantiate(tilePrefab, tileGrid.GetCellCenterWorld(Position), Quaternion.identity, tileGrid.transform);
			spriteRenderer = view.SpriteRenderer;

			SetProperties();

			#if UNITY_EDITOR
			view.name = $"Tile_{Position}";
			#endif
		}

		public void Reset(Vector3Int position, Grid tileGrid) {
			Position = position;
			view.transform.position = tileGrid.GetCellCenterWorld(Position);

			SetProperties();
		}

		private void SetProperties() {
			SetHeight();

			view.gameObject.SetActive(true);
		}

		private void SetHeight() {
			Height = MapGenerator.GetTileHeight(this);
			view.SetColour(Height);
			#if UNITY_EDITOR
			view.name += $"_H:{Height}";
			#endif
		}

		public void Release() {
			view.gameObject.SetActive(false);
		}
	}
}
