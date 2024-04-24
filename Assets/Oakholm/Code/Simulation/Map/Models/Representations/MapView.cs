using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Pool;

namespace Oakholm {
	public class MapView : MonoBehaviour {

		public bool Initialized { get; private set; }

		private ObjectPool<ChunkView> chunkPool;
		private readonly List<ChunkView> chunkViews = new();

		private Grid tileGrid;

		private ChunkView chunkPrefab;
		private TileView tilePrefab;

		[SuppressMessage("ReSharper", "ParameterHidesMember")]
		public void Initialize(
			Grid tileGrid,
			ChunkView chunkPrefab,
			TileView tilePrefab
		) {
			this.tileGrid = tileGrid;
			this.chunkPrefab = chunkPrefab;
			this.tilePrefab = tilePrefab;

			CreateChunkPool();

			Initialized = true;
		}

		public void CreateChunks(IEnumerable<Chunk> chunks) {
			foreach (ChunkView chunkView in chunkViews) {
				chunkPool.Release(chunkView);
			}
			chunkViews.Clear();

			foreach (Chunk chunk in chunks) {
				ChunkView chunkView = chunkPool.Get();
				if (chunkView.Initialized) {
					chunkView.Reinitialize(chunk);
				} else {
					chunkView.Initialize(chunk, tileGrid, tilePrefab);
				}
			}
		}

		private void CreateChunkPool() {
			chunkPool = new ObjectPool<ChunkView>(
				createFunc: CreatePoolChunk,
				actionOnRelease: chunkView => chunkView.Release()
			);
		}

		private ChunkView CreatePoolChunk() {
			ChunkView chunkView = Instantiate(chunkPrefab, transform);
			chunkViews.Add(chunkView);
			return chunkView;
		}
	}
}
