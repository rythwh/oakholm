using Unity.Burst;
using Unity.Mathematics;

namespace Oakholm {

	[BurstCompile]
	public struct Tile {

		public int2 Position { get; private set; }
		public (float value, Height obj) Height { get; private set; }

		public bool Initialized { get; set; }
		public bool Dirty { get; set; }

		public Tile(int2 position) : this() {
			Set(position);
			Initialized = true;
		}

		public void Set(int2 position) {
			Position = position;
			Height = MapGenerator.GetTileHeight(this);
		}
	}
}
