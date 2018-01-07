using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Hertzole.GoldPlayer.Example
{
    public class GoldPlayerTweaker : MonoBehaviour
    {
        [SerializeField]
        private GoldPlayerController m_TargetPlayer;
        public GoldPlayerController TargetPlayer { get { return m_TargetPlayer; } set { m_TargetPlayer = value; } }
        [SerializeField]
        private Text m_TweakText;
        public Text TweakText { get { return m_TweakText; } set { m_TweakText = value; } }
        [SerializeField]
        private GameObject m_Panel;
        public GameObject Panel { get { return m_Panel; } set { m_Panel = value; } }
        [SerializeField]
        private Text m_HeaderLabel;
        public Text HeaderLabel { get { return m_HeaderLabel; } set { m_HeaderLabel = value; } }
        [SerializeField]
        private GoldPlayerTweakField m_TweakField;
        public GoldPlayerTweakField TweakField { get { return m_TweakField; } set { m_TweakField = value; } }
        [SerializeField]
        private KeyCode m_ToggleKey = KeyCode.F1;
        public KeyCode ToggleKey { get { return m_ToggleKey; } set { m_ToggleKey = value; } }

        private bool m_Showing = false;
        private bool m_PreviousCanLook = false;
        private bool m_PreviousCanMove = false;
        private bool m_PreviousLockCursor = false;

        // Use this for initialization
        void Start()
        {
            Panel.gameObject.SetActive(false);
            m_HeaderLabel.gameObject.SetActive(false);
            m_TweakField.gameObject.SetActive(false);
            m_TweakText.gameObject.SetActive(m_TargetPlayer != null);

            if (m_TargetPlayer)
            {
                m_PreviousCanLook = TargetPlayer.Camera.CanLookAround;
                m_PreviousCanMove = TargetPlayer.Movement.CanMoveAround;
                m_PreviousLockCursor = TargetPlayer.Camera.ShouldLockCursor;
                m_TweakText.text = "Press " + ToggleKey + " to tweak settings";

                SetupUI();
            }
        }

        private void SetupUI()
        {
            CreateHeader("Camera");
            CreateTweaker("Invert X Axis", m_TargetPlayer.Camera.GetType().GetProperty("InvertXAxis"), m_TargetPlayer.Camera);
            CreateTweaker("Invert Y Axis", m_TargetPlayer.Camera.GetType().GetProperty("InvertYAxis"), m_TargetPlayer.Camera);
            CreateTweaker("Mouse Sensitivity", m_TargetPlayer.Camera.GetType().GetProperty("MouseSensitivity"), m_TargetPlayer.Camera);
            CreateTweaker("Mouse Damping", m_TargetPlayer.Camera.GetType().GetProperty("MouseDamping"), m_TargetPlayer.Camera);
            CreateTweaker("Minimum X", m_TargetPlayer.Camera.GetType().GetProperty("MinimumX"), m_TargetPlayer.Camera);
            CreateTweaker("Maximum X", m_TargetPlayer.Camera.GetType().GetProperty("MaximumX"), m_TargetPlayer.Camera);
            CreateSubHeader("FOV Kick");
            CreateTweaker("Enable FOV Kick", m_TargetPlayer.Camera.FOVKick.GetType().GetProperty("EnableFOVKick"), m_TargetPlayer.Camera.FOVKick);
            CreateTweaker("Kick Amount", m_TargetPlayer.Camera.FOVKick.GetType().GetProperty("KickAmount"), m_TargetPlayer.Camera.FOVKick);
            CreateTweaker("Lerp Time To", m_TargetPlayer.Camera.FOVKick.GetType().GetProperty("LerpTimeTo"), m_TargetPlayer.Camera.FOVKick);
            CreateTweaker("Lerp Time From", m_TargetPlayer.Camera.FOVKick.GetType().GetProperty("LerpTimeFrom"), m_TargetPlayer.Camera.FOVKick);

            CreateHeader("Movement");
            CreateSubHeader("Running");
            CreateTweaker("Can Run", m_TargetPlayer.Movement.GetType().GetProperty("CanRun"), m_TargetPlayer.Movement);
            CreateSubHeader("Jumping");
            CreateTweaker("Can Jump", m_TargetPlayer.Movement.GetType().GetProperty("CanJump"), m_TargetPlayer.Movement);
            CreateTweaker("Jump Height", m_TargetPlayer.Movement.GetType().GetProperty("JumpHeight"), m_TargetPlayer.Movement);
            CreateTweaker("Air Jump", m_TargetPlayer.Movement.GetType().GetProperty("AirJump"), m_TargetPlayer.Movement);
            CreateTweaker("Air Jump Time", m_TargetPlayer.Movement.GetType().GetProperty("AirJumpTime"), m_TargetPlayer.Movement);
            CreateSubHeader("Crouching");
            CreateTweaker("Can Crouch", m_TargetPlayer.Movement.GetType().GetProperty("CanCrouch"), m_TargetPlayer.Movement);
            CreateTweaker("Crouch Jumping", m_TargetPlayer.Movement.GetType().GetProperty("CrouchJumping"), m_TargetPlayer.Movement);
            CreateTweaker("Crouch Height", m_TargetPlayer.Movement.GetType().GetProperty("CrouchHeight"), m_TargetPlayer.Movement);
            CreateTweaker("Crouch Head Lerp", m_TargetPlayer.Movement.GetType().GetProperty("CrouchHeadLerp"), m_TargetPlayer.Movement);
            CreateSubHeader("Other");
            CreateTweaker("Acceleration", m_TargetPlayer.Movement.GetType().GetProperty("Acceleration"), m_TargetPlayer.Movement);
            CreateTweaker("Gravity", m_TargetPlayer.Movement.GetType().GetProperty("Gravity"), m_TargetPlayer.Movement);
            CreateTweaker("Air Control", m_TargetPlayer.Movement.GetType().GetProperty("AirControl"), m_TargetPlayer.Movement, true, 0, 1);
            CreateTweaker("Enable Ground Stick", m_TargetPlayer.Movement.GetType().GetProperty("EnableGroundStick"), m_TargetPlayer.Movement);
            CreateTweaker("GroundStick", m_TargetPlayer.Movement.GetType().GetProperty("GroundStick"), m_TargetPlayer.Movement);

            CreateHeader("Head bob");
            CreateTweaker("Enable Bob", m_TargetPlayer.HeadBob.GetType().GetProperty("EnableBob"), m_TargetPlayer.HeadBob);
            CreateTweaker("Bob Frequency", m_TargetPlayer.HeadBob.GetType().GetProperty("BobFrequency"), m_TargetPlayer.HeadBob);
            CreateTweaker("Bob Height", m_TargetPlayer.HeadBob.GetType().GetProperty("BobHeight"), m_TargetPlayer.HeadBob);
            CreateTweaker("Sway Angle", m_TargetPlayer.HeadBob.GetType().GetProperty("SwayAngle"), m_TargetPlayer.HeadBob);
            CreateTweaker("Side Movement", m_TargetPlayer.HeadBob.GetType().GetProperty("SideMovement"), m_TargetPlayer.HeadBob);
            CreateTweaker("height Multiplier", m_TargetPlayer.HeadBob.GetType().GetProperty("HeightMultiplier"), m_TargetPlayer.HeadBob);
            CreateTweaker("Stride Multiplier", m_TargetPlayer.HeadBob.GetType().GetProperty("StrideMultiplier"), m_TargetPlayer.HeadBob);
            CreateTweaker("Land Move", m_TargetPlayer.HeadBob.GetType().GetProperty("LandMove"), m_TargetPlayer.HeadBob);
            CreateTweaker("Land Tilt", m_TargetPlayer.HeadBob.GetType().GetProperty("LandTilt"), m_TargetPlayer.HeadBob);
            CreateTweaker("Strafe Tilt", m_TargetPlayer.HeadBob.GetType().GetProperty("StrafeTilt"), m_TargetPlayer.HeadBob);
        }

        public void CreateHeader(string text)
        {
            Text newText = Instantiate(m_HeaderLabel, m_HeaderLabel.transform.parent);
            newText.text = text;
            newText.gameObject.SetActive(true);
        }

        public void CreateSubHeader(string text)
        {
            Text newText = Instantiate(m_HeaderLabel, m_HeaderLabel.transform.parent);
            newText.text = text;
            newText.fontStyle = FontStyle.Normal;
            newText.gameObject.SetActive(true);
        }

        public void CreateTweaker(string label, PropertyInfo info, object caller, bool slider = false, float minSliderNum = 0, float maxSliderNum = 1)
        {
            if (info == null)
            {
                Debug.LogError(label + ": No property with the name provided!");
                return;
            }

            GoldPlayerTweakField newField = Instantiate(m_TweakField, m_TweakField.transform.parent);
            newField.SetupField(label, info, caller, slider, minSliderNum, maxSliderNum);
        }

        // Update is called once per frame
        void Update()
        {
            if (!m_TargetPlayer)
                return;

            if (Input.GetKeyDown(ToggleKey))
            {
                SetShowing(!m_Showing);
            }
        }

        public void SetShowing(bool toggle)
        {
            if (toggle)
            {
                m_PreviousCanLook = TargetPlayer.Camera.CanLookAround;
                m_PreviousCanMove = TargetPlayer.Movement.CanMoveAround;
                m_PreviousLockCursor = TargetPlayer.Camera.ShouldLockCursor;
            }

            m_Showing = toggle;
            TargetPlayer.Camera.CanLookAround = toggle ? false : m_PreviousCanLook;
            TargetPlayer.Movement.CanMoveAround = toggle ? false : m_PreviousCanMove;
            TargetPlayer.Camera.ShouldLockCursor = toggle ? false : m_PreviousLockCursor;
            TargetPlayer.Camera.LockCursor(!toggle);
            m_TweakText.gameObject.SetActive(!toggle);
            Panel.SetActive(m_Showing);
        }
    }
}
