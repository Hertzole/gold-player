#if GOLD_PLAYER_DISABLE_UI
#define OBSOLETE
#endif

#if OBSOLETE && !UNITY_EDITOR
#define STRIP
#endif

#if !STRIP

// If Unity 2018 or newer is running, use TextMeshPro instead,
// as it's the recommended text solution.
#if UNITY_2018_1_OR_NEWER || GOLD_PLAYER_TMP
#define USE_TMP
#endif

#if !UNITY_2019_2_OR_NEWER || (UNITY_2019_2_OR_NEWER && GOLD_PLAYER_UGUI)
#define USE_GUI
#endif

#if USE_TMP
using TMPro;
#endif
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
#if USE_GUI
using UnityEngine.UI;
#endif

namespace Hertzole.GoldPlayer
{
#if !OBSOLETE
    [AddComponentMenu("Gold Player/Gold Player UI", 10)]
#else
    [System.Obsolete("Gold Player UI has been disabled. GoldPlayerUI will be removed on build.")]
    [AddComponentMenu("")]
#endif
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
        [Header("Stamina")]
#endif
        [SerializeField]
        [Tooltip("The type of progress bar that will be used.")]
        [FormerlySerializedAs("m_SprintingBarType")]
        [FormerlySerializedAs("sprintingBarType")]
        private ProgressBarType staminaBarType = ProgressBarType.Image;
#if USE_GUI
        [SerializeField]
        [Tooltip("The progress bar as an image.")]
        [FormerlySerializedAs("m_SprintingBarImage")]
        [FormerlySerializedAs("sprintingBarImage")]
        private Image staminaBarImage;
        [SerializeField]
        [Tooltip("The progress bar as a slider.")]
        [FormerlySerializedAs("m_SprintingBarSlider")]
        [FormerlySerializedAs("sprintingBarSlider")]
        private Slider staminaBarSlider;
        [SerializeField]
        [Tooltip("The label for showing player stamina.")]
        [FormerlySerializedAs("m_SprintingLabel")]
        [FormerlySerializedAs("sprintingLabel")]
        private Text staminaLabel;
#if USE_TMP
        [SerializeField]
        [Tooltip("The TextMeshPro label for showing player stamina.")]
        private TextMeshProUGUI staminaLabelPro;
#endif
#endif
        [SerializeField]
        [Tooltip("The type of display if there's a label.")]
        [FormerlySerializedAs("m_SprintingLabelDisplay")]
        [FormerlySerializedAs("sprintingLabelDisplay")]
        private LabelDisplayType staminaLabelDisplay = LabelDisplayType.Percentage;
        [SerializeField] 
        [Tooltip("The amount of stamina change required before the label is updated.")]
        private float staminaLabelChangeRequired = 0.1f;
        [SerializeField]
        [Tooltip("The format of the stamina percentage value.")]
        private string staminaPercentageFormat = "F0";
        [SerializeField]
        [Tooltip("The format of the current stamina value.")]
        private string staminaDirectValueFormat = "F0";
        [SerializeField]
        [Tooltip("The format of the max stamina value.")]
        private string staminaDirectMaxFormat = "F0";

        // Only show if GoldPlayer interaction is enabled.
#if !GOLD_PLAYER_DISABLE_INTERACTION
#if UNITY_EDITOR
        [Header("Interaction")]
#endif
        [SerializeField]
        [Tooltip("If true, the component will always attempt to find the interaction component.\nIf false, you will have to manually set the player.")]
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
        public ProgressBarType StaminaBarType { get { return staminaBarType; } set { staminaBarType = value; AdaptSprintingUI(); } }
#if USE_GUI
        /// <summary> The progress bar as an image. </summary>
        public Image StaminaBarImage { get { return staminaBarImage; } set { staminaBarImage = value; } }
        /// <summary> The progress bar as a slider. </summary>
        public Slider StaminaBarSlider { get { return staminaBarSlider; } set { staminaBarSlider = value; } }
        /// <summary> The label for showing player stamina. </summary>
        public Text StaminaLabel { get { return staminaLabel; } set { staminaLabel = value; } }
#if USE_TMP
        /// <summary> The TextMeshPro label for showing player stamina. </summary>
        public TextMeshProUGUI StaminaLabelPro { get { return staminaLabelPro; } set { staminaLabelPro = value; } }
#endif
#endif
	    /// <summary> The amount of stamina change required before the label is updated. </summary>
	    public float StaminaLabelChangeRequired { get { return staminaLabelChangeRequired; } set { staminaLabelChangeRequired = value; } }
        /// <summary> The format of the stamina percentage value. </summary>
        public string PercentageFormat { get { return staminaPercentageFormat; } set { staminaPercentageFormat = value; } }
        /// <summary> The format of the current stamina value. </summary>
        public string DirectValueFormat { get { return staminaDirectValueFormat; } set { staminaDirectValueFormat = value; } }
        /// <summary> The format of the max stamina value. </summary>
        public string DirectMaxFormat { get { return staminaDirectMaxFormat; } set { staminaDirectMaxFormat = value; } }

        /// <summary> The type of display if there's a label. </summary>
        public LabelDisplayType StaminaLabelDisplay { get { return staminaLabelDisplay; } set { staminaLabelDisplay = value; } }

#if !GOLD_PLAYER_DISABLE_INTERACTION
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
            get { if (!player && autoFindPlayer) { player = FindFirstObject<GoldPlayerController>(); } return player; }
            set { SetPlayer(value); }
        }

#if !GOLD_PLAYER_DISABLE_INTERACTION
        public bool AutoFindInteraction { get { return autoFindInteraction; } set { autoFindInteraction = value; } }
        // Player interaction reference.
        protected GoldPlayerInteraction PlayerInteraction
        {
            // If the player interaction is null, and auto find is on, find the player interaction.
            get { if (!playerInteraction && autoFindInteraction) { playerInteraction = FindFirstObject<GoldPlayerInteraction>(); } return playerInteraction; }
            set { playerInteraction = value; }
        }
#endif

        #region Obsolete
#if UNITY_EDITOR
        [System.Obsolete("Use 'StaminaBarType' instead. This will be removed on build.", true)]
        public ProgressBarType SprintingBarType { get { return staminaBarType; } set { staminaBarType = value; AdaptSprintingUI(); } }
#if USE_GUI
        [System.Obsolete("Use 'StaminaBarImage' instead. This will be removed on build.", true)]
        public Image SprintingBarImage { get { return staminaBarImage; } set { staminaBarImage = value; } }
        [System.Obsolete("Use 'StaminaBarSlider' instead. This will be removed on build.", true)]
        public Slider SprintingBarSlider { get { return staminaBarSlider; } set { staminaBarSlider = value; } }
        [System.Obsolete("Use 'StaminaLabel' instead. This will be removed on build.", true)]
        public Text SprintingLabel { get { return staminaLabel; } set { staminaLabel = value; } }
#endif
        [System.Obsolete("Use 'StaminaLabelDisplay' instead. This will be removed on build.", true)]
        public LabelDisplayType SprintingLabelDisplay { get { return staminaLabelDisplay; } set { staminaLabelDisplay = value; } }
#endif
        #endregion

        private void Awake()
        {
#if OBSOLETE
            Debug.LogError(gameObject.name + " has GoldPlayerUI attached. It will be removed on build. Please remove this component if you don't intend to use it.", gameObject);
#else

            // Call all the Player Sprinting awake stuff.
            AwakePlayerSprinting();
#if !GOLD_PLAYER_DISABLE_INTERACTION
            // Call all Player Interaction awake stuff.
            AwakePlayerInteraction();
#endif

            OnAwake();
#endif
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
                    if (staminaBarImage != null)
                    {
                        staminaBarImage.gameObject.SetActive(false);
                    }

                    if (staminaBarSlider != null)
                    {
                        staminaBarSlider.gameObject.SetActive(false);
                    }

                    if (staminaLabel != null)
                    {
                        staminaLabel.gameObject.SetActive(false);
                    }

                    return;
                }

                switch (staminaBarType)
                {
                    case ProgressBarType.Slider:
                        if (staminaBarImage != null)
                        {
                            staminaBarImage.gameObject.SetActive(false);
                        }

                        if (staminaBarSlider != null)
                        {
                            staminaBarSlider.gameObject.SetActive(true);
                            staminaBarSlider.minValue = 0;
                            staminaBarSlider.maxValue = 1;
                        }
                        break;
                    case ProgressBarType.Image:
                        if (staminaBarImage != null)
                        {
                            staminaBarImage.gameObject.SetActive(true);
                        }

                        if (staminaBarSlider != null)
                        {
                            staminaBarSlider.gameObject.SetActive(false);
                        }

                        break;
                    default:
                        throw new System.NotImplementedException("There's no support for progress bar type '" + staminaBarType + "' in GoldPlayerUI!");
                }
            }
#else
            Debug.LogWarning("GoldPlayerUI is being used but there's no UGUI in this project!");
#endif
        }

#if !GOLD_PLAYER_DISABLE_INTERACTION
        protected virtual void AwakePlayerInteraction()
        {
            // Get the player interaction.
            if (player)
            {
                playerInteraction = player.GetComponent<GoldPlayerInteraction>();
            }
            else if (autoFindPlayer)
            {
                playerInteraction = FindFirstObject<GoldPlayerInteraction>();
            }
        }
#endif

        private void Update()
        {
            SprintingUpdate();
#if !GOLD_PLAYER_DISABLE_INTERACTION
            InteractionUpdate();
#endif
        }

        private float previousStamina = 0;
        
        protected virtual void SprintingUpdate()
        {
#if USE_GUI
            if (Player && Player.Movement.CanRun && Player.Movement.Stamina.EnableStamina)
            {
                switch (staminaBarType)
                {
                    case ProgressBarType.Slider:
                        if (staminaBarSlider != null)
                        {
                            staminaBarSlider.value = Player.Movement.Stamina.CurrentStamina / Player.Movement.Stamina.MaxStamina;
                        }

                        break;
                    case ProgressBarType.Image:
                        if (staminaBarImage != null)
                        {
                            staminaBarImage.fillAmount = Player.Movement.Stamina.CurrentStamina / Player.Movement.Stamina.MaxStamina;
                        }

                        break;
                    default:
                        throw new System.NotImplementedException("There's no support for progress bar type '" + staminaBarType + "' in GoldPlayerUI!");
                }

                if (Math.Abs(Player.Movement.Stamina.CurrentStamina - previousStamina) > staminaLabelChangeRequired)
                {
	                previousStamina = Player.Movement.Stamina.CurrentStamina;
					string sprintString = GetLabel(staminaLabelDisplay, Player.Movement.Stamina.CurrentStamina, Player.Movement.Stamina.MaxStamina, staminaDirectValueFormat, staminaDirectMaxFormat, staminaPercentageFormat);
					if (staminaLabel != null)
					{
						staminaLabel.text = sprintString;
					}

#if USE_TMP
					if (staminaLabelPro != null)
					{
						staminaLabelPro.text = sprintString;
					}
#endif
                }
            }
#else
            Debug.LogWarning("GoldPlayerUI is being used but there's no UGUI in this project!");
#endif
        }

#if !GOLD_PLAYER_DISABLE_INTERACTION
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
#if !GOLD_PLAYER_DISABLE_INTERACTION
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
        protected virtual string GetLabel(LabelDisplayType displayType, float current, float max, string directValueFormat, string directMaxFormat, string percentageFormat)
        {
            switch (displayType)
            {
                case LabelDisplayType.Direct:
                    return string.Format("{0}/{1}", current.ToString(directValueFormat), max.ToString(directMaxFormat));
                case LabelDisplayType.Percentage:
                    return string.Format("{0}%", ((current / max) * 100).ToString(percentageFormat));
                default:
#if DEBUG || UNITY_EDITOR
                    throw new System.NotImplementedException("There's no support for label display type '" + staminaBarType + "' in GoldPlayerUI!");
#else
                    return string.Empty;
#endif
            }
        }

        private static T FindFirstObject<T>() where T : Object
        {
#if UNITY_2023_1_OR_NEWER
	        return FindFirstObjectByType<T>();
#else
	        return FindObjectOfType<T>();
#endif
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
