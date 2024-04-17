using UnityEngine;
using UnityEngine.Pool;

namespace Oakholm {
	public class Tile {

		public Vector3Int Position { get; }
		public float Height { get; private set; }

		private TileView view;
		private SpriteRenderer spriteRenderer;

		public Tile(Vector3Int position, IObjectPool<TileView> tilePool, Grid tileGrid) {
			Position = position;
			UsePooledGameObject(tilePool, tileGrid);
		}

		private void UsePooledGameObject(IObjectPool<TileView> tilePool, Grid tileGrid) {
			view = tilePool.Get();
			view.name = $"Tile_{Position}";
			view.transform.position = tileGrid.GetCellCenterWorld(Position);
			spriteRenderer = view.SpriteRenderer;

			SetProperties();

			view.gameObject.SetActive(true);
		}

		public void Release(IObjectPool<TileView> tilePool) {
			tilePool.Release(view);
		}

		private void SetProperties() {
			MapGenerator.SetTileHeight(this);
		}

		public void SetHeight(float height) {
			Height = height;
			spriteRenderer.color = new Color(height, height, height, 1);
			view.name += $"_H:{Height}";
		}
	}
}
