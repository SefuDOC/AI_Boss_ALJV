using UnityEngine;

namespace DOC
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        protected override void Awake()
        {
            base.Awake();
            //DO MORE STUFF FOR PLAYER
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        }

        protected override void Start()
        {
            PlayerCamera.instance.playerManager = this;
            PlayerInputManager.instance.player = this;
        }
        protected override void Update()
        {
            base.Update();
            //HANDLE ALL CHARACTER MOVEMENT
            playerLocomotionManager.HandleAllMovement();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            PlayerCamera.instance.HandleAllCameraActions();
        }
    }
}