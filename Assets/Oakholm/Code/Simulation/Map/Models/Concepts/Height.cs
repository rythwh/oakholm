using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace Oakholm {

	[BurstCompile]
	public struct Height {

		public NativeText Type { get; }
		public (float min, float max) Range { get; set; }
		public Color DebugColour { get; }

		public Height(
			NativeText type,
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
