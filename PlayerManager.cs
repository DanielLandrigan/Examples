using UnityEngine;

namespace Cawblin
{
    public class PlayerManager : CharacterManager
    {
        public PlayerNetworkManager PlayerNetworkManager { get; private set; }
        public PlayerLocomotionManager PlayerLocomotionManager { get; private set; }
        public PlayerAnimatorController PlayerAnimatorController { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            PlayerNetworkManager = GetComponent<PlayerNetworkManager>();
            PlayerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            PlayerAnimatorController = GetComponent<PlayerAnimatorController>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                PlayerCamera.Instance.Player = this;
                PlayerInputManager.Instance.Player = this;
                WorldSaveGameManager.Instance.Player = this;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!IsOwner) return;

            PlayerLocomotionManager.HandleAllMovement();
        }
        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (!IsOwner) return;

            PlayerCamera.Instance.HandleAllCameraActions();
        }

        public void SaveGameFile(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.CharacterName = PlayerNetworkManager.PlayerDisplayName.Value;
            currentCharacterData.XPosition = transform.position.x;
            currentCharacterData.YPosition = transform.position.y;
            currentCharacterData.ZPosition = transform.position.z;
        }

        public void LoadGameFile(ref CharacterSaveData currentCharacterData)
        {
            PlayerNetworkManager.PlayerDisplayName.Value = currentCharacterData.CharacterName;
            Vector3 worldPosition = new(currentCharacterData.XPosition, currentCharacterData.YPosition, currentCharacterData.ZPosition);

            transform.position = worldPosition;
        }
    }
}
