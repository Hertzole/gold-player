using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer.Core
{
    [System.Serializable]
    public class MovingPlatformsClass : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if support for moving platforms should be enabled.")]
        [FormerlySerializedAs("m_Enabled")]
        private bool enabled = true;
        [SerializeField]
        [Tooltip("The tags the moving platforms are using.")]
        [FormerlySerializedAs("m_PlatformTags")]
        private string[] platformTags;

        /// <summary> Determines if support for moving platforms should be enabled. </summary>
        public bool Enabled { get { return enabled; } set { enabled = value; } }
        /// <summary> The tags the moving platforms are using.. </summary>
        public string[] PlatformTags { get { return platformTags; } set { platformTags = value; } }

        // The parent the player was using from the start.
        private Transform originalParent = null;
        // The current platform the player should be moving with.
        private Transform currentPlatform = null;

        // All the colliders currently under the player.
        private Collider[] groundColliders = new Collider[0];

        // The current hit directly underneath the player.
        private RaycastHit groundHit;

        protected override void OnInitialize()
        {
            // Set the original parent.
            originalParent = PlayerTransform.parent;
        }

        public override void OnUpdate(float deltaTime)
        {
            // If it isn't enabled, just stop here.
            if (!enabled)
                return;

            // Call the platform checking.
            CheckPlatform();
            // Call the parent switching.
            DoParentSwitching();
        }

        /// <summary>
        /// Checks for platforms and assigns the current platform variable.
        /// </summary>
        protected virtual void CheckPlatform()
        {
            // Update the ground hit.
            CheckRaycast();
            // Update the ground colliders.
            CheckBox();

            // If the ground colliders are more than 1, try to determine with the ground hit.
            // Else if the ground colliders are just one, only use that.
            // Else if both ground colliders are empty and the ground hit is empty, set the current platform to null.
            if (groundColliders.Length > 1)
            {
                // If the ground hit isn't null, check if the hit contains a platform tag.
                if (groundHit.transform != null)
                {
                    // Go through every platform tag and see if the ground hit has a tag.
                    for (int i = 0; i < platformTags.Length; i++)
                    {
                        // If the ground hit has a platform tag, assign the current platform.
                        if (groundHit.transform.CompareTag(platformTags[i]))
                        {
                            // Set the current platform to the ground hit.
                            currentPlatform = groundHit.transform;
                            // Break out of the for loop.
                            break;
                        }

                        // There was no transform with the right tag, set the current platform to null.
                        currentPlatform = null;
                    }
                }
                else
                {
                    // Go through every ground collider.
                    for (int i = 0; i < groundColliders.Length; i++)
                    {
                        // Go through every platform tag to see if the ground collider has a platform tag.
                        for (int j = 0; j < platformTags.Length; j++)
                        {
                            // Check if the platform has a platform tag.
                            if (groundColliders[i].CompareTag(platformTags[j]))
                            {
                                // Assign the current platform.
                                currentPlatform = groundColliders[i].transform;
                                // Break out of the for loop.
                                break;
                            }

                            // There was no platform matching. Set the current platform to null.
                            currentPlatform = null;
                        }
                    }
                }
            }
            else if (groundColliders.Length == 1)
            {
                // Go through and check if the one ground collider has a platform tag.
                for (int i = 0; i < platformTags.Length; i++)
                {
                    // Compare the tag.
                    if (groundColliders[0].CompareTag(platformTags[i]))
                    {
                        // Set the current platform.
                        currentPlatform = groundColliders[0].transform;
                        // Break out of the for loop.
                        break;
                    }

                    // There was no matching tags. Set the current platform to null.
                    currentPlatform = null;
                }
            }
            else if (groundHit.transform == null)
            {
                // If there are no ground colliders and no ground hit, set the current platform to null.
                currentPlatform = null;
            }
        }

        /// <summary>
        /// Updates the ground hit raycast hit.
        /// </summary>
        protected virtual void CheckRaycast()
        {
            Physics.Raycast(PlayerTransform.position, -PlayerTransform.up, out groundHit, 0.2f, PlayerController.Movement.GroundLayer, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// Updates the ground colliders.
        /// </summary>
        protected virtual void CheckBox()
        {
            groundColliders = Physics.OverlapBox(PlayerTransform.position, new Vector3(CharacterController.radius, 0.2f, CharacterController.radius), Quaternion.identity, PlayerController.Movement.GroundLayer, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// Handles the parent switching.
        /// </summary>
        protected virtual void DoParentSwitching()
        {
            // If the current platform isn't null and the player parent isn't the current platform, set the player parent to the platform.
            // Else if the current platform is null the player parent isn't the original parent, set the player parent to the original parent.
            if (currentPlatform != null && PlayerTransform.parent != currentPlatform)
                PlayerTransform.SetParent(currentPlatform, true);
            else if (currentPlatform == null && PlayerTransform.parent != originalParent)
                PlayerTransform.SetParent(originalParent, true);
        }
    }
}
