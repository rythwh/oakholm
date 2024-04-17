using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;
using UnityEngine.Pool;

namespace Oakholm {
	public class Map {

		private readonly IObjectPool<TileView> tilePool;
		private readonly Grid tileGrid;
		private readonly List<Chunk> chunks = new List<Chunk>();
		private readonly HashSet<Vector2Int> chunkPositions = new HashSet<Vector2Int>();

		private readonly SimplePriorityQueue<Vector2Int, int> createChunkQueue = new SimplePriorityQueue<Vector2Int, int>();
		private readonly Queue<Chunk> unloadChunkQueue = new Queue<Chunk>();

		public Map(IObjectPool<TileView> tilePool, Grid tileGrid) {
			this.tilePool = tilePool;
			this.tileGrid = tileGrid;
		}

		public void CreateChunks(RectInt validAreaRect) {
			for (int y = validAreaRect.yMin; y < validAreaRect.yMax; y++) {
				for (int x = validAreaRect.xMin; x < validAreaRect.xMax; x++) {
					Vector2Int chunkPosition = new Vector2Int(x, y);
					if (chunkPositions.Contains(chunkPosition)) {
						continue;
					}
					if (createChunkQueue.Contains(chunkPosition)) {
						continue;
					}
					createChunkQueue.Enqueue(chunkPosition, (int)Vector2.Distance(chunkPosition, validAreaRect.center));
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
				if (unloadChunkQueue.Contains(chunk)) {
					continue;
				}
				unloadChunkQueue.Enqueue(chunk);
			}
		}

		public void ProcessQueuedChunksToUnload() {
			if (!unloadChunkQueue.TryDequeue(out Chunk chunkToUnload)) {
				return;
			}
			UnloadChunk(chunkToUnload);
		}

		private void UnloadChunk(Chunk chunk) {
			chunk.Release(tilePool);
			chunks.Remove(chunk);
			chunkPositions.Remove(chunk.Position);
		}
	}
}
