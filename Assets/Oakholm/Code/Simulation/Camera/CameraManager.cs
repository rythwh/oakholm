using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Oakholm {

	[UsedImplicitly]
	public class CameraManager : Manager {

		private readonly StateManager stateManager;
		private readonly Camera camera;
		private readonly Transform cameraTransform;

		private readonly InputSystemActions inputSystemActions;

		private const int CameraMoveSpeedMultiplier = 50;
		private Vector2 moveVector = Vector2.zero;
		public event Action<Vector2> OnCameraPositionChanged;

		private const int CameraZoomSpeedMultiplier = 10;
		private const int ZoomMin = 5;
		private const int ZoomMax = 100;
		private const float ZoomTweenDuration = 0.5f;
		private float zoomAxis = 0;
		private UniTask zoomTaskHandle;
		public event Action<float> OnCameraZoomChanged;

		public CameraManager(SceneReferenceProvider sceneReferenceProvider, StateManager stateManager) {
			this.stateManager = stateManager;

			camera = sceneReferenceProvider.Camera;
			cameraTransform = camera.transform;

			inputSystemActions = new InputSystemActions();
		}

		public override void OnCreate() {
			inputSystemActions.Enable();

			inputSystemActions.Simulation.MoveCamera.performed += OnMoveCameraPerformed;
			inputSystemActions.Simulation.MoveCamera.canceled += OnMoveCameraCancelled;

			inputSystemActions.Simulation.ZoomCamera.performed += OnZoomCameraPerformed;
			inputSystemActions.Simulation.ZoomCamera.canceled += OnZoomCameraCancelled;
		}

		public override void OnGameSetupComplete() {
			OnCameraPositionChanged?.Invoke(cameraTransform.position);
		}

		public override void OnUpdate() {
			MoveCamera();
		}

		public override void OnClose() {
			inputSystemActions.Disable();

			inputSystemActions.Simulation.MoveCamera.performed -= OnMoveCameraPerformed;
			inputSystemActions.Simulation.MoveCamera.canceled -= OnMoveCameraCancelled;

			inputSystemActions.Simulation.ZoomCamera.performed -= OnZoomCameraPerformed;
			inputSystemActions.Simulation.ZoomCamera.canceled -= OnZoomCameraCancelled;
		}

		private void OnMoveCameraPerformed(InputAction.CallbackContext callbackContext) {
			moveVector = callbackContext.ReadValue<Vector2>() * CameraMoveSpeedMultiplier;
		}

		private void OnMoveCameraCancelled(InputAction.CallbackContext callbackContext) {
			moveVector = Vector2.zero;
		}

		private void MoveCamera() {
			if (stateManager.State != EState.Simulation) {
				return;
			}
			cameraTransform.Translate(moveVector * Time.deltaTime);
			OnCameraPositionChanged?.Invoke(cameraTransform.position);
		}

		private void OnZoomCameraPerformed(InputAction.CallbackContext callbackContext) {
			zoomAxis = callbackContext.ReadValue<float>();
			ZoomCamera();
		}

		private void OnZoomCameraCancelled(InputAction.CallbackContext callbackContext) {
			zoomAxis = 0;
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

		public RectInt GetCameraWorldRect() {
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
