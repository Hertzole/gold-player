#if !UNITY_2019_2_OR_NEWER || (UNITY_2019_2_OR_NEWER && USE_UGUI)
#define USE_GUI
#endif

using System.Reflection;
using UnityEngine;
#if USE_GUI
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#endif

namespace Hertzole.GoldPlayer.Example
{
    [AddComponentMenu("Gold Player/Examples/Gold Player Tweaker", 21)]
    public class GoldPlayerTweaker : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("m_TargetPlayer")]
        private GoldPlayerController targetPlayer;
        public GoldPlayerController TargetPlayer { get { return targetPlayer; } set { targetPlayer = value; } }
#if USE_GUI
        [SerializeField]
        [FormerlySerializedAs("m_TweakText")]
        private Text tweakText;
        public Text TweakText { get { return tweakText; } set { tweakText = value; } }
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
        private Text headerLabel;
        public Text HeaderLabel { get { return headerLabel; } set { headerLabel = value; } }
#endif
        [SerializeField]
        [FormerlySerializedAs("m_TweakField")]
        private GoldPlayerTweakField tweakField;
        public GoldPlayerTweakField TweakField { get { return tweakField; } set { tweakField = value; } }
        [Space]
        [SerializeField]
        [FormerlySerializedAs("m_ToggleKey")]
        private KeyCode toggleKey = KeyCode.F1;
        public KeyCode ToggleKey { get { return toggleKey; } set { toggleKey = value; } }
        [SerializeField]
        [FormerlySerializedAs("m_ResetSceneKey")]
        private KeyCode resetSceneKey = KeyCode.F2;
        public KeyCode ResetSceneKey { get { return resetSceneKey; } set { resetSceneKey = value; } }

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
                tweakText.text = "Press " + ToggleKey + " to tweak settings";
                viewport.anchorMin = new Vector2(0, 0);
                viewport.anchorMax = new Vector2(1, 1);
                viewport.sizeDelta = new Vector2(0, 0);

                SetupUI();
            }
#else
            Debug.LogWarning("GoldPlayerTweaker can't be used without UGUI!");
#endif
        }

        private void SetupUI()
        {
            CreateHeader("Camera");
            CreateTweaker("Invert X Axis", targetPlayer.Camera.GetType().GetProperty("InvertXAxis"), targetPlayer.Camera);
            CreateTweaker("Invert Y Axis", targetPlayer.Camera.GetType().GetProperty("InvertYAxis"), targetPlayer.Camera);
            CreateTweaker("Mouse Sensitivity", targetPlayer.Camera.GetType().GetProperty("MouseSensitivity"), targetPlayer.Camera);
            CreateTweaker("Mouse Damping", targetPlayer.Camera.GetType().GetProperty("MouseDamping"), targetPlayer.Camera);
            CreateTweaker("Minimum X", targetPlayer.Camera.GetType().GetProperty("MinimumX"), targetPlayer.Camera);
            CreateTweaker("Maximum X", targetPlayer.Camera.GetType().GetProperty("MaximumX"), targetPlayer.Camera);
            CreateSubHeader("FOV Kick");
            CreateTweaker("Enable FOV Kick", targetPlayer.Camera.FieldOfViewKick.GetType().GetProperty("EnableFOVKick"), targetPlayer.Camera.FieldOfViewKick);
            CreateTweaker("Kick Amount", targetPlayer.Camera.FieldOfViewKick.GetType().GetProperty("KickAmount"), targetPlayer.Camera.FieldOfViewKick);
            CreateTweaker("Lerp Time To", targetPlayer.Camera.FieldOfViewKick.GetType().GetProperty("LerpTimeTo"), targetPlayer.Camera.FieldOfViewKick);
            CreateTweaker("Lerp Time From", targetPlayer.Camera.FieldOfViewKick.GetType().GetProperty("LerpTimeFrom"), targetPlayer.Camera.FieldOfViewKick);

            CreateHeader("Movement");
            CreateSubHeader("Running");
            CreateTweaker("Can Run", targetPlayer.Movement.GetType().GetProperty("CanRun"), targetPlayer.Movement);
            CreateSubHeader("Jumping");
            CreateTweaker("Can Jump", targetPlayer.Movement.GetType().GetProperty("CanJump"), targetPlayer.Movement);
            CreateTweaker("Jump Height", targetPlayer.Movement.GetType().GetProperty("JumpHeight"), targetPlayer.Movement);
            CreateTweaker("Air Jump", targetPlayer.Movement.GetType().GetProperty("AirJump"), targetPlayer.Movement);
            CreateTweaker("Air Jump Time", targetPlayer.Movement.GetType().GetProperty("AirJumpTime"), targetPlayer.Movement);
            CreateTweaker("Air Jumps Amount", targetPlayer.Movement.GetType().GetProperty("AirJumpsAmount"), targetPlayer.Movement);
            CreateTweaker("Allow Air Jump Direction Change", targetPlayer.Movement.GetType().GetProperty("AllowAirJumpDirectionChange"), targetPlayer.Movement);
            CreateSubHeader("Crouching");
            CreateTweaker("Can Crouch", targetPlayer.Movement.GetType().GetProperty("CanCrouch"), targetPlayer.Movement);
            CreateTweaker("Crouch Jumping", targetPlayer.Movement.GetType().GetProperty("CrouchJumping"), targetPlayer.Movement);
            CreateTweaker("Crouch Height", targetPlayer.Movement.GetType().GetProperty("CrouchHeight"), targetPlayer.Movement);
            CreateTweaker("Crouch Head Lerp", targetPlayer.Movement.GetType().GetProperty("CrouchHeadLerp"), targetPlayer.Movement);
            CreateSubHeader("Other");
            CreateTweaker("Acceleration", targetPlayer.Movement.GetType().GetProperty("Acceleration"), targetPlayer.Movement);
            CreateTweaker("Gravity", targetPlayer.Movement.GetType().GetProperty("Gravity"), targetPlayer.Movement);
            CreateTweaker("Air Control", targetPlayer.Movement.GetType().GetProperty("AirControl"), targetPlayer.Movement, true, 0, 1);
            CreateTweaker("Enable Ground Stick", targetPlayer.Movement.GetType().GetProperty("EnableGroundStick"), targetPlayer.Movement);
            CreateTweaker("GroundStick", targetPlayer.Movement.GetType().GetProperty("GroundStick"), targetPlayer.Movement);

            CreateHeader("Head bob");
            CreateTweaker("Enable Bob", targetPlayer.HeadBob.GetType().GetProperty("EnableBob"), targetPlayer.HeadBob);
            CreateTweaker("Bob Frequency", targetPlayer.HeadBob.GetType().GetProperty("BobFrequency"), targetPlayer.HeadBob);
            CreateTweaker("Bob Height", targetPlayer.HeadBob.GetType().GetProperty("BobHeight"), targetPlayer.HeadBob);
            CreateTweaker("Sway Angle", targetPlayer.HeadBob.GetType().GetProperty("SwayAngle"), targetPlayer.HeadBob);
            CreateTweaker("Side Movement", targetPlayer.HeadBob.GetType().GetProperty("SideMovement"), targetPlayer.HeadBob);
            CreateTweaker("height Multiplier", targetPlayer.HeadBob.GetType().GetProperty("HeightMultiplier"), targetPlayer.HeadBob);
            CreateTweaker("Stride Multiplier", targetPlayer.HeadBob.GetType().GetProperty("StrideMultiplier"), targetPlayer.HeadBob);
            CreateTweaker("Land Move", targetPlayer.HeadBob.GetType().GetProperty("LandMove"), targetPlayer.HeadBob);
            CreateTweaker("Land Tilt", targetPlayer.HeadBob.GetType().GetProperty("LandTilt"), targetPlayer.HeadBob);
            CreateTweaker("Strafe Tilt", targetPlayer.HeadBob.GetType().GetProperty("StrafeTilt"), targetPlayer.HeadBob);
        }

        public void CreateHeader(string text)
        {
#if USE_GUI
            Text newText = Instantiate(headerLabel, headerLabel.transform.parent);
            newText.text = text;
            newText.gameObject.SetActive(true);
#endif
        }

        public void CreateSubHeader(string text)
        {
#if USE_GUI
            Text newText = Instantiate(headerLabel, headerLabel.transform.parent);
            newText.text = text;
            newText.fontStyle = FontStyle.Normal;
            newText.gameObject.SetActive(true);
#endif
        }

        public void CreateTweaker(string label, PropertyInfo info, object caller, bool slider = false, float minSliderNum = 0, float maxSliderNum = 1)
        {
            if (info == null)
            {
                Debug.LogError(label + ": No property with the name provided!");
                return;
            }

            GoldPlayerTweakField newField = Instantiate(tweakField, tweakField.transform.parent);
            newField.SetupField(label, info, caller, slider, minSliderNum, maxSliderNum);
        }

#if USE_GUI
        // Update is called once per frame
        void Update()
        {
            if (!targetPlayer)
                return;

            if (Input.GetKeyDown(toggleKey))
            {
                SetShowing(!showing);
            }

            if (Input.GetKeyDown(resetSceneKey))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
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
            TargetPlayer.Camera.CanLookAround = toggle ? false : previousCanLook;
            TargetPlayer.Movement.CanMoveAround = toggle ? false : previousCanMove;
            TargetPlayer.Camera.ShouldLockCursor = toggle ? false : previousLockCursor;
            TargetPlayer.Camera.LockCursor(!toggle);
            tweakText.gameObject.SetActive(!toggle);
            Panel.SetActive(showing);
#endif
        }
    }
}
