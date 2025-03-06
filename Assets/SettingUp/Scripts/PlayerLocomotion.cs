using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
namespace DOC
{
    public class PlayerLocomotion : MonoBehaviour
    {

        Transform cameraObject;
        InputHandler inputHandler;
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float rotationSpeed = 10;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            animatorHandler.Initialize();

        }

        public void Update()
        {
            float delta = Time.deltaTime;

            inputHandler.TickInput(delta);
            HandleMovement(delta);
            HandleRollingandSprinting(delta);
        }



        public void HandleMovement(float delta)
        {

            moveDirection = Vector3.Normalize(new Vector3(cameraObject.forward.x, 0, cameraObject.forward.z)) * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();

            moveDirection *= movementSpeed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
            rigidbody.linearVelocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);
            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        Vector3 targetPosition;
        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();

            targetDir.y = 0;

            if(targetDir == Vector3.zero)
                targetDir = myTransform.forward;

            float rs = rotationSpeed;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr , rs * delta);

            myTransform.rotation = targetRotation;
        }

        public void HandleRollingandSprinting(float delta)
        {
            if (animatorHandler.anim.GetBool("isInteracting"))
                return;

            if (inputHandler.rollFlag)
            {
                moveDirection = Vector3.Normalize(new Vector3(cameraObject.forward.x, 0, cameraObject.forward.z)) * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;
                moveDirection.Normalize();

                if(inputHandler.moveAmount > 0)
                {
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Backstep", true);
                }
            }
        }

    }
}