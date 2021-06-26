using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
    /// <summary>
    /// Used for movement related calculations.
    /// </summary>
    [System.Serializable]
    public sealed class PlayerMovement : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if the player can move at all.")]
        [FormerlySerializedAs("m_CanMoveAround")]
        internal bool canMoveAround = true;
        [SerializeField]
        [Tooltip("If true, movement will use unscaled delta time.")]
        private bool unscaledTime = false;

        //////// WALKING
        [SerializeField]
        [Tooltip("The movement speeds when walking.")]
        [FormerlySerializedAs("m_WalkingSpeeds")]
        private MovementSpeeds walkingSpeeds = new MovementSpeeds(3f, 2.5f, 2f);

        //////// RUNNING
        [SerializeField]
        [Tooltip("Determines if the player can run.")]
        [FormerlySerializedAs("m_CanRun")]
        private bool canRun = true;
        [SerializeField]
        [Tooltip("Configuration of running as a toggle.")]
        [FormerlySerializedAs("m_RunToggleMode")]
        private RunToggleMode runToggleMode = RunToggleMode.Hold;
        [SerializeField]
        [Tooltip("The movement speeds when running.")]
        [FormerlySerializedAs("m_RunSpeeds")]
        private MovementSpeeds runSpeeds = new MovementSpeeds(7f, 5.5f, 5f);
        [SerializeField]
        [Tooltip("Everything related to stamina (limited running).")]
        [FormerlySerializedAs("m_Stamina")]
        private StaminaClass stamina = new StaminaClass();

        //////// JUMPING
        [SerializeField]
        [Tooltip("Determines if the player can jump.")]
        [FormerlySerializedAs("m_CanJump")]
        private bool canJump = true;
        [SerializeField]
        [Tooltip("If stamina is enabled and this is true, jumping will require some stamina.")]
        private bool jumpingRequiresStamina = false;
        [SerializeField]
        [Tooltip("How much stamina that is required to jump.")]
        private float jumpStaminaRequire = 1;
        [SerializeField]
        [Tooltip("How much stamina that jumping will take away.")]
        private float jumpStaminaCost = 1;
        [SerializeField]
        [Tooltip("The height the player can jump in Unity units.")]
        [FormerlySerializedAs("m_JumpHeight")]
        private float jumpHeight = 2f;
        [SerializeField]
        [Tooltip("Determines if the player can jump for some time when falling.")]
        [FormerlySerializedAs("m_AirJump")]
        private bool airJump = true;
        [SerializeField]
        [Tooltip("How long the player can be in the air and still jump.")]
        [FormerlySerializedAs("m_AirJumpTime")]
        private float airJumpTime = 0.1f;
        [SerializeField]
        [Tooltip("How many times the player can jump while in the air.")]
        [FormerlySerializedAs("m_AirJumpsAmount")]
        private int airJumpsAmount = 0;
        [SerializeField]
        [Tooltip("If true, the player can change direction when air jumping.")]
        [FormerlySerializedAs("m_AllowAirJumpDirectionChange")]
        private bool allowAirJumpDirectionChange = true;

        //////// CROUCHING
        [SerializeField]
        [Tooltip("Determines if the player can crouch.")]
        [FormerlySerializedAs("m_CanCrouch")]
        private bool canCrouch = true;
        [SerializeField]
        [Tooltip("Configuration of crouching as a toggle.")]
        [FormerlySerializedAs("m_CrouchToggleMode")]
        private CrouchToggleMode crouchToggleMode = CrouchToggleMode.Hold;
        [SerializeField]
        [Tooltip("The movement speeds when crouching.")]
        [FormerlySerializedAs("m_CrouchSpeeds")]
        private MovementSpeeds crouchSpeeds = new MovementSpeeds(2f, 1.5f, 1f);
        [SerializeField]
        [Tooltip("Determines if the player can jump while crouched.")]
        [FormerlySerializedAs("m_CrouchJumping")]
        private bool crouchJumping = false;
        [SerializeField]
        [Tooltip("The height of the character controller when crouched.")]
        [FormerlySerializedAs("m_CrouchHeight")]
        private float crouchHeight = 1f;
        [SerializeField]
        [Tooltip("How long it takes to crouch.")]
        private float crouchTime = 0.25f;     
        [SerializeField] 
        private AnimationCurve crouchCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField]
        [Tooltip("How long it takes to crouch.")]
        private float standUpTime = 0.25f;
        [SerializeField] 
        private AnimationCurve standUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        //////// OTHER
        [SerializeField]
        [Tooltip("The layers the player will treat as ground. SHOULD NOT INCLUDE THE LAYER THE PLAYER IS ON!")]
        [FormerlySerializedAs("m_GroundLayer")]
        private LayerMask groundLayer = -1;
        [SerializeField]
        [Tooltip("How long is takes for the player to reach top speed.")]
        [FormerlySerializedAs("m_Acceleration")]
        private float acceleration = 0.10f;
        [SerializeField]
        [Tooltip("Sets the gravity of the player.")]
        [FormerlySerializedAs("m_Gravity")]
        internal float gravity = 20;
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("How much control the player will have in the air.")]
        [FormerlySerializedAs("m_AirControl")]
        private float airControl = 0.5f;
        [SerializeField]
        [Tooltip("Determines if ground stick should be enabled. This would stop the player for bouncing down slopes.")]
        [FormerlySerializedAs("m_EnableGroundStick")]
        private bool enableGroundStick = true;
        [SerializeField]
        [Tooltip("Sets how much the player will stick to the ground.")]
        [FormerlySerializedAs("m_GroundStick")]
        internal float groundStick = 10;
        [SerializeField]
        [Tooltip("The way the player will check if it's grounded.")]
        private GroundCheckType groundCheck = GroundCheckType.Sphere;
        [SerializeField]
        [Tooltip("The amount of rays to use for ground checking.")]
        internal int rayAmount = 8;
        [SerializeField]
        [Tooltip("How high up the rays will be when using ray ground checking.")]
        private float rayHeight = 0.3f;
        [SerializeField]
        [Tooltip("How far down the rays will reach when using ray ground checking.")]
        private float rayLength = 0.4f;
        [SerializeField]
        [Tooltip("Everything related to moving platforms.")]
        [FormerlySerializedAs("m_MovingPlatforms")]
        private MovingPlatformsClass movingPlatforms = new MovingPlatformsClass();

        //////// INPUT
        [SerializeField]
        [Tooltip("Move action for the new Input System.")]
        private string input_Move = "Move";
        [SerializeField]
        [Tooltip("Jump input action.")]
        private string input_Jump = "Jump";
        [SerializeField]
        [Tooltip("Run input action.")]
        private string input_Run = "Run";
        [SerializeField]
        [Tooltip("Crouch input action.")]
        private string input_Crouch = "Crouch";

        // The real calculated jump height.
        private float realJumpHeight;
        // The original character controller height.
        private float originalControllerHeight;
        // The original camera position.
        private float originalCameraPosition;
        // The character controller center set when the player is crouching.
        private float controllerCrouchCenter;
        // The current camera position, related to crouching.
        private float currentCrouchCameraPosition;
        // The position to set the camera to when crouching.
        private float crouchCameraPosition;
        // Just used in Input smoothing.
        private float forwardSpeedVelocity;
        // Just used in Input smoothing.
        private float sidewaysSpeedVelocity;
        // The current air time of the player.
        private float currentAirTime;
        // The current move speed multiplier.
        private float moveSpeedMultiplier = 1;
        // The current jump height multiplier,
        private float jumpHeightMultiplier = 1;
        // The start position of the player when they jump.
        private float jumpStartYPosition;
        // The max height the player reached in their jump.
        private float maxAirHeight;
        // Timer used for lerping crouching.
        private float crouchTimer;
        // The position of the camera when a crouching event starts.
        private float crouchStartPosition;

        // The current amount of times an air jump has been performed.
        internal int currentJumps;

        // Hash for move input.
        private int moveHash;
        // Hash for jump input.
        private int jumpHash;
        // Hash for run input.
        private int runHash;
        // Hash for crouch input.
        private int crouchHash;

        // Is the player grounded?
        private bool isGrounded;
        // Is the player moving at all?
        private bool isMoving = false;
        // Does the player want to be running?
        internal bool shouldRun;
        // Is the player running?
        private bool isRunning;
        // Did the player run at all since their last break in move input?
        private bool didRunSinceLastBreakInMovement;
        // Is the player jumping?
        private bool isJumping;
        // Is the player falling?
        private bool isFalling;
        // Does the player want to be crouching?
        private bool shouldCrouch;
        // Is the player crouching?
        private bool isCrouching;
        // Can the player stand up while crouching?
        private bool canStandUp;
        // Was the player previously grounded?
        private bool previouslyGrounded;
        // Was the player previously crouched?
        private bool previouslyCrouched;
        // Was the player previously running?
        private bool previouslyRunning;
        // Determines if the player should jump.
        private bool shouldJump;
        // Checking if the jump button is pressed.
        private bool pressedJump;
        // Whether or not the player registered movement input this frame. This can be false while
        // movementInput is non-zero due to the smoothing applied to movementInput.
        private bool hasUserInput;

        // Raw input values for movement on the X and Z axis.
        private Vector2 movementInput = Vector2.zero;
        // Input values for movement on the X and Z axis, automatically dampened for smoothing.
        private Vector2 smoothedMovementInput = Vector2.zero;

        // The original character controller center.
        private Vector3 originalControllerCenter;
        // The direction the player is moving in.
        private Vector3 moveDirection;
        // The current ground velocity.
        internal Vector3 groundVelocity;
        // The velocity while the player is in the air.
        internal Vector3 airVelocity;
        // The position the player was at when jumping.
        private Vector3 jumpPosition;
        // The impact of the applied force.
        private Vector3 forceImpact;
        // The previous player position.
        private Vector3 previousPosition;
        // The current velocity.
        private Vector3 velocity;
        // The rays used for raycast ground check.
        internal Vector3[] groundCheckRays;

        // The move speed that will be used when moving. Can be changed and it will be reflected in movement.
        private MovementSpeeds moveSpeed;

        /// <summary> Determines if the player can move at all. </summary>
        public bool CanMoveAround { get { return canMoveAround; } set { canMoveAround = value; if (!value) { ResetMovementInput(); } } }
        /// <summary> If true, movement will use unscaled delta time. </summary>
        public bool UnscaledTime { get { return unscaledTime; } set { unscaledTime = value; } }

        /// <summary> The speeds when walking. </summary>
        public MovementSpeeds WalkingSpeeds { get { return walkingSpeeds; } set { walkingSpeeds = value; if (!isRunning) { moveSpeed = value; } } }

        /// <summary> Multiplies the current move speed. </summary>
        public float MoveSpeedMultiplier { get { return moveSpeedMultiplier; } set { moveSpeedMultiplier = value; } }
        /// <summary> Multiplies the current jump height. </summary>
        public float JumpHeightMultiplier { get { return jumpHeightMultiplier; } set { jumpHeightMultiplier = value; realJumpHeight = CalculateJumpHeight(jumpHeight * value); } }

        /// <summary> Determines if the player can run. </summary>
        public bool CanRun { get { return canRun; } set { canRun = value; if (!value && IsRunning) { moveSpeed = walkingSpeeds; } } }
        /// <summary> Configuration of running as a toggle. </summary>
        public RunToggleMode RunToggleMode { get { return runToggleMode; } set { runToggleMode = value; } }
        /// <summary> The speeds when running. </summary>
        public MovementSpeeds RunSpeeds { get { return runSpeeds; } set { runSpeeds = value; if (isRunning) { moveSpeed = value; } } }
        /// <summary> Everything related to stamina (limited running). </summary>
        public StaminaClass Stamina { get { return stamina; } set { stamina = value; } }

        /// <summary> Determines if the player can jump. </summary>
        public bool CanJump { get { return canJump; } set { canJump = value; } }
        /// <summary> If stamina is enabled and this is true, jumping will require some stamina. </summary>
        public bool JumpingRequiresStamina { get { return jumpingRequiresStamina; } set { jumpingRequiresStamina = value; } }
        /// <summary> How much stamina that is required to jump. </summary>
        public float JumpStaminaRequire { get { return jumpStaminaRequire; } set { jumpStaminaRequire = value; } }
        /// <summary> How much stamina that jumping will take away. </summary>
        public float JumpStaminaCost { get { return jumpStaminaCost; } set { jumpStaminaCost = value; } }
        /// <summary> The height the player can jump in Unity units. </summary>
        public float JumpHeight { get { return jumpHeight; } set { jumpHeight = value; realJumpHeight = CalculateJumpHeight(value * jumpHeightMultiplier); } }
        /// <summary> Determines if the player can jump for some time when falling. </summary>
        public bool AirJump { get { return airJump; } set { airJump = value; } }
        /// <summary> How long the player can be in the air and still jump. </summary>
        public float AirJumpTime { get { return airJumpTime; } set { airJumpTime = value; } }
        /// <summary> How many times the player can jump while in the air. </summary>
        public int AirJumpsAmount { get { return airJumpsAmount; } set { airJumpsAmount = value; } }
        /// <summary> If true, the player can change direction when air jumping. </summary>
        public bool AllowAirJumpDirectionChange { get { return allowAirJumpDirectionChange; } set { allowAirJumpDirectionChange = value; } }

        /// <summary> Determines if the player can crouch. </summary>
        public bool CanCrouch { get { return canCrouch; } set { canCrouch = value; } }
        /// <summary> Configuration of crouching as a toggle. </summary>
        public CrouchToggleMode CrouchToggleMode { get { return crouchToggleMode; } set { crouchToggleMode = value; } }
        /// <summary> The movement speeds when crouching. </summary>
        public MovementSpeeds CrouchSpeeds { get { return crouchSpeeds; } set { crouchSpeeds = value; } }
        /// <summary> Determines if the player can jump while crouched. </summary>
        public bool CrouchJumping { get { return crouchJumping; } set { crouchJumping = value; } }
        /// <summary> The height of the character controller when crouched. </summary>
        public float CrouchHeight { get { return crouchHeight; } set { crouchHeight = value; } }
        /// <summary> How long it takes to crouch. </summary>
        public float CrouchTime { get { return crouchTime; } set { crouchTime = value; } }
        /// <summary> How long it takes to stand up. </summary>
        public float StandUpTime { get { return standUpTime; } set { standUpTime = value; } }

        /// <summary> The layers the player will treat as ground. SHOULD NOT INCLUDE THE LAYER THE PLAYER IS ON! </summary>
        public LayerMask GroundLayer { get { return groundLayer; } set { groundLayer = value; } }
        /// <summary> How long is takes for the player to reach top speed. </summary>
        public float Acceleration { get { return acceleration; } set { acceleration = value; } }
        /// <summary> Sets the gravity of the player. </summary>
        public float Gravity { get { return gravity; } set { float v = value; if (v < 0) { v = -v; } gravity = v; } }
        /// <summary> How much control the player will have in the air. </summary>
        public float AirControl { get { return airControl; } set { airControl = value > 1 ? 1 : value < 0 ? 0 : value; } }
        /// <summary> Determines if ground stick should be enabled. This would stop the player for bouncing down slopes. </summary>
        public bool EnableGroundStick { get { return enableGroundStick; } set { enableGroundStick = value; } }
        /// <summary> Sets how much the player will stick to the ground. </summary>
        public float GroundStick { get { return groundStick; } set { float v = value; if (v < 0) { v = -v; } groundStick = v; } }
        /// <summary> The way the player will check if it's grounded. </summary>
        public GroundCheckType GroundCheck
        {
            get { return groundCheck; }
            set
            {
                if (groundCheck != value)
                {
                    groundCheck = value;
                    // If the ground check is set to raycast and the ground check rays haven't been initialized, create them.
                    if (value == GroundCheckType.Raycast && (groundCheckRays == null || groundCheckRays.Length != rayAmount + 1))
                    {
                        groundCheckRays = new Vector3[rayAmount + 1];
                    }
                }
            }
        }
        /// <summary> The amount of rays to use for ground checking. </summary>
        public int RayAmount
        {
            get { return rayAmount; }
            set
            {
                if (rayAmount != value)
                {
                    rayAmount = value;
                    if (groundCheckRays == null || groundCheckRays.Length != value + 1)
                    {
                        groundCheckRays = new Vector3[value + 1];
                    }
                }
            }
        }
        /// <summary> How high up the rays will be when using ray ground checking. </summary>
        public float RayHeight { get { return rayHeight; } set { rayHeight = value; } }
        /// <summary> How far down the rays will reach when using ray ground checking. </summary>
        public float RayLength { get { return rayLength; } set { rayLength = value; } }
        /// <summary> Everything related to moving platforms. </summary>
        public MovingPlatformsClass MovingPlatforms { get { return movingPlatforms; } set { movingPlatforms = value; } }

        /// <summary> Move action for the new Input System. </summary>
        public string MoveInput { get { return input_Move; } set { input_Move = value; moveHash = GoldPlayerController.InputNameToHash(value); } }
        /// <summary> Jump input action. </summary>
        public string JumpInput { get { return input_Jump; } set { input_Jump = value; jumpHash = GoldPlayerController.InputNameToHash(value); } }
        /// <summary> Run input action. </summary>
        public string RunInput { get { return input_Run; } set { input_Run = value; runHash = GoldPlayerController.InputNameToHash(value); } }
        /// <summary> Crouch input action. </summary>
        public string CrouchInput { get { return input_Crouch; } set { input_Crouch = value; crouchHash = GoldPlayerController.InputNameToHash(value); } }

        /// <summary> Is the player grounded? </summary>
        public bool IsGrounded { get { return isGrounded; } }
        /// <summary> Is the player moving at all? </summary>
        public bool IsMoving { get { return isMoving; } }
        /// <summary> Is the player running? NOTE: This is true when move speed is above walk speed, not just when the run button is held down. </summary>
        public bool IsRunning { get { return isRunning; } }
        /// <summary> Is the player jumping? </summary>
        public bool IsJumping { get { return isJumping; } }
        /// <summary> Is the player falling? </summary>
        public bool IsFalling { get { return isFalling; } }
        /// <summary> Is the player crouching? </summary>
        public bool IsCrouching { get { return isCrouching; } }
        /// <summary> Can the player stand up while crouching? </summary>
        public bool CanStandUp { get { return canStandUp; } }
        /// <summary> Was the jump button pressed? </summary>
        public bool PressedJump { get { return pressedJump; } set { pressedJump = value; } }
        /// <summary> Should the player try to jump? </summary>
        public bool ShouldJump { get { return ShouldPlayerJump(); } }
        /// <summary> Should the player run? </summary>
        public bool ShouldRun { get { return shouldRun; } set { shouldRun = value; } }
        /// <summary> Should the player crouch? </summary>
        public bool ShouldCrouch { get { return shouldCrouch; } set { shouldCrouch = value; } }

        /// <summary> Raw input values for movement on the X and Z axis. </summary>
        public Vector2 MovementInput { get { return movementInput; } set { movementInput = value; } }
        /// <summary> Input values for movement on the X and Z axis, automatically dampened for smoothing. </summary>
        public Vector2 SmoothedMovementInput { get { return smoothedMovementInput; } set { smoothedMovementInput = value; } }

        /// <summary> The current velocity of the player. </summary>
        public Vector3 Velocity { get { return velocity; } }

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

        #region Obsolete
#if UNITY_EDITOR
        /// <summary> Horizontal move axis for the old Input Manager. </summary>
        [System.Obsolete("Use 'MoveInput' instead along with GetVector2. This will be removed on build.", true)]
        public string HorizontalAxis
        {
            [UnityEngine.TestTools.ExcludeFromCoverage]
            get { return null; }
            [UnityEngine.TestTools.ExcludeFromCoverage]
            set { }
        }
        /// <summary> Vertical move axis for the old Input Manager. </summary>
        [System.Obsolete("Use 'MoveInput' instead along with GetVector2. This will be removed on build.", true)]
        public string VerticalAxis
        {
            [UnityEngine.TestTools.ExcludeFromCoverage]
            get { return null; }
            [UnityEngine.TestTools.ExcludeFromCoverage]
            set { }
        }
        
        /// <summary> How fast the lerp for the head is when crouching/standing up. </summary>
        [System.Obsolete("Use 'CrouchTime' or 'StandUpTime' instead This will be removed on build.", true)]
        public float CrouchHeadLerp { get { return 0; } set { } }
        
#endif
        #endregion

        protected override void OnInitialize()
        {
            // Calculate max on all the speeds to make sure it's correct when spawning the player.
            walkingSpeeds.CalculateMax();
            runSpeeds.CalculateMax();
            crouchSpeeds.CalculateMax();
            
            // Cache the hashes for input.
            moveHash = GoldPlayerController.InputNameToHash(input_Move);
            jumpHash = GoldPlayerController.InputNameToHash(input_Jump);
            crouchHash = GoldPlayerController.InputNameToHash(input_Crouch);
            runHash = GoldPlayerController.InputNameToHash(input_Run);

            // Initialize the stamina module.
            stamina.Initialize(PlayerController, PlayerInput);
            // Initialize the moving platforms module.
            movingPlatforms.Initialize(PlayerController, PlayerInput);

            crouchTimer = standUpTime;

            // Make the gravity + if needed.
            if (gravity < 0)
            {
                gravity = -gravity;
            }
            // Make the ground stick + if needed.
            if (groundStick < 0)
            {
                groundStick = -groundStick;
            }

            // Reset move speed multiplier.
            moveSpeedMultiplier = 1f;
            // Reset jump height multiplier.
            jumpHeightMultiplier = 1f;

            // Set the move to the walking speeds.
            moveSpeed = walkingSpeeds;
            // Calculate the real jump height.
            realJumpHeight = CalculateJumpHeight(jumpHeight * jumpHeightMultiplier);
            // Set the original controller height.
            originalControllerHeight = CharacterController.height;
            // Set the original controller center.
            originalControllerCenter = CharacterController.center;
            // Set the original camera position.
            originalCameraPosition = PlayerController.Camera.CameraHead.localPosition.y;
            // Calculate the crouch center for the character controller.
            controllerCrouchCenter = CrouchHeight / 2;
            // Calculate the camera position for when crouching.
            crouchCameraPosition = PlayerController.Camera.CameraHead.localPosition.y - (CharacterController.height - crouchHeight);
            // Set the current crouch camera position to the original camera position.
            currentCrouchCameraPosition = originalCameraPosition;

            if (groundCheck == GroundCheckType.Raycast)
            {
                // Add one extra because we use a center ray too.
                groundCheckRays = new Vector3[rayAmount + 1];
            }
        }

        /// <summary>
        /// Calculates the real jump height.
        /// </summary>
        /// <param name="height">The height in Unity units.</param>
        /// <returns></returns>
        private float CalculateJumpHeight(float height)
        {
            return Mathf.Sqrt(2 * height * gravity);
        }

        /// <summary>
        /// Updates the "isGrounded" value and also returns if the player is grounded.
        /// </summary>
        /// <returns>Is the player grounded?</returns>
        public bool CheckGrounded()
        {
            // If the player is jumping, not falling, and is within their "jump grace height", just return false.
            // This allows the player to jump even on really low time scales where they don't get off the ground
            // quickly enough before IsGrounded is true again.
            if (isJumping && !isFalling)
            {
                float difference = PlayerTransform.position.y - jumpStartYPosition;
                if (difference < 0.1f)
                {
                    return false;
                }
            }

            switch (groundCheck)
            {
                case GroundCheckType.Raycast:
                    // Create the ray.
                    Ray ray = new Ray(Vector3.zero, Vector3.down);

                    // Update the circle of rays.
                    CreateGroundCheckRayCircle(ref groundCheckRays, PlayerTransform.position, CharacterController.radius);
                    // Go through each ray.
                    for (int i = 0; i < groundCheckRays.Length; i++)
                    {
                        // Set the origin on the ray to the new position.
                        ray.origin = groundCheckRays[i];
                        // If we hit something, we're grounded.
                        if (Physics.Raycast(ray, rayLength, groundLayer, QueryTriggerInteraction.Ignore))
                        {
                            return true;
                        }
                    }

                    return false;
                default: // Sphere
                    // Check using a sphere at the player's feet.
                    return Physics.CheckSphere(new Vector3(PlayerTransform.position.x, PlayerTransform.position.y + CharacterController.radius - 0.1f, PlayerTransform.position.z), CharacterController.radius, groundLayer, QueryTriggerInteraction.Ignore);
            }
        }

        /// <summary>
        /// Creates a circle of ray positions around the player's base.
        /// </summary>
        /// <param name="rays">The array to fill.</param>
        /// <param name="origin">The origin point to create the circle around.</param>
        /// <param name="radius">The radius of the circle.</param>
        public void CreateGroundCheckRayCircle(ref Vector3[] rays, Vector3 origin, float radius)
        {
#if DEBUG
            if (rays.Length != rayAmount + 1)
            {
                Debug.LogError($"The provided array needs to be the same as Ray Amount + 1 ({rayAmount + 1})");
                return;
            }
#endif

            rays[0] = new Vector3(origin.x, origin.y + rayHeight, origin.z);

            for (int i = 0; i < rayAmount; i++)
            {
                float angle = i * Mathf.PI * 2 / rayAmount;
                rays[i + 1] = new Vector3(origin.x + (Mathf.Cos(angle) * radius), origin.y + rayHeight, origin.z + (Mathf.Sin(angle) * radius));
            }
        }

        /// <summary>
        /// Updates the movement input values and also returns the current user input.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetInput(float deltaTime)
        {
            Vector2 input = GetVector2Input(moveHash);
            if (canMoveAround)
            {
                movementInput.x = input.x;
                movementInput.y = input.y;
            }

            hasUserInput = movementInput.x != 0 || movementInput.y != 0;

            if (!hasUserInput)
            {
                didRunSinceLastBreakInMovement = false;
            }

            // Take the X input and smooth it.
            smoothedMovementInput.x = Mathf.SmoothDamp(smoothedMovementInput.x, movementInput.x, ref forwardSpeedVelocity, acceleration, Mathf.Infinity, deltaTime);
            // Take the Y input and smooth it.
            smoothedMovementInput.y = Mathf.SmoothDamp(smoothedMovementInput.y, movementInput.y, ref sidewaysSpeedVelocity, acceleration, Mathf.Infinity, deltaTime);

            // Normalize the input so the player doesn't run faster when running diagonally.
            if (smoothedMovementInput.sqrMagnitude > 1)
            {
                smoothedMovementInput.Normalize();
            }

            // Return the input.
            return smoothedMovementInput;
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            previousPosition = PlayerTransform.position;

            // Call update on the stamina module.
            stamina.OnUpdate(deltaTime, unscaledDeltaTime);
            // Call update on the moving platforms module.
            movingPlatforms.OnUpdate(deltaTime, unscaledDeltaTime);

            // Check the grounded state.
            isGrounded = CheckGrounded();
            // Update the input.
            GetInput(unscaledTime ? unscaledDeltaTime : deltaTime);

            // Do movement.
            BasicMovement(unscaledTime ? unscaledDeltaTime : deltaTime);
            // Do crouching.
            Crouching(unscaledTime ? unscaledDeltaTime : deltaTime);
            // Do running.
            Running();
            // Do force update.
            ForceUpdate(unscaledTime ? unscaledDeltaTime : deltaTime);

            // Move the player using the character controller.
            CharacterController.Move(moveDirection * (unscaledTime ? unscaledDeltaTime : deltaTime));

            if (!movingPlatforms.IsMoving || movementInput != Vector2.zero)
            {
                velocity = -((previousPosition - PlayerTransform.position) / (unscaledTime ? unscaledDeltaTime : deltaTime));
            }
            else
            {
                velocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Handles the basic movement, like walking and jumping.
        /// </summary>
        private void BasicMovement(float deltaTime)
        {
            // Only run if air jump is enabled.
            if (airJump)
            {
                // If the current air time is above 0, decrease it.
                if (currentAirTime > 0)
                {
                    currentAirTime -= deltaTime;
                }

                // If current air time is less than or equal to 0, the player should no longer jump.
                if (currentAirTime <= 0)
                {
                    shouldJump = false;
                }
            }

            // If the player isn't grounded, handle gravity.
            // Else apply the ground stick so the player sticks to the ground.
            if (!isGrounded)
            {
                if (PlayerTransform.position.y > maxAirHeight)
                {
                    maxAirHeight = PlayerTransform.position.y;
                }

                // If the player was just grounded, set the jump position.
                // The player probably just jumped or started falling.
                if (previouslyGrounded)
                {
                    // Set the jump position to the current player transform.
                    jumpPosition = PlayerTransform.position;
                    if (!isJumping)
                    {
                        currentJumps++;
                    }
                }

                // When the step offset is above 0 the player can get stuck on the ceiling. Try and prevent that.
                if (CharacterController.stepOffset > 0)
                {
                    // If the player's head is touching the ceiling, move the player down so they don't
                    // get stuck on the ceiling.
                    if ((CharacterController.collisionFlags & CollisionFlags.Above) != 0)
                    {
                        moveDirection.y = -5f;
                        isJumping = false;
                        isFalling = true;
                    }
                }

                // The player was previously not grounded.
                previouslyGrounded = false;

                // If the player isn't grounded and not jumping, it is probably falling.
                // So set falling to true and reset the Y move direction.
                if (!isFalling && !isJumping)
                {
                    currentAirTime = airJumpTime;
                    isFalling = true;
                    moveDirection.y = 0;
                }

                // If we're below the max air height, we're falling.
                if (!isFalling && PlayerTransform.position.y < maxAirHeight)
                {
                    isFalling = true;
                }

                // Apply gravity to the Y axis.
                moveDirection.y -= gravity * deltaTime;
            }
            else
            {
                // If the player is grounded now and wasn't previously, the player just landed.
                if (!previouslyGrounded)
                {
                    // Calculate the fall height.
                    float fallHeight = jumpPosition.y - PlayerTransform.position.y;

                    // Reset the air jumps.
                    currentJumps = -1;

                    // Invoke the OnPlayerLand event.
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                    OnLand?.Invoke(fallHeight);
#else
                    if (OnLand != null)
                        OnLand.Invoke(fallHeight);
#endif
                }

                // The player is on the ground so it is not falling or jumping.
                isFalling = false;
                isJumping = false;
                maxAirHeight = 0;
                // The player was previously grounded.
                previouslyGrounded = true;

                // If ground stick is enabled and the player isn't jumping, apply the ground stick effect.
                moveDirection.y = enableGroundStick ? -groundStick : 0;
            }

            if (canMoveAround)
            {
                pressedJump = GetButtonDown(jumpHash);
            }

            // Make sure the player is moving in the right direction.
            HandleMovementDirection();
            // Tell the player it should jump if the jump button is pressed, the player can jump, and if the player can move around.
            if (canJump && pressedJump)
            {
                // Check if the player should jump.
                shouldJump = ShouldPlayerJump();
            }

            // If the player should jump, jump!
            if (shouldJump)
            {
                Jump();
            }
        }

        /// <summary>
        /// Determines if the player should jump.
        /// </summary>
        /// <returns>True if the player should jump.</returns>
        private bool ShouldPlayerJump()
        {
            if (jumpingRequiresStamina && stamina.EnableStamina && stamina.CurrentStamina < jumpStaminaRequire)
            {
                return false;
            }

            if (isCrouching && !crouchJumping)
            {
                return false;
            }

            if (isGrounded && !isJumping)
            {
                return true;
            }

            if (airJumpsAmount > 0 && currentJumps < airJumpsAmount)
            {
                return true;
            }

            if (airJump && isFalling && currentAirTime > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Controls the movement direction and applying the correct speeds.
        /// </summary>
        private void HandleMovementDirection()
        {
            if (isGrounded)
            {
                // Get the move direction from the movement input X and Y (on the Z axis).
                moveDirection = new Vector3(smoothedMovementInput.x, moveDirection.y, smoothedMovementInput.y);
                // If movement input Y is above 0, we're moving forward, so apply forward move speed.
                // Else if below 0, we're moving backwards, so apply backwards move speed.
                if (smoothedMovementInput.y > 0)
                {
                    moveDirection.z *= moveSpeed.ForwardSpeed * moveSpeedMultiplier;
                }
                else
                {
                    moveDirection.z *= moveSpeed.BackwardsSpeed * moveSpeedMultiplier;
                }

                // Apply the sideways movement speed to the X movement.
                moveDirection.x *= moveSpeed.SidewaysSpeed * moveSpeedMultiplier;

                // Update the grounded velocity to the current move direction.
                groundVelocity = moveDirection;
            }
            else
            {
                // Get the "inverted air control". (Inspector value being 1, this is 0. Value is 0.2, this is 0.8)
                float invertedAirControl = 1 - airControl;
                // Set the air velocity based on the ground velocity multiplied with the air control.
                airVelocity = new Vector3(groundVelocity.x * invertedAirControl, moveDirection.y, groundVelocity.z * invertedAirControl);
                // Apply the same movement speeds as when grounded.
                if (smoothedMovementInput.y > 0)
                {
                    airVelocity.z += ((moveSpeed.ForwardSpeed * moveSpeedMultiplier) * this.airControl) * smoothedMovementInput.y;
                }
                else
                {
                    airVelocity.z += ((moveSpeed.BackwardsSpeed * moveSpeedMultiplier) * this.airControl) * smoothedMovementInput.y;
                }

                // Sideways movement speed.
                airVelocity.x += ((moveSpeed.SidewaysSpeed * moveSpeedMultiplier) * this.airControl) * smoothedMovementInput.x;

                // Set the move direction to the air velocity.
                moveDirection = airVelocity;
            }

            // Make sure we're moving in the player's forward direction.
            moveDirection = Quaternion.LookRotation(PlayerController.Camera.BodyForward, PlayerTransform.up) * moveDirection;
        }

        /// <summary>
        /// Makes the player jump.
        /// </summary>
        private void Jump()
        {
            // If jumping requires stamina and stamina is enabled, remove the cost of the jump on the stamina.
            if (jumpingRequiresStamina && stamina.EnableStamina)
            {
                stamina.CurrentStamina -= jumpStaminaCost;
            }

            // Set 'isJumping' to true so the player tells everything we're jumping.
            isJumping = true;
            // The player should no longer jump.
            shouldJump = false;
            // Reset the current air time.
            currentAirTime = 0;
            jumpStartYPosition = PlayerTransform.position.y;

            // If the player is crouching when trying to jump, check if the player can jump while crouched.
            // If the player isn't crouching, just jump.
            if (isCrouching)
            {
                // If crouch jumping is enabled, jump. Else do nothing.
                if (crouchJumping)
                {
                    moveDirection.y = realJumpHeight;
                }
            }
            else
            {
                moveDirection.y = realJumpHeight;
            }

            // Increment the air jumps.
            currentJumps++;
            if (currentJumps > 0 && allowAirJumpDirectionChange)
            {
                // Get the move direction from the movement input X and Y (on the Z axis).
                moveDirection = new Vector3(smoothedMovementInput.x, moveDirection.y, smoothedMovementInput.y);
                // If movement input Y is above 0, we're moving forward, so apply forward move speed.
                // Else if below 0, we're moving backwards, so apply backwards move speed.
                if (smoothedMovementInput.y > 0)
                {
                    moveDirection.z *= moveSpeed.ForwardSpeed * moveSpeedMultiplier;
                }
                else
                {
                    moveDirection.z *= moveSpeed.BackwardsSpeed * moveSpeedMultiplier;
                }

                // Apply the sideways movement speed to the X movement.
                moveDirection.x *= moveSpeed.SidewaysSpeed * moveSpeedMultiplier;

                // Update the grounded velocity to the current move direction.
                groundVelocity = moveDirection;
            }

            // Invoke the OnPlayerJump event.
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
            OnJump?.Invoke(jumpHeight);
#else
            if (OnJump != null)
                OnJump.Invoke(jumpHeight);
#endif
        }

        /// <summary>
        /// Handles running.
        /// </summary>
        private void Running()
        {
            // If the player can't run, just stop here.
            if (!canRun)
            {
                return;
            }

            // Set 'isRunning' to true if the player velocity is above the walking speed max.
            isRunning = new Vector2(velocity.x, velocity.z).magnitude > (walkingSpeeds.Max + 0.5f);

            // Only set shouldRun if the player can move around.
            if (canMoveAround)
            {
                bool runButtonPressed = GetButtonDown(runHash);
                bool runButtonDown = GetButton(runHash);

                switch (runToggleMode)
                {
                    case RunToggleMode.Hold:
                        {
                            shouldRun = runButtonDown;
                            break;
                        }
                    case RunToggleMode.Toggle:
                        {
                            if (runButtonPressed)
                            {
                                shouldRun = !shouldRun;
                            }
                            break;
                        }
                    case RunToggleMode.UntilNoInput:
                        {
                            if (!hasUserInput)
                            {
                                shouldRun = false;
                            }
                            else if (!isRunning && !didRunSinceLastBreakInMovement && runButtonDown)
                            {
                                shouldRun = true;
                            }
                            else if (runButtonPressed)
                            {
                                shouldRun = !shouldRun;
                            }
                            break;
                        }
                }
            }

            // Just set shouldRun to false if there's not enough stamina. The player should not be running.
            if (shouldRun && stamina.EnableStamina && stamina.CurrentStamina <= 0)
            {
                shouldRun = false;
            }

            // Only run if we're not crouching, can run, and the player wants to be running.
            if (!isCrouching && canRun && shouldRun)
            {
                // If stamina is enabled, only set move speed when stamina is above 0.
                // Else if stamina is not enabled, simply set move speed to run speeds.
                // Else if stamina is enabled and current stamina is 0 (or less), set move speed to walking speed.
                if (stamina.EnableStamina && stamina.CurrentStamina > 0)
                {
                    moveSpeed = runSpeeds;
                }
                else if (!stamina.EnableStamina)
                {
                    moveSpeed = runSpeeds;
                }
            }
            else if (!isCrouching && !shouldRun)
            {
                // If we're not crouching and not holding down the run button, walk.
                moveSpeed = walkingSpeeds;
            }

            // Only run if m_isRunning is true.
            if (isRunning)
            {
                didRunSinceLastBreakInMovement = true;

                // If the player wasn't previously running, they just started. Fire the OnBeginRun event.
                if (!previouslyRunning)
                {
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                    OnBeginRun?.Invoke();
#else
                    if (OnBeginRun != null)
                        OnBeginRun.Invoke();
#endif
                }

                // The player was previously running.
                previouslyRunning = true;
            }
            else
            {
                // If the player was previously running, fire the OnEndRun event.
                if (previouslyRunning)
                {
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                    OnEndRun?.Invoke();
#else
                    if (OnEndRun != null)
                        OnEndRun.Invoke();
#endif
                }

                // The player is no longer running.
                previouslyRunning = false;
            }
        }

        /// <summary>
        /// Handles crouching.
        /// </summary>
        private void Crouching(float deltaTime)
        {
            // Only run the code if we can crouch. If we can't, always set 'isCrouching' to false.
            if (canCrouch)
            {
                if (canMoveAround)
                {
                    switch (crouchToggleMode)
                    {
                        case CrouchToggleMode.Hold:
                            {
                                shouldCrouch = GetButton(crouchHash);
                                break;
                            }
                        case CrouchToggleMode.Toggle:
                            {
                                bool crouchButtonPressed = GetButtonDown(crouchHash);
                                if (crouchButtonPressed)
                                {
                                    shouldCrouch = !shouldCrouch;
                                }
                                break;
                            }
                    }
                }

                // If the player wants to be crouching, set is crouching to true.
                // Else if we can stand up and we are crouching, stop crouching.
                if (shouldCrouch)
                {
                    isCrouching = true;
                }
                else if (canStandUp && isCrouching && !shouldCrouch)
                {
                    // If the player was previously crouched, fire the OnEndCrouch event, as the player is longer crouching.
                    if (previouslyCrouched)
                    {
                        crouchTimer = 0;
                        crouchStartPosition = PlayerController.Camera.CameraHead.localPosition.y;
     
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                        OnEndCrouch?.Invoke();
#else
                        if (OnEndCrouch != null)
                            OnEndCrouch.Invoke();
#endif
                    }

                    // Set 'isCrouching' to false.
                    isCrouching = false;
                    // Set the character controller height to the original height we got at the start.
                    CharacterController.height = originalControllerHeight;
                    // Set the character controller center to the original center we got at the start.
                    CharacterController.center = originalControllerCenter;

                    // Set the move speed to the walking speed.
                    moveSpeed = walkingSpeeds;

                    // The player was not previously crouched.
                    previouslyCrouched = false;
                }

                // Only do the code if the player is crouching.
                if (isCrouching)
                {
                    // If the player wasn't previously crouched, fire the OnBeginCrouch event, as the player is now crouching.
                    if (!previouslyCrouched)
                    {
                        crouchTimer = 0;
                        crouchStartPosition = PlayerController.Camera.CameraHead.localPosition.y;
                        
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                        OnBeginCrouch?.Invoke();
#else
                        if (OnBeginCrouch != null)
                            OnBeginCrouch.Invoke();
#endif
                    }

                    // Check if we can stand up and update the 'canStandUp' value.
                    canStandUp = CheckCanStandUp();
                    // Set the character controller height to the crouch height.
                    CharacterController.height = crouchHeight;
                    // Set the character controller center to the crouch center.
                    CharacterController.center = new Vector3(CharacterController.center.x, controllerCrouchCenter, CharacterController.center.z);

                    // Set the move speed to the crouch speed.
                    moveSpeed = crouchSpeeds;

                    // The player was previously crouched.
                    previouslyCrouched = true;
                }

                // Used to determine the player head crouch position.
                float percent = 0;
                
                if (isCrouching)
                {
                    // If crouch time is more than 0, smoothly lerp the value. Otherwise just set the value.
                    if (crouchTime > 0)
                    {
                        if (crouchTimer < crouchTime)
                        {
                            percent = crouchTimer / crouchTime;
                            crouchTimer += deltaTime;
                        }
                        else
                        {
                            percent = 1;
                        }   
                        
                        currentCrouchCameraPosition = Mathf.Lerp(crouchStartPosition, crouchCameraPosition, crouchCurve.Evaluate(percent));
                    }
                    else
                    {
                        currentCrouchCameraPosition = crouchCameraPosition;
                    }
                }
                else
                {
                    if (standUpTime > 0)
                    {
                        if (crouchTimer < standUpTime)
                        {
                            percent = crouchTimer / standUpTime;
                            crouchTimer += deltaTime;
                        }
                        else
                        {
                            percent = 1;
                        }   
                        
                        currentCrouchCameraPosition = Mathf.Lerp(crouchStartPosition, originalCameraPosition, standUpCurve.Evaluate(percent));
                    }
                    else
                    {
                        currentCrouchCameraPosition = originalCameraPosition;
                    }
                }

                Vector3 localPos = PlayerController.Camera.CameraHead.localPosition;

                // Lerp the current crouch camera position to either the crouch camera position or the original camera position.
                // Set the camera head position to the current crouch camera position.
                PlayerController.Camera.CameraHead.localPosition = new Vector3(localPos.x, currentCrouchCameraPosition, localPos.z);
            }
            else
            {
                // We can't crouch, always set 'isCrouching' to false.
                isCrouching = false;
            }
        }

        /// <summary>
        /// Checks if there's anything above the player that could stop the player from standing up.
        /// </summary>
        /// <returns></returns>
        private bool CheckCanStandUp()
        {
            // Cache the values to avoid too many native calls.
            Vector3 position = PlayerTransform.position;
            float radius = CharacterController.radius;
            // Check if we can stand up using a capsule from the player bottom to the player top.
            return !Physics.CheckCapsule(position + Vector3.up * radius, position + (Vector3.up * originalControllerHeight) - (Vector3.up * radius), radius, groundLayer, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// Do updates related to force.
        /// </summary>
        private void ForceUpdate(float deltaTime)
        {
            // If the force impact is over 0.2, apply the force impact to the move direction.
            if (forceImpact.magnitude > 0.2f)
            {
                moveDirection = new Vector3(forceImpact.x, moveDirection.y, forceImpact.z);
            }

            // Lerp the force impact to zero over time.
            forceImpact = Vector3.Lerp(forceImpact, Vector3.zero, 5 * deltaTime);
        }

        /// <summary>
        /// Applies force to the player.
        /// </summary>
        /// <param name="direction">The direction of the force.</param>
        /// <param name="force">Force multiplier.</param>
        public void AddForce(Vector3 direction, float force)
        {
            // Normalize the direction.
            direction.Normalize();

            // Reflect down force on the ground.
            if (direction.y < 0)
            {
                direction.y = -direction.y;
            }

            if (direction.y > 0)
            {
                // Set 'isJumping' to true so the player tells everything we're jumping.
                isJumping = true;
                // The player shouldn't jump if they are being knocked upwards.
                shouldJump = false;
                // Reset the current air time.
                currentAirTime = 0;
            }

            // Apply the direction and force to the force impact.
            forceImpact += direction.normalized * force;
            // Set the move direction to the force impact.
            moveDirection = forceImpact;
        }

        /// <summary>
        /// Resets all movement related input.
        /// </summary>
        private void ResetMovementInput()
        {
            movementInput = Vector2.zero;
            pressedJump = false;
            shouldRun = false;
            shouldCrouch = false;
        }

#if UNITY_EDITOR
        public override void OnValidate()
        {
            if (stamina.PlayerController == null)
            {
                stamina.PlayerController = PlayerController;
            }

            if (movingPlatforms.PlayerController == null)
            {
                movingPlatforms.PlayerController = PlayerController;
            }

            // Update the values if they have been changed during play-mode.
            if (Application.isPlaying)
            {
                WalkingSpeeds = walkingSpeeds;
                RunSpeeds = runSpeeds;
                CrouchSpeeds = crouchSpeeds;
                JumpHeight = jumpHeight;

                walkingSpeeds.OnValidate();
                runSpeeds.OnValidate();
                crouchSpeeds.OnValidate();

                if (!canMoveAround)
                {
                    ResetMovementInput();
                }

                if (groundCheck == GroundCheckType.Raycast)
                {
                    groundCheckRays = new Vector3[rayAmount + 1];
                }
            }

            // Make sure gravity is always positive.
            if (gravity < 0)
            {
                gravity = -gravity;
            }
            // Make sure ground stick is always positive.
            if (groundStick < 0)
            {
                groundStick = -groundStick;
            }
        }
#endif
    }
}
