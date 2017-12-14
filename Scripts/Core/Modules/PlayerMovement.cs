using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
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
        private MovementSpeeds m_CrouchSpeeds = new MovementSpeeds(2f, 1.5f, 1f);
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
        protected float m_RealJumpHeight = 0;
        // The original character controller height.
        protected float m_OriginalControllerHeight = 0;
        // The original camera position.
        protected float m_OriginalCameraPosition = 0;
        // The character controller center set when the player is crouching.
        protected float m_ControllerCrouchCenter = 0;
        // The current camera position, related to crouching.
        protected float m_CurrentCrouchCameraPosition = 0;
        // The position to set the camera to when crouching.
        protected float m_CrouchCameraPosition = 0;
        // Just used in Input smoothing.
        protected float m_ForwardSpeedVelocity = 0;
        // Just used in Input smoothing.
        protected float m_SidewaysSpeedVelocity = 0;

        // Is the player grounded?
        protected bool m_IsGrounded = false;
        // Is the player moving at all?
        protected bool m_IsMoving = false;
        // Is the player running?
        protected bool m_IsRunning = false;
        // Is the player jumping?
        protected bool m_IsJumping = false;
        // Is the player falling?
        protected bool m_IsFalling = false;
        // Is the player crouching?
        protected bool m_IsCrouching = false;
        // Can the player stand up while crouching?
        protected bool m_CanStandUp = false;

        // Input values for movement on the X and Z axis.
        protected Vector2 m_MovementInput = Vector2.zero;

        // The original character controller center.
        protected Vector3 m_OriginalControllerCenter = Vector3.zero;
        // The direction the player is moving in.
        protected Vector3 m_MoveDirection = Vector3.zero;

        // The move speed that will be used when moving. Can be changed and it will be reflected in movement.
        protected MovementSpeeds m_MoveSpeed = new MovementSpeeds();

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
        /// <summary> Can the player stand up while crouching </summary>
        public bool CanStandUp { get { return m_CanStandUp; } }

        /// <summary> Used for moving left to right. </summary>
        public const string HORIZONTAL_AXIS = "Horizontal";
        /// <summary> Used for moving up and down. </summary>
        public const string VERTICAL_AXIS = "Vertical";

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
            //TODO: Fetch camera head in a more elegant way.
            m_OriginalCameraPosition = PlayerController.Camera.CameraHead.localPosition.y;
            m_ControllerCrouchCenter = CrouchHeight / 2;
            m_CrouchCameraPosition = PlayerController.Camera.CameraHead.localPosition.y - m_CrouchHeight;
            m_CurrentCrouchCameraPosition = m_OriginalCameraPosition;
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
            m_MovementInput.x = Mathf.SmoothDamp(m_MovementInput.x, GetAxisRaw(HORIZONTAL_AXIS), ref m_ForwardSpeedVelocity, m_Acceleration);
            m_MovementInput.y = Mathf.SmoothDamp(m_MovementInput.y, GetAxisRaw(VERTICAL_AXIS), ref m_SidewaysSpeedVelocity, m_Acceleration);

            if (m_MovementInput.sqrMagnitude > 1)
                m_MovementInput.Normalize();

            return m_MovementInput;
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public override void OnUpdate()
        {
            // Check the grounded state.
            CheckGrounded();
            // Update the input.
            GetInput();

            // Do movement.
            BasicMovement();
            // Do crouching.
            Crouching();
            // Do running.
            Running();

            // Move the player using the character controller.
            PlayerController.Controller.Move(m_MoveDirection * Time.deltaTime);
        }

        /// <summary>
        /// Handles the basic movement, like walking and jumping.
        /// </summary>
        protected virtual void BasicMovement()
        {
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

            HandleMovementDirection();

            if (GetButtonDown("Jump", KeyCode.Space) && m_CanJump && m_IsGrounded && m_CanMove)
            {
                Jump();
            }
        }

        /// <summary>
        /// Controls the movement direction and applying the correct speeds.
        /// </summary>
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

        /// <summary>
        /// Makes the player jump.
        /// </summary>
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

        /// <summary>
        /// Handles running.
        /// </summary>
        protected virtual void Running()
        {
            m_IsRunning = new Vector2(PlayerController.Controller.velocity.x, PlayerController.Controller.velocity.z).magnitude > m_RunSpeeds.Max() + 0.5f;

            if (!m_IsCrouching && m_CanRun && GetButton("Sprint", KeyCode.LeftShift))
            {
                m_MoveSpeed = m_RunSpeeds;
            }
            else
            {
                m_MoveSpeed = m_WalkingSpeeds;
            }
        }

        /// <summary>
        /// Handles crouching.
        /// </summary>
        protected virtual void Crouching()
        {
            if (m_CanCrouch)
            {
                if (GetButton("Crouch", KeyCode.C))
                {
                    m_IsCrouching = true;
                }
                else if (m_CanStandUp)
                {
                    m_CurrentCrouchCameraPosition = Mathf.Lerp(m_CurrentCrouchCameraPosition, m_OriginalCameraPosition, m_CrouchHeadLerp * Time.deltaTime);
                    m_IsCrouching = false;
                    PlayerController.Controller.height = m_OriginalControllerHeight;
                    PlayerController.Controller.center = m_OriginalControllerCenter;

                    m_MoveSpeed = m_WalkingSpeeds;
                }

                if (m_IsCrouching)
                {
                    m_CurrentCrouchCameraPosition = Mathf.Lerp(m_CurrentCrouchCameraPosition, m_CrouchCameraPosition, m_CrouchHeadLerp * Time.deltaTime);
                    m_CanStandUp = CheckCanStandUp(PlayerController.Controller.radius);
                    PlayerController.Controller.height = m_CrouchHeight;
                    PlayerController.Controller.center = new Vector3(PlayerController.Controller.center.x, m_ControllerCrouchCenter, PlayerController.Controller.center.z);

                    m_MoveSpeed = m_CrouchSpeeds;
                }

                //TODO: Fetch camera head in a more elegant way.
                PlayerController.Camera.CameraHead.localPosition = new Vector3(PlayerController.Camera.CameraHead.localPosition.x, m_CurrentCrouchCameraPosition, PlayerController.Camera.CameraHead.localPosition.z);
            }
            else
            {
                m_IsCrouching = false;
            }
        }

        /// <summary>
        /// Checks if there's anything above the player that could stop the player from standing up.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        protected virtual bool CheckCanStandUp(float radius)
        {
            return !Physics.CheckSphere(new Vector3(PlayerController.transform.position.x, PlayerController.transform.position.y + m_OriginalControllerHeight, PlayerController.transform.position.z), radius - 0.1f, m_GroundLayer);
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
