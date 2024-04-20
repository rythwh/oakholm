using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Oakholm {

	public enum ENoise {
		Continentalness,
		Hilliness,
		Bumpiness,
		Temperature
	}

	public class Noise {

		public ENoise Type { get; }
		public float Multiplier { get; set; }
		public float Weight { get; set; }
		public int Offset { get; private set; }

		public Noise(ENoise type, float multiplier, float weight) {
			Type = type;
			Multiplier = multiplier;
			Weight = weight;
			Offset = Mathf.RoundToInt(Random.Range(short.MinValue, short.MaxValue));
		}

		[BurstCompile]
		public float GetPerlinNoise(Vector2 position) {
			position += Vector2.one * Offset;
			position *= Multiplier;
			return Mathf.PerlinNoise(position.x, position.y) * Weight;
		}

		public override string ToString() {
			return Type.ToString();
		}
	}
}
