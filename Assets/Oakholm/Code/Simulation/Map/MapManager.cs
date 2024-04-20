using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

namespace Oakholm {

	[UsedImplicitly]
	public class MapManager : Manager {

		private const int EdgeChunkBufferSize = 2;

		private readonly StateManager stateManager;
		private readonly CameraManager cameraManager;
		private readonly LoadingManager loadingManager;

		private readonly Grid tileGrid;
		private TileView tilePrefab;
		private readonly ObjectPool<TileView> tilePool;
		private readonly HashSetPool<Chunk> chunkPool;

		private const int TilePoolCapacity = Chunk.Size << 12;

		public Map Map { get; private set; }

		private readonly int seed;

		public MapManager(SceneReferenceProvider sceneReferenceProvider, StateManager stateManager, CameraManager cameraManager, LoadingManager loadingManager) {

			this.stateManager = stateManager;
			this.cameraManager = cameraManager;
			this.loadingManager = loadingManager;

			tileGrid = sceneReferenceProvider.TileGrid;
			tilePool = new ObjectPool<TileView>(
				CreateTileView,
				actionOnRelease: ReleaseTileView,
				defaultCapacity: TilePoolCapacity,
				maxSize: TilePoolCapacity
			);
			chunkPool = new HashSetPool<Chunk>(

			);

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
				loadingManager.AddLoadingTaskHandle(UniTask.Create(CreateMap));
			}
		}

		private async UniTask CreateMap() {
			MapGenerator.Initialize(seed);
			tilePrefab = (await GetTilePrefab()).GetComponent<TileView>();
			//await PrefillPool();
			Map = new Map(tilePool, tileGrid);
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
			Map?.UnloadHiddenChunks(validAreaRect);
			Map?.CreateChunks(validAreaRect);
		}

		public override void OnUpdate() {
			Map?.ProcessQueuedChunksToUnload();
			Map?.ProcessQueuedChunksToCreate();
		}

		private async UniTask<GameObject> GetTilePrefab() {
			return await Addressables.LoadAssetAsync<GameObject>("Simulation/Tile");
		}

		private TileView CreateTileView() {
			return Object.Instantiate(tilePrefab, Vector2.zero, Quaternion.identity, tileGrid.transform);
		}

		private void ReleaseTileView(TileView tileView) {
			tileView.name = "Pooled";
			tileView.gameObject.SetActive(false);
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
