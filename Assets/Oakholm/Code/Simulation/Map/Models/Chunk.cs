using UnityEngine;

namespace Oakholm {
	public class Chunk {

		private readonly TileView tilePrefab;
		private readonly Grid tileGrid;

		public const int Size = 16;

		public Vector2Int Position { get; private set; }
		private (Vector2Int min, Vector2Int max) positionBounds;
		public RectInt Rect { get; private set; }

		private readonly Tile[][] tiles = new Tile[Size][];
		private readonly Tile[] tilesLinear = new Tile[Size * Size];

		public Chunk(Vector2Int position, TileView tilePrefab, Grid tileGrid) {
			this.tilePrefab = tilePrefab;
			this.tileGrid = tileGrid;

			Position = position;
			Create();
		}

		private void Create() {

			SetPrerequisiteValues();

			for (int y = positionBounds.min.y, yIndex = 0; y < positionBounds.max.y; y++, yIndex++) {
				tiles[yIndex] = new Tile[Size];
				for (int x = positionBounds.min.x, xIndex = 0; x < positionBounds.max.x; x++, xIndex++) {
					Tile tile = new Tile(
						new Vector3Int(x, y),
						tilePrefab,
						tileGrid
					);
					tiles[yIndex][xIndex] = tile;
					tilesLinear[yIndex * Size + xIndex] = tile;
				}
			}
		}

		public void Reset(Vector2Int position) {
			Position = position;
			SetPrerequisiteValues();

			for (int y = positionBounds.min.y, yIndex = 0; y < positionBounds.max.y; y++, yIndex++) {
				for (int x = positionBounds.min.x, xIndex = 0; x < positionBounds.max.x; x++, xIndex++) {
					tiles[yIndex][xIndex].Reset(new Vector3Int(x, y), tileGrid);
				}
			}
		}

		private void SetPrerequisiteValues() {
			CalculatePositionBounds();
			CalculateRect();
		}

		private void CalculatePositionBounds() {
			Vector2Int min = Position * Size;
			Vector2Int max = Position * Size + (Vector2Int.one * Size);
			positionBounds = (min, max);
		}

		private void CalculateRect() {
			Rect = new RectInt(
				positionBounds.min.x / Size,
				positionBounds.min.y / Size,
				(positionBounds.max.x - positionBounds.min.x) / Size,
				(positionBounds.max.y - positionBounds.min.y) / Size
			);
		}

		private void InitializeTiles(bool createTiles) {

		}

		public void Release() {
			foreach (Tile tile in tilesLinear) {
				tile.Release();
			}
		}
	}
}
