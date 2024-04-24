using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Oakholm {

	[UsedImplicitly]
	public class MapManager : Manager {

		private const int EdgeChunkBufferSize = 2;

		private readonly StateManager stateManager;
		private readonly CameraManager cameraManager;
		private readonly LoadingManager loadingManager;

		private readonly Grid tileGrid;

		private MapView mapPrefab;
		private ChunkView chunkPrefab;
		private TileView tilePrefab;

		public Map Map { get; private set; }
		public MapView MapView { get; private set; }

		private readonly int seed;

		public MapManager(
			SceneReferenceProvider sceneReferenceProvider,
			StateManager stateManager,
			CameraManager cameraManager,
			LoadingManager loadingManager
		) {
			this.stateManager = stateManager;
			this.cameraManager = cameraManager;
			this.loadingManager = loadingManager;

			tileGrid = sceneReferenceProvider.TileGrid;

			stateManager.OnStateChanged += OnStateChanged;

			cameraManager.OnCameraPositionChanged += OnCameraPositionChanged;
			cameraManager.OnCameraZoomChanged += OnCameraZoomChanged;

			seed = Random.Range(int.MinValue, int.MaxValue);
		}

		public override void OnClose() {

			stateManager.OnStateChanged -= OnStateChanged;

			cameraManager.OnCameraPositionChanged -= OnCameraPositionChanged;
			cameraManager.OnCameraZoomChanged -= OnCameraZoomChanged;
		}

		private void OnStateChanged((EState oldState, EState newState) states) {
			if (states.newState == EState.Loading) {
				loadingManager.AddLoadingTaskHandle(CreateMap());
				loadingManager.AddLoadingTaskHandle(CreateMapRepresentation());
			}
		}

		private async UniTask CreateMap() {
			MapGenerator.Initialize(seed);
			Map = new Map();
			await UniTask.CompletedTask;
		}

		private async UniTask CreateMapRepresentation() {
			await GetRepresentationPrefabs();
			MapView = Object.Instantiate(mapPrefab, tileGrid.transform);
			MapView.Initialize(tileGrid, chunkPrefab, tilePrefab);
		}

		private void OnCameraPositionChanged(Vector2 _) {
			OnCameraChanged();
		}

		private void OnCameraZoomChanged(float _) {
			OnCameraChanged();
		}

		private void OnCameraChanged() {

			RectInt cameraWorldRect = cameraManager.GetCameraWorldRect();
			RectInt validAreaRect = GetValidAreaRectChunkScale(cameraWorldRect);

			Map?.SetValidAreaRect(validAreaRect);
			Map?.UnloadHiddenChunks();
			Map?.CreateChunks();
		}

		public override void OnUpdate() {
			Map?.ProcessQueuedChunksToUnload();
			Map?.ProcessQueuedChunksToCreate();

			MapBinder.ApplyChunks(Map?.GetChunks(), MapView);
		}

		private async UniTask GetRepresentationPrefabs() {
			mapPrefab = (await Addressables.LoadAssetAsync<GameObject>("Simulation/Map")).GetComponent<MapView>();
			chunkPrefab = (await Addressables.LoadAssetAsync<GameObject>("Simulation/Chunk")).GetComponent<ChunkView>();
			tilePrefab = (await Addressables.LoadAssetAsync<GameObject>("Simulation/Tile")).GetComponent<TileView>();
		}

		private RectInt GetValidAreaRectChunkScale(RectInt rect) {
			RectInt newRect = new RectInt {
				yMin = rect.yMin / Chunk.Size - EdgeChunkBufferSize,
				yMax = rect.yMax / Chunk.Size + EdgeChunkBufferSize,
				xMin = rect.xMin / Chunk.Size - EdgeChunkBufferSize,
				xMax = rect.xMax / Chunk.Size + EdgeChunkBufferSize
			};
			return newRect;
		}
	}
}
