using UnityEngine;
using UnityEngine.InputSystem;

namespace Oakholm {
	public partial class CameraManager {

		private readonly InputSystemActions inputSystemActions = new InputSystemActions();

		private Vector3 moveVector = Vector2.zero;
		private float zoomAxis = 0;

		private void EnableInputSystem() {
			inputSystemActions.Enable();

			inputSystemActions.Simulation.MoveCamera.performed += OnMoveCameraPerformed;
			inputSystemActions.Simulation.MoveCamera.canceled += OnMoveCameraCancelled;

			inputSystemActions.Simulation.ZoomCamera.performed += OnZoomCameraPerformed;
			inputSystemActions.Simulation.ZoomCamera.canceled += OnZoomCameraCancelled;
		}

		private void DisableInputSystem() {
			inputSystemActions.Disable();

			inputSystemActions.Simulation.MoveCamera.performed -= OnMoveCameraPerformed;
			inputSystemActions.Simulation.MoveCamera.canceled -= OnMoveCameraCancelled;

			inputSystemActions.Simulation.ZoomCamera.performed -= OnZoomCameraPerformed;
			inputSystemActions.Simulation.ZoomCamera.canceled -= OnZoomCameraCancelled;
		}

		private void OnMoveCameraPerformed(InputAction.CallbackContext callbackContext) {
			moveVector = callbackContext.ReadValue<Vector2>() * CameraMoveSpeedMultiplier;
			MoveCamera();
		}

		private void OnMoveCameraCancelled(InputAction.CallbackContext callbackContext) {
			moveVector = Vector2.zero;
		}

		private void OnZoomCameraPerformed(InputAction.CallbackContext callbackContext) {
			zoomAxis = callbackContext.ReadValue<float>();
			ZoomCamera();
		}

		private void OnZoomCameraCancelled(InputAction.CallbackContext callbackContext) {
			zoomAxis = 0;
		}
	}
}
