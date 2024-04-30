using Unity.Mathematics;
using UnityEngine;

namespace Oakholm {
	public class Chunk {

		private readonly TileView tilePrefab;
		private readonly Grid tileGrid;

		public const int Size = 16;

		public int2 Position { get; private set; }
		private (int2 min, int2 max) positionBounds;
		public RectInt Rect { get; private set; }

		private readonly Tile[][] tiles = new Tile[Size][];
		private readonly Tile[] tilesLinear = new Tile[Size * Size];

		public Chunk(int2 position, TileView tilePrefab, Grid tileGrid) {
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

		public void Reset(int2 position) {
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
			int2 min = Position * Size;
			int2 max = Position * Size + Size;
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

		public static bool operator ==(Chunk chunk1, Chunk chunk2) {
			if (chunk1 == null && chunk2 == null) {
				return true;
			}
			if (chunk1 == null ^ chunk2 == null) {
				return false;
			}
			return chunk1.Equals(chunk2);
		}

		public static bool operator !=(Chunk chunk1, Chunk chunk2) {
			return !(chunk1 == chunk2);
		}

		public override bool Equals(object otherObject) {
			return otherObject is Chunk otherChunk && Position.Equals(otherChunk.Position);
		}

		// ReSharper disable once NonReadonlyMemberInGetHashCode
		public override int GetHashCode() => Position.GetHashCode();
	}
}
