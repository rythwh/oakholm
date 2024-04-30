using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

namespace Oakholm {
	public class Map {

		private ObjectPool<Chunk> chunkPool;
		private readonly HashSet<Chunk> chunks = new();
		private readonly HashSet<int2> chunkPositions = new();

		private readonly SimplePriorityQueue<int2, int> createChunkQueue = new(((i1, i2) => i1.CompareTo(i2)));
		private JobHandle updateCreateChunkQueuePrioritiesHandle;
		private readonly SimplePriorityQueue<Chunk, int> unloadChunkQueue = new(((i1, i2) => i2.CompareTo(i1)));

		private int2 previousValidAreaRectCenter;
		private RectInt currentValidAreaRect;
		private int2 currentValidAreaRectCenter;

		private const int MaxChunkDistance = 16 * Chunk.Size;

		public Map(TileView tilePrefab, Grid tileGrid) {
			CreateChunkPool(tilePrefab, tileGrid);
		}

		private void CreateChunkPool(TileView tilePrefab, Grid tileGrid) {
			chunkPool = new ObjectPool<Chunk>(
				createFunc: () => CreatePoolChunk(tilePrefab, tileGrid)
			);
		}

		private Chunk CreatePoolChunk(TileView tilePrefab, Grid tileGrid) {
			return new Chunk(int2.zero, tilePrefab, tileGrid);
		}

		// Create Chunks

		public void CreateChunks() {
			for (int y = currentValidAreaRect.yMin; y < currentValidAreaRect.yMax; y++) {
				for (int x = currentValidAreaRect.xMin; x < currentValidAreaRect.xMax; x++) {
					int2 chunkPosition = new int2(x, y);
					int distance = CalculateChunkQueuePriority(chunkPosition, currentValidAreaRectCenter);
					if (distance * Chunk.Size > MaxChunkDistance) {
						continue;
					}
					if (chunkPositions.Contains(chunkPosition)) {
						continue;
					}
					if (createChunkQueue.Contains(chunkPosition)) {
						continue;
					}
					unloadChunkQueue.TryRemove(unloadChunkQueue.FirstOrDefault(chunk => chunk.Position.Equals(chunkPosition)));

					createChunkQueue.Enqueue(chunkPosition, distance);
				}
			}
		}


		private void UpdateCreateChunkQueuePriorities() {

			if (currentValidAreaRectCenter.Equals(previousValidAreaRectCenter)) {
				return;
			}

			// createChunkQueue
			// 	.AsParallel()
			// 	.ForAll(chunkPosition => {
			// 		createChunkQueue.TryUpdatePriority(chunkPosition, CalculateChunkQueuePriority(chunkPosition, currentValidAreaRectCenter));
			// 	});

			if (!updateCreateChunkQueuePrioritiesHandle.IsCompleted) {
				return;
			}
			UpdatePrioritiesJob updatePrioritiesJob = new UpdatePrioritiesJob(
				currentValidAreaRectCenter,
				new NativeArray<int2>(createChunkQueue.ToArray(), Allocator.TempJob),
				new NativeArray<int>());
			updateCreateChunkQueuePrioritiesHandle = updatePrioritiesJob.Schedule();
//			for (int prioritiesIndex = 0; prioritiesIndex < updatePrioritiesJob
//			createChunkQueue.TryUpdatePriority()
		}

		public void ProcessQueuedChunksToCreate() {
			if (!createChunkQueue.TryDequeue(out int2 nextChunkPosition)) {
				return;
			}
			CreateChunk(nextChunkPosition);
		}

		private void CreateChunk(int2 chunkPosition) {

			Chunk chunk = chunkPool.Get();
			chunk.Reset(chunkPosition);

			chunks.Add(chunk);
			chunkPositions.Add(chunkPosition);
		}

		// Unload Chunks

		public void UnloadHiddenChunks() {
			foreach (Chunk chunk in chunks) {
				if (currentValidAreaRect.Overlaps(chunk.Rect)) {
					continue;
				}
				if (unloadChunkQueue.Contains(chunk)) {
					continue;
				}
				createChunkQueue.TryRemove(chunk.Position);
				unloadChunkQueue.Enqueue(chunk, CalculateChunkQueuePriority(chunk.Position, currentValidAreaRectCenter));
			}
		}

		private void UpdateUnloadChunkQueuePriorities() {

			if (currentValidAreaRectCenter.Equals(previousValidAreaRectCenter)) {
				return;
			}

			unloadChunkQueue
				.AsParallel()
				.ForAll(chunk => {
					unloadChunkQueue.TryUpdatePriority(chunk, CalculateChunkQueuePriority(chunk.Position, currentValidAreaRectCenter));
				});
		}

		public void ProcessQueuedChunksToUnload() {
			if (!unloadChunkQueue.TryDequeue(out Chunk chunkToUnload)) {
				return;
			}
			UnloadChunk(chunkToUnload);
		}

		private void UnloadChunk(Chunk chunk) {

			chunk.Release();
			chunkPool.Release(chunk);

			chunks.Remove(chunk);
			chunkPositions.Remove(chunk.Position);
		}

		public void RebuildAllChunks() {
			createChunkQueue.Clear();
			unloadChunkQueue.Clear();
			SetValidAreaRect(RectInt.zero);
			UnloadHiddenChunks();
		}

		private static int CalculateChunkQueuePriority(int2 chunkPosition, int2 validAreaCenterPosition) {
			return (int)math.distance(chunkPosition, validAreaCenterPosition);
		}

		public void SetValidAreaRect(RectInt validAreaRect) {
			previousValidAreaRectCenter = currentValidAreaRectCenter;
			currentValidAreaRect = validAreaRect;
			Vector2Int newValidAreaRectCenterV2 = Vector2Int.RoundToInt(currentValidAreaRect.center);
			currentValidAreaRectCenter = new int2(newValidAreaRectCenterV2.x, newValidAreaRectCenterV2.y);

			UpdateCreateChunkQueuePriorities();
			UpdateUnloadChunkQueuePriorities();
		}

		public struct UpdatePrioritiesJob : IJob {

			private readonly int2 validAreaRectCenter;
			private NativeArray<int2> positions;
			private NativeArray<int> priorities;

			public UpdatePrioritiesJob(int2 validAreaRectCenter, NativeArray<int2> positions, NativeArray<int> priorities) {
				this.validAreaRectCenter = validAreaRectCenter;
				this.positions = positions;
				this.priorities = priorities;
			}

			public void Execute() {
				for (int i = 0; i < positions.Length; i++) {
					priorities[i] = CalculateChunkQueuePriority(positions[i], validAreaRectCenter);
				}
			}

			public void Dispose() {
				positions.Dispose();
				priorities.Dispose();
			}
		}
	}
}
