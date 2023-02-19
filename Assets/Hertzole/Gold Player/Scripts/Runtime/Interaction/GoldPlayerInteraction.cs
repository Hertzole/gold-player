#if GOLD_PLAYER_DISABLE_INTERACTION
#define OBSOLETE
#endif

#if OBSOLETE && !UNITY_EDITOR
#define STRIP
#endif

#if !STRIP
using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
#if !OBSOLETE
    [AddComponentMenu("Gold Player/Gold Player Interaction", 20)]
#else
    [System.Obsolete("Gold Player Interaction has been disabled. GoldPlayerInteraction will be removed on build.")]
    [AddComponentMenu("")]
#endif
    [DisallowMultipleComponent]
    public class GoldPlayerInteraction : PlayerBehaviour
    {
        [SerializeField]
        [Tooltip("The player camera head.")]
        [FormerlySerializedAs("m_CameraHead")]
        internal Transform cameraHead;

#if UNITY_EDITOR
        [Space]
#endif

        [SerializeField]
        [Tooltip("Sets how far the interaction reach is.")]
        [FormerlySerializedAs("m_InteractionRange")]
        internal float interactionRange = 2f;
        [SerializeField]
        [Tooltip("Sets the layers that the player can interact with.")]
        [FormerlySerializedAs("m_InteractionLayer")]
        internal LayerMask interactionLayer = 1;
        [SerializeField]
        [Tooltip("Determines if colliders marked as triggers should be detected.")]
        [FormerlySerializedAs("m_IgnoreTriggers")]
        internal bool ignoreTriggers = true;

#if UNITY_EDITOR
        [Header("UI")]
#endif
        [SerializeField]
        [Tooltip("A default message for UI elements to show when the player can interact.")]
        [FormerlySerializedAs("m_InteractMessage")]
        internal string interactMessage = "Press E to interact";

#if UNITY_EDITOR
        [Header("Input")]
#endif
        [SerializeField]
        [Tooltip("The input name for interaction to use.")]
        [FormerlySerializedAs("m_InteractInput")]
        internal string interactInput = "Interact";

        private int interactHash;
        private int? previousHitColliderId = null;

        // Flag to determine if we have checked for a interactable.
        internal bool hasCheckedInteractable = false;

        // How it should behave with triggers.
        private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        // The current hit collider.
        private Collider currentHit;

        // The raycast hit.
        private RaycastHit interactableHit;

#if UNITY_EDITOR || GOLD_PLAYER_DISABLE_OPTIMIZATIONS
        /// <summary> True if the player can currently interact. </summary>
        public bool CanInteract { get; private set; }
#else
        [System.NonSerialized]
        public bool CanInteract;
#endif

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
        public string InteractInput { get { return interactInput; } set { interactInput = value; interactHash = GoldPlayerController.InputNameToHash(interactInput); } }

#if UNITY_EDITOR || GOLD_PLAYER_DISABLE_OPTIMIZATIONS
        /// <summary> The current hit interactable. </summary>
        public IGoldPlayerInteractable CurrentHitInteractable { get; private set; }
#else
        [System.NonSerialized]
        public IGoldPlayerInteractable CurrentHitInteractable;
#endif

        protected virtual void Awake()
        {
#if OBSOLETE
            Debug.LogError(gameObject.name + " has GoldPlayerInteraction attached. It will be removed on build. Please remove this component if you don't intend to use it.", gameObject);
#endif

	        interactHash = GoldPlayerController.InputNameToHash(interactInput);

            // Apply the trigger interaction.
            SetTriggerInteraction();
        }

        /// <summary>
        /// Sets how it should behave with triggers.
        /// </summary>
        private void SetTriggerInteraction()
        {
            triggerInteraction = ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide;
        }

        // Update is called once per frame
        private void Update()
        {
            DoInteraction();
        }

        protected virtual void DoInteraction()
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
                HitInteraction(interactableHit);
            }
            else
            {
                // There's nothing to interact with.
                CanInteract = false;
                CurrentHitInteractable = null;
                currentHit = null;
                previousHitColliderId = null;
            }
        }

        internal void HitInteraction(RaycastHit hit)
        {
	        // If there's no current hit or the hits doesn't match, update it and
	        // the player need to check for a interactable again.
	        if (hit.colliderInstanceID != previousHitColliderId)
	        {
		        previousHitColliderId = hit.colliderInstanceID;
		        currentHit = hit.collider;
		        hasCheckedInteractable = false;

		        if (hit.collider == null)
		        {
			        return;
		        }
	        }

	        // If the player hasn't checked for an interactable, do so ONCE.
            // We don't want to call GetComponent every frame, you know!
            if (!hasCheckedInteractable)
            {
                // Prefer interactables on the collider itself, but if the collider doesn't
                // have one, then look on the rigidbody.
                CurrentHitInteractable = hit.collider.GetComponent<IGoldPlayerInteractable>();
                if (CurrentHitInteractable == null && hit.rigidbody != null)
                {
                    CurrentHitInteractable = hit.rigidbody.GetComponent<IGoldPlayerInteractable>();
                }

                hasCheckedInteractable = true;
            }

            // Set Can Interact depending on if the player has a interactable object
            // and it can be interacted with.
            CanInteract = CurrentHitInteractable != null && CurrentHitInteractable.CanInteract;

            // If the player presses the interact key and it can react, call interact.
            if (CanInteract && GetButtonDown(interactHash))
            {
                CurrentHitInteractable.Interact();
            }
        }

#if UNITY_EDITOR
        [UnityEngine.TestTools.ExcludeFromCoverage]
        private void OnValidate()
        {
            // If we change "Ignore Triggers" at runtime and in the editor,
            // make sure to update it.
            if (Application.isPlaying)
            {
                SetTriggerInteraction();
            }
        }

        internal void Reset()
        {
            // If the controller exists, default the camera head to the one provided on the controller.
            GoldPlayerController controller = GetComponent<GoldPlayerController>();
            if (controller != null && cameraHead == null)
            {
                cameraHead = controller.Camera.CameraHead;
            }
        }
#endif
    }
}
#endif
