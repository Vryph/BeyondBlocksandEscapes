using UnityEngine;

namespace BBE
{
    [CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
    public class PlayerController : InputController
    {
        private PlayerInputActions _inputActions;
        private bool _isJumping;
        private bool _isDashing;

        private void OnEnable()
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Gameplay.Enable();
            _inputActions.Gameplay.Jump.started += JumpStarted;
            _inputActions.Gameplay.Jump.canceled += JumpCancelled;
            _inputActions.Gameplay.Dash.started += DashStarted;
            _inputActions.Gameplay.Dash.canceled += DashCancelled;
        }

        private void JumpStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            _isJumping = true;
        }

        private void JumpCancelled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            _isJumping = false;
        }

        private void DashStarted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            _isDashing = true;
        }
        private void DashCancelled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            _isDashing = false;
        }


        private void OnDisable()
        {
            _inputActions.Gameplay.Disable();
            _inputActions.Gameplay.Jump.started -= JumpStarted;
            _inputActions.Gameplay.Jump.canceled -= JumpCancelled;
            _inputActions.Gameplay.Dash.started -= DashStarted;
            _inputActions.Gameplay.Dash.canceled -= DashCancelled;
            _inputActions = null;
        }


        public override bool RetrieveJumpInput()
        {
            return _isJumping;
        }

        public override float RetrieveMoveInput()
        {
            return _inputActions.Gameplay.Move.ReadValue<Vector2>().x;
        }

        public override bool RetrieveDashInput()
        {
            return _isDashing;
        }
    }
}
