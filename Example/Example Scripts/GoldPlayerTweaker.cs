#if !UNITY_2019_2_OR_NEWER || (UNITY_2019_2_OR_NEWER && GOLD_PLAYER_UGUI)
#define USE_GUI
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
#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT && GOLD_PLAYER_NEW_INPUT
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

#if !ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
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

#if USE_GUI
        private bool showing = false;
        private bool previousCanLook = false;
        private bool previousCanMove = false;
        private bool previousLockCursor = false;
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

#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
            tweakText.text = "Press " + toggleAction.GetBindingDisplayString() + " to tweak settings. Press " + resetSceneAction.GetBindingDisplayString() + " to reset scene.";
#else
            tweakText.text = "Press " + toggleKey.ToString() + " to tweak settings. Press " + resetSceneKey.ToString() + " to reset scene.";
#endif
#else
            Debug.LogWarning("GoldPlayerTweaker can't be used without UGUI!");
#endif
        }

#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
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

        private void SetupUI()
        {
            CreateHeader("Game");
            CreateTweaker("Timescale", x => { Time.timeScale = x / 10f; }, 10, true, 1, 20, 10f);

            CreateHeader("Camera");
            CreateTweaker("Invert X Axis", x => { targetPlayer.Camera.InvertXAxis = x; }, targetPlayer.Camera.InvertXAxis);
            CreateTweaker("Invert Y Axis", x => { targetPlayer.Camera.InvertYAxis = x; }, targetPlayer.Camera.InvertYAxis);
            CreateTweaker("Mouse Sensitivity", x => { targetPlayer.Camera.MouseSensitivity = new Vector2(x, x); }, targetPlayer.Camera.MouseSensitivity.x);
            CreateTweaker("Mouse Damping", x => { targetPlayer.Camera.MouseDamping = x; }, targetPlayer.Camera.MouseDamping);
            CreateTweaker("Minimum X", x => { targetPlayer.Camera.MinimumX = x; }, targetPlayer.Camera.MinimumX);
            CreateTweaker("Maximum X", x => { targetPlayer.Camera.MaximumX = x; }, targetPlayer.Camera.MaximumX);
            CreateSubHeader("FOV Kick");
            CreateTweaker("Enable FOV Kick", x => { targetPlayer.Camera.FieldOfViewKick.EnableFOVKick = x; }, targetPlayer.Camera.FieldOfViewKick.EnableFOVKick);
            CreateTweaker("Kick Amount", x => { targetPlayer.Camera.FieldOfViewKick.KickAmount = x; }, targetPlayer.Camera.FieldOfViewKick.KickAmount);
            CreateTweaker("Lerp Time To", x => { targetPlayer.Camera.FieldOfViewKick.LerpTimeTo = x; }, targetPlayer.Camera.FieldOfViewKick.LerpTimeTo);
            CreateTweaker("Lerp Time From", x => { targetPlayer.Camera.FieldOfViewKick.LerpTimeFrom = x; }, targetPlayer.Camera.FieldOfViewKick.LerpTimeFrom);

            CreateHeader("Movement");
            CreateSubHeader("Running");
            CreateTweaker("Can Run", x => { targetPlayer.Movement.CanRun = x; }, targetPlayer.Movement.CanRun);
            CreateSubHeader("Jumping");
            CreateTweaker("Can Jump", x => { targetPlayer.Movement.CanJump = x; }, targetPlayer.Movement.CanJump);
            CreateTweaker("Jump Height", x => { targetPlayer.Movement.JumpHeight = x; }, targetPlayer.Movement.JumpHeight);
            CreateTweaker("Air Jump", x => { targetPlayer.Movement.AirJump = x; }, targetPlayer.Movement.AirJump);
            CreateTweaker("Air Jump Time", x => { targetPlayer.Movement.AirJumpTime = x; }, targetPlayer.Movement.AirJumpTime);
            CreateTweaker("Air Jumps Amount", x => { targetPlayer.Movement.AirJumpsAmount = x; }, targetPlayer.Movement.AirJumpsAmount);
            CreateTweaker("Allow Air Jump Direction Change", x => { targetPlayer.Movement.AllowAirJumpDirectionChange = x; }, targetPlayer.Movement.AllowAirJumpDirectionChange);
            CreateSubHeader("Crouching");
            CreateTweaker("Can Crouch", x => { targetPlayer.Movement.CanCrouch = x; }, targetPlayer.Movement.CanCrouch);
            CreateTweaker("Crouch Jumping", x => { targetPlayer.Movement.CrouchJumping = x; }, targetPlayer.Movement.CrouchJumping);
            CreateTweaker("Crouch Height", x => { targetPlayer.Movement.CrouchHeight = x; }, targetPlayer.Movement.CrouchHeight);
            CreateTweaker("Crouch Head Lerp", x => { targetPlayer.Movement.CrouchHeadLerp = x; }, targetPlayer.Movement.CrouchHeadLerp);
            CreateSubHeader("Other");
            CreateTweaker("Acceleration", x => { targetPlayer.Movement.Acceleration = x; }, targetPlayer.Movement.Acceleration);
            CreateTweaker("Gravity", x => { targetPlayer.Movement.Gravity = x; }, targetPlayer.Movement.Gravity);
            CreateTweaker("Air Control", x => { targetPlayer.Movement.AirControl = x; }, targetPlayer.Movement.AirControl, true, 0, 1);
            CreateTweaker("Enable Ground Stick", x => { targetPlayer.Movement.EnableGroundStick = x; }, targetPlayer.Movement.EnableGroundStick);
            CreateTweaker("GroundStick", x => { targetPlayer.Movement.GroundStick = x; }, targetPlayer.Movement.GroundStick);

            CreateHeader("Head bob");
            CreateTweaker("Enable Bob", x => { targetPlayer.HeadBob.EnableBob = x; }, targetPlayer.HeadBob.EnableBob);
            CreateTweaker("Bob Frequency", x => { targetPlayer.HeadBob.BobFrequency = x; }, targetPlayer.HeadBob.BobFrequency);
            CreateTweaker("Bob Height", x => { targetPlayer.HeadBob.BobHeight = x; }, targetPlayer.HeadBob.BobHeight);
            CreateTweaker("Sway Angle", x => { targetPlayer.HeadBob.SwayAngle = x; }, targetPlayer.HeadBob.SwayAngle);
            CreateTweaker("Side Movement", x => { targetPlayer.HeadBob.SideMovement = x; }, targetPlayer.HeadBob.SideMovement);
            CreateTweaker("height Multiplier", x => { targetPlayer.HeadBob.HeightMultiplier = x; }, targetPlayer.HeadBob.HeightMultiplier);
            CreateTweaker("Stride Multiplier", x => { targetPlayer.HeadBob.StrideMultiplier = x; }, targetPlayer.HeadBob.StrideMultiplier);
            CreateTweaker("Land Move", x => { targetPlayer.HeadBob.LandMove = x; }, targetPlayer.HeadBob.LandMove);
            CreateTweaker("Land Tilt", x => { targetPlayer.HeadBob.LandTilt = x; }, targetPlayer.HeadBob.LandTilt);
            CreateTweaker("Strafe Tilt", x => { targetPlayer.HeadBob.StrafeTilt = x; }, targetPlayer.HeadBob.StrafeTilt);
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

        public void CreateTweaker(string label, Action<bool> onChanged, bool defaultValue)
        {
            GoldPlayerTweakField newField = Instantiate(tweakField, tweakField.transform.parent);
            newField.SetupField(label, onChanged, defaultValue);
        }

        public void CreateTweaker(string label, Action<float> onChanged, float defaultValue, bool slider = false, float minSlider = 0, float maxSlider = 1, float labelDivide = 1f)
        {
            GoldPlayerTweakField newField = Instantiate(tweakField, tweakField.transform.parent);
            newField.SetupField(label, onChanged, defaultValue, slider, minSlider, maxSlider, labelDivide);
        }

        public void CreateTweaker(string label, Action<int> onChanged, int defaultValue, bool slider = false, int minSlider = 0, int maxSlider = 1, float labelDivide = 1f)
        {
            GoldPlayerTweakField newField = Instantiate(tweakField, tweakField.transform.parent);
            newField.SetupField(label, onChanged, defaultValue, slider, minSlider, maxSlider, labelDivide);
        }

#if USE_GUI
        // Update is called once per frame
        void Update()
        {
            if (!targetPlayer)
            {
                return;
            }

#if !ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
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
    }
}
