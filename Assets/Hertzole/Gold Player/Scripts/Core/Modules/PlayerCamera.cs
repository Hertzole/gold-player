using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    //FIX: Transform rotation can not be set at runtime and it's reflected here.
    [System.Serializable]
    public class PlayerCamera : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if the player can look around.")]
        private bool m_CanLook = true;
        [SerializeField]
        [Tooltip("Determines if the cursor should be locked.")]
        private bool m_ShouldLockCursor = true;

        [Space]
        [SerializeField]
        [Tooltip("Determines if the X axis should be inverted.")]
        private bool m_InvertXAxis = false;
        [SerializeField]
        [Tooltip("Determines if the Y axis should be inverted.")]
        private bool m_InvertYAxis = false;

        [Space]

        [SerializeField]
        [Tooltip("How fast the camera head should move when looking around.")]
        private float m_MouseSensitivity = 10f;
        [SerializeField]
        [Tooltip("Sets how smooth the movement should be.")]
        private float m_MouseDamping = 0.05f;
        [SerializeField]
        [Tooltip("Sets how far down the player can look.")]
        private float m_MinimumX = -90f;
        [SerializeField]
        [Tooltip("Sets how far up the player can look.")]
        private float m_MaxiumumX = 90f;

        [Space]

        [SerializeField]
        [Tooltip("The camera head that should be moved around.")]
        private Transform m_CameraHead = null;

        private Vector2 m_MouseInput = Vector2.zero;

        private Vector3 m_TargetHeadAngles = Vector3.zero;
        private Vector3 m_TargetBodyAngles = Vector3.zero;
        private Vector3 m_FollowHeadAngles = Vector3.zero;
        private Vector3 m_FollowBodyAngles = Vector3.zero;
        private Vector3 m_FollowHeadVelocity = Vector3.zero;
        private Vector3 m_FollowBodyVelocity = Vector3.zero;

        private Quaternion m_OriginalPlayerRotation = Quaternion.identity;
        private Quaternion m_OriginalHeadRotation = Quaternion.identity;

        /// <summary> Determines if the player can look around. </summary>
        public bool CanLook { get { return m_CanLook; } set { m_CanLook = value; } }
        /// <summary> Determines if the cursor should be locked. </summary>
        public bool ShouldLockCursor { get { return m_ShouldLockCursor; } set { m_ShouldLockCursor = value; } }
        /// <summary> Determines if the X axis should be inverted. </summary>
        public bool InvertXAxis { get { return m_InvertXAxis; } set { m_InvertXAxis = value; } }
        /// <summary> Determines if the Y axis should be inverted. </summary>
        public bool InvertYAxis { get { return m_InvertYAxis; } set { m_InvertYAxis = value; } }
        /// <summary> How fast the camera head should move when looking around. </summary>
        public float MouseSensitivity { get { return m_MouseSensitivity; } set { m_MouseSensitivity = value; } }
        /// <summary> Sets how smooth the movement should be. </summary>
        public float MouseDamping { get { return m_MouseDamping; } set { m_MouseDamping = value; } }
        /// <summary> Sets how far down the player can look. </summary>
        public float MinimumX { get { return m_MinimumX; } set { m_MinimumX = value; } }
        /// <summary> Sets how far up the player can look. </summary>
        public float MaxiumumX { get { return m_MaxiumumX; } set { m_MaxiumumX = value; } }
        /// <summary> The camera head that should be moved around. </summary>
        public Transform CameraHead { get { return m_CameraHead; } set { m_CameraHead = value; } }

        protected override void OnInit()
        {
            if (m_CameraHead == null)
            {
                Debug.LogError("'" + PlayerController.gameObject.name + "' needs to have Camera Head assigned in the Camera settings!");
                return;
            }

            LockCursor(m_ShouldLockCursor);

            m_OriginalHeadRotation = m_CameraHead.localRotation;
            m_OriginalPlayerRotation = PlayerController.transform.rotation;
        }

        /// <summary>
        /// Locks/unlocks the cursor and makes it invisible/visible.
        /// </summary>
        /// <param name="lockCursor">Should the cursor be locked?</param>
        public void LockCursor(bool lockCursor)
        {
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !lockCursor;
        }

        public override void OnUpdate()
        {
            MouseHandler();
        }

        /// <summary>
        /// Does all the mouse work for looking around.
        /// </summary>
        protected virtual void MouseHandler()
        {
            // If the camera head field is null, stop here.
            if (m_CameraHead == null)
                return;

            // If we can't look around, stop here.
            if (!m_CanLook)
                return;

            // Make sure to lock the cursor when pressing the mouse button, but only if ShouldLockCursor is true.
            if (Input.GetMouseButtonDown(0) && m_ShouldLockCursor)
                LockCursor(m_ShouldLockCursor);

            // Set the input.
            m_MouseInput = new Vector2(m_InvertXAxis ? -GetAxis("Mouse X") : GetAxis("Mouse X"), m_InvertYAxis ? -GetAxis("Mouse Y") : GetAxis("Mouse Y")) * m_MouseSensitivity;

            // Apply the input and mouse sensitivity.
            m_TargetHeadAngles.x += m_MouseInput.y * m_MouseSensitivity * Time.deltaTime;
            m_TargetBodyAngles.y += m_MouseInput.x * m_MouseSensitivity * Time.deltaTime;

            // Clamp the head angle.
            m_TargetHeadAngles = ClampValues(m_TargetHeadAngles, m_FollowHeadAngles, m_MinimumX, m_MaxiumumX);

            // Smooth the movement.
            m_FollowHeadAngles = Vector3.SmoothDamp(m_FollowHeadAngles, m_TargetHeadAngles, ref m_FollowHeadVelocity, m_MouseDamping);
            m_FollowBodyAngles = Vector3.SmoothDamp(m_FollowBodyAngles, m_TargetBodyAngles, ref m_FollowBodyVelocity, m_MouseDamping);

            // Set the rotation on the camera head and player.
            m_CameraHead.localRotation = m_OriginalHeadRotation * Quaternion.Euler(-m_FollowHeadAngles.x, m_CameraHead.rotation.y, m_CameraHead.rotation.z);
            PlayerController.transform.rotation = m_OriginalPlayerRotation * Quaternion.Euler(-m_FollowBodyAngles.x, m_FollowBodyAngles.y, 0);
        }

        protected virtual Vector3 ClampValues(Vector3 target, Vector3 follow, float minimum, float maximum)
        {
            if (target.y > 180) { target.y -= 360; follow.y -= 360; }
            if (target.x > 180) { target.x -= 360; follow.x -= 360; }
            if (target.y < -180) { target.y += 360; follow.y += 360; }
            if (target.x < -180) { target.x += 360; follow.x += 360; }

            target.x = Mathf.Clamp(target.x, minimum, maximum);

            return target;
        }
    }
}
