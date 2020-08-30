#if !GOLD_PLAYER_DISABLE_UI
// If Unity 2018 or newer is running, use TextMeshPro instead,
// as it's the recommended text solution.
#if UNITY_2018_1_OR_NEWER && GOLD_PLAYER_TMP
#define USE_TMP
#endif

#if !UNITY_2019_2_OR_NEWER || (UNITY_2019_2_OR_NEWER && GOLD_PLAYER_UGUI)
#define USE_GUI
#endif

#if USE_TMP
using TMPro;
#endif
using UnityEngine;
using UnityEngine.Serialization;
#if USE_GUI
using UnityEngine.UI;
#endif

namespace Hertzole.GoldPlayer
{
    [AddComponentMenu("Gold Player/Gold Player UI", 10)]
    public class GoldPlayerUI : MonoBehaviour
    {
        // The type of progress bar.
        public enum ProgressBarType { Slider = 0, Image = 1 }
        // The type of label display.
        // Direct is basically current/max, so for example, 70/110.
        // Percentage is self-explanatory. Shows a percentage.
        public enum LabelDisplayType { Direct = 0, Percentage = 1 }

        [SerializeField]
        [Tooltip("If true, the component will always attempt to find the player.\nIf false, you will have to manually set the player.")]
        [FormerlySerializedAs("m_AutoFindPlayer")]
        private bool autoFindPlayer = false;
        [SerializeField]
        [Tooltip("The target player.")]
        [FormerlySerializedAs("m_Player")]
        private GoldPlayerController player = null;

#if UNITY_EDITOR
        [Header("Sprinting")]
#endif
        [SerializeField]
        [Tooltip("The type of progress bar that will be used.")]
        [FormerlySerializedAs("m_SprintingBarType")]
        private ProgressBarType sprintingBarType = ProgressBarType.Image;
#if USE_GUI
        [SerializeField]
        [Tooltip("The progress bar as an image.")]
        [FormerlySerializedAs("m_SprintingBarImage")]
        private Image sprintingBarImage;
        [SerializeField]
        [Tooltip("The progress bar as a slider.")]
        [FormerlySerializedAs("m_SprintingBarSlider")]
        private Slider sprintingBarSlider;
        [SerializeField]
        [Tooltip("The label for showing player stamina.")]
        [FormerlySerializedAs("m_SprintingLabel")]
        private Text sprintingLabel;
#if USE_TMP
        [SerializeField]
        [Tooltip("The TextMeshPro label for showing player stamina.")]
        private TextMeshProUGUI sprintingLabelPro;
#endif
#endif
        [SerializeField]
        [Tooltip("The type of display if there's a label.")]
        [FormerlySerializedAs("m_SprintingLabelDisplay")]
        private LabelDisplayType sprintingLabelDisplay = LabelDisplayType.Percentage;

        // Only show if GoldPlayer interaction is enabled.
#if GOLD_PLAYER_INTERACTION && !GOLD_PLAYER_DISABLE_INTERACTION
#if UNITY_EDITOR
        [Header("Interaction")]
#endif
        [SerializeField]
        [FormerlySerializedAs("m_AutoFindInteraction")]
        private bool autoFindInteraction = true;
        [SerializeField]
        [FormerlySerializedAs("m_PlayerInteraction")]
        private GoldPlayerInteraction playerInteraction;
        [SerializeField]
        [Tooltip("The box/label that should be toggled when the player can interact.")]
        [FormerlySerializedAs("m_InteractionBox")]
        private GameObject interactionBox;
#if USE_GUI
        [SerializeField]
        [Tooltip("The label for the interaction message.")]
        [FormerlySerializedAs("m_InteractionLabel")]
        private Text interactionLabel;
#if USE_TMP
        [SerializeField]
        [Tooltip("The TextMeshPro label for the interaction message.")]
        private TextMeshProUGUI interactionLabelPro;
#endif
#endif
#endif

        /// <summary> The type of progress bar that will be used. </summary>
        public ProgressBarType SprintingBarType { get { return sprintingBarType; } set { sprintingBarType = value; AdaptSprintingUI(); } }
#if USE_GUI
        /// <summary> The progress bar as an image. </summary>
        public Image SprintingBarImage { get { return sprintingBarImage; } set { sprintingBarImage = value; } }
        /// <summary> The progress bar as a slider. </summary>
        public Slider SprintingBarSlider { get { return sprintingBarSlider; } set { sprintingBarSlider = value; } }
        /// <summary> The label for showing player stamina. </summary>
        public Text SprintingLabel { get { return sprintingLabel; } set { sprintingLabel = value; } }
#if USE_TMP
        /// <summary> The TextMeshPro label for showing player stamina. </summary>
        public TextMeshProUGUI SprintingLabelPro { get { return sprintingLabelPro; } set { sprintingLabelPro = value; } }
#endif
#endif
        /// <summary> The type of display if there's a label. </summary>
        public LabelDisplayType SprintingLabelDisplay { get { return sprintingLabelDisplay; } set { sprintingLabelDisplay = value; } }

#if GOLD_PLAYER_INTERACTION && !GOLD_PLAYER_DISABLE_INTERACTION
        /// <summary> The box/label that should be toggled when the player can interact. </summary>
        public GameObject InteractionBox { get { return interactionBox; } set { interactionBox = value; } }
#if USE_GUI
        /// <summary> The label for the interaction message. </summary>
        public Text InteractionLabel { get { return interactionLabel; } set { interactionLabel = value; } }
#if USE_TMP
        /// <summary> The TextMeshPro label for the interaction message. </summary>
        public TextMeshProUGUI InteractionLabelPro { get { return interactionLabelPro; } set { interactionLabelPro = value; } }
#endif
#endif
#endif

        /// <summary> If true, the component will always attempt to find the player. If false, you will have to manually set the player. </summary>
        public bool AutoFindPlayer { get { return autoFindPlayer; } set { autoFindPlayer = value; } }
        /// <summary> The target player. </summary>
        public GoldPlayerController Player
        {
            // If the player is null, and auto find is on, find the player.
            get { if (!player && autoFindPlayer) { player = FindObjectOfType<GoldPlayerController>(); } return player; }
            set { SetPlayer(value); }
        }

#if GOLD_PLAYER_INTERACTION && !GOLD_PLAYER_DISABLE_INTERACTION
        public bool AutoFindInteraction { get { return autoFindInteraction; } set { autoFindInteraction = value; } }
        // Player interaction reference.
        protected GoldPlayerInteraction PlayerInteraction
        {
            // If the player interaction is null, and auto find is on, find the player interaction.
            get { if (!playerInteraction && autoFindInteraction) { playerInteraction = FindObjectOfType<GoldPlayerInteraction>(); } return playerInteraction; }
            set { playerInteraction = value; }
        }
#endif

        private void Awake()
        {
            // Call all the Player Sprinting awake stuff.
            AwakePlayerSprinting();
#if GOLD_PLAYER_INTERACTION && !GOLD_PLAYER_DISABLE_INTERACTION
            // Call all Player Interaction awake stuff.
            AwakePlayerInteraction();
#endif

            OnAwake();
        }

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
#if USE_GUI
            if (Player != null)
            {
                // If the player can't run or no stamina enabled, disable all elements.
                if (!Player.Movement.CanRun || !Player.Movement.Stamina.EnableStamina)
                {
                    if (sprintingBarImage != null)
                    {
                        sprintingBarImage.gameObject.SetActive(false);
                    }

                    if (sprintingBarSlider != null)
                    {
                        sprintingBarSlider.gameObject.SetActive(false);
                    }

                    if (sprintingLabel != null)
                    {
                        sprintingLabel.gameObject.SetActive(false);
                    }

                    return;
                }

                switch (sprintingBarType)
                {
                    case ProgressBarType.Slider:
                        if (sprintingBarImage != null)
                        {
                            sprintingBarImage.gameObject.SetActive(false);
                        }

                        if (sprintingBarSlider != null)
                        {
                            sprintingBarSlider.gameObject.SetActive(true);
                            sprintingBarSlider.minValue = 0;
                            sprintingBarSlider.maxValue = Player.Movement.Stamina.MaxStamina;
                        }
                        break;
                    case ProgressBarType.Image:
                        if (sprintingBarImage != null)
                        {
                            sprintingBarImage.gameObject.SetActive(true);
                        }

                        if (sprintingBarSlider != null)
                        {
                            sprintingBarSlider.gameObject.SetActive(false);
                        }

                        break;
                    default:
                        throw new System.NotImplementedException("There's no support for progress bar type '" + sprintingBarType + "' in GoldPlayerUI!");
                }
            }
#else
            Debug.LogWarning("GoldPlayerUI is being used but there's no UGUI in this project!");
#endif
        }

#if GOLD_PLAYER_INTERACTION && !GOLD_PLAYER_DISABLE_INTERACTION
        protected virtual void AwakePlayerInteraction()
        {
            // Get the player interaction.
            if (player)
            {
                playerInteraction = player.GetComponent<GoldPlayerInteraction>();
            }
            else if (autoFindPlayer)
            {
                playerInteraction = FindObjectOfType<GoldPlayerInteraction>();
            }
        }
#endif

        private void Update()
        {
            SprintingUpdate();
#if GOLD_PLAYER_INTERACTION && !GOLD_PLAYER_DISABLE_INTERACTION
            InteractionUpdate();
#endif
        }

        protected virtual void SprintingUpdate()
        {
#if USE_GUI
            if (Player && Player.Movement.CanRun && Player.Movement.Stamina.EnableStamina)
            {
                switch (sprintingBarType)
                {
                    case ProgressBarType.Slider:
                        if (sprintingBarSlider != null)
                        {
                            sprintingBarSlider.value = Player.Movement.Stamina.CurrentStamina;
                        }

                        break;
                    case ProgressBarType.Image:
                        if (sprintingBarImage != null)
                        {
                            sprintingBarImage.fillAmount = Player.Movement.Stamina.CurrentStamina / Player.Movement.Stamina.MaxStamina;
                        }

                        break;
                    default:
                        throw new System.NotImplementedException("There's no support for progress bar type '" + sprintingBarType + "' in GoldPlayerUI!");
                }

                string sprintString = GetLabel(sprintingLabelDisplay, Player.Movement.Stamina.CurrentStamina, Player.Movement.Stamina.MaxStamina);

                if (sprintingLabel != null)
                {
                    sprintingLabel.text = sprintString;
                }

#if USE_TMP
                if (sprintingLabelPro != null)
                {
                    sprintingLabelPro.text = sprintString;
                }
#endif
            }
#else
            Debug.LogWarning("GoldPlayerUI is being used but there's no UGUI in this project!");
#endif
        }

#if GOLD_PLAYER_INTERACTION && !GOLD_PLAYER_DISABLE_INTERACTION
        protected virtual void InteractionUpdate()
        {
#if USE_GUI
            // Only call if player interaction is added to the player.
            if (PlayerInteraction && PlayerInteraction.CurrentHitInteractable != null)
            {
                // Toggle the interaction box based on if it can be seen.
                if (interactionBox != null)
                {
                    interactionBox.SetActive(PlayerInteraction.CanInteract && !PlayerInteraction.CurrentHitInteractable.IsHidden);
                }

                // If the player can interact the the interactable isn't hidden,
                // set the message to either a custom message or the one in Player Interaction.
                if (PlayerInteraction.CanInteract && !PlayerInteraction.CurrentHitInteractable.IsHidden)
                {
                    string message = PlayerInteraction.CurrentHitInteractable.UseCustomMessage ? PlayerInteraction.CurrentHitInteractable.CustomMessage : PlayerInteraction.InteractMessage;

                    if (interactionLabel != null)
                    {
                        interactionLabel.text = message;
                    }

#if USE_TMP
                    if (interactionLabelPro != null)
                    {
                        interactionLabelPro.text = message;
                    }
#endif
                }
            }
            else
            {
                if (interactionBox != null)
                {
                    interactionBox.SetActive(false);
                }
            }
#else
            Debug.LogWarning("GoldPlayerUI is being used but there's no UGUI in this project!");
#endif
        }
#endif

        /// <summary>
        /// Sets the player and finds all required components.
        /// </summary>
        private void SetPlayer(GoldPlayerController player)
        {
#if GOLD_PLAYER_INTERACTION && !GOLD_PLAYER_DISABLE_INTERACTION
            // Only get the interaction if the previous set player isn't the new player.
            if (player != null && this.player != player)
            {
                playerInteraction = player.GetComponent<GoldPlayerInteraction>();
            }
#endif
            // Set the player.
            this.player = player;
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
                    throw new System.NotImplementedException("There's no support for label display type '" + sprintingBarType + "' in GoldPlayerUI!");
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
#endif
