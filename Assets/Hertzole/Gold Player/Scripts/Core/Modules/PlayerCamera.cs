using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    /// <summary>
    /// Used to move a player camera around.
    /// </summary>
    [System.Serializable]
    public class PlayerCamera : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if the player can look around.")]
        private bool m_CanLookAround = true;
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
        [Tooltip("Settings related to field of view kick.")]
        private FOVKickClass m_FOVKick = new FOVKickClass();

        [Space]

        [SerializeField]
        [Tooltip("The camera head that should be moved around.")]
        private Transform m_CameraHead = null;

        // The current input from the mouse.
        private Vector2 m_MouseInput = Vector2.zero;

        // Where the head should be looking.
        private Vector3 m_TargetHeadAngles = Vector3.zero;
        // Where the body should be looking.
        private Vector3 m_TargetBodyAngles = Vector3.zero;
        // Where the head should be looking, smoothed.
        private Vector3 m_FollowHeadAngles = Vector3.zero;
        // Where the body should be looking, smoothed.
        private Vector3 m_FollowBodyAngles = Vector3.zero;
        // The head smooth velocity.
        private Vector3 m_FollowHeadVelocity = Vector3.zero;
        // The body smooth velocity.
        private Vector3 m_FollowBodyVelocity = Vector3.zero;

        // The original head rotation.
        private Quaternion m_OriginalHeadRotation = Quaternion.identity;

        /// <summary> Determines if the player can look around. </summary>
        public bool CanLookAround { get { return m_CanLookAround; } set { m_CanLookAround = value; } }
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
        /// <summary> Settings related to field of view kick. </summary>
        public FOVKickClass FOVKick { get { return m_FOVKick; } set { m_FOVKick = value; } }
        /// <summary> The camera head that should be moved around. </summary>
        public Transform CameraHead { get { return m_CameraHead; } set { m_CameraHead = value; } }

        /// <summary> Where the head should be looking. </summary>
        public Vector3 TargetHeadAngles { get { return m_TargetHeadAngles; } }
        /// <summary> Where the body should be looking. </summary>
        public Vector3 TargetBodyAngles { get { return m_TargetBodyAngles; } }
        /// <summary> Where the head should be looking, smoothed. </summary>
        public Vector3 FollowHeadAngles { get { return m_FollowHeadAngles; } }
        /// <summary> Where the body should be looking, smoothed. </summary>
        public Vector3 FollowBodyAngles { get { return m_FollowBodyAngles; } }

        protected override void OnInit()
        {
            // If the camera head is null, complain.
            if (m_CameraHead == null)
            {
                Debug.LogError("'" + PlayerController.gameObject.name + "' needs to have Camera Head assigned in the Camera settings!");
                return;
            }

            // Lock the cursor, if it should.
            LockCursor(m_ShouldLockCursor);

            // Set the original head rotation to the one on the camera head.
            m_OriginalHeadRotation = m_CameraHead.localRotation;

            // Initialize the FOV kick module.
            FOVKick.Init(PlayerController, PlayerInput);
        }

        /// <summary>
        /// Locks/unlocks the cursor and makes it invisible/visible.
        /// </summary>
        /// <param name="lockCursor">Should the cursor be locked?</param>
        public void LockCursor(bool lockCursor)
        {
            // Set the cursor lock state to either locked or none, depennding on the lock cursor paramater.
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            // Hide/Show the cursor based on the lock cursor paramater.
            Cursor.visible = !lockCursor;
        }

        public override void OnUpdate()
        {
            MouseHandler();
            FOVKick.OnUpdate();
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
            if (!m_CanLookAround)
                return;

            // Make sure to lock the cursor when pressing the mouse button, but only if ShouldLockCursor is true.
            if (Input.GetMouseButtonDown(0) && m_ShouldLockCursor)
                LockCursor(m_ShouldLockCursor);

            // Set the input.
            m_MouseInput = new Vector2(m_InvertXAxis ? -GetAxis(GoldPlayerConstants.MOUSE_X) : GetAxis(GoldPlayerConstants.MOUSE_X), m_InvertYAxis ? -GetAxis(GoldPlayerConstants.MOUSE_Y) : GetAxis(GoldPlayerConstants.MOUSE_Y)) * m_MouseSensitivity;

            // Apply the input and mouse sensitivity.
            m_TargetHeadAngles.x += m_MouseInput.y * m_MouseSensitivity * Time.deltaTime;
            m_TargetBodyAngles.y += m_MouseInput.x * m_MouseSensitivity * Time.deltaTime;

            // Clamp the head angle.
            m_TargetHeadAngles.x = Mathf.Clamp(m_TargetHeadAngles.x, m_MinimumX, m_MaxiumumX);

            // Smooth the movement.
            m_FollowHeadAngles = Vector3.SmoothDamp(m_FollowHeadAngles, m_TargetHeadAngles, ref m_FollowHeadVelocity, m_MouseDamping);
            m_FollowBodyAngles = Vector3.SmoothDamp(m_FollowBodyAngles, m_TargetBodyAngles, ref m_FollowBodyVelocity, m_MouseDamping);

            // Set the rotation on the camera head and player.
            m_CameraHead.localRotation = m_OriginalHeadRotation * Quaternion.Euler(-m_FollowHeadAngles.x, m_CameraHead.rotation.y, m_CameraHead.rotation.z);
            PlayerTransform.rotation = PlayerTransform.rotation * Quaternion.Euler(-m_FollowBodyAngles.x, m_FollowBodyAngles.y, 0);

            // Reset the target body angles so we can set the transform rotation from other places.
            m_TargetBodyAngles = Vector3.zero;
        }

        public override void OnValidate()
        {
            m_FOVKick.OnValidate();
        }
    }
}
