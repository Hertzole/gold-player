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
        [Tooltip("Configuration of running as a toggle.")]
        private RunToggleMode m_RunToggleMode = RunToggleMode.Off;
        [SerializeField]
        [Tooltip("The movement speeds when running.")]
        private MovementSpeeds m_RunSpeeds = new MovementSpeeds(7f, 5.5f, 5f);
        [SerializeField]
        [Tooltip("Everything related to stamina (limited running).")]
        private StaminaClass m_Stamina = new StaminaClass();

        //////// JUMPING
        [Header("Jumping")]
        [SerializeField]
        [Tooltip("Determines if the player can jump.")]
        private bool m_CanJump = true;
        [SerializeField]
        [Tooltip("The height the player can jump in Unity units.")]
        private float m_JumpHeight = 2f;
        [SerializeField]
        [Tooltip("Determines if the player can jump for some time when falling.")]
        private bool m_AirJump = true;
        [SerializeField]
        [Tooltip("How long the player can be in the air and still jump.")]
        private float m_AirJumpTime = 0.1f;
        [SerializeField]
        [Tooltip("How many times the player can jump while in the air.")]
        private int m_AirJumpsAmount = 0;
        [SerializeField]
        [Tooltip("If true, the player can change direction when air jumping.")]
        private bool m_AllowAirJumpDirectionChange = true;

        //////// CROUCHING
        [Header("Crouching")]
        [SerializeField]
        [Tooltip("Determines if the player can crouch.")]
        private bool m_CanCrouch = true;
        [SerializeField]
        [Tooltip("Configuration of crouching as a toggle.")]
        private CrouchToggleMode m_CrouchToggleMode = CrouchToggleMode.Off;
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
        [Range(0f, 1f)]
        [Tooltip("How much control the player will have in the air.")]
        private float m_AirControl = 0.5f;
        [SerializeField]
        [Tooltip("Determines if ground stick should be enabled. This would stop the player for bouncing down slopes.")]
        private bool m_EnableGroundStick = true;
        [SerializeField]
        [Tooltip("Sets how much the player will stick to the ground.")]
        private float m_GroundStick = 10;
        [SerializeField]
        [Tooltip("Everything related to moving platforms.")]
        private MovingPlatformsClass m_MovingPlatforms = new MovingPlatformsClass();

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
        // The current air time of the player.
        protected float m_CurrentAirTime = 0;

        // The current amount of times an air jump has been performed.
        protected int m_CurrentJumps = 0;

        // Is the player grounded?
        protected bool m_IsGrounded = false;
        // Is the player moving at all?
        protected bool m_IsMoving = false;
        // Does the player want to be running?
        protected bool m_ShouldRun = false;
        // Is the player running?
        protected bool m_IsRunning = false;
        // Did the player run at all since their last break in move input?
        private bool m_DidRunSinceLastBreakInMovement;
        // Is the player jumping?
        protected bool m_IsJumping = false;
        // Is the player falling?
        protected bool m_IsFalling = false;
        // Does the player want to be crouching?
        protected bool m_ShouldCrouch = false;
        // Is the player crouching?
        protected bool m_IsCrouching = false;
        // Can the player stand up while crouching?
        protected bool m_CanStandUp = false;
        // Was the player previously grounded?
        protected bool m_PreviouslyGrounded = false;
        // Was the player previously crouched?
        protected bool m_PreviouslyCrouched = false;
        // Was the player previously running?
        protected bool m_PreviouslyRunning = false;
        // Determines if the player should jump.
        protected bool m_ShouldJump = false;

        // Input values for movement on the X and Z axis, automatically dampened for smoothing.
        protected Vector2 m_MovementInput = Vector2.zero;
        // Whether or not the player registered movement input this frame. This can be false while
        // m_MovementInput is non-zero due to the smoothing applied to m_MovementInput.
        protected bool m_HasUserInput = false;

        // The original character controller center.
        protected Vector3 m_OriginalControllerCenter = Vector3.zero;
        // The direction the player is moving in.
        protected Vector3 m_MoveDirection = Vector3.zero;
        // The current ground velocity.
        protected Vector3 m_GroundVelocity = Vector3.zero;
        // The velocity while the player is in the air.
        protected Vector3 m_AirVelocity = Vector3.zero;
        // The position the player was at when jumping.
        protected Vector3 m_JumpPosition = Vector3.zero;
        // The impact of the applied force.
        protected Vector3 m_ForceImpact = Vector3.zero;

        // The move speed that will be used when moving. Can be changed and it will be reflected in movement.
        protected MovementSpeeds m_MoveSpeed = new MovementSpeeds();

        /// <summary> Determines if the player can move at all. </summary>
        public bool CanMoveAround { get { return m_CanMoveAround; } set { m_CanMoveAround = value; } }

        /// <summary> The speeds when walking. </summary>
        public MovementSpeeds WalkingSpeeds { get { return m_WalkingSpeeds; } set { m_WalkingSpeeds = value; if (!m_IsRunning) m_MoveSpeed = value; } }

        /// <summary> Determines if the player can run. </summary>
        public bool CanRun { get { return m_CanRun; } set { m_CanRun = value; } }
        /// <summary> Configuration of running as a toggle. </summary>
        public RunToggleMode RunToggleMode { get { return m_RunToggleMode; } set { m_RunToggleMode = value; } }
        /// <summary> The speeds when running. </summary>
        public MovementSpeeds RunSpeeds { get { return m_RunSpeeds; } set { m_RunSpeeds = value; if (m_IsRunning) m_MoveSpeed = value; } }
        /// <summary> Everything related to stamina (limited running). </summary>
        public StaminaClass Stamina { get { return m_Stamina; } set { m_Stamina = value; } }

        /// <summary> Determines if the player can jump. </summary>
        public bool CanJump { get { return m_CanJump; } set { m_CanJump = value; } }
        /// <summary> The height the player can jump in Unity units. </summary>
        public float JumpHeight { get { return m_JumpHeight; } set { m_JumpHeight = value; m_RealJumpHeight = CalculateJumpHeight(value); } }
        /// <summary> Determines if the player can jump for some time when falling. </summary>
        public bool AirJump { get { return m_AirJump; } set { m_AirJump = value; } }
        /// <summary> How long the player can be in the air and still jump. </summary>
        public float AirJumpTime { get { return m_AirJumpTime; } set { m_AirJumpTime = value; } }
        /// <summary> How many times the player can jump while in the air. </summary>
        public int AirJumpsAmount { get { return m_AirJumpsAmount; } set { m_AirJumpsAmount = value; } }
        /// <summary> If true, the player can change direction when air jumping. </summary>
        public bool AllowAirJumpDirectionChange { get { return m_AllowAirJumpDirectionChange; } set { m_AllowAirJumpDirectionChange = value; } }

        /// <summary> Determines if the player can crouch. </summary>
        public bool CanCrouch { get { return m_CanCrouch; } set { m_CanCrouch = value; } }
        /// <summary> Configuration of crouching as a toggle. </summary>
        public CrouchToggleMode CrouchToggleMode { get { return m_CrouchToggleMode; } set { m_CrouchToggleMode = value; } }
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
        /// <summary> How much control the player will have in the air. </summary>
        public float AirControl { get { return m_AirControl; } set { m_AirControl = value > 1 ? 1 : value < 0 ? 0 : value; } }
        /// <summary> Determines if ground stick should be enabled. This would stop the player for bouncing down slopes. </summary>
        public bool EnableGroundStick { get { return m_EnableGroundStick; } set { m_EnableGroundStick = value; } }
        /// <summary> Sets how much the player will stick to the ground. </summary>
        public float GroundStick { get { return m_GroundStick; } set { float v = value; if (v < 0) v = -v; m_GroundStick = v; } }
        /// <summary> The type of ground check method. </summary>
        [System.Obsolete("No longer used.")]
        public GroundCheckType GroundCheckType { get { return GroundCheckType.Sphere; } set { } }
        /// <summary> Everything related to moving platforms. </summary>
        public MovingPlatformsClass MovingPlatforms { get { return m_MovingPlatforms; } set { m_MovingPlatforms = value; } }

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
        /// <summary> Can the player stand up while crouching? </summary>
        public bool CanStandUp { get { return m_CanStandUp; } }

        /// <summary> Fires when the player jumps. </summary>
        public event GoldPlayerDelegates.JumpEvent OnJump;
        /// <summary> Fires when the player lands. </summary>
        public event GoldPlayerDelegates.LandEvent OnLand;
        /// <summary> Fires when the player begins crouching. </summary>
        public event GoldPlayerDelegates.PlayerEvent OnBeginCrouch;
        /// <summary> Fires when the player stops crouching. </summary>
        public event GoldPlayerDelegates.PlayerEvent OnEndCrouch;
        /// <summary> Fires when the player begins running. </summary>
        public event GoldPlayerDelegates.PlayerEvent OnBeginRun;
        /// <summary> Fires when the player stops running. </summary>
        public event GoldPlayerDelegates.PlayerEvent OnEndRun;

        protected override void OnInitialize()
        {
            // Initialize the stamina module.
            m_Stamina.Initialize(PlayerController, PlayerInput);
            // Initialize the moving platforms module.
            m_MovingPlatforms.Initialize(PlayerController, PlayerInput);

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
            m_CrouchCameraPosition = PlayerController.Camera.CameraHead.localPosition.y - (CharacterController.height - m_CrouchHeight);
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
            return Physics.CheckSphere(new Vector3(PlayerTransform.position.x, PlayerTransform.position.y + CharacterController.radius - 0.1f, PlayerTransform.position.z), CharacterController.radius, m_GroundLayer, QueryTriggerInteraction.Ignore); ;
        }

        /// <summary>
        /// Updates the movement input values and also returns the current user input.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetInput()
        {
            float horizontal = GetAxisRaw(GoldPlayerConstants.HORIZONTAL_AXIS);
            float vertical = GetAxisRaw(GoldPlayerConstants.VERTICAL_AXIS);

            m_HasUserInput = horizontal != 0 || vertical != 0;

            if (!m_HasUserInput)
                m_DidRunSinceLastBreakInMovement = false;

            // Take the X input and smooth it.
            m_MovementInput.x = Mathf.SmoothDamp(m_MovementInput.x, horizontal, ref m_ForwardSpeedVelocity, m_Acceleration);
            // Take the Y input and smooth it.
            m_MovementInput.y = Mathf.SmoothDamp(m_MovementInput.y, vertical, ref m_SidewaysSpeedVelocity, m_Acceleration);

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
            // Call update on the moving platforms module.
            m_MovingPlatforms.OnUpdate();

            // Check the grounded state.
            m_IsGrounded = CheckGrounded();
            // Update the input.
            GetInput();

            // Do movement.
            BasicMovement();
            // Do crouching.
            Crouching();
            // Do running.
            Running();
            // Do force update.
            ForceUpdate();

            // Move the player using the character controller.
            CharacterController.Move(m_MoveDirection * Time.deltaTime);
        }

        /// <summary>
        /// Handles the basic movement, like walking and jumping.
        /// </summary>
        protected virtual void BasicMovement()
        {
            // Only run if air jump is enabled.
            if (m_AirJump)
            {
                // If the current air time is above 0, decrease it.
                if (m_CurrentAirTime > 0)
                    m_CurrentAirTime -= Time.deltaTime;

                // If current air time is less than or equal to 0, the player should no longer jump.
                if (m_CurrentAirTime <= 0)
                    m_ShouldJump = false;
            }

            // If the player isn't grounded, handle gravity.
            // Else apply the ground stick so the player sticks to the ground.
            if (!m_IsGrounded)
            {
                // If the player was just grounded, set the jump position.
                // The player probably just jumped or started falling.
                if (m_PreviouslyGrounded)
                {
                    // Set the jump position to the current player transform.
                    m_JumpPosition = PlayerTransform.position;
                    //
                    if (!m_IsJumping)
                        m_CurrentJumps++;
                }

                // If the player's head is touching the ceiling, move the player down so they don't
                // get stuck on the ceiling.
                if ((CharacterController.collisionFlags & CollisionFlags.Above) != 0)
                {
                    m_MoveDirection.y = -5f;
                    m_IsJumping = false;
                    m_IsFalling = true;
                }

                // The player was previously not grounded.
                m_PreviouslyGrounded = false;

                // If the player isn't grounded and not jumping, it is probably falling.
                // So set falling to true and reset the Y move direction.
                if (!m_IsFalling && !m_IsJumping)
                {
                    m_CurrentAirTime = m_AirJumpTime;
                    m_IsFalling = true;
                    m_MoveDirection.y = 0;
                }

                // Apply gravity to the Y axis.
                m_MoveDirection.y -= m_Gravity * Time.deltaTime;
            }
            else
            {
                // If the player is grounded now and wasn't previously, the player just landed.
                if (!m_PreviouslyGrounded)
                {
                    // Calculate the fall height.
                    float fallHeight = m_JumpPosition.y - PlayerTransform.position.y;

                    // Reset the air jumps.
                    m_CurrentJumps = -1;

                    // Invoke the OnPlayerLand event.
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                    OnLand?.Invoke(fallHeight);
#else
                    if (OnLand != null)
                        OnLand.Invoke(fallHeight);
#endif
                }

                // The player is on the ground so it is not falling or jumping.
                m_IsFalling = false;
                m_IsJumping = false;
                // The player was previously grounded.
                m_PreviouslyGrounded = true;

                // If ground stick is enabled and the player isn't jumping, apply the ground stick effect.
                m_MoveDirection.y = m_EnableGroundStick ? -m_GroundStick : 0;
            }

            // Make sure the player is moving in the right direction.
            HandleMovementDirection();
            // Tell the player it should jump if the jump button is pressed, the player can jump, and if the player can move around.
            if (GetButtonDown(GoldPlayerConstants.JUMP_BUTTON_NAME, GoldPlayerConstants.JUMP_DEFAULT_KEY) && m_CanJump && m_CanMoveAround)
            {
                // Check if the player should jump.
                m_ShouldJump = ShouldJump();
            }

            // If the player should jump, jump!
            if (m_ShouldJump)
                Jump();
        }

        /// <summary>
        /// Determines if the player should jump.
        /// </summary>
        /// <returns>True if the player should jump.</returns>
        protected virtual bool ShouldJump()
        {
            if (m_IsGrounded && !m_IsJumping)
                return true;
            else if (m_AirJumpsAmount > 0 && m_CurrentJumps < m_AirJumpsAmount)
                return true;
            else if (m_AirJump && m_IsFalling && m_CurrentAirTime > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Controls the movement direction and applying the correct speeds.
        /// </summary>
        protected virtual void HandleMovementDirection()
        {
            // Only run if we can move around.
            if (m_CanMoveAround)
            {
                if (m_IsGrounded)
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

                    // Update the grounded velocity to the current move direction.
                    m_GroundVelocity = m_MoveDirection;
                }
                else
                {
                    // Get the "inverted air control". (Inspector value being 1, this is 0. Value is 0.2, this is 0.8)
                    float airControl = 1 - m_AirControl;
                    // Set the air velocity based on the ground velocity multiplied with the air control.
                    m_AirVelocity = new Vector3(m_GroundVelocity.x * airControl, m_MoveDirection.y, m_GroundVelocity.z * airControl);
                    // Apply the same movement speeds as when grounded.
                    if (m_MovementInput.y > 0)
                        m_AirVelocity.z += (m_MoveSpeed.ForwardSpeed * m_AirControl) * m_MovementInput.y;
                    else
                        m_AirVelocity.z += (m_MoveSpeed.BackwardsSpeed * m_AirControl) * m_MovementInput.y;

                    // Sideways movement speed.
                    m_AirVelocity.x += (m_MoveSpeed.SidewaysSpeed * m_AirControl) * m_MovementInput.x;

                    // Set the move direction to the air velocity.
                    m_MoveDirection = m_AirVelocity;
                }

                // Make sure we're moving in the direction the transform is facing.
                m_MoveDirection = PlayerTransform.TransformDirection(m_MoveDirection);
            }
            else
            {
                if (m_IsGrounded)
                {
                    // If we can't move around, just return zero.
                    m_MoveDirection = new Vector3(0, m_MoveDirection.y, 0);
                }
            }
        }

        /// <summary>
        /// Makes the player jump.
        /// </summary>
        protected virtual void Jump()
        {
            // Set 'isJumping' to true so the player tells everything we're jumping.
            m_IsJumping = true;
            // The player should no longer jump.
            m_ShouldJump = false;
            // Reset the current air time.
            m_CurrentAirTime = 0;

            // If the player is crouching when trying to jump, check if the player can jump while crouched.
            // If the player isn't crouching, just jump.
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

            // Increment the air jumps.
            m_CurrentJumps++;
            if (m_CurrentJumps > 0 && m_AllowAirJumpDirectionChange)
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

                // Update the grounded velocity to the current move direction.
                m_GroundVelocity = m_MoveDirection;
            }

            // Invoke the OnPlayerJump event.
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
            OnJump?.Invoke(m_JumpHeight);
#else
            if (OnJump != null)
                OnJump.Invoke(m_JumpHeight);
#endif
        }

        /// <summary>
        /// Handles running.
        /// </summary>
        protected virtual void Running()
        {
            // Set 'isRunning' to true if the player velocity is above the walking speed max.
            m_IsRunning = new Vector2(CharacterController.velocity.x, CharacterController.velocity.z).magnitude > m_WalkingSpeeds.Max + 0.5f;

            bool runButtonPressed = GetButtonDown(
                GoldPlayerConstants.RUN_BUTTON_NAME,
                GoldPlayerConstants.RUN_DEFAULT_KEY);
            bool runButtonDown = GetButton(
                GoldPlayerConstants.RUN_BUTTON_NAME,
                GoldPlayerConstants.RUN_DEFAULT_KEY);

            switch (m_RunToggleMode)
            {
                case RunToggleMode.Off:
                    {
                        m_ShouldRun = runButtonDown;
                        break;
                    }
                case RunToggleMode.Permanent:
                    {
                        if (runButtonPressed)
                            m_ShouldRun = !m_ShouldRun;
                        break;
                    }
                case RunToggleMode.UntilNoInput:
                    {
                        if (!m_HasUserInput)
                            m_ShouldRun = false;
                        else if (!m_IsRunning && !m_DidRunSinceLastBreakInMovement && runButtonDown)
                            m_ShouldRun = true;
                        else if (runButtonPressed)
                            m_ShouldRun = !m_ShouldRun;
                        break;
                    }
            }

            // Only run if we're not crouching, can run, and the player wants to be running.
            if (!m_IsCrouching && m_CanRun && m_ShouldRun)
            {
                // If stamina is enabled, only set move speed when stamina is above 0.
                // Else if stamina is not enabled, simply set move speed to run speeds.
                // Else if stamina is enabled and current stamina is 0 (or less), set move speed to walking speed.
                if (m_Stamina.EnableStamina && m_Stamina.CurrentStamina > 0)
                    m_MoveSpeed = m_RunSpeeds;
                else if (!m_Stamina.EnableStamina)
                    m_MoveSpeed = m_RunSpeeds;
                else if (m_Stamina.CurrentStamina <= 0)
                    m_MoveSpeed = m_WalkingSpeeds;
            }
            else if (!m_IsCrouching && !m_ShouldRun)
            {
                // If we're not crouching and not holding down the run button, walk.
                m_MoveSpeed = m_WalkingSpeeds;
            }

            // Only run if m_isRunning is true.
            if (m_IsRunning)
            {
                m_DidRunSinceLastBreakInMovement = true;

                // If the player wasn't previously running, they just started. Fire the OnBeginRun event.
                if (!m_PreviouslyRunning)
                {
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                    OnBeginRun?.Invoke();
#else
                    if (OnBeginRun != null)
                        OnBeginRun.Invoke();
#endif
                }

                // The player was previously running.
                m_PreviouslyRunning = true;
            }
            else
            {
                // If the player was previously running, fire the OnEndRun event.
                if (m_PreviouslyRunning)
                {
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                    OnEndRun?.Invoke();
#else
                    if (OnEndRun != null)
                        OnEndRun.Invoke();
#endif
                }

                // The player is no longer running.
                m_PreviouslyRunning = false;
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
                switch (m_CrouchToggleMode)
                {
                    case CrouchToggleMode.Off:
                        {
                            m_ShouldCrouch = GetButton(
                                GoldPlayerConstants.CROUCH_BUTTON_NAME,
                                GoldPlayerConstants.CROUCH_DEFAULT_KEY);
                            break;
                        }
                    case CrouchToggleMode.Permanent:
                        {
                            bool crouchButtonPressed = GetButtonDown(
                            GoldPlayerConstants.CROUCH_BUTTON_NAME,
                            GoldPlayerConstants.CROUCH_DEFAULT_KEY);
                            if (crouchButtonPressed)
                                m_ShouldCrouch = !m_ShouldCrouch;
                            break;
                        }
                }

                // If the player wants to be crouching, set is crouching to true.
                // Else if we can stand up and we are crouching, stop crouching.
                if (m_ShouldCrouch)
                {
                    m_IsCrouching = true;
                }
                else if (m_CanStandUp && m_IsCrouching && !m_ShouldCrouch)
                {
                    // If the player was previously crouched, fire the OnEndCrouch event, as the player is longer crouching.
                    if (m_PreviouslyCrouched)
                    {
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                        OnEndCrouch?.Invoke();
#else
                        if (OnEndCrouch != null)
                            OnEndCrouch.Invoke();
#endif
                    }

                    // Set 'isCrouching' to false.
                    m_IsCrouching = false;
                    // Set the character controller height to the original height we got at the start.
                    CharacterController.height = m_OriginalControllerHeight;
                    // Set the character controller center to the original center we got at the start.
                    CharacterController.center = m_OriginalControllerCenter;

                    // Set the move speed to the walking speed.
                    m_MoveSpeed = m_WalkingSpeeds;

                    // The player was not previously crouched.
                    m_PreviouslyCrouched = false;
                }

                // Only do the code if the player is crouching.
                if (m_IsCrouching)
                {
                    // If the player wasn't previously crouched, fire the OnBeginCrouch event, as the player is now crouching.
                    if (!m_PreviouslyCrouched)
                    {
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                        OnBeginCrouch?.Invoke();
#else
                        if (OnBeginCrouch != null)
                            OnBeginCrouch.Invoke();
#endif
                    }

                    // Check if we can stand up and update the 'canStandUp' value.
                    m_CanStandUp = CheckCanStandUp();
                    // Set the character controller height to the crouch height.
                    CharacterController.height = m_CrouchHeight;
                    // Set the character controller center to the crouch center.
                    CharacterController.center = new Vector3(CharacterController.center.x, m_ControllerCrouchCenter, CharacterController.center.z);

                    // Set the move speed to the crouch speed.
                    m_MoveSpeed = m_CrouchSpeeds;

                    // The player was previously crouched.
                    m_PreviouslyCrouched = true;
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

        /// <summary>
        /// Do updates related to force.
        /// </summary>
        protected virtual void ForceUpdate()
        {
            // If the force impact is over 0.2, apply the force impact to the move direction.
            if (m_ForceImpact.magnitude > 0.2f)
                m_MoveDirection = new Vector3(m_ForceImpact.x, m_MoveDirection.y, m_ForceImpact.z);

            // Lerp the force impact to zero over time.
            m_ForceImpact = Vector3.Lerp(m_ForceImpact, Vector3.zero, 5 * Time.deltaTime);
        }

        /// <summary>
        /// Applies force to the player.
        /// </summary>
        /// <param name="direction">The direction of the force.</param>
        /// <param name="force">Force multiplier.</param>
        public virtual void AddForce(Vector3 direction, float force)
        {
            // Normalize the direction.
            direction.Normalize();

            // Reflect down force on the ground.
            if (direction.y < 0)
                direction.y = -direction.y;
            if (direction.y > 0)
            {
                // Set 'isJumping' to true so the player tells everything we're jumping.
                m_IsJumping = true;
                // The player shouldn't jump if they are being knocked upwards.
                m_ShouldJump = false;
                // Reset the current air time.
                m_CurrentAirTime = 0;
            }

            // Apply the direction and force to the force impact.
            m_ForceImpact += direction.normalized * force;
            // Set the move direction to the force impact.
            m_MoveDirection = m_ForceImpact;
        }

        /// <summary>
        /// Applies an explosion like force to the player.
        /// </summary>
        /// <param name="explosionForce">The force amount.</param>
        /// <param name="explosionPosition">Where the explosion originated.</param>
        [System.Obsolete("Doesn't work properly.", true)]
        private void AddExplosionForce(float explosionForce, Vector3 explosionPosition)
        {
            // Calculate the direction and the force amount.
            AddForce(PlayerTransform.position - explosionPosition, Mathf.Clamp(explosionForce / 3, 0, 15));
        }

#if UNITY_EDITOR
        public override void OnValidate()
        {
            // Update the values if they have been changed during play-mode.
            if (Application.isPlaying)
            {
                WalkingSpeeds = m_WalkingSpeeds;
                RunSpeeds = m_RunSpeeds;
                CrouchSpeeds = m_CrouchSpeeds;
                JumpHeight = m_JumpHeight;

                m_WalkingSpeeds.OnValidate();
                m_RunSpeeds.OnValidate();
                m_CrouchSpeeds.OnValidate();
            }

            // Make sure gravity is always positive.
            if (m_Gravity < 0)
                m_Gravity = -m_Gravity;
            // Make sure ground stick is always positive.
            if (m_GroundStick < 0)
                m_GroundStick = -m_GroundStick;
        }
#endif
    }
}
