﻿using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
    [System.Serializable]
    public class MovingPlatformsClass : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if support for moving platforms should be enabled.")]
        [FormerlySerializedAs("m_Enabled")]
        private bool enabled = true;
        [SerializeField]
        [Tooltip("If enabled, the player will move with platforms.")]
        private bool movePosition = true;
        [SerializeField]
        [Tooltip("If enabled, the player will rotate with platforms.")]
        private bool moveRotation = true;
        [SerializeField]
        [Tooltip("Sets the max angle of the platforms the player can stand on.")]
        private float maxAngle = 45f;

        /// <summary> Determines if support for moving platforms should be enabled. </summary>
        public bool Enabled { get { return enabled; } set { enabled = value; } }
        /// <summary> If enabled, the player will move with platforms. </summary>
        public bool MovePosition { get { return movePosition; } set { movePosition = value; } }
        /// <summary> If enabled, the player will rotate with platforms. </summary>
        public bool MoveRotation { get { return moveRotation; } set { moveRotation = value; } }
        /// <summary> Sets the max angle of the platforms the player can stand on. </summary>
        public float MaxAngle { get { return maxAngle; } set { maxAngle = value; } }

        private bool DidPlatformMove { get { return currentPlatform != null && currentPlatformLastPosition != currentPlatform.position; } }

        private float minNormalY;
        private const float CHECK_DISTANCE = 0.2f;

        // The current platform the player should be moving with.
        private Transform currentPlatform = null;
        private Transform recordedPlatform = null;

        private Vector3 currentPlatformLastPosition = Vector3.zero;
        private Vector3 currentPlatformLocalPoint = Vector3.zero;
        private Vector3 currentPlatformGlobalPoint = Vector3.zero;

        private Quaternion currentPlatformLocalRotation = Quaternion.identity;
        private Quaternion currentPlatformGlobalRotation = Quaternion.identity;

        // The current hit directly underneath the player.
        private RaycastHit groundHit;

        protected override void OnInitialize()
        {
            Vector3 vector = Quaternion.Euler(maxAngle, 0, 0) * Vector3.up;
            minNormalY = vector.y;
        }

        public override void OnUpdate(float deltaTime)
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
            UpdatePlatform(deltaTime);
            PostUpdatePlatform(previousPlatform);
        }

        protected virtual void UpdatePlatform(float deltaTime)
        {
            if (currentPlatform == null || recordedPlatform == null)
            {
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
                    // If the move distance is really small the player won't move. If it's really small, just 
                    // add it to the position as the small amount won't be noticable. 
                    // Otherwise just move normally.
                    if (moveDistance.magnitude < 0.001f)
                    {
                        PlayerTransform.position += moveDistance;
                    }
                    else
                    {
                        CharacterController.Move(moveDistance);
                    }
                }
            }

            if (moveRotation)
            {
                Quaternion newGlobalPlatformRotation = usePlatform.rotation * currentPlatformLocalRotation;
                Quaternion rotationDiff = newGlobalPlatformRotation * Quaternion.Inverse(currentPlatformGlobalRotation);
                rotationDiff = Quaternion.FromToRotation(rotationDiff * PlayerTransform.up, PlayerTransform.up) * rotationDiff;
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

            currentPlatformGlobalPoint = PlayerTransform.position;
            currentPlatformLastPosition = currentPlatform.position;
            currentPlatformLocalPoint = currentPlatform.InverseTransformPoint(PlayerTransform.position);

            currentPlatformGlobalRotation = PlayerTransform.rotation;
            currentPlatformLocalRotation = Quaternion.Inverse(currentPlatform.rotation) * PlayerTransform.rotation;
        }

        private void CheckUnderneath()
        {
            if (Physics.Raycast(PlayerTransform.position, new Vector3(0, -1, 0), out groundHit, CHECK_DISTANCE, PlayerController.Movement.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                CheckPlatformCollision(new Vector3(0f, -CHECK_DISTANCE, 0f), groundHit.normal, groundHit.transform);
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
