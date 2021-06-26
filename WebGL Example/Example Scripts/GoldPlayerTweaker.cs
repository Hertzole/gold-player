#if !UNITY_2019_2_OR_NEWER || (UNITY_2019_2_OR_NEWER && GOLD_PLAYER_UGUI)
#define USE_GUI
#endif

#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
#define NEW_INPUT
#endif

using System;
using UnityEngine;
using UnityEngine.Serialization;
#if USE_GUI
#if GOLD_PLAYER_TMP
using TMPro;
#else
using UnityEngine.UI;
#endif
using UnityEngine.SceneManagement;
#endif
#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
using UnityEngine.InputSystem;
#endif

namespace Hertzole.GoldPlayer.Example
{
    [AddComponentMenu("Gold Player/Examples/Gold Player Tweaker", 100)]
    public class GoldPlayerTweaker : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("m_TargetPlayer")]
        private GoldPlayerController targetPlayer;
        public GoldPlayerController TargetPlayer { get { return targetPlayer; } set { targetPlayer = value; } }
#if USE_GUI
        [SerializeField]
        [FormerlySerializedAs("m_TweakText")]
#if GOLD_PLAYER_TMP
        private TextMeshProUGUI tweakText;
        public TextMeshProUGUI TweakText { get { return tweakText; } set { tweakText = value; } }
#else
        private Text tweakText;
        public Text TweakText { get { return tweakText; } set { tweakText = value; } }
#endif
#endif
        [SerializeField]
        [FormerlySerializedAs("m_Panel")]
        private GameObject panel;
        public GameObject Panel { get { return panel; } set { panel = value; } }
#if USE_GUI
        [SerializeField]
        [FormerlySerializedAs("m_Viewport")]
        private RectTransform viewport;
        public RectTransform Viewport { get { return viewport; } set { viewport = value; } }
        [SerializeField]
        [FormerlySerializedAs("m_HeaderLabel")]
#if GOLD_PLAYER_TMP
        private TextMeshProUGUI headerLabel;
        public TextMeshProUGUI HeaderLabel { get { return headerLabel; } set { headerLabel = value; } }
#else
        private Text headerLabel;
        public Text HeaderLabel { get { return headerLabel; } set { headerLabel = value; } }
#endif
#endif
        [SerializeField]
        [FormerlySerializedAs("m_TweakField")]
        private GoldPlayerTweakField tweakField;
        public GoldPlayerTweakField TweakField { get { return tweakField; } set { tweakField = value; } }
        [Space]

#if !NEW_INPUT
        [SerializeField]
        [FormerlySerializedAs("m_ToggleKey")]
        private KeyCode toggleKey = KeyCode.F1;
        public KeyCode ToggleKey { get { return toggleKey; } set { toggleKey = value; } }
        [SerializeField]
        [FormerlySerializedAs("m_ResetSceneKey")]
        private KeyCode resetSceneKey = KeyCode.F2;
        public KeyCode ResetSceneKey { get { return resetSceneKey; } set { resetSceneKey = value; } }
#else
        [SerializeField]
        private InputAction toggleAction = new InputAction();
        public InputAction ToggleAction { get { return toggleAction; } set { toggleAction = value; } }
        [SerializeField]
        private InputAction resetSceneAction = new InputAction();
        public InputAction ResetSceneAction { get { return resetSceneAction; } set { resetSceneAction = value; } }
#endif

        [SerializeField]
        [HideInInspector]
        private GoldPlayerUI ui = null;

#if USE_GUI
        private bool showing = false;
        private bool previousCanLook = false;
        private bool previousCanMove = false;
        private bool previousLockCursor = false;
#endif

#if !UNITY_EDITOR && UNITY_WEBGL
        private void Awake()
        {
            WebGLInput.captureAllKeyboardInput = true;
        }
#endif

        // Use this for initialization
        void Start()
        {
#if USE_GUI
            Panel.gameObject.SetActive(false);
            headerLabel.gameObject.SetActive(false);
            tweakField.gameObject.SetActive(false);
            tweakText.gameObject.SetActive(targetPlayer != null);

            if (targetPlayer)
            {
                previousCanLook = TargetPlayer.Camera.CanLookAround;
                previousCanMove = TargetPlayer.Movement.CanMoveAround;
                previousLockCursor = TargetPlayer.Camera.ShouldLockCursor;
                viewport.anchorMin = new Vector2(0, 0);
                viewport.anchorMax = new Vector2(1, 1);
                viewport.sizeDelta = new Vector2(0, 0);

                SetupUI();
            }

#if NEW_INPUT
            tweakText.text = "Press " + toggleAction.GetBindingDisplayString() + " to tweak settings. Press " + resetSceneAction.GetBindingDisplayString() + " to reset scene.";
#else
            tweakText.text = "Press " + toggleKey.ToString() + " to tweak settings. Press " + resetSceneKey.ToString() + " to reset scene.";
#endif
#else
            Debug.LogWarning("GoldPlayerTweaker can't be used without UGUI!");
#endif
        }

#if NEW_INPUT
        private void OnEnable()
        {
            toggleAction.Enable();
            resetSceneAction.Enable();
        }

        private void OnDisable()
        {
            toggleAction.Disable();
            resetSceneAction.Disable();
        }
#endif

        private GoldPlayerTweakField targetZoom;
        private GoldPlayerTweakField zoomInTime;
        private GoldPlayerTweakField zoomOutTime;
        private GoldPlayerTweakField kickAmount;
        private GoldPlayerTweakField lerpTimeTo;
        private GoldPlayerTweakField lerpTimeFrom;
        private GoldPlayerTweakField maxStamina;
        private GoldPlayerTweakField drainRate;
        private GoldPlayerTweakField stillThreshold;
        private GoldPlayerTweakField regenRateStill;
        private GoldPlayerTweakField regenRateMoving;
        private GoldPlayerTweakField regenWait;
        private GoldPlayerTweakField jumpRequireStamina;
        private GoldPlayerTweakField jumpStaminaRequire;
        private GoldPlayerTweakField jumpStaminaCost;
        private GoldPlayerTweakField jumpHeight;
        private GoldPlayerTweakField airJump;
        private GoldPlayerTweakField airJumpTime;
        private GoldPlayerTweakField airJumpAmount;
        private GoldPlayerTweakField allowAirJumpDirectionChange;
        private GoldPlayerTweakField crouchJumping;
        private GoldPlayerTweakField crouchHeight;
        private GoldPlayerTweakField crouchTime;
        private GoldPlayerTweakField standUpTime;
        private GoldPlayerTweakField groundStick;
        private GoldPlayerTweakField bobFrequency;
        private GoldPlayerTweakField bobHeight;
        private GoldPlayerTweakField swayAngle;
        private GoldPlayerTweakField sideMovement;
        private GoldPlayerTweakField heightMultiplier;
        private GoldPlayerTweakField strideMultiplier;
        private GoldPlayerTweakField landMove;
        private GoldPlayerTweakField landTilt;
        private GoldPlayerTweakField enableStrafeTilt;
        private GoldPlayerTweakField strafeTilt;

        private void SetupUI()
        {
            CreateHeader("Game");
            CreateTweaker("Timescale", x => { Time.timeScale = x / 10f; }, Mathf.RoundToInt(Time.timeScale * 10), true, 0, 20, 10f);
            CreateTweaker("Target FPS", x => { Application.targetFrameRate = x; }, Application.targetFrameRate, true, 0, 165);
            CreateTweaker("V-Sync", x => { QualitySettings.vSyncCount = x ? 1 : 0; }, QualitySettings.vSyncCount == 1);
            CreateTweaker("Unscaled Movement", x => { targetPlayer.UnscaledTime = x; }, false);

            CreateTweaker("Movement Multiplier", x => { targetPlayer.Movement.MoveSpeedMultiplier = x; }, targetPlayer.Movement.MoveSpeedMultiplier);
            CreateTweaker("Jump Multiplier", x => { targetPlayer.Movement.JumpHeightMultiplier = x; }, targetPlayer.Movement.JumpHeightMultiplier);

            CreateHeader("Camera");
            CreateTweaker("Invert X Axis", x => { targetPlayer.Camera.InvertXAxis = x; }, targetPlayer.Camera.InvertXAxis);
            CreateTweaker("Invert Y Axis", x => { targetPlayer.Camera.InvertYAxis = x; }, targetPlayer.Camera.InvertYAxis);
            CreateTweaker("Mouse Sensitivity", x => { targetPlayer.Camera.MouseSensitivity = new Vector2(x, x); }, targetPlayer.Camera.MouseSensitivity.x);
            CreateTweaker("Mouse Damping", x => { targetPlayer.Camera.MouseDamping = x; }, targetPlayer.Camera.MouseDamping);
            CreateTweaker("Minimum X", x => { targetPlayer.Camera.MinimumX = x; }, targetPlayer.Camera.MinimumX);
            CreateTweaker("Maximum X", x => { targetPlayer.Camera.MaximumX = x; }, targetPlayer.Camera.MaximumX);

            CreateSubHeader("Zooming");
            CreateTweaker("Enable Zooming", x =>
            {
	            targetPlayer.Camera.EnableZooming = x;
	            targetZoom.SetInteractable(x);
	            zoomInTime.SetInteractable(x);
	            zoomOutTime.SetInteractable(x);
            }, targetPlayer.Camera.EnableZooming);

            targetZoom = CreateTweaker("Target Zoom", x => { targetPlayer.Camera.TargetZoom = x; }, targetPlayer.Camera.TargetZoom);
            zoomInTime = CreateTweaker("Zoom In Time", x => { targetPlayer.Camera.ZoomInTime = x; }, targetPlayer.Camera.ZoomInTime);
            zoomOutTime = CreateTweaker("Zoom Out Time", x => { targetPlayer.Camera.ZoomOutTime = x; }, targetPlayer.Camera.ZoomOutTime);
            
            CreateSubHeader("FOV Kick");
            CreateTweaker("Enable FOV Kick", x =>
            {
                targetPlayer.Camera.FieldOfViewKick.EnableFOVKick = x;
                kickAmount.SetInteractable(x);
                lerpTimeTo.SetInteractable(x);
                lerpTimeFrom.SetInteractable(x);
            }, targetPlayer.Camera.FieldOfViewKick.EnableFOVKick);
            kickAmount = CreateTweaker("Kick Amount", x => { targetPlayer.Camera.FieldOfViewKick.KickAmount = x; }, targetPlayer.Camera.FieldOfViewKick.KickAmount);
            lerpTimeTo = CreateTweaker("Lerp Time To", x => { targetPlayer.Camera.FieldOfViewKick.LerpTimeTo = x; }, targetPlayer.Camera.FieldOfViewKick.LerpTimeTo);
            lerpTimeFrom = CreateTweaker("Lerp Time From", x => { targetPlayer.Camera.FieldOfViewKick.LerpTimeFrom = x; }, targetPlayer.Camera.FieldOfViewKick.LerpTimeFrom);

            CreateHeader("Movement");
            CreateSubHeader("Running");
            CreateTweaker("Can Run", x => { targetPlayer.Movement.CanRun = x; ui.AdaptSprintingUI(); }, targetPlayer.Movement.CanRun);
            CreateSubHeader("Stamina");
            CreateTweaker("Enable Stamina", x =>
            {
                targetPlayer.Movement.Stamina.EnableStamina = x;
                maxStamina.SetInteractable(x);
                drainRate.SetInteractable(x);
                stillThreshold.SetInteractable(x);
                regenRateStill.SetInteractable(x);
                regenRateMoving.SetInteractable(x);
                regenWait.SetInteractable(x);
                ui.AdaptSprintingUI();

                jumpRequireStamina.SetInteractable(x && targetPlayer.Movement.CanJump);
                jumpStaminaRequire.SetInteractable(x && targetPlayer.Movement.CanJump);
                jumpStaminaCost.SetInteractable(x && targetPlayer.Movement.CanJump);
            }, targetPlayer.Movement.Stamina.EnableStamina);
            maxStamina = CreateTweaker("Max Stamina", x => { targetPlayer.Movement.Stamina.MaxStamina = x; }, targetPlayer.Movement.Stamina.MaxStamina);
            drainRate = CreateTweaker("Drain Rate", x => { targetPlayer.Movement.Stamina.DrainRate = x; }, targetPlayer.Movement.Stamina.DrainRate);
            stillThreshold = CreateTweaker("Still Threshold", x => { targetPlayer.Movement.Stamina.StillThreshold = x; }, targetPlayer.Movement.Stamina.StillThreshold);
            regenRateStill = CreateTweaker("Regen Rate Still", x => { targetPlayer.Movement.Stamina.RegenRateStill = x; }, targetPlayer.Movement.Stamina.RegenRateStill);
            regenRateMoving = CreateTweaker("Regen Rate Moving", x => { targetPlayer.Movement.Stamina.RegenRateMoving = x; }, targetPlayer.Movement.Stamina.RegenRateMoving);
            regenWait = CreateTweaker("Regen Wait", x => { targetPlayer.Movement.Stamina.RegenWait = x; }, targetPlayer.Movement.Stamina.RegenWait);

            CreateSubHeader("Jumping");
            CreateTweaker("Can Jump", x =>
            {
                targetPlayer.Movement.CanJump = x;
                jumpHeight.SetInteractable(x);
                jumpRequireStamina.SetInteractable(x && targetPlayer.Movement.Stamina.EnableStamina);
                jumpStaminaRequire.SetInteractable(x && targetPlayer.Movement.Stamina.EnableStamina);
                jumpStaminaCost.SetInteractable(x && targetPlayer.Movement.Stamina.EnableStamina);
                airJump.SetInteractable(x);
                airJumpTime.SetInteractable(x && targetPlayer.Movement.AirJump);
                airJumpAmount.SetInteractable(x && targetPlayer.Movement.AirJump);
                allowAirJumpDirectionChange.SetInteractable(x && targetPlayer.Movement.AirJump);
            }, targetPlayer.Movement.CanJump);
            jumpRequireStamina = CreateTweaker("Jumping Requires Stamina", x => { targetPlayer.Movement.JumpingRequiresStamina = x; }, targetPlayer.Movement.JumpingRequiresStamina);
            jumpStaminaRequire = CreateTweaker("Jump Stamina Require", x => { targetPlayer.Movement.JumpStaminaRequire = x; }, targetPlayer.Movement.JumpStaminaRequire);
            jumpStaminaCost = CreateTweaker("Jump Stamina Cost", x => { targetPlayer.Movement.JumpStaminaCost = x; }, targetPlayer.Movement.JumpStaminaCost);
            jumpHeight = CreateTweaker("Jump Height", x => { targetPlayer.Movement.JumpHeight = x; }, targetPlayer.Movement.JumpHeight);
            airJump = CreateTweaker("Air Jump", x =>
            {
                targetPlayer.Movement.AirJump = x;
                airJumpTime.SetInteractable(x && targetPlayer.Movement.CanJump);
                airJumpAmount.SetInteractable(x && targetPlayer.Movement.CanJump);
                allowAirJumpDirectionChange.SetInteractable(x && targetPlayer.Movement.CanJump);
            }, targetPlayer.Movement.AirJump);
            airJumpTime = CreateTweaker("Air Jump Time", x => { targetPlayer.Movement.AirJumpTime = x; }, targetPlayer.Movement.AirJumpTime);
            airJumpAmount = CreateTweaker("Air Jumps Amount", x => { targetPlayer.Movement.AirJumpsAmount = x; }, targetPlayer.Movement.AirJumpsAmount);
            allowAirJumpDirectionChange = CreateTweaker("Allow Air Jump Direction Change", x => { targetPlayer.Movement.AllowAirJumpDirectionChange = x; }, targetPlayer.Movement.AllowAirJumpDirectionChange);

            CreateSubHeader("Crouching");
            CreateTweaker("Can Crouch", x =>
            {
                targetPlayer.Movement.CanCrouch = x;
                crouchJumping.SetInteractable(x);
                crouchHeight.SetInteractable(x);
                crouchTime.SetInteractable(x);
                standUpTime.SetInteractable(x);
            }, targetPlayer.Movement.CanCrouch);
            crouchJumping = CreateTweaker("Crouch Jumping", x => { targetPlayer.Movement.CrouchJumping = x; }, targetPlayer.Movement.CrouchJumping);
            crouchHeight = CreateTweaker("Crouch Height", x => { targetPlayer.Movement.CrouchHeight = x; }, targetPlayer.Movement.CrouchHeight);
            crouchTime = CreateTweaker("Crouch Time", x => { targetPlayer.Movement.CrouchTime = x; }, targetPlayer.Movement.CrouchTime);
            standUpTime = CreateTweaker("Stand Up Time", x => { targetPlayer.Movement.StandUpTime = x; }, targetPlayer.Movement.StandUpTime);
            CreateSubHeader("Other");
            CreateTweaker("Acceleration", x => { targetPlayer.Movement.Acceleration = x; }, targetPlayer.Movement.Acceleration);
            CreateTweaker("Gravity", x => { targetPlayer.Movement.Gravity = x; }, targetPlayer.Movement.Gravity);
            CreateTweaker("Air Control", x => { targetPlayer.Movement.AirControl = x; }, targetPlayer.Movement.AirControl, true, 0, 1);
            CreateTweaker("Enable Ground Stick", x =>
            {
                targetPlayer.Movement.EnableGroundStick = x;
                groundStick.SetInteractable(x);
            }, targetPlayer.Movement.EnableGroundStick);
            groundStick = CreateTweaker("GroundStick", x => { targetPlayer.Movement.GroundStick = x; }, targetPlayer.Movement.GroundStick);

            CreateHeader("Head bob");
            CreateTweaker("Enable Bob", x =>
            {
                targetPlayer.HeadBob.EnableBob = x;
                bobFrequency.SetInteractable(x);
                bobHeight.SetInteractable(x);
                swayAngle.SetInteractable(x);
                sideMovement.SetInteractable(x);
                heightMultiplier.SetInteractable(x);
                strideMultiplier.SetInteractable(x);
                landMove.SetInteractable(x);
                landTilt.SetInteractable(x);
                enableStrafeTilt.SetInteractable(x);
                strafeTilt.SetInteractable(x && targetPlayer.HeadBob.EnableStrafeTilting);
            }, targetPlayer.HeadBob.EnableBob);
            bobFrequency = CreateTweaker("Bob Frequency", x => { targetPlayer.HeadBob.BobFrequency = x; }, targetPlayer.HeadBob.BobFrequency);
            bobHeight = CreateTweaker("Bob Height", x => { targetPlayer.HeadBob.BobHeight = x; }, targetPlayer.HeadBob.BobHeight);
            swayAngle = CreateTweaker("Sway Angle", x => { targetPlayer.HeadBob.SwayAngle = x; }, targetPlayer.HeadBob.SwayAngle);
            sideMovement = CreateTweaker("Side Movement", x => { targetPlayer.HeadBob.SideMovement = x; }, targetPlayer.HeadBob.SideMovement);
            heightMultiplier = CreateTweaker("Height Multiplier", x => { targetPlayer.HeadBob.HeightMultiplier = x; }, targetPlayer.HeadBob.HeightMultiplier);
            strideMultiplier = CreateTweaker("Stride Multiplier", x => { targetPlayer.HeadBob.StrideMultiplier = x; }, targetPlayer.HeadBob.StrideMultiplier);
            landMove = CreateTweaker("Land Move", x => { targetPlayer.HeadBob.LandMove = x; }, targetPlayer.HeadBob.LandMove);
            landTilt = CreateTweaker("Land Tilt", x => { targetPlayer.HeadBob.LandTilt = x; }, targetPlayer.HeadBob.LandTilt);
            enableStrafeTilt = CreateTweaker("Enable Strafe Tilt", x =>
            {
                targetPlayer.HeadBob.EnableStrafeTilting = x;
                strafeTilt.SetInteractable(x && targetPlayer.HeadBob.EnableBob);
            }, targetPlayer.HeadBob.EnableStrafeTilting);
            strafeTilt = CreateTweaker("Strafe Tilt", x => { targetPlayer.HeadBob.StrafeTilt = x; }, targetPlayer.HeadBob.StrafeTilt);
        }

        public void CreateHeader(string text)
        {
#if USE_GUI
            TextMeshProUGUI newText = Instantiate(headerLabel, headerLabel.transform.parent);
            newText.text = text;
            newText.gameObject.SetActive(true);
#endif
        }

        public void CreateSubHeader(string text)
        {
#if USE_GUI
            TextMeshProUGUI newText = Instantiate(headerLabel, headerLabel.transform.parent);
            newText.text = text;
            newText.fontStyle = FontStyles.Normal;
            newText.gameObject.SetActive(true);
#endif
        }

        public GoldPlayerTweakField CreateTweaker(string label, Action<bool> onChanged, bool defaultValue)
        {
            GoldPlayerTweakField newField = Instantiate(tweakField, tweakField.transform.parent);
            newField.SetupField(label, onChanged, defaultValue);

            return newField;
        }

        public GoldPlayerTweakField CreateTweaker(string label, Action<float> onChanged, float defaultValue, bool slider = false, float minSlider = 0, float maxSlider = 1, float labelDivide = 1f)
        {
            GoldPlayerTweakField newField = Instantiate(tweakField, tweakField.transform.parent);
            newField.SetupField(label, onChanged, defaultValue, slider, minSlider, maxSlider, labelDivide);

            return newField;
        }

        public GoldPlayerTweakField CreateTweaker(string label, Action<int> onChanged, int defaultValue, bool slider = false, int minSlider = 0, int maxSlider = 1, float labelDivide = 1f)
        {
            GoldPlayerTweakField newField = Instantiate(tweakField, tweakField.transform.parent);
            newField.SetupField(label, onChanged, defaultValue, slider, minSlider, maxSlider, labelDivide);

            return newField;
        }

#if USE_GUI
        // Update is called once per frame
        void Update()
        {
            if (!targetPlayer)
            {
                return;
            }

#if !NEW_INPUT
            if (Input.GetKeyDown(toggleKey))
            {
                SetShowing(!showing);
            }

            if (Input.GetKeyDown(resetSceneKey))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
#else
            if (toggleAction.triggered)
            {
                SetShowing(!showing);
            }

            if (resetSceneAction.triggered)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
#endif
        }
#endif

        public void SetShowing(bool toggle)
        {
#if USE_GUI
            if (toggle)
            {
                previousCanLook = TargetPlayer.Camera.CanLookAround;
                previousCanMove = TargetPlayer.Movement.CanMoveAround;
                previousLockCursor = TargetPlayer.Camera.ShouldLockCursor;
            }

            showing = toggle;
            TargetPlayer.Camera.CanLookAround = !toggle && previousCanLook;
            TargetPlayer.Movement.CanMoveAround = !toggle && previousCanMove;
            TargetPlayer.Camera.ShouldLockCursor = !toggle && previousLockCursor;
            TargetPlayer.Camera.LockCursor(!toggle);
            tweakText.gameObject.SetActive(!toggle);
            Panel.SetActive(showing);
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            GetStandardComponents();
        }

        private void Reset()
        {
            GetStandardComponents();
        }

        private void GetStandardComponents()
        {
            if (ui == null)
            {
                ui = GetComponent<GoldPlayerUI>();
            }
        }
#endif
    }
}
