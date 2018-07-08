using UnityEngine;
using UnityEngine.Events;

namespace Hertzole.GoldPlayer.Interaction
{
    [AddComponentMenu("Gold Player/Interaction/Player Interactable")]
    [DisallowMultipleComponent]
    public class GoldPlayerInteractable : MonoBehaviour
    {
        [System.Serializable]
        public class InteractionEvent : UnityEvent { }

        [SerializeField]
        [Tooltip("Determines if the object can be interacted with.")]
        private bool m_CanInteract = true;
        [SerializeField]
        [Tooltip("Determines if the object should be hidden.\n(Used for UI to not show a interaction message)")]
        private bool m_IsHidden;

#if UNITY_EDITOR
        [Space]
#endif

        [SerializeField]
        [Tooltip("Determines if a custom interaction message should be shown.")]
        private bool m_UseCustomMessage = false;
        [SerializeField]
        [Tooltip("A custom interaction message for UI elements.")]
        private string m_CustomMessage = "Press {key} to interact";

#if UNITY_EDITOR
        [Space]
#endif
        [SerializeField]
        [Tooltip("Called when the object is interacted with.")]
        private InteractionEvent m_OnInteract;

        /// <summary> Determines if the object can be interacted with. </summary>
        public bool CanInteract { get { return m_CanInteract; } set { m_CanInteract = value; } }
        /// <summary> Determines if the object should be hidden.\n(Used for UI to not show a interaction message) </summary>
        public bool IsHidden { get { return m_IsHidden; } set { m_IsHidden = value; } }
        /// <summary> Determines if a custom interaction message should be shown. </summary>
        public bool UseCustomMessage { get { return m_UseCustomMessage; } set { m_UseCustomMessage = value; } }
        /// <summary> A custom interaction message for UI elements. </summary>
        public string CustomMessage { get { return m_CustomMessage; } set { m_CustomMessage = value; } }
        /// <summary> Called when the object is interacted with. </summary>
        public InteractionEvent OnInteract { get { return m_OnInteract; } set { m_OnInteract = value; } }

        /// <summary>
        /// Invokes the interact event.
        /// </summary>
        public void Interact()
        {
            m_OnInteract.Invoke();
        }
    }
}
