using System.Collections.Generic;

namespace Oakholm {
	public static class MapBinder {
		public static void ApplyChunks(IEnumerable<Chunk> chunks, MapView mapView) {

			if (chunks == null) {
				return;
			}
			if (mapView == null) {
				return;
			}

			mapView.CreateChunks(chunks);
		}
	}
}
