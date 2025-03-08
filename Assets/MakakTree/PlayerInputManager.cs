using UnityEngine;


namespace DOC
{
    public class PlayerInputManager : MonoBehaviour
    {
        PlayerControls playerControls;

        
        public static PlayerInputManager instance{ get; private set; }
        public PlayerManager player;
        [Header("Movement Input")]
        [SerializeField] Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;

        [Header("Camera Input")]
        [SerializeField] Vector2 cameraInput;
        public float cameraVerticalInput;
        public float cameraHorizontalInput;

        public void Awake()
        {
            if (instance == null) { instance = this; } else { Destroy(gameObject); }
        }
        private void OnEnable()
        {
            if (playerControls == null) { 
                playerControls = new PlayerControls();
                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerMovement.Camera.performed += j => cameraInput = j.ReadValue<Vector2>();
            }


            playerControls.Enable();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (enabled) {
                if (focus) { 
                    playerControls.Enable();
                } else {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandleMovementInput();
            HandleCameraInput();
        }
        private void HandleMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;
            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
            if (moveAmount <= 0.5 && moveAmount > 0) 
            { 
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5 && moveAmount <= 1) 
            {
                moveAmount = 1f;
            }

            // we pass 0 on the horizontal because were not locked on
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount);
        }

        private void HandleCameraInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }
    }
}