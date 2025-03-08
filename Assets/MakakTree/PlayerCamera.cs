using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace DOC
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance { get; private set; }  

        public Camera cameraObject;
        public PlayerManager playerManager;

        [SerializeField] Transform cameraPivotTransform;

        //CHANGE THESE TO TWEAK CAMERA PERFORMANCE
        [Header("Camera Settings")]
        private float cameraSmoothSpeed = 1; // the bigger the number , the longer it takes for the camera to reach its position during movement
        [SerializeField] private float leftAndRightRotationSpeed = 220;
        [SerializeField] private float upAndDownRotationSpeed = 220;
        [SerializeField] float minimumPivot = -30; // lowest point to look down
        [SerializeField] float maximumPivot = 60; // highest point to look up
        [SerializeField] float cameraCollisionRadius = 0.2f; // larger = collide with stuff further away
        [SerializeField] LayerMask collideWithLayers;

        //JUST DISPLAYS CAMERA VALUES
        [Header("Camera Values")]
        [SerializeField] float leftAndRightLookAngle;
        [SerializeField] float upAndDownLookAngle;
        private float cameraZPosition; // values used for camera collision
        private float targetCameraZPosition; // values used for camera collision
        private Vector3 cameraObjectPosition; // used for camera collisions (moves the camera object to this position upon colliding)
        private Vector3 cameraVelocity;

        private void Awake()
        {
            if (instance == null) { instance = this; } else { Destroy(gameObject); }
        }
        private void Start()
        {
            //nu strica
            DontDestroyOnLoad(gameObject);
            cameraZPosition = cameraObject.transform.localPosition.z;
        }
   
        public void HandleAllCameraActions()
        {
            //FOLLOW the PLAYER
            // ROTATE AROUND THE PLAYER
            // COLLIDE WITH OBJECTS and NOT GO THROUGH WALLS

            if (playerManager != null) {

                HandleFollowTarget();
                HandleRotations();
                HandleCollisions();
            }
        }
    
        private void HandleFollowTarget()
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, playerManager.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }
        private void HandleRotations()
        {
            // IF LOCKED ONE FORCE ROTATION TOWARDS TARGET
            // ELSE ROTATE NORMALLY


            //Normal rotations
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
            upAndDownLookAngle -= PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed * Time.deltaTime;
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation  = targetRotation;

            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleCollisions()
        {
            targetCameraZPosition = cameraZPosition;
            RaycastHit hit;
            // direction for collision check
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            // Check if an object is in front of the camera
            if(Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
            {
                // if there is , we get distance from it
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                // we then equate our target to position to the following
                targetCameraZPosition = -( distanceFromHitObject - cameraCollisionRadius );
            }

            //if our target position is less than our collision radius, we subtract our collision radius (making it snap back)
            if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius) {
                targetCameraZPosition = -cameraCollisionRadius;
            }
            //we then apply our final positin using a lerp over a time of 0.2f
            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z , targetCameraZPosition, 0.2f);
            cameraObject.transform.localPosition = cameraObjectPosition;
        }
    }      
}