using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    [System.Serializable]
    public struct MovementSpeeds
    {
        [SerializeField]
        [Tooltip("The speed when moving forward.")]
        private float m_ForwardSpeed;
        public float ForwardSpeed { get { return m_ForwardSpeed; } }
        [SerializeField]
        [Tooltip("The speed when moving sideways.")]
        private float m_SidewaysSpeed;
        public float SidewaysSpeed { get { return m_SidewaysSpeed; } }
        [SerializeField]
        [Tooltip("The speed when moving backwards.")]
        private float m_BackwardsSpeed;
        public float BackwardsSpeed { get { return m_BackwardsSpeed; } }

        public MovementSpeeds(float forwardSpeed, float sidewaysSpeed, float backwardsSpeed)
        {
            m_ForwardSpeed = forwardSpeed;
            m_SidewaysSpeed = sidewaysSpeed;
            m_BackwardsSpeed = backwardsSpeed;
        }
    }

    [System.Serializable]
    public class PlayerMovement : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if the player can move at all.")]
        private bool m_CanMove;

        //////// WALKING
        [Header("Walking")]
        [SerializeField]
        [Tooltip("The movement speeds when walking.")]
        private MovementSpeeds m_WalkingSpeeds = new MovementSpeeds(4f, 3.5f, 2.5f);

        //////// RUNNING
        [Header("Running")]
        [SerializeField]
        [Tooltip("Determines if the player can run.")]
        private bool m_CanRun = true;
        [SerializeField]
        [Tooltip("The movement speeds when running.")]
        private MovementSpeeds m_RunSpeeds = new MovementSpeeds(7f, 5.5f, 5f);

        //////// JUMPING
        [Header("Jumping")]
        [SerializeField]
        [Tooltip("Determines if the player can jump.")]
        private bool m_CanJump = true;
        [SerializeField]
        [Tooltip("The height the player can jump in Unity units.")]
        private float m_JumpHeight = 2f;

        //////// CROUCHING
        [Header("Crouching")]
        [SerializeField]
        [Tooltip("Determines if the player can crouch.")]
        private bool m_CanCrouch = true;
        [SerializeField]
        [Tooltip("The movement speeds when crouching.")]
        private MovementSpeeds m_CrouchSpeeds;
        [SerializeField]
        [Tooltip("Determines if the player can jump while crouched.")]
        private bool m_CrouchJumping = false;
        [SerializeField]
        [Tooltip("The height of the character controller when crouched.")]
        private float m_CrouchHeight = 1f;
        [SerializeField]
        [Tooltip("How fast the lerp for the head is when crouching/standing up.")]
        private float m_CrouchHeadLerp = 10;

        //////// OTHER
        [Header("Other")]
        [SerializeField]
        [Tooltip("The layers the player will treat as ground. SHOULD NOT INCLUDE THE LAYER THE PLAYER IS ON!")]
        private LayerMask m_GroundLayer = -1;
        [SerializeField]
        [Tooltip("How long is takes for the player to reach top speed.")]
        private float m_Acceleration = 0.10f;
        [SerializeField]
        [Tooltip("Sets the gravity of the player.")]
        private float m_Gravity = 20;
        [SerializeField]
        [Tooltip("Determines if groundstick should be enabled. This would stop the player for bouncing down slopes.")]
        private bool m_EnableGroundStick = true;
        [SerializeField]
        [Tooltip("Sets how much the player will stick to the ground.")]
        private float m_GroundStick = 10;

        // The real calculated jump height.
        private float m_RealJumpHeight = 0;
        // The original character controller height.
        private float m_OriginalControllerHeight = 0;
        // Just used in Input smoothing.
        private float m_ForwardSpeedVelocity = 0;
        // Just used in Input smoothing.
        private float m_SidewaysSpeedVelocity = 0;

        // Is the player grounded?
        private bool m_IsGrounded = false;
        // Is the player moving at all?
        private bool m_IsMoving = false;
        // Is the player running?
        private bool m_IsRunning = false;
        // Is the player jumping?
        private bool m_IsJumping = false;
        // Is the player falling?
        private bool m_IsFalling = false;
        // Is the player crouching?
        private bool m_IsCrouching = false;

        // Input values for movement on the X and Z axis.
        private Vector2 m_MovementInput = Vector2.zero;

        // The original character controller center.
        private Vector3 m_OriginalControllerCenter = Vector3.zero;
        // The direction the player is moving in.
        private Vector3 m_MoveDirection = Vector3.zero;

        // The move speed that will be used when moving. Can be changed and it will be reflected in movement.
        private MovementSpeeds m_MoveSpeed = new MovementSpeeds();

        /// <summary> Determines if the player can move at all. </summary>
        public bool CanMove { get { return m_CanMove; } set { m_CanMove = value; } }

        /// <summary> The speeds when walking. </summary>
        public MovementSpeeds WalkingSpeeds { get { return m_WalkingSpeeds; } set { m_WalkingSpeeds = value; if (!m_IsRunning) m_MoveSpeed = value; } }

        /// <summary> Determines if the player can run. </summary>
        public bool CanRun { get { return m_CanRun; } set { m_CanRun = value; } }
        /// <summary> The speeds when running. </summary>
        public MovementSpeeds RunSpeeds { get { return m_RunSpeeds; } set { m_RunSpeeds = value; if (m_IsRunning) m_MoveSpeed = value; } }

        /// <summary> Determines if the player can jump. </summary>
        public bool CanJump { get { return m_CanJump; } set { m_CanJump = value; } }
        /// <summary> The height the player can jump in Unity units. </summary>
        public float JumpHeight { get { return m_JumpHeight; } set { m_JumpHeight = value; m_RealJumpHeight = CalculateJumpHeight(value); } }

        /// <summary> Determines if the player can crouch. </summary>
        public bool CanCrouch { get { return m_CanCrouch; } set { m_CanCrouch = value; } }
        /// <summary> The movement speeds when crouching. </summary>
        public MovementSpeeds CrouchSpeeds { get { return m_CrouchSpeeds; } set { m_CrouchSpeeds = value; } }
        /// <summary> Determines if the player can jump while crouched. </summary>
        public bool CrouchJumping { get { return m_CrouchJumping; } set { m_CrouchJumping = value; } }
        /// <summary> The height of the character controller when crouched. </summary>
        public float CrouchHeight { get { return m_CrouchHeight; } set { m_CrouchHeight = value; } }
        /// <summary> How fast the lerp for the head is when crouching/standing up. </summary>
        public float CrouchHeadLerp { get { return m_CrouchHeadLerp; } set { m_CrouchHeadLerp = value; } }

        /// <summary> The layers the player will treat as ground. SHOULD NOT INCLUDE THE LAYER THE PLAYER IS ON! </summary>
        public LayerMask GroundLayer { get { return m_GroundLayer; } set { m_GroundLayer = value; } }
        /// <summary> How long is takes for the player to reach top speed. </summary>
        public float Acceleration { get { return m_Acceleration; } set { m_Acceleration = value; } }
        /// <summary> Sets the gravity of the player. </summary>
        public float Gravity { get { return m_Gravity; } set { float v = value; if (v < 0) v = -v; m_Gravity = v; } }
        /// <summary> Determines if groundstick should be enabled. This would stop the player for bouncing down slopes. </summary>
        public bool EnableGroundStick { get { return m_EnableGroundStick; } set { m_EnableGroundStick = value; } }
        /// <summary> Sets how much the player will stick to the ground. </summary>
        public float GroundStick { get { return m_GroundStick; } set { float v = value; if (v < 0) v = -v; m_GroundStick = v; } }

        /// <summary> Is the player grounded? </summary>
        public bool IsGrounded { get { return m_IsGrounded; } }
        /// <summary> Is the player moving at all? </summary>
        public bool IsMoving { get { return m_IsMoving; } }
        /// <summary> Is the player running? </summary>
        public bool IsRunning { get { return m_IsRunning; } }
        /// <summary> Is the player jumping? </summary>
        public bool IsJumping { get { return m_IsJumping; } }
        /// <summary> Is the player falling? </summary>
        public bool IsFalling { get { return m_IsFalling; } }
        /// <summary> Is the player crouching? </summary>
        public bool IsCrouching { get { return m_IsCrouching; } }

        protected override void OnInit()
        {
            // Make the gravity + if needed.
            if (m_Gravity < 0)
                m_Gravity = -m_Gravity;
            // Make the ground stick + if needed.
            if (m_GroundStick < 0)
                m_GroundStick = -m_GroundStick;

            // Set the move to the walking speeds.
            m_MoveSpeed = m_WalkingSpeeds;
            // Calculate the real jump height.
            m_RealJumpHeight = CalculateJumpHeight(m_JumpHeight);
            // Set the original controller height.
            m_OriginalControllerHeight = PlayerController.Controller.height;
            // Set the original controller center.
            m_OriginalControllerCenter = PlayerController.Controller.center;
        }

        /// <summary>
        /// Calculates the real jump height.
        /// </summary>
        /// <param name="height">The height in Unity units.</param>
        /// <returns></returns>
        private float CalculateJumpHeight(float height)
        {
            return Mathf.Sqrt(2 * height * m_Gravity);
        }

        /// <summary>
        /// Updates the "isGrounded" value and also returns if the player is grounded.
        /// </summary>
        /// <returns>Is the player grounded?</returns>
        public bool CheckGrounded()
        {
            m_IsGrounded = Physics.CheckSphere(new Vector3(PlayerController.transform.position.x, PlayerController.transform.position.y + PlayerController.Controller.radius - 0.1f, PlayerController.transform.position.z), PlayerController.Controller.radius, m_GroundLayer, QueryTriggerInteraction.Ignore);
            return m_IsGrounded;
        }

        /// <summary>
        /// Updates the movement input values and also returns the current user input.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetInput()
        {
            m_MovementInput.x = Mathf.SmoothDamp(m_MovementInput.x, GetAxisRaw("Horizontal"), ref m_ForwardSpeedVelocity, m_Acceleration);
            m_MovementInput.y = Mathf.SmoothDamp(m_MovementInput.y, GetAxisRaw("Vertical"), ref m_SidewaysSpeedVelocity, m_Acceleration);

            if (m_MovementInput.sqrMagnitude > 1)
                m_MovementInput.Normalize();

            return m_MovementInput;
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public override void OnUpdate()
        {
            CheckGrounded();
            GetInput();

            if (!m_IsGrounded)
            {
                // Make sure the player can't walk around in the ceiling.
                if ((PlayerController.Controller.collisionFlags & CollisionFlags.Above) != 0)
                    m_MoveDirection.y = -1f;

                if (!m_IsFalling && !m_IsJumping)
                {
                    m_IsFalling = true;
                    m_MoveDirection.y = 0;
                }

                m_MoveDirection.y -= m_Gravity * Time.deltaTime;
            }
            else
            {
                m_IsFalling = false;
                m_IsJumping = false;

                if (m_EnableGroundStick && !m_IsJumping)
                    m_MoveDirection.y = -m_GroundStick;
                else
                    m_MoveDirection.y = 0;
            }

            //m_IsRunning = new Vector2(PlayerController.Controller.velocity.x, PlayerController.Controller.velocity.y).magnitude > 

            HandleMovementDirection();

            if (GetButtonDown("Jump", KeyCode.Space) && m_CanJump && m_IsGrounded && m_CanMove)
            {
                Jump();
            }

            PlayerController.Controller.Move(m_MoveDirection * Time.deltaTime);
        }

        protected virtual void HandleMovementDirection()
        {
            if (m_CanMove)
            {
                m_MoveDirection = PlayerController.transform.TransformDirection(new Vector3(m_MovementInput.x, m_MoveDirection.y, m_MovementInput.y));
                if (m_MovementInput.y > 0)
                    m_MoveDirection.z *= m_MoveSpeed.ForwardSpeed;
                else
                    m_MoveDirection.z *= m_MoveSpeed.BackwardsSpeed;

                m_MoveDirection.x *= m_MoveSpeed.SidewaysSpeed;
            }
            else
            {
                m_MoveDirection = Vector3.zero;
            }
        }

        protected virtual void Jump()
        {
            m_IsJumping = true;

            if (m_IsCrouching)
            {
                if (m_CrouchJumping)
                {
                    m_MoveDirection.y = m_RealJumpHeight;
                }
            }
            else
            {
                m_MoveDirection.y = m_RealJumpHeight;
            }
        }

#if UNITY_EDITOR
        public override void OnValidate()
        {
            WalkingSpeeds = m_WalkingSpeeds;
            RunSpeeds = m_RunSpeeds;
            JumpHeight = m_JumpHeight;
        }
#endif
    }
}
