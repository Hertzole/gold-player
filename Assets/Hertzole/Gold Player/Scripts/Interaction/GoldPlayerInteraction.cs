using Hertzole.GoldPlayer.Core;
using UnityEngine;

namespace Hertzole.GoldPlayer.Interaction
{
    [AddComponentMenu("Gold Player/Interaction/Player Interaction")]
    [DisallowMultipleComponent]
    public class GoldPlayerInteraction : PlayerBehaviour
    {
        [SerializeField]
        [Tooltip("Sets how far the interaction reach is.")]
        private float m_InteractionRange = 2f;
        [SerializeField]
        [Tooltip("Sets the layers that the player can interact with.")]
        private LayerMask m_InteractionLayer = 0;
        [SerializeField]
        [Tooltip("Determines if colliders marked as triggers should be detected.")]
        private bool m_IgnoreTriggers = true;

#if UNITY_EDITOR
        [Header("UI")]
#endif
        [SerializeField]
        [Tooltip("A default message for UI elements to show when the player can interact.")]
        private string m_InteractMessage = "Press E to interact";

#if UNITY_EDITOR
        [Header("Input")]
#endif
        [SerializeField]
        [Tooltip("The input name for interaction to use.")]
        private string m_InteractInput = "Interact";

        // Flag to determine if we have checked for a interactable.
        private bool m_HaveCheckedInteractable = false;

        // How it should behave with triggers.
        private QueryTriggerInteraction m_TriggerInteraction = QueryTriggerInteraction.Ignore;

        // The camera head.
        private Transform m_CameraHead;
        // The current hit transform.
        private Transform m_CurrentHit;

        // The current hit interactable.
        public GoldPlayerInteractable CurrentHitInteractable { get; private set; }

        // The raycast hit.
        private RaycastHit m_InteractableHit;

        /// <summary> True if the player can currently interact. </summary>
        public bool CanInteract { get; private set; }

        /// <summary> Sets how far the interaction reach is. </summary>
        public float InteractionRange { get { return m_InteractionRange; } set { m_InteractionRange = value; } }
        /// <summary> Sets the layers that the player can interact with. </summary>
        public LayerMask InteractionLayer { get { return m_InteractionLayer; } set { m_InteractionLayer = value; } }
        /// <summary> Determines if colliders marked as triggers should be detected. </summary>
        public bool IgnoreTriggers { get { return m_IgnoreTriggers; } set { m_IgnoreTriggers = value; SetTriggerInteraction(); } }
        /// <summary> A default message for UI elements to show when the player can interact. </summary>
        public string InteractMessage { get { return m_InteractMessage; } set { m_InteractMessage = value; } }
        /// <summary> The input name for interaction to use. </summary>
        public string InteractInput { get { return m_InteractInput; } set { m_InteractInput = value; } }

        protected override void OnAwake()
        {
            // Apply the trigger interaction.
            SetTriggerInteraction();
            // Fetch the camera head for easy access.
            m_CameraHead = PlayerController.Camera.CameraHead;
        }

        /// <summary>
        /// Sets how it should behave with triggers.
        /// </summary>
        private void SetTriggerInteraction()
        {
            m_TriggerInteraction = m_IgnoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide;
        }

        // Update is called once per frame
        private void Update()
        {
            // Do the raycast.
            if (Physics.Raycast(m_CameraHead.position, m_CameraHead.forward, out m_InteractableHit, m_InteractionRange, m_InteractionLayer, m_TriggerInteraction))
            {
                // If there's no hit transform, stop here.
                if (m_InteractableHit.transform == null)
                    return;

                // If there's no current hit or the hits doesn't match, update it and
                // the player need to check for a interactable again.
                if (m_CurrentHit == null || m_CurrentHit != m_InteractableHit.transform)
                {
                    m_CurrentHit = m_InteractableHit.transform;
                    m_HaveCheckedInteractable = false;
                }

                // If the player hasn't checked for an interactable, do so ONCE.
                // We don't want to call GetComponent every frame, you know!
                if (!m_HaveCheckedInteractable)
                {
                    CurrentHitInteractable = m_InteractableHit.transform.GetComponent<GoldPlayerInteractable>();
                    m_HaveCheckedInteractable = true;
                }

                // Set Can Interact depending on if the player has a interactable object
                // and it can be interacted with.
                CanInteract = CurrentHitInteractable != null && CurrentHitInteractable.CanInteract;

                // If the player presses the interact key and it can react, call interact.
                if (GetButtonDown(m_InteractInput, KeyCode.E) && CanInteract)
                {
                    CurrentHitInteractable.Interact();
                }
            }
            else
            {
                // There's nothing to interact with.
                CanInteract = false;
                CurrentHitInteractable = null;
                m_CurrentHit = null;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // If we change "Ignore Triggers" at runtime and in the editor,
            // make sure to update it.
            if (Application.isPlaying)
                SetTriggerInteraction();
        }
#endif
    }
}
