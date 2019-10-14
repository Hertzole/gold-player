#if HERTZLIB_UPDATE_MANAGER
using Hertzole.HertzLib;
#endif
using Hertzole.GoldPlayer.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer.Interaction
{
    [AddComponentMenu("Gold Player/Interaction/Player Interaction")]
    [DisallowMultipleComponent]
#if HERTZLIB_UPDATE_MANAGER
    public class GoldPlayerInteraction : PlayerBehaviour, IUpdate
#else
    public class GoldPlayerInteraction : PlayerBehaviour
#endif
    {
        [SerializeField]
        [Tooltip("The player camera head.")]
        [FormerlySerializedAs("m_CameraHead")]
        private Transform cameraHead;

#if UNITY_EDITOR
        [Space]
#endif

        [SerializeField]
        [Tooltip("Sets how far the interaction reach is.")]
        [FormerlySerializedAs("m_InteractionRange")]
        private float interactionRange = 2f;
        [SerializeField]
        [Tooltip("Sets the layers that the player can interact with.")]
        [FormerlySerializedAs("m_InteractionLayer")]
        private LayerMask interactionLayer = 1;
        [SerializeField]
        [Tooltip("Determines if colliders marked as triggers should be detected.")]
        [FormerlySerializedAs("m_IgnoreTriggers")]
        private bool ignoreTriggers = true;

#if UNITY_EDITOR
        [Header("UI")]
#endif
        [SerializeField]
        [Tooltip("A default message for UI elements to show when the player can interact.")]
        [FormerlySerializedAs("m_InteractMessage")]
        private string interactMessage = "Press E to interact";

#if UNITY_EDITOR
        [Header("Input")]
#endif
        [SerializeField]
        [Tooltip("The input name for interaction to use.")]
        [FormerlySerializedAs("m_InteractInput")]
        private string interactInput = "Interact";

        // Flag to determine if we have checked for a interactable.
        private bool haveCheckedInteractable = false;

        // How it should behave with triggers.
        private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        // The current hit collider.
        private Collider currentHit;

        // The raycast hit.
        private RaycastHit interactableHit;

        /// <summary> True if the player can currently interact. </summary>
        public bool CanInteract { get; private set; }

        /// <summary> The player camera head. </summary>
        public Transform CameraHead { get { return cameraHead; } set { cameraHead = value; } }
        /// <summary> Sets how far the interaction reach is. </summary>
        public float InteractionRange { get { return interactionRange; } set { interactionRange = value; } }
        /// <summary> Sets the layers that the player can interact with. </summary>
        public LayerMask InteractionLayer { get { return interactionLayer; } set { interactionLayer = value; } }
        /// <summary> Determines if colliders marked as triggers should be detected. </summary>
        public bool IgnoreTriggers { get { return ignoreTriggers; } set { ignoreTriggers = value; SetTriggerInteraction(); } }
        /// <summary> A default message for UI elements to show when the player can interact. </summary>
        public string InteractMessage { get { return interactMessage; } set { interactMessage = value; } }
        /// <summary> The input name for interaction to use. </summary>
        public string InteractInput { get { return interactInput; } set { interactInput = value; } }
        /// <summary> The current hit interactable. </summary>
        public GoldPlayerInteractable CurrentHitInteractable { get; private set; }

        protected virtual void Awake()
        {
            // Apply the trigger interaction.
            SetTriggerInteraction();
        }

        protected virtual void OnEnable()
        {
#if HERTZLIB_UPDATE_MANAGER
            UpdateManager.AddUpdate(this);
#endif
        }

        protected virtual void OnDisable()
        {
#if HERTZLIB_UPDATE_MANAGER
            UpdateManager.RemoveUpdate(this);
#endif
        }

        /// <summary>
        /// Sets how it should behave with triggers.
        /// </summary>
        private void SetTriggerInteraction()
        {
            triggerInteraction = ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide;
        }

        // Update is called once per frame
#if HERTZLIB_UPDATE_MANAGER
        public virtual void OnUpdate()
#else
        public virtual void Update()
#endif
        {
            // Do the raycast.
            if (Physics.Raycast(
                cameraHead.position,
                cameraHead.forward,
                out interactableHit,
                interactionRange,
                interactionLayer,
                triggerInteraction))
            {
                // If there's no hit transform, stop here.
                if (interactableHit.collider == null)
                {
                    return;
                }

                // If there's no current hit or the hits doesn't match, update it and
                // the player need to check for a interactable again.
                if (currentHit == null || currentHit != interactableHit.collider)
                {
                    currentHit = interactableHit.collider;
                    haveCheckedInteractable = false;
                }

                // If the player hasn't checked for an interactable, do so ONCE.
                // We don't want to call GetComponent every frame, you know!
                if (!haveCheckedInteractable)
                {
                    // Prefer interactables on the collider itself, but if the collider doesn't
                    // have one, then look on the rigidbody.
                    CurrentHitInteractable =
                        interactableHit.collider.GetComponent<GoldPlayerInteractable>() ??
                        interactableHit.rigidbody.GetComponent<GoldPlayerInteractable>();
                    haveCheckedInteractable = true;
                }

                // Set Can Interact depending on if the player has a interactable object
                // and it can be interacted with.
                CanInteract = CurrentHitInteractable != null && CurrentHitInteractable.CanInteract;

                // If the player presses the interact key and it can react, call interact.
                if (GetButtonDown(interactInput) && CanInteract)
                {
                    CurrentHitInteractable.Interact();
                }
            }
            else
            {
                // There's nothing to interact with.
                CanInteract = false;
                CurrentHitInteractable = null;
                currentHit = null;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // If we change "Ignore Triggers" at runtime and in the editor,
            // make sure to update it.
            if (Application.isPlaying)
            {
                SetTriggerInteraction();
            }
        }
#endif
    }
}
