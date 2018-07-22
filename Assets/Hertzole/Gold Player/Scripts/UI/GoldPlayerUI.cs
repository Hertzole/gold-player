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
        [SerializeField]
        [Tooltip("If true, the component will always attempt to find the player.\nIf false, you will have to manually set the player.")]
        private bool m_AutoFindPlayer;
        [SerializeField]
        [Tooltip("The target player.")]
        private GoldPlayerController m_Player;

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
        /// <summary> If true, the component will always attempt to find the player. If false, you will have to manually set the player. </summary>
        public bool AutoFindPlayer { get { return m_AutoFindPlayer; } set { m_AutoFindPlayer = value; } }
        /// <summary> The target player. </summary>
        public GoldPlayerController Player { get { return m_Player; } set { SetPlayer(value); } }

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
            get { if (!m_PlayerInteraction && m_AutoFindPlayer) m_PlayerInteraction = FindObjectOfType<GoldPlayerInteraction>(); return m_PlayerInteraction; }
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
            if (m_Player)
                m_PlayerInteraction = m_Player.GetComponent<GoldPlayerInteraction>();
            else if (m_AutoFindPlayer)
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

        /// <summary>
        /// Sets the player and finds all required components.
        /// </summary>
        private void SetPlayer(GoldPlayerController player)
        {
#if GOLD_PLAYER_INTERACTION
            // Only get the interaction if the previous set player isn't the new player.
            if (!m_Player || (m_Player && m_Player != player))
                m_PlayerInteraction = player.GetComponent<GoldPlayerInteraction>();
#endif
            // Set the player.
            m_Player = player;
        }
    }
}
