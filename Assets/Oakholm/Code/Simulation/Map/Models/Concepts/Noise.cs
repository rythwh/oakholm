using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Oakholm {

	[BurstCompile]
	public class Noise {

		private string Type { get; }
		public float Multiplier { get; set; }
		public float Weight { get; set; }
		private int Offset { get; set; }

		public Noise(string type, float multiplier, float weight) {
			Type = type;
			Multiplier = multiplier;
			Weight = weight;
			Offset = Mathf.RoundToInt(Random.Range(short.MinValue, short.MaxValue));
		}

		[BurstCompile]
		public float GetPerlinNoise(float2 position) {
			return noise.cnoise((position + Offset) * Multiplier) * Weight;
		}

		public override string ToString() {
			return Type;
		}
	}
}
