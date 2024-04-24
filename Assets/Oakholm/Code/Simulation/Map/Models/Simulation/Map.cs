using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

namespace Oakholm {

	[BurstCompile]
	public class Map {

		private const int MaxChunkDistance = 16 * Chunk.Size;

		private readonly List<Chunk> chunks = new List<Chunk>();
		private List<int2> chunkPositions = new List<int2>();

		private SimplePriorityQueue<int2, int> createChunkQueue = new SimplePriorityQueue<int2, int>();
		private SimplePriorityQueue<Chunk, int> unloadChunkQueue = new SimplePriorityQueue<Chunk, int>();
		private int2 previousValidAreaRectCenter;
		private RectInt currentValidAreaRect;
		private int2 currentValidAreaRectCenter;

		public void SetValidAreaRect(RectInt validAreaRect) {
			previousValidAreaRectCenter = currentValidAreaRectCenter;
			currentValidAreaRect = validAreaRect;
			Vector2Int newValidAreaRectCenterV2 = Vector2Int.RoundToInt(currentValidAreaRect.center);
			currentValidAreaRectCenter = new int2(newValidAreaRectCenterV2.x, newValidAreaRectCenterV2.y);

			UpdateCreateChunkQueuePriorities();
			UpdateUnloadChunkQueuePriorities();
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
					if (createChunkQueue.AsParallel().Contains(chunkPosition)) {
						continue;
					}
					Chunk chunkQueuedToUnload = unloadChunkQueue.AsParallel().FirstOrDefault(chunk => chunk.Position.Equals(chunkPosition));
					if (unloadChunkQueue.AsParallel().Contains(chunkQueuedToUnload)) {
						unloadChunkQueue.Remove(chunkQueuedToUnload);
					}

					createChunkQueue.EnqueueWithoutDuplicates(chunkPosition, distance);
				}
			}
		}

		private void UpdateCreateChunkQueuePriorities() {

			if (previousValidAreaRectCenter.Equals(currentValidAreaRectCenter)) {
				return;
			}

			Map map = this;
			SimplePriorityQueue<int2, int> queue = createChunkQueue;
			int2 validAreaCenterPosition = currentValidAreaRectCenter;
			createChunkQueue
				.AsParallel()
				.ForAll(chunkPosition => {
					queue.TryUpdatePriority(chunkPosition, map.CalculateChunkQueuePriority(chunkPosition, validAreaCenterPosition));
				});
		}

		public void ProcessQueuedChunksToCreate() {
			if (!createChunkQueue.TryDequeue(out int2 nextChunkPosition)) {
				return;
			}
			CreateChunk(nextChunkPosition);
		}

		private void CreateChunk(int2 chunkPosition) {

			if (chunkPositions.Contains(chunkPosition)) {
				chunkPositions.Remove(chunkPosition);
			}

			Chunk chunk = new Chunk(chunkPosition);

			chunks.Add(chunk);
			chunkPositions.Add(chunkPosition);
		}

		// Unload Chunks

		public void UnloadHiddenChunks() {
			foreach (Chunk chunk in chunks) {
				if (currentValidAreaRect.Overlaps(chunk.Rect)) {
					continue;
				}
				if (createChunkQueue.AsParallel().Contains(chunk.Position)) {
					createChunkQueue.Remove(chunk.Position);
				}
				unloadChunkQueue.EnqueueWithoutDuplicates(chunk, CalculateChunkQueuePriority(chunk.Position, currentValidAreaRectCenter));
			}
		}

		private void UpdateUnloadChunkQueuePriorities() {

			if (previousValidAreaRectCenter.Equals(currentValidAreaRectCenter)) {
				return;
			}

			Map map = this;
			SimplePriorityQueue<Chunk, int> queue = unloadChunkQueue;
			int2 validAreaCenterPosition = currentValidAreaRectCenter;
			unloadChunkQueue
				.AsParallel()
				.ForAll(chunk => {
					queue.TryUpdatePriority(chunk, map.CalculateChunkQueuePriority(chunk.Position, validAreaCenterPosition));
				});
		}

		public void ProcessQueuedChunksToUnload() {
			if (!unloadChunkQueue.TryDequeue(out Chunk chunkToUnload)) {
				return;
			}
			unloadChunkQueue.Remove(chunkToUnload);
			UnloadChunk(chunkToUnload);
		}

		private void UnloadChunk(Chunk chunk) {

			chunk.Release();
			chunk.Dispose();

			chunks.Remove(chunk);
			chunkPositions.Remove(chunk.Position);
		}

		//

		public void RebuildAllChunks() {
			createChunkQueue.Clear();
			unloadChunkQueue.Clear();
			SetValidAreaRect(RectInt.zero);
			UnloadHiddenChunks();
		}

		private int CalculateChunkQueuePriority(int2 chunkPosition, int2 validAreaCenterPosition) {
			return (int)math.distance(chunkPosition, validAreaCenterPosition);
		}

		public IEnumerable<Chunk> GetChunks() {
			return chunks;
		}
	}
}
