using UnityEngine;


namespace DOC
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager playerManager;
        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;


        [SerializeField] float walkingSpeed = 2;
        [SerializeField] float runningSpeed = 5;
        [SerializeField] float rotationSpeed = 15;
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        protected override void Awake()
        {
            base.Awake();  
            playerManager = GetComponent<PlayerManager>();
        }
        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
            //GROUNDED MOVEMENT
            //AERIAL MOVEMENT
        }


        private void GetVerticalAndHorizontalInputs()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
            //clamp the movements (for animations)
        }
        private void HandleGroundedMovement()
        { 
            GetVerticalAndHorizontalInputs();
            moveDirection = (new Vector3(PlayerCamera.instance.transform.forward.x, 0, PlayerCamera.instance.transform.forward.z).normalized) * verticalMovement;
            moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();

            if(PlayerInputManager.instance.moveAmount > 0.5f)
            {
                playerManager.characterController.Move(moveDirection* runningSpeed * Time.deltaTime);
                //Runing
            }
            else if(PlayerInputManager.instance.moveAmount <= 0.5f)
            {
                playerManager.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                //Walking
            }
        }

        private void HandleRotation()
        {
            targetRotationDirection = (new Vector3(PlayerCamera.instance.transform.forward.x, 0, PlayerCamera.instance.transform.forward.z).normalized) * verticalMovement;
            targetRotationDirection += PlayerCamera.instance.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();

            if(targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }
            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }
}