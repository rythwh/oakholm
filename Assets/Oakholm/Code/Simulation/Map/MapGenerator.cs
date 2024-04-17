using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Oakholm {
	public static class MapGenerator {

		public static bool Initialized = false;

		private static readonly List<Noise> Noises = new List<Noise>();

		public static void Initialize(int seed) {
			SetSeed(seed);
			SetNoises();
			Initialized = true;
		}

		private static void SetSeed(int seed) {
			Random.InitState(seed);
		}

		private static void SetNoises() {
			Noises.Add(new Noise(ENoise.Continentalness, 0.002f));
			Noises.Add(new Noise(ENoise.Hilliness, 0.01f));
			Noises.Add(new Noise(ENoise.Bumpiness, 0.05f));
			Noises.Add(new Noise(ENoise.Temperature, 0.0005f));
		}

		[BurstCompile]
		public static void SetTileHeight(Tile tile) {
			Vector2 position = (Vector3)tile.Position;
			float noiseValue = 0;
			foreach (Noise noise in Noises) {
				noiseValue += noise.GetPerlinNoise(position);
			}
			noiseValue /= Noises.Count;
			noiseValue = Mathf.Clamp01(noiseValue);

			tile.SetHeight(noiseValue);
		}


	}
}
