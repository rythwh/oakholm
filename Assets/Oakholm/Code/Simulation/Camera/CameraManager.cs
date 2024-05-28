using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace Oakholm {

	[UsedImplicitly]
	public partial class CameraManager : Manager {

		private readonly StateManager stateManager;
		private readonly Camera camera;
		private readonly Transform cameraTransform;

		private const int CameraMoveSpeedMultiplier = 1;
		public event Action<Vector2> OnCameraPositionChanged;

		private const int CameraZoomSpeedMultiplier = 10;
		private const int ZoomMin = 5;
		private const int ZoomMax = 100;
		private const float ZoomTweenDuration = 0.5f;

		private UniTask zoomTaskHandle;
		public event Action<float> OnCameraZoomChanged;

		public CameraManager(SceneReferenceProvider sceneReferenceProvider, StateManager stateManager) {
			this.stateManager = stateManager;

			camera = sceneReferenceProvider.Camera;
			cameraTransform = camera.transform;
		}

		public override void OnCreate() {
			EnableInputSystem();
		}

		public override void OnGameSetupComplete() {
			OnCameraPositionChanged?.Invoke(cameraTransform.position);
		}

		public override void OnUpdate() {
			if (!Mathf.Approximately(moveVector.magnitude, 0)) {
				MoveCamera();
			}
		}

		public override void OnClose() {
			DisableInputSystem();
		}

		private void MoveCamera() {

			if (stateManager.State != EState.Simulation) {
				return;
			}

			cameraTransform.Translate(moveVector * (CameraMoveSpeedMultiplier * camera.orthographicSize * Time.deltaTime));
			OnCameraPositionChanged?.Invoke(cameraTransform.position);
		}

		private void ZoomCamera() {

			if (stateManager.State != EState.Simulation) {
				return;
			}
			if (Mathf.Approximately(zoomAxis, 0)) {
				return;
			}

			float currentZoom = camera.orthographicSize;
			float newZoom = currentZoom + (zoomAxis * CameraZoomSpeedMultiplier);
			newZoom = Mathf.Clamp(newZoom, ZoomMin, ZoomMax);

			if (!zoomTaskHandle.GetAwaiter().IsCompleted) {
				zoomTaskHandle.Forget();
			}

			zoomTaskHandle = DOTween
				.To(
					() => camera.orthographicSize,
					value => camera.orthographicSize = value,
					newZoom,
					ZoomTweenDuration)
				.SetEase(Ease.OutCubic)
				.Play()
				.OnComplete(() => OnCameraZoomChanged?.Invoke(newZoom))
				.AsyncWaitForCompletion()
				.AsUniTask();
		}

		public RectInt CalculateCameraWorldRect() {
			Vector2Int bottomLeftCorner = Vector2Int.FloorToInt(camera.ViewportToWorldPoint(new Vector3(0, 0, 0)));
			Vector2Int topRightCorner = Vector2Int.CeilToInt(camera.ViewportToWorldPoint(new Vector3(1, 1, 1)));

			int xMin = bottomLeftCorner.x;
			int yMin = bottomLeftCorner.y;

			int width = topRightCorner.x - bottomLeftCorner.x;
			int height = topRightCorner.y - bottomLeftCorner.y;

			return new RectInt(
				xMin,
				yMin,
				width,
				height
			);
		}
	}
}
