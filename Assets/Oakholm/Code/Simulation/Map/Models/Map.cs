using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Priority_Queue;
using UnityEngine;
using UnityEngine.Pool;

namespace Oakholm {
	public class Map {

		private readonly IObjectPool<TileView> tilePool;
		private readonly Grid tileGrid;
		private readonly HashSet<Chunk> chunks = new HashSet<Chunk>();
		private readonly HashSet<Vector2Int> chunkPositions = new HashSet<Vector2Int>();

		private readonly SimplePriorityQueue<Vector2Int, int> createChunkQueue = new SimplePriorityQueue<Vector2Int, int>();
		private readonly SimplePriorityQueue<Chunk, int> unloadChunkQueue = new SimplePriorityQueue<Chunk, int>();

		private const int MaxChunkDistance = 16 * Chunk.Size;

		public Map(IObjectPool<TileView> tilePool, Grid tileGrid) {
			this.tilePool = tilePool;
			this.tileGrid = tileGrid;
		}

		public void CreateChunks(RectInt validAreaRect) {
			for (int y = validAreaRect.yMin; y < validAreaRect.yMax; y++) {
				for (int x = validAreaRect.xMin; x < validAreaRect.xMax; x++) {
					Vector2Int chunkPosition = new Vector2Int(x, y);
					int distance = (int)Vector2.Distance(chunkPosition, validAreaRect.center);
					if (distance * Chunk.Size > MaxChunkDistance) {
						continue;
					}
					if (chunkPositions.Contains(chunkPosition)) {
						continue;
					}
					if (createChunkQueue.Contains(chunkPosition)) {
						continue;
					}
					Chunk chunkQueuedToUnload = unloadChunkQueue.FirstOrDefault(chunk => chunk.Position == chunkPosition);
					if (chunkQueuedToUnload != null) {
						unloadChunkQueue.Remove(chunkQueuedToUnload);
					}
					createChunkQueue.EnqueueWithoutDuplicates(chunkPosition, distance);
				}
			}
		}

		public void ProcessQueuedChunksToCreate() {
			if (!createChunkQueue.TryDequeue(out Vector2Int nextChunkPosition)) {
				return;
			}
			CreateChunk(nextChunkPosition);
		}

		private void CreateChunk(Vector2Int chunkPosition) {
			Chunk chunk = new Chunk(chunkPosition, tilePool, tileGrid);
			chunks.Add(chunk);
			chunkPositions.Add(chunkPosition);
		}

		public void UnloadHiddenChunks(RectInt validAreaRect) {
			foreach (Chunk chunk in chunks) {
				if (validAreaRect.Overlaps(chunk.Rect)) {
					continue;
				}
				if (createChunkQueue.Contains(chunk.Position)) {
					createChunkQueue.Remove(chunk.Position);
				}
				unloadChunkQueue.EnqueueWithoutDuplicates(chunk, (int)Vector2.Distance(chunk.Position, validAreaRect.center));
			}
		}

		public void ProcessQueuedChunksToUnload() {
			Chunk chunkToUnload = unloadChunkQueue.ElementAtOrDefault(0);
			if (chunkToUnload == null) {
				return;
			}
			unloadChunkQueue.Remove(chunkToUnload);
			UnloadChunk(chunkToUnload);
		}

		private void UnloadChunk(Chunk chunk) {
			chunk.Release(tilePool);
			chunks.Remove(chunk);
			chunkPositions.Remove(chunk.Position);
		}

		public void RebuildAllChunks() {
			createChunkQueue.Clear();
			unloadChunkQueue.Clear();
			UnloadHiddenChunks(RectInt.zero);
		}
	}
}
