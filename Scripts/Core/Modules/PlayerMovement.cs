using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    /// <summary>
    /// Used for movement related calculations.
    /// </summary>
    [System.Serializable]
    public class PlayerMovement : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if the player can move at all.")]
        private bool m_CanMoveAround = true;

        //////// WALKING
        [Header("Walking")]
        [SerializeField]
        [Tooltip("The movement speeds when walking.")]
        private MovementSpeeds m_WalkingSpeeds = new MovementSpeeds(3f, 2.5f, 2f);

        //////// RUNNING
        [Header("Running")]
        [SerializeField]
        [Tooltip("Determines if the player can run.")]
        private bool m_CanRun = true;
        [SerializeField]
        [Tooltip("The movement speeds when running.")]
        private MovementSpeeds m_RunSpeeds = new MovementSpeeds(7f, 5.5f, 5f);
        [SerializeField]
        [Tooltip("Everything related to stamina (limited running).")]
        private StaminaClass m_Stamina;

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
        public bool CanMoveAround { get { return m_CanMoveAround; } set { m_CanMoveAround = value; } }

        /// <summary> The speeds when walking. </summary>
        public MovementSpeeds WalkingSpeeds { get { return m_WalkingSpeeds; } set { m_WalkingSpeeds = value; if (!m_IsRunning) m_MoveSpeed = value; } }

        /// <summary> Determines if the player can run. </summary>
        public bool CanRun { get { return m_CanRun; } set { m_CanRun = value; } }
        /// <summary> The speeds when running. </summary>
        public MovementSpeeds RunSpeeds { get { return m_RunSpeeds; } set { m_RunSpeeds = value; if (m_IsRunning) m_MoveSpeed = value; } }
        /// <summary> Everything related to stamina (limited running). </summary>
        public StaminaClass Stamina { get { return m_Stamina; } set { m_Stamina = value; } }

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
        /// <summary> Is the player running? NOTE: This is true when move speed is above walk speed, not just when the run button is held down. </summary>
        public bool IsRunning { get { return m_IsRunning; } }
        /// <summary> Is the player jumping? </summary>
        public bool IsJumping { get { return m_IsJumping; } }
        /// <summary> Is the player falling? </summary>
        public bool IsFalling { get { return m_IsFalling; } }
        /// <summary> Is the player crouching? </summary>
        public bool IsCrouching { get { return m_IsCrouching; } }
        /// <summary> Can the player stand up while crouching </summary>
        public bool CanStandUp { get { return m_CanStandUp; } }

        protected override void OnInit()
        {
            // Initialize the stamina module.
            m_Stamina.Init(PlayerController, PlayerInput);

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
            m_OriginalControllerHeight = CharacterController.height;
            // Set the original controller center.
            m_OriginalControllerCenter = CharacterController.center;
            // Set the original camera position.
            m_OriginalCameraPosition = PlayerController.Camera.CameraHead.localPosition.y;
            // Calculate the crouch center for the character controller.
            m_ControllerCrouchCenter = CrouchHeight / 2;
            // Calculate the camera position for when crouching.
            m_CrouchCameraPosition = PlayerController.Camera.CameraHead.localPosition.y - m_CrouchHeight;
            // Set the current crouch camera position to the original camera position.
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
            // Check using a sphere at the player's feet.
            m_IsGrounded = Physics.CheckSphere(new Vector3(PlayerTransform.position.x, PlayerTransform.position.y + CharacterController.radius - 0.1f, PlayerTransform.position.z), CharacterController.radius, m_GroundLayer, QueryTriggerInteraction.Ignore);
            return m_IsGrounded;
        }

        /// <summary>
        /// Updates the movement input values and also returns the current user input.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetInput()
        {
            // Take the X input and smooth it.
            m_MovementInput.x = Mathf.SmoothDamp(m_MovementInput.x, GetAxisRaw(GoldPlayerConstants.HORIZONTAL_AXIS), ref m_ForwardSpeedVelocity, m_Acceleration);
            // Take the Y input and smooth it.
            m_MovementInput.y = Mathf.SmoothDamp(m_MovementInput.y, GetAxisRaw(GoldPlayerConstants.VERTICAL_AXIS), ref m_SidewaysSpeedVelocity, m_Acceleration);

            // Normalize the input so the player doesn't run faster when running diagonally.
            if (m_MovementInput.sqrMagnitude > 1)
                m_MovementInput.Normalize();

            // Return the input.
            return m_MovementInput;
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public override void OnUpdate()
        {
            // Call update on the stamina module.
            m_Stamina.OnUpdate();

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
            CharacterController.Move(m_MoveDirection * Time.deltaTime);
        }

        /// <summary>
        /// Handles the basic movement, like walking and jumping.
        /// </summary>
        protected virtual void BasicMovement()
        {
            // If the player isn't grounded, handle gravity.
            // Else apply the ground stick so the player sticks to the ground.
            if (!m_IsGrounded)
            {
                // If the player's head is touching the ceiling, move the player down so they don't
                // get stuck on the ceiling.
                if ((CharacterController.collisionFlags & CollisionFlags.Above) != 0)
                {
                    m_MoveDirection.y = -5f;
                    m_IsJumping = false;
                    m_IsFalling = true;
                }

                // If we're not grounded and not jumping, we are probably falling.
                // So set falling to true and reset the Y move direction.
                if (!m_IsFalling && !m_IsJumping)
                {
                    m_IsFalling = true;
                    m_MoveDirection.y = 0;
                }

                // Apply gravity to the Y axis.
                m_MoveDirection.y -= m_Gravity * Time.deltaTime;
            }
            else
            {
                // We're on the ground so we're not falling or jumping.
                m_IsFalling = false;
                m_IsJumping = false;

                // If ground stick is enabled and we're not jumping, apply the ground stick effect.
                if (m_EnableGroundStick && !m_IsJumping)
                    m_MoveDirection.y = -m_GroundStick;
                else
                    m_MoveDirection.y = 0;
            }

            // Make sure we're moving in the right direction.
            HandleMovementDirection();

            // Jump if the jump button is pressed, we can jump, we're grounded, and if we can move around.
            if (GetButtonDown(GoldPlayerConstants.JUMP_BUTTON_NAME, GoldPlayerConstants.JUMP_DEFAULT_KEY) && m_CanJump && m_IsGrounded && m_CanMoveAround)
            {
                Jump();
            }
        }

        /// <summary>
        /// Controls the movement direction and applying the correct speeds.
        /// </summary>
        protected virtual void HandleMovementDirection()
        {
            // Only run if we can move around.
            if (m_CanMoveAround)
            {
                // Get the move direction from the movement input X and Y (on the Z axis).
                m_MoveDirection = new Vector3(m_MovementInput.x, m_MoveDirection.y, m_MovementInput.y);
                // If movement input Y is above 0, we're moving forward, so apply forward move speed.
                // Else if below 0, we're moving backwards, so apply backwards move speed.
                if (m_MovementInput.y > 0)
                    m_MoveDirection.z *= m_MoveSpeed.ForwardSpeed;
                else
                    m_MoveDirection.z *= m_MoveSpeed.BackwardsSpeed;

                // Apply the sideways movement speed to the X movement.
                m_MoveDirection.x *= m_MoveSpeed.SidewaysSpeed;

                // Make sure we're moving in the direction the transform is facing.
                m_MoveDirection = PlayerTransform.TransformDirection(m_MoveDirection);
            }
            else
            {
                // If we can't move around, just return zero.
                m_MoveDirection = Vector3.zero;
            }
        }

        /// <summary>
        /// Makes the player jump.
        /// </summary>
        protected virtual void Jump()
        {
            // Set 'isJumping' to true to we tell everything we're jumping.
            m_IsJumping = true;

            // If we're crouching when trying to jump, check if we can jump while crouched.
            // If we're not crouching, just jump.
            if (m_IsCrouching)
            {
                // If crouch jumping is enabled, jump. Else do nothing.
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
            // Set 'isRunning' to true if the player velocity is above the walking speed max.
            m_IsRunning = new Vector2(CharacterController.velocity.x, CharacterController.velocity.z).magnitude > m_WalkingSpeeds.Max() + 0.5f;

            // Only run if we're not crouching, can run, and the run button is being held down.
            if (!m_IsCrouching && m_CanRun && GetButton(GoldPlayerConstants.RUN_BUTTON_NAME, GoldPlayerConstants.RUN_DEFAULT_KEY))
            {
                // If stamina is enabled, only set move speed when stamina is above 0.
                // Else if stamina is not enabled, simply set move speed to run speeds.
                // Else if stamina is enabled and current stamina is 0 (or less), set move speed to walking speed.
                if (m_Stamina.EnableStamina && m_Stamina.CurrentStamina > 0)
                    m_MoveSpeed = m_RunSpeeds;
                else if (!m_Stamina.EnableStamina)
                    m_MoveSpeed = m_RunSpeeds;
                else if (m_Stamina.EnableStamina && m_Stamina.CurrentStamina <= 0)
                    m_MoveSpeed = m_WalkingSpeeds;
            }
            else if (!m_IsCrouching && !GetButton(GoldPlayerConstants.RUN_BUTTON_NAME, GoldPlayerConstants.RUN_DEFAULT_KEY))
            {
                // If we're not crouching and not holding down the run button, walk.
                m_MoveSpeed = m_WalkingSpeeds;
            }
        }

        /// <summary>
        /// Handles crouching.
        /// </summary>
        protected virtual void Crouching()
        {
            // Only run the code if we can crouch. If we can't, always set 'isCrouching' to false.
            if (m_CanCrouch)
            {
                // If the crouch button is being held down, set is crouching to true.
                // Else if we can stand up and we are crouching, stop crouching.
                if (GetButton(GoldPlayerConstants.CROUCH_BUTTON_NAME, GoldPlayerConstants.CROUCH_DEFAULT_KEY))
                {
                    m_IsCrouching = true;
                }
                else if (m_CanStandUp && m_IsCrouching)
                {
                    // Set 'isCrouching' to false.
                    m_IsCrouching = false;
                    // Set the character controller height to the original height we got at the start.
                    CharacterController.height = m_OriginalControllerHeight;
                    // Set the character controller center to the origianl center we got at the start.
                    CharacterController.center = m_OriginalControllerCenter;

                    // Set the move speed to the walking speed.
                    m_MoveSpeed = m_WalkingSpeeds;
                }

                // Only run if we're crouching.
                if (m_IsCrouching)
                {
                    // Check if we can stand up and update the 'canStandUp' value.
                    m_CanStandUp = CheckCanStandUp();
                    // Set the character controller height to the crouch height.
                    CharacterController.height = m_CrouchHeight;
                    // Set the character controller center to the crouch center.
                    CharacterController.center = new Vector3(CharacterController.center.x, m_ControllerCrouchCenter, CharacterController.center.z);

                    // Set the move speed to the crouch speed.
                    m_MoveSpeed = m_CrouchSpeeds;
                }

                // Lerp the current crouch camera position to either the crouch camera position or the original camera position.
                m_CurrentCrouchCameraPosition = Mathf.Lerp(m_CurrentCrouchCameraPosition, m_IsCrouching ? m_CrouchCameraPosition : m_OriginalCameraPosition, m_CrouchHeadLerp * Time.deltaTime);
                // Set the camera head position to the current crouch camera position.
                PlayerController.Camera.CameraHead.localPosition = new Vector3(PlayerController.Camera.CameraHead.localPosition.x, m_CurrentCrouchCameraPosition, PlayerController.Camera.CameraHead.localPosition.z);
            }
            else
            {
                // We can't crouch, always set 'isCrouching' to false.
                m_IsCrouching = false;
            }
        }

        /// <summary>
        /// Checks if there's anything above the player that could stop the player from standing up.
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckCanStandUp()
        {
            // Check if we can stand up using a capsule from the player bottom to the player top.
            return !Physics.CheckCapsule(PlayerTransform.position + Vector3.up * CharacterController.radius, PlayerTransform.position + (Vector3.up * m_OriginalControllerHeight) - (Vector3.up * CharacterController.radius), CharacterController.radius, m_GroundLayer, QueryTriggerInteraction.Ignore);
        }

#if UNITY_EDITOR
        public override void OnValidate()
        {
            WalkingSpeeds = m_WalkingSpeeds;
            RunSpeeds = m_RunSpeeds;
            CrouchSpeeds = m_CrouchSpeeds;
            JumpHeight = m_JumpHeight;
        }
#endif
    }
}
