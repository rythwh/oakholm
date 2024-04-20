using UnityEngine;

namespace Oakholm {

	public enum EHeight {
		Water,
		Ground,
		Mountain
	}

	public class Height {

		public EHeight Type { get; }
		public (float min, float max) Range { get; set; }
		public Color DebugColour { get; }

		public Height(
			EHeight type,
			(float min, float max) range,
			Color debugColour
		) {
			Type = type;
			Range = range;
			DebugColour = debugColour;
		}

		public bool IsInRange(float height) {
			return height >= Range.min && height <= Range.max;
		}

		public override string ToString() {
			return Type.ToString();
		}
	}
}
