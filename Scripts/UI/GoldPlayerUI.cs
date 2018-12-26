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
using UnityEngine.UI;
#if HERTZLIB_UPDATE_MANAGER
using Hertzole.HertzLib;
#endif

namespace Hertzole.GoldPlayer.UI
{
    [AddComponentMenu("Gold Player/UI/Gold Player UI")]
#if HERTZLIB_UPDATE_MANAGER
    public class GoldPlayerUI : MonoBehaviour, IUpdate
#else
    public class GoldPlayerUI : MonoBehaviour
#endif
    {
        // The type of progress bar.
        public enum ProgressBarType { Slider = 0, Image = 1 }
        // The type of label display.
        // Direct is basically current/max, so for example, 70/110.
        // Percentage is self-explanatory. Shows a percentage.
        public enum LabelDisplayType { Direct = 0, Percentage = 1 }

        [SerializeField]
        [Tooltip("If true, the component will always attempt to find the player.\nIf false, you will have to manually set the player.")]
        private bool m_AutoFindPlayer;
        [SerializeField]
        [Tooltip("The target player.")]
        private GoldPlayerController m_Player;

#if UNITY_EDITOR
        [Header("Sprinting")]
#endif
        [SerializeField]
        [Tooltip("The type of progress bar that will be used.")]
        private ProgressBarType m_SprintingBarType = ProgressBarType.Image;
        [SerializeField]
        [Tooltip("The progress bar as an image.")]
        private Image m_SprintingBarImage;
        [SerializeField]
        [Tooltip("The progress bar as a slider.")]
        private Slider m_SprintingBarSlider;
        [SerializeField]
        [Tooltip("The label for showing player stamina.")]
#if USE_TMP
        private TextMeshProUGUI m_SprintingLabel;
#else
        private Text m_SprintingLabel;
#endif
        [SerializeField]
        [Tooltip("The type of display if there's a label.")]
        private LabelDisplayType m_SprintingLabelDisplay = LabelDisplayType.Percentage;

        // Only show if GoldPlayer interaction is enabled.
#if GOLD_PLAYER_INTERACTION
#if UNITY_EDITOR
        [Header("Interaction")]
#endif
        [SerializeField]
        private bool m_AutoFindInteraction = true;
        [SerializeField]
        private GoldPlayerInteraction m_PlayerInteraction;
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

        /// <summary> The type of progress bar that will be used. </summary>
        public ProgressBarType SprintingBarType { get { return m_SprintingBarType; } set { m_SprintingBarType = value; AdaptSprintingUI(); } }
        /// <summary> The progress bar as an image. </summary>
        public Image SprintingBarImage { get { return m_SprintingBarImage; } set { m_SprintingBarImage = value; } }
        /// <summary> The progress bar as a slider. </summary>
        public Slider SprintingBarSlider { get { return m_SprintingBarSlider; } set { m_SprintingBarSlider = value; } }
        /// <summary> The label for showing player stamina. </summary>
#if USE_TMP
        public TextMeshProUGUI SprintingLabel { get { return m_SprintingLabel; } set { m_SprintingLabel = value; } }
#else
        public Text SprintingLabel { get { return m_SprintingLabel; } set { m_SprintingLabel = value; } }
#endif
        /// <summary> The type of display if there's a label. </summary>
        public LabelDisplayType SprintingLabelDisplay { get { return m_SprintingLabelDisplay; } set { m_SprintingLabelDisplay = value; } }

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

        /// <summary> If true, the component will always attempt to find the player. If false, you will have to manually set the player. </summary>
        public bool AutoFindPlayer { get { return m_AutoFindPlayer; } set { m_AutoFindPlayer = value; } }
        /// <summary> The target player. </summary>
        public GoldPlayerController Player
        {
            // If the player is null, and auto find is on, find the player.
            get { if (!m_Player && m_AutoFindPlayer) m_Player = FindObjectOfType<GoldPlayerController>(); return m_Player; }
            set { SetPlayer(value); }
        }

#if GOLD_PLAYER_INTERACTION
        public bool AutoFindInteraction { get { return m_AutoFindInteraction; } set { m_AutoFindInteraction = value; } }
        // Player interaction reference.
        protected GoldPlayerInteraction PlayerInteraction
        {
            // If the player interaction is null, and auto find is on, find the player interaction.
            get { if (!m_PlayerInteraction && m_AutoFindInteraction) m_PlayerInteraction = FindObjectOfType<GoldPlayerInteraction>(); return m_PlayerInteraction; }
            set { m_PlayerInteraction = value; }
        }
#endif

        private void Awake()
        {
            // Call all the Player Sprinting awake stuff.
            AwakePlayerSprinting();
#if GOLD_PLAYER_INTERACTION
            // Call all Player Interaction awake stuff.
            AwakePlayerInteraction();
#endif

            OnAwake();
        }

#if HERTZLIB_UPDATE_MANAGER
        protected virtual void OnEnable()
        {
            UpdateManager.AddUpdate(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.RemoveUpdate(this);
        }
#endif

        protected virtual void OnAwake() { }

        protected virtual void AwakePlayerSprinting()
        {
            AdaptSprintingUI();
        }

        /// <summary>
        /// Enables and disables sprinting UI elements based
        /// on how it's setup.
        /// </summary>
        public virtual void AdaptSprintingUI()
        {
            if (Player != null)
            {
                // If the player can't run or no stamina enabled, disable all elements.
                if (!Player.Movement.CanRun || !Player.Movement.Stamina.EnableStamina)
                {
                    if (m_SprintingBarImage != null)
                        m_SprintingBarImage.gameObject.SetActive(false);
                    if (m_SprintingBarSlider != null)
                        m_SprintingBarSlider.gameObject.SetActive(false);
                    if (m_SprintingLabel != null)
                        m_SprintingLabel.gameObject.SetActive(false);

                    return;
                }

                switch (m_SprintingBarType)
                {
                    case ProgressBarType.Slider:
                        if (m_SprintingBarImage != null)
                        {
                            m_SprintingBarImage.gameObject.SetActive(false);
                        }

                        if (m_SprintingBarSlider != null)
                        {
                            m_SprintingBarSlider.gameObject.SetActive(true);
                            m_SprintingBarSlider.minValue = 0;
                            m_SprintingBarSlider.maxValue = Player.Movement.Stamina.MaxStamina;
                        }
                        break;
                    case ProgressBarType.Image:
                        if (m_SprintingBarImage != null)
                            m_SprintingBarImage.gameObject.SetActive(true);
                        if (m_SprintingBarSlider != null)
                            m_SprintingBarSlider.gameObject.SetActive(false);
                        break;
                    default:
                        throw new System.NotImplementedException("There's no support for progress bar type '" + m_SprintingBarType + "' in GoldPlayerUI!");
                }
            }
        }

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

#if HERTZLIB_UPDATE_MANAGER
        public void OnUpdate()
#else
        private void Update()
#endif
        {
            SprintingUpdate();
#if GOLD_PLAYER_INTERACTION
            InteractionUpdate();
#endif
        }

        protected virtual void SprintingUpdate()
        {
            if (Player && Player.Movement.CanRun && Player.Movement.Stamina.EnableStamina)
            {
                switch (m_SprintingBarType)
                {
                    case ProgressBarType.Slider:
                        if (m_SprintingBarSlider != null)
                            m_SprintingBarSlider.value = Player.Movement.Stamina.CurrentStamina;
                        break;
                    case ProgressBarType.Image:
                        if (m_SprintingBarImage != null)
                            m_SprintingBarImage.fillAmount = Player.Movement.Stamina.CurrentStamina / Player.Movement.Stamina.MaxStamina;
                        break;
                    default:
                        throw new System.NotImplementedException("There's no support for progress bar type '" + m_SprintingBarType + "' in GoldPlayerUI!");
                }

                if (m_SprintingLabel != null)
                    m_SprintingLabel.text = GetLabel(m_SprintingLabelDisplay, Player.Movement.Stamina.CurrentStamina, Player.Movement.Stamina.MaxStamina);
            }
        }

#if GOLD_PLAYER_INTERACTION
        protected virtual void InteractionUpdate()
        {
            // Only call if player interaction is added to the player.
            if (PlayerInteraction && PlayerInteraction.CurrentHitInteractable != null)
            {
                // Toggle the interaction box based on if it can be seen.
                if (m_InteractionBox != null)
                    m_InteractionBox.SetActive(PlayerInteraction.CanInteract && !PlayerInteraction.CurrentHitInteractable.IsHidden);

                // If the player can interact the the interactable isn't hidden,
                // set the message to either a custom message or the one in Player Interaction.
                if (PlayerInteraction.CanInteract && !PlayerInteraction.CurrentHitInteractable.IsHidden && m_InteractionLabel != null)
                {
                    m_InteractionLabel.text = PlayerInteraction.CurrentHitInteractable.UseCustomMessage ?
                        PlayerInteraction.CurrentHitInteractable.CustomMessage : PlayerInteraction.InteractMessage;
                }
            }
            else
            {
                if (m_InteractionBox != null)
                    m_InteractionBox.SetActive(false);
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
            if (player != null && m_Player != player)
                m_PlayerInteraction = player.GetComponent<GoldPlayerInteraction>();
#endif
            // Set the player.
            m_Player = player;
        }

        /// <summary>
        /// Returns a formatted label based on the display type.
        /// </summary>
        protected virtual string GetLabel(LabelDisplayType displayType, float current, float max)
        {
            switch (displayType)
            {
                case LabelDisplayType.Direct:
                    return string.Format("{0}/{1}", current.ToString("F2"), max);
                case LabelDisplayType.Percentage:
                    return string.Format("{0}%", ((current / max) * 100).ToString("F0"));
                default:
                    throw new System.NotImplementedException("There's no support for label display type '" + m_SprintingBarType + "' in GoldPlayerUI!");
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// ONLY TO BE CALLED IN UNITY EDITOR!
        /// Called every time something is changed in the inspector.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (Application.isPlaying)
            {
                AdaptSprintingUI();
            }
        }
#endif
    }
}
