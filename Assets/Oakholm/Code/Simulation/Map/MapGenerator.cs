using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Oakholm {

	[BurstCompile]
	public static class MapGenerator {

		public static bool Initialized = false;

		private static List<Noise> noises;
		private static List<Height> heights;

		public static void Initialize(int seed) {

			SetSeed(seed);

			SetNoises();
			SetHeights();

			Initialized = true;
		}

		private static void SetSeed(int seed) {
			Random.InitState(seed);
		}

		private static void SetNoises() {
			noises = new List<Noise> {
				new Noise("Continentalness", 0.002f, 20),
				new Noise("Hilliness", 0.01f, 10),
				new Noise("Bumpiness", 0.05f, 5)
			};
		}

		private static void SetHeights() {
			heights = new List<Height> {
				new Height(new NativeText("Water", Allocator.Persistent), (0, 0.35f), Color.blue),
				new Height(new NativeText("Ground", Allocator.Persistent), (0.35f, 0.7f), Color.green),
				new Height(new NativeText("Mountain", Allocator.Persistent), (0.7f, 1), Color.white)
			};
		}

		public static IEnumerable<Noise> GetNoises() {
			return noises.AsReadOnly();
		}

		public static IEnumerable<Height> GetHeights() {
			return heights.AsReadOnly();
		}

		public static Height GetHeightFromValue(float heightValue) {
			foreach (Height height in heights) {
				if (height.IsInRange(heightValue)) {
					return height;
				}
			}
			return default;
		}

		public static (float value, Height obj) GetTileHeight(Tile tile) {
			int2 position = tile.Position;
			float noiseValue = 0;
			float weightSum = 0;
			foreach (Noise noise in noises) {
				noiseValue += noise.GetPerlinNoise(position);
				weightSum += noise.Weight;
			}
			noiseValue = ((noiseValue / weightSum) + 1) / 2f;
			noiseValue = math.clamp(noiseValue, 0, 1);

			return (noiseValue, GetHeightFromValue(noiseValue));
		}
	}
}
