using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Oakholm {
	public class Chunk {

		public const int Size = 16;

		public Vector2Int Position { get; }
		public RectInt Rect { get; private set; }

		private readonly List<Tile> tiles = new List<Tile>();

		public Chunk(Vector2Int position, IObjectPool<TileView> tilePool, Grid tileGrid) {
			Position = position;
			Create(tilePool, tileGrid);
		}

		private void Create(IObjectPool<TileView> tilePool, Grid tileGrid) {
			Vector2Int min = Position * Size;
			Vector2Int max = Position * Size + (Vector2Int.one * Size);
			Rect = new RectInt(
				min.x / Size,
				min.y / Size,
				(max.x - min.x) / Size,
				(max.y - min.y) / Size
			);

			for (int y = min.y; y < max.y; y++) {
				for (int x = min.x; x < max.x; x++) {
					Tile tile = new Tile(
						new Vector3Int(x, y),
						tilePool,
						tileGrid
					);
					tiles.Add(tile);
				}
			}
		}

		public void Release(IObjectPool<TileView> tilePool) {
			foreach (Tile tile in tiles) {
				tile.Release(tilePool);
			}
			tiles.Clear();
		}
	}
}
