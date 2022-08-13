#if GOLD_PLAYER_DISABLE_INTERACTION
#define OBSOLETE
#endif

#if OBSOLETE && !UNITY_EDITOR
#define STRIP
#endif

#if !STRIP
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
#if !OBSOLETE
    [AddComponentMenu("Gold Player/Gold Player Interactable", 20)]
#else
    [System.Obsolete("Gold Player Interaction has been disabled. GoldPlayerInteractable will be removed on build.")]
    [AddComponentMenu("")]
#endif
    [DisallowMultipleComponent]
    public class GoldPlayerInteractable : MonoBehaviour, IGoldPlayerInteractable
    {
        [System.Serializable]
        public class InteractionEvent : UnityEvent { }

        [SerializeField]
        [Tooltip("Determines if the object can be interacted with.")]
        [FormerlySerializedAs("m_CanInteract")]
        [FormerlySerializedAs("canInteract")]
        private bool isInteractable = true;
        [SerializeField]
        [Tooltip("Determines if the object should be hidden.\n(Used for UI to not show a interaction message)")]
        [FormerlySerializedAs("m_IsHidden")]
        private bool isHidden = false;

#if UNITY_EDITOR
        [Space]
#endif

        [SerializeField]
        [Tooltip("Determines if a custom interaction message should be shown.")]
        [FormerlySerializedAs("m_UseCustomMessage")]
        private bool useCustomMessage = false;
        [SerializeField]
        [Tooltip("A custom interaction message for UI elements.")]
        [FormerlySerializedAs("m_CustomMessage")]
        private string customMessage = "Press E to interact";

#if UNITY_EDITOR
        [Space]
#endif
        [SerializeField]
        [Tooltip("If true, you can only interact with this object a certain amount of times.")]
        private bool limitedInteractions = false;
        [SerializeField]
        [Tooltip("The amount of times you can interact with this object.")]
        private int maxInteractions = 1;

#if UNITY_EDITOR
        [Space]
#endif

        [SerializeField]
        [Tooltip("Called when the object is interacted with.")]
        [FormerlySerializedAs("m_OnInteract")]
        private InteractionEvent onInteract = new InteractionEvent();
        [SerializeField]
        [Tooltip("Called when the object has reached it's max interactions.")]
        private InteractionEvent onReachedMaxInteractions = new InteractionEvent();

        // The amount of times this object has been interacted with.
        private int interactions;

        /// <summary> Determines if the object can be interacted with. </summary>
        public bool IsInteractable { get { return isInteractable; } set { isInteractable = value; } }
        /// <summary> Determines if the object should be hidden. (Used for UI to not show a interaction message) </summary>
        public bool IsHidden { get { return isHidden; } set { isHidden = value; } }
        /// <summary> Determines if a custom interaction message should be shown. </summary>
        public bool UseCustomMessage { get { return useCustomMessage; } set { useCustomMessage = value; } }
        /// <summary> A custom interaction message for UI elements. </summary>
        public string CustomMessage { get { return customMessage; } set { customMessage = value; } }
        /// <summary> If true, you can only interact with this object a certain amount of times. </summary>
        public bool LimitedInteractions { get { return limitedInteractions; } set { limitedInteractions = value; } }
        /// <summary> The amount of times you can interact with this object. </summary>
        public int MaxInteractions { get { return maxInteractions; } set { maxInteractions = value; } }
        /// <summary> Called when the object is interacted with. </summary>
        public InteractionEvent OnInteract { get { return onInteract; } set { onInteract = value; } }
        /// <summary> Called when the object has reached it's max interactions. </summary>
        public InteractionEvent OnReachedMaxInteractions { get { return onReachedMaxInteractions; } set { onReachedMaxInteractions = value; } }

        /// <summary> The amount of times this object has been interacted with. </summary>
        public int Interactions { get { return interactions; } set { interactions = value; } }

        /// <summary> Determines of the object can be interacted with. Takes the interaction limit into account. </summary>
        public bool CanInteract { get { return limitedInteractions && interactions >= maxInteractions ? false : isInteractable; } }

#if OBSOLETE
        private void Awake()
        {
            Debug.LogError(gameObject.name + " has GoldPlayerInteractable attached. It will be removed on build. Please remove this component if you don't intend to use it.", gameObject);
        }
#endif

        /// <summary>
        /// Invokes the interact event.
        /// </summary>
        /// <param name="bypassIsInteractable">If true, it will ignore the Is Interactable property.</param>
        /// <param name="bypassLimit">If true, it will ignore the max limit property.</param>
        public void Interact(bool bypassIsInteractable, bool bypassLimit)
        {
            // If we can't interact and we're not bypassing the check, stop here.
            if (!isInteractable && !bypassIsInteractable)
            {
                return;
            }

            // If there's a limit on interactions, we've reached the max interactions, and we're not bypassing the check, stop here.
            if (limitedInteractions && interactions >= maxInteractions && !bypassLimit)
            {
                return;
            }

            // If limited interactions are enabled, add one to the interaction amount.
            if (limitedInteractions)
            {
                interactions++;
            }

            // Call the interact event.
            onInteract.Invoke();

            // If the interactions has reached it's max, invoke the reached max interactions event.
            if (interactions == maxInteractions)
            {
                onReachedMaxInteractions.Invoke();
            }
        }

        /// <summary>
        /// Invokes the interact event.
        /// </summary>
        public void Interact()
        {
            // Don't bypass the checks.
            Interact(false, false);
        }
    }
}
#endif
