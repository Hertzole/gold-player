// If Unity 2018 or newer is running, use TextMeshPro instead,
// as it's the recommended text solution.
#if UNITY_2018_1_OR_NEWER
#define USE_TMP
#endif

#if GOLD_PLAYER_INTERACTION
using Hertzole.GoldPlayer.Interaction;
#endif
#if USE_TMP
using TMPro;
#endif
using UnityEngine;

namespace Hertzole.GoldPlayer.UI
{
    [AddComponentMenu("Gold Player/UI/Gold Player UI")]
    public class GoldPlayerUI : MonoBehaviour
    {
        // Only show if GoldPlayer interaction is enabled.
#if GOLD_PLAYER_INTERACTION
#if UNITY_EDITOR
        [Header("Interaction")]
#endif
        [SerializeField]
        [Tooltip("The box/label that should be toggled when the player can interact.")]
        private GameObject m_InteractionBox;
        [SerializeField]
        [Tooltip("The label for the interaction message.")]
#if USE_TMP
        private TextMeshProUGUI m_InteractionLabel;
#else
        private Text m_InteractionLabel;
#endif
#endif

#if GOLD_PLAYER_INTERACTION
        /// <summary> The box/label that should be toggled when the player can interact. </summary>
        public GameObject InteractionBox { get { return m_InteractionBox; } set { m_InteractionBox = value; } }
        /// <summary> The label for the interaction message. </summary>
#if USE_TMP
        public TextMeshProUGUI InteractionLabel { get { return m_InteractionLabel; } set { m_InteractionLabel = value; } }
#else
        public Text InteractionLabel { get { return m_InteractionLabel; } set { m_InteractionLabel = value; } }
#endif
#endif

#if GOLD_PLAYER_INTERACTION
        // Player interaction reference.
        private GoldPlayerInteraction m_PlayerInteraction;
        protected GoldPlayerInteraction PlayerInteraction
        {
            // If the player is null, find it.
            get { if (!m_PlayerInteraction) m_PlayerInteraction = FindObjectOfType<GoldPlayerInteraction>(); return m_PlayerInteraction; }
        }
#endif

        private void Awake()
        {
#if GOLD_PLAYER_INTERACTION
            // Call all Player Interaction awake stuff.
            AwakePlayerInteraction();
#endif

            OnAwake();
        }

        protected virtual void OnAwake() { }

#if GOLD_PLAYER_INTERACTION
        protected virtual void AwakePlayerInteraction()
        {
            // Get the player interaction.
            m_PlayerInteraction = FindObjectOfType<GoldPlayerInteraction>();
        }
#endif

        private void Update()
        {
#if GOLD_PLAYER_INTERACTION
            InteractionUpdate();
#endif
        }

#if GOLD_PLAYER_INTERACTION
        protected virtual void InteractionUpdate()
        {
            // Only call if player interaction is added to the player.
            if (PlayerInteraction)
            {
                // Toggle the interaction box based on if it can be seen.
#if NET_4_6
                m_InteractionBox?.SetActive(PlayerInteraction.CanInteract && !PlayerInteraction.CurrentHitInteractable.IsHidden);
#else
                if (m_InteractionBox != null)
                    m_InteractionBox.SetActive(PlayerInteraction.CanInteract && !PlayerInteraction.CurrentHitInteractable.IsHidden);
#endif

                // If the player can interact the the interactable isn't hidden,
                // set the message to either a custom message or the one in Player Interaction.
                if (PlayerInteraction.CanInteract && !PlayerInteraction.CurrentHitInteractable.IsHidden && m_InteractionLabel != null)
                {
                    if (PlayerInteraction.CurrentHitInteractable != null && PlayerInteraction.CurrentHitInteractable.UseCustomMessage)
                        m_InteractionLabel.text = PlayerInteraction.CurrentHitInteractable.CustomMessage;
                    else
                        m_InteractionLabel.text = PlayerInteraction.InteractMessage;
                }
            }
        }
#endif
    }
}
