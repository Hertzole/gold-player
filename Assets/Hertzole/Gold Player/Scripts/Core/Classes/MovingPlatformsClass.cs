using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    [System.Serializable]
    public class MovingPlatformsClass : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if support for moving platforms should be enabled.")]
        private bool m_Enabled = true;
        [SerializeField]
        [Tooltip("The tags the moving platforms are using.")]
        private string[] m_PlatformTags;

        /// <summary> Determines if support for moving platforms should be enabled. </summary>
        public bool Enabled { get { return m_Enabled; } set { m_Enabled = value; } }
        /// <summary> The tags the moving platforms are using.. </summary>
        public string[] PlatformTags { get { return m_PlatformTags; } set { m_PlatformTags = value; } }

        // The parent the player was using from the start.
        private Transform m_OriginalParent = null;
        // The current platform the player should be moving with.
        private Transform m_CurrentPlatform = null;

        // All the colliders currently under the player.
        private Collider[] m_GroundColliders = new Collider[0];

        // The current hit directly underneat the player.
        private RaycastHit m_GroundHit;

        protected override void OnInit()
        {
            // Set the original parent.
            m_OriginalParent = PlayerTransform.parent;
        }

        public override void OnUpdate()
        {
            // If it isn't enabled, just stop here.
            if (!m_Enabled)
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
            if (m_GroundColliders.Length > 1)
            {
                // If the ground hit isn't null, check if the hit contains a platform tag.
                if (m_GroundHit.transform != null)
                {
                    // Go through every platform tag and see if the ground hit has a tag.
                    for (int i = 0; i < m_PlatformTags.Length; i++)
                    {
                        // If the ground hit isn't null and the ground hit has a platform tag, assign the current platform.
                        if (m_GroundHit.transform != null && m_GroundHit.transform.CompareTag(m_PlatformTags[i]))
                        {
                            // Set the current platform to the ground hit.
                            m_CurrentPlatform = m_GroundHit.transform;
                            // Break out of the for loop.
                            break;
                        }

                        // There was no transform with the right tag, set the current platform to null.
                        m_CurrentPlatform = null;
                    }
                }
                else
                {
                    // Go through every ground collider.
                    for (int i = 0; i < m_GroundColliders.Length; i++)
                    {
                        // Go through every platform tag to see if the ground collider has a platform tag.
                        for (int j = 0; j < m_PlatformTags.Length; j++)
                        {
                            // Check if the platform has a platform tag.
                            if (m_GroundColliders[i].CompareTag(m_PlatformTags[j]))
                            {
                                // Assign the current platform.
                                m_CurrentPlatform = m_GroundColliders[i].transform;
                                // Break out of the for loop.
                                break;
                            }

                            // There was no platform matching. Set the current platform to null.
                            m_CurrentPlatform = null;
                        }
                    }
                }
            }
            else if (m_GroundColliders.Length == 1)
            {
                // Go through and check if the one ground collider has a platform tag.
                for (int i = 0; i < m_PlatformTags.Length; i++)
                {
                    // Compare the tag.
                    if (m_GroundColliders[0].CompareTag(m_PlatformTags[i]))
                    {
                        // Set the current platform.
                        m_CurrentPlatform = m_GroundColliders[0].transform;
                        // Break out of the for loop.
                        break;
                    }

                    // There was no matching tags. Set the current platform to null.
                    m_CurrentPlatform = null;
                }
            }
            else if (m_GroundColliders.Length == 0 && m_GroundHit.transform == null)
            {
                // If there are no ground colliders and no ground hit, set the current platform to null.
                m_CurrentPlatform = null;
            }
        }

        /// <summary>
        /// Updates the ground hit raycast hit.
        /// </summary>
        protected virtual void CheckRaycast()
        {
            Physics.Raycast(PlayerTransform.position, -PlayerTransform.up, out m_GroundHit, 0.2f, PlayerController.Movement.GroundLayer, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// Updates the ground colliders.
        /// </summary>
        protected virtual void CheckBox()
        {
            m_GroundColliders = Physics.OverlapBox(PlayerTransform.position, new Vector3(CharacterController.radius, 0.2f, CharacterController.radius), Quaternion.identity, PlayerController.Movement.GroundLayer, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// Handles the parent switching.
        /// </summary>
        protected virtual void DoParentSwitching()
        {
            // If the current platform isn't null and the player parent isn't the current platform, set the player parent to the platform.
            // Else if the current platform is null the player parent isn't the original parent, set the player parent to the original parent.
            if (m_CurrentPlatform != null && PlayerTransform.parent != m_CurrentPlatform)
                PlayerTransform.SetParent(m_CurrentPlatform, true);
            else if (m_CurrentPlatform == null && PlayerTransform.parent != m_OriginalParent)
                PlayerTransform.SetParent(m_OriginalParent, true);
        }
    }
}
