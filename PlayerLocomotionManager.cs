using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Cawblin
{
    [RequireComponent (typeof(PlayerManager))]
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        private PlayerManager player;

        [Header("Movement Settings")]
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float rotateSpeed = 15f;

        float moveVelocity;
        private Vector3 movementDirection;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            if (IsOwner)
            {
                player.CharacterNetworkManager.NetworkRunningAnimationSpeed.Value = moveVelocity;
            }
            else
            {
                moveVelocity = player.CharacterNetworkManager.NetworkRunningAnimationSpeed.Value;
                player.PlayerAnimatorController.UpdateManagerMovementParameters(moveVelocity);
            }

            player.Animator.SetFloat("MoveSpeed", movementSpeed, 0.1f, Time.deltaTime);
        }

        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
        }

        private void HandleRotation()
        {
            if (!player.CanRotate || PlayerInputManager.Instance.MovementInputVector == Vector2.zero) return;

            Vector3 fixedInputVector = GetFixedInputVector();

            Quaternion lookRotation = Quaternion.LookRotation(fixedInputVector, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotateSpeed);
        }

        private void HandleGroundedMovement()
        {
            if (!player.CanMove) return;

            if (PlayerInputManager.Instance.MovementInputVector != Vector2.zero)
            {
                Vector3 fixedInputVector = GetFixedInputVector();

                movementDirection = fixedInputVector * movementSpeed;
                player.CharacterController.Move(movementDirection * Time.deltaTime);

                moveVelocity = movementSpeed / 10;
            }
            else
            {
                moveVelocity = 0f;
            }

            player.PlayerAnimatorController.UpdateManagerMovementParameters(moveVelocity);
        }

        private Vector3 GetFixedInputVector()
        {
            Vector3 fixedInputVector = new Vector3(PlayerInputManager.Instance.MovementInputVector.x, 0, PlayerInputManager.Instance.MovementInputVector.y);
            fixedInputVector.Normalize();

            return fixedInputVector;
        }

        public void TryDodge()
        {
            if (player.IsPerformingAction) return;

            Vector3 fixedInputVector = GetFixedInputVector();

            if (fixedInputVector != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(fixedInputVector, Vector3.up);
                transform.rotation = lookRotation;
                player.CharacterNetworkManager.NetworkRotation.Value = lookRotation;
            }

            player.PlayerAnimatorController.PlayTargetActionAnimation("Roll", true, true);
        }
    }
}
