using System.Collections.Generic;
using Oakholm.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Oakholm {

	[BurstCompile]
	public struct Chunk : System.IEquatable<Chunk>, INativeDisposable {

		public const int Size = 16;
		public const int Area = Size * Size;

		public int2 Position { get; private set; }
		public RectInt Rect { get; private set; }
		private (int2 min, int2 max) positionBounds;

		private NativeArray<Tile> tiles;

		public bool Initialized { get; set; }
		public bool Dirty { get; set; }

		public Chunk(int2 position) : this() {

			tiles = new NativeArray<Tile>(Size * Size, Allocator.Persistent, NativeArrayOptions.ClearMemory);

			Set(position);

			Initialized = true;
		}

		private void Set(int2 position) {

			Position = position;

			SetPrerequisiteValues();

			for (int y = positionBounds.min.y, yIndex = 0; y < positionBounds.max.y; y++, yIndex++) {
				for (int x = positionBounds.min.x, xIndex = 0; x < positionBounds.max.x; x++, xIndex++) {
					Tile tile = new Tile(new int2(x, y));
					tiles[yIndex * Size + xIndex] = tile;
				}
			}
		}

		public void Reset(int2 position) {

			Position = position;

			SetPrerequisiteValues();

			for (int y = positionBounds.min.y, yIndex = 0; y < positionBounds.max.y; y++, yIndex++) {
				for (int x = positionBounds.min.x, xIndex = 0; x < positionBounds.max.x; x++, xIndex++) {
					GetTileAtPosition(xIndex, yIndex).Set(new int2(x, y));
				}
			}
		}

		private void SetPrerequisiteValues() {
			CalculatePositionBounds();
			CalculateRect();
		}

		private void CalculatePositionBounds() {
			int2 min = Position * Size;
			int2 max = Position * Size + (new int2(1) * Size);
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

		public Tile[] GetTiles() {
			return tiles.ToArray();
		}

		private Tile GetTileAtPosition(int x, int y) {
			return tiles[y * Size + x];
		}

		public void Release() {

		}

		public bool Equals(Chunk other) {
			return Position.Equals(other.Position);
		}

		public override bool Equals(object obj) {
			return obj is Chunk other && Equals(other);
		}

		public override int GetHashCode() {
			return Position.GetHashCode();
		}

		public void Dispose() {
			tiles.Dispose();
		}

		public JobHandle Dispose(JobHandle inputDeps) {
			Dispose();
			return inputDeps;
		}
	}
}
