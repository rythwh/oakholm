using Unity.Burst;
using UnityEngine;

namespace Oakholm {

	public enum ENoise {
		Continentalness,
		Hilliness,
		Bumpiness,
		Temperature
	}

	public class Noise {

		public ENoise Type { get; }
		public float Multiplier { get; private set; }
		public int Offset { get; private set; }

		public Noise(ENoise type, float multiplier) {
			Type = type;
			Multiplier = multiplier;
			Offset = Mathf.RoundToInt(Random.Range(short.MinValue, short.MaxValue));
		}

		[BurstCompile]
		public float GetPerlinNoise(Vector2 position) {
			position += Vector2.one * Offset;
			position *= Multiplier;
			return Mathf.PerlinNoise(position.x, position.y);
		}
	}
}
