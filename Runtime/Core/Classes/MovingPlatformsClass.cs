using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEngine.TestTools;
#endif

namespace Hertzole.GoldPlayer
{
    [System.Serializable]
    public class MovingPlatformsClass : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if support for moving platforms should be enabled.")]
        [FormerlySerializedAs("m_Enabled")]
        internal bool enabled = true;
        [SerializeField]
        [Tooltip("If enabled, the player will move with platforms.")]
        internal bool movePosition = true;
        [SerializeField]
        [Tooltip("If enabled, the player will rotate with platforms.")]
        internal bool moveRotation = true;
        [SerializeField]
        [Tooltip("Sets the max angle of the platforms the player can stand on.")]
        internal float maxAngle = 45f;

        private int? previousHitColliderId = null;
        
        /// <summary> Determines if support for moving platforms should be enabled. </summary>
        public bool Enabled { get { return enabled; } set { enabled = value; } }
        /// <summary> If enabled, the player will move with platforms. </summary>
        public bool MovePosition { get { return movePosition; } set { movePosition = value; } }
        /// <summary> If enabled, the player will rotate with platforms. </summary>
        public bool MoveRotation { get { return moveRotation; } set { moveRotation = value; } }
        /// <summary> Sets the max angle of the platforms the player can stand on. </summary>
        public float MaxAngle { get { return maxAngle; } set { maxAngle = value; } }

        public bool DidPlatformMove { get { return currentPlatform != null && currentPlatformLastPosition != currentPlatform.position; } }

        public bool IsMoving { get; private set; }

        private float minNormalY;
        private const float CHECK_DISTANCE = 0.2f;

        // The current platform the player should be moving with.
        private Transform currentPlatform;
        private Transform recordedPlatform;
        private Transform hitPlatform;

        private Vector3 currentPlatformLastPosition = Vector3.zero;
        private Vector3 currentPlatformLocalPoint = Vector3.zero;
        private Vector3 currentPlatformGlobalPoint = Vector3.zero;

        internal Quaternion currentPlatformLocalRotation = Quaternion.identity;
        internal Quaternion currentPlatformGlobalRotation = Quaternion.identity;

        // The current hit directly underneath the player.
        private RaycastHit groundHit;

        protected override void OnInitialize()
        {
            Vector3 vector = Quaternion.Euler(maxAngle, 0, 0) * Vector3.up;
            minNormalY = vector.y;
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            // If it isn't enabled, just stop here.
            if (!enabled)
            {
                return;
            }

            if (currentPlatform == null)
            {
                CheckUnderneath();
            }

            Transform previousPlatform = currentPlatform;
            UpdatePlatform();
            PostUpdatePlatform(previousPlatform);
        }

        protected virtual void UpdatePlatform()
        {
            if (currentPlatform == null || recordedPlatform == null)
            {
                IsMoving = false;
                return;
            }

            Transform usePlatform = currentPlatform;

            if (!currentPlatform != recordedPlatform)
            {
                usePlatform = recordedPlatform;
            }

            if (movePosition)
            {
                Vector3 newGlobalPlatformPoint = usePlatform.TransformPoint(currentPlatformLocalPoint);
                Vector3 moveDistance = (newGlobalPlatformPoint - currentPlatformGlobalPoint);
                if (DidPlatformMove)
                {
                    // If the move distance is really small the player won't move. In that case,
                    // just add it to the position as the small amount won't be noticeable. 
                    // Otherwise just move normally.
                    if (moveDistance.magnitude < 0.001f)
                    {
                        PlayerTransform.position += moveDistance;
                    }
                    else
                    {
                        CharacterController.Move(moveDistance);
                    }

                    IsMoving = true;
                }
                else
                {
                    IsMoving = false;
                }
            }
            else
            {
                IsMoving = false;
            }

            if (moveRotation)
            {
                Vector3 up = PlayerTransform.up;
                Quaternion newGlobalPlatformRotation = usePlatform.rotation * currentPlatformLocalRotation;
                Quaternion rotationDiff = newGlobalPlatformRotation * Quaternion.Inverse(currentPlatformGlobalRotation);
                rotationDiff = Quaternion.FromToRotation(rotationDiff * up, up) * rotationDiff;
                PlayerTransform.rotation = rotationDiff * PlayerTransform.rotation;
            }

            currentPlatform = null;
        }

        private void PostUpdatePlatform(Transform previousPlatform)
        {
            if (currentPlatform == null && previousPlatform != null)
            {
                CheckUnderneath();
            }

            recordedPlatform = currentPlatform;
            if (currentPlatform == null)
            {
                return;
            }

            Vector3 position = PlayerTransform.position;
            currentPlatformGlobalPoint = position;
            currentPlatformLastPosition = currentPlatform.position;
            currentPlatformLocalPoint = currentPlatform.InverseTransformPoint(position);

            Quaternion rotation = PlayerTransform.rotation;
            currentPlatformGlobalRotation = rotation;
            currentPlatformLocalRotation = Quaternion.Inverse(currentPlatform.rotation) * rotation;
        }

        private void CheckUnderneath()
        {
            if (Physics.Raycast(PlayerTransform.position, new Vector3(0, -1, 0), out groundHit, CHECK_DISTANCE, PlayerController.Movement.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                if (previousHitColliderId != groundHit.colliderInstanceID)
                {
                    hitPlatform = groundHit.collider.transform;
                    previousHitColliderId = groundHit.colliderInstanceID;
                }
                
                CheckPlatformCollision(new Vector3(0f, -CHECK_DISTANCE, 0f), groundHit.normal, hitPlatform);
            }
            else
            {
                previousHitColliderId = null;
                hitPlatform = null;
            }
        }

        private void CheckPlatformCollision(Vector3 hitDirection, Vector3 hitNormal, Transform hitTransform)
        {
            // Did character move down and hit an up-facing normal?
            if (hitDirection.y < 0.0f && hitNormal.y >= minNormalY)
            {
                currentPlatform = hitTransform;
            }
        }

#if UNITY_EDITOR
        [ExcludeFromCoverage]
        public override void OnValidate()
        {
            if (Application.isPlaying)
            {
                Vector3 vector = Quaternion.Euler(maxAngle, 0, 0) * Vector3.up;
                minNormalY = vector.y;
            }
        }
#endif
    }
}
