using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer.Interaction
{
    [AddComponentMenu("Gold Player/Interaction/Gold Player Interactable")]
    [DisallowMultipleComponent]
    public class GoldPlayerInteractable : MonoBehaviour, IGoldPlayerInteractable
    {
        [System.Serializable]
        public class InteractionEvent : UnityEvent { }

        [SerializeField]
        [Tooltip("Determines if the object can be interacted with.")]
        [FormerlySerializedAs("m_CanInteract")]
        private bool canInteract = true;
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
        [Tooltip("Called when the object is interacted with.")]
        [FormerlySerializedAs("m_OnInteract")]
        private InteractionEvent onInteract;

        /// <summary> Determines if the object can be interacted with. </summary>
        public bool CanInteract { get { return canInteract; } set { canInteract = value; } }
        /// <summary> Determines if the object should be hidden. (Used for UI to not show a interaction message) </summary>
        public bool IsHidden { get { return isHidden; } set { isHidden = value; } }
        /// <summary> Determines if a custom interaction message should be shown. </summary>
        public bool UseCustomMessage { get { return useCustomMessage; } set { useCustomMessage = value; } }
        /// <summary> A custom interaction message for UI elements. </summary>
        public string CustomMessage { get { return customMessage; } set { customMessage = value; } }
        /// <summary> Called when the object is interacted with. </summary>
        public InteractionEvent OnInteract { get { return onInteract; } set { onInteract = value; } }

        /// <summary>
        /// Invokes the interact event.
        /// </summary>
        public void Interact()
        {
            onInteract.Invoke();
        }
    }
}
