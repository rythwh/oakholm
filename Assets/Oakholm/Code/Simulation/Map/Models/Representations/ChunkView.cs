using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Oakholm {
	public class ChunkView : MonoBehaviour {

		public bool Initialized { get; private set; }

		private Chunk chunk;

		private readonly List<TileView> tileViews = new List<TileView>();

		public void Initialize(Chunk chunk, Grid tileGrid, TileView tilePrefab) {
			this.chunk = chunk;
			CreateTiles(tileGrid, tilePrefab);
			gameObject.SetActive(true);
			Initialized = true;
		}

		private void CreateTiles(Grid tileGrid, TileView tilePrefab) {
			foreach (Tile tile in chunk.GetTiles()) {
				int2 tilePosition = tile.Position;
				TileView tileView = Instantiate(
					tilePrefab,
					tileGrid.GetCellCenterWorld(new Vector3Int(tilePosition.x, tilePosition.y)),
					Quaternion.identity,
					transform);
				tileView.Initialize(tile);
				tileViews.Add(tileView);
			}
		}

		public void Reinitialize(Chunk chunk) {
			this.chunk = chunk;
			ResetTiles();
			gameObject.SetActive(true);
		}

		private void ResetTiles() {
			Tile[] tiles = chunk.GetTiles();
			for (int i = 0; i < Chunk.Area; i++) {
				tileViews[i].Set(tiles[i]);
			}
		}

		public void Release() {
			gameObject.SetActive(false);
		}
	}
}
