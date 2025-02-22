﻿using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Oakholm {
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
				new Noise(ENoise.Continentalness, 0.002f, 20),
				new Noise(ENoise.Hilliness, 0.01f, 10),
				new Noise(ENoise.Bumpiness, 0.05f, 5)
			};
		}

		private static void SetHeights() {
			heights = new List<Height> {
				new Height(EHeight.Water, (0, 0.35f), Color.blue),
				new Height(EHeight.Ground, (0.35f, 0.6f), Color.green),
				new Height(EHeight.Mountain, (0.6f, 1), Color.white)
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
			return null;
		}

		public static (float value, Height obj) GetTileHeight(Tile tile) {
			Vector2 position = (Vector3)tile.Position;
			float noiseValue = 0;
			float weightSum = 0;
			foreach (Noise noise in noises) {
				noiseValue += noise.GetPerlinNoise(position);
				weightSum += noise.Weight;
			}
			noiseValue /= weightSum;
			noiseValue = Mathf.Clamp01(noiseValue);

			return (noiseValue, GetHeightFromValue(noiseValue));
		}
	}
}
