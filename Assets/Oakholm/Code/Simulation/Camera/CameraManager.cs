using System;
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

		private const int CameraZoomSpeedMultiplier = 100;
		private double zoomAxis = 0;
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
			ZoomCamera();
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
			zoomAxis = callbackContext.ReadValue<double>();
			Debug.Log(zoomAxis);
		}

		private void OnZoomCameraCancelled(InputAction.CallbackContext callbackContext) {
			zoomAxis = 0;
		}

		private void ZoomCamera() {
			if (stateManager.State != EState.Simulation) {
				return;
			}
			float orthographicSize = camera.orthographicSize;
			orthographicSize += (float)zoomAxis * CameraZoomSpeedMultiplier * Time.deltaTime;
			camera.orthographicSize = orthographicSize;
			OnCameraZoomChanged?.Invoke(orthographicSize);
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
