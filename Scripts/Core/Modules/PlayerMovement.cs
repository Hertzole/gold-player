using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("m_CanMoveAround")]
        private bool canMoveAround = true;

        //////// WALKING
        [Header("Walking")]
        [SerializeField]
        [Tooltip("The movement speeds when walking.")]
        [FormerlySerializedAs("m_WalkingSpeeds")]
        private MovementSpeeds walkingSpeeds = new MovementSpeeds(3f, 2.5f, 2f);

        //////// RUNNING
        [Header("Running")]
        [SerializeField]
        [Tooltip("Determines if the player can run.")]
        [FormerlySerializedAs("m_CanRun")]
        private bool canRun = true;
        [SerializeField]
        [Tooltip("Configuration of running as a toggle.")]
        [FormerlySerializedAs("m_RunToggleMode")]
        private RunToggleMode runToggleMode = RunToggleMode.Off;
        [SerializeField]
        [Tooltip("The movement speeds when running.")]
        [FormerlySerializedAs("m_RunSpeeds")]
        private MovementSpeeds runSpeeds = new MovementSpeeds(7f, 5.5f, 5f);
        [SerializeField]
        [Tooltip("Everything related to stamina (limited running).")]
        [FormerlySerializedAs("m_Stamina")]
        private StaminaClass stamina = new StaminaClass();

        //////// JUMPING
        [Header("Jumping")]
        [SerializeField]
        [Tooltip("Determines if the player can jump.")]
        [FormerlySerializedAs("m_CanJump")]
        private bool canJump = true;
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
        [Header("Crouching")]
        [SerializeField]
        [Tooltip("Determines if the player can crouch.")]
        [FormerlySerializedAs("m_CanCrouch")]
        private bool canCrouch = true;
        [SerializeField]
        [Tooltip("Configuration of crouching as a toggle.")]
        [FormerlySerializedAs("m_CrouchToggleMode")]
        private CrouchToggleMode crouchToggleMode = CrouchToggleMode.Off;
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
        [Tooltip("How fast the lerp for the head is when crouching/standing up.")]
        [FormerlySerializedAs("m_CrouchHeadLerp")]
        private float crouchHeadLerp = 10;

        //////// OTHER
        [Header("Other")]
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
        private float gravity = 20;
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
        private float groundStick = 10;
        [SerializeField]
        [Tooltip("Everything related to moving platforms.")]
        [FormerlySerializedAs("m_MovingPlatforms")]
        private MovingPlatformsClass movingPlatforms = new MovingPlatformsClass();

        //////// INPUT
        [SerializeField]
#if !ENABLE_INPUT_SYSTEM || !UNITY_2019_3_OR_NEWER
        [HideInInspector]
#endif
        [Tooltip("Move action for the new Input System.")]
        private string input_Move = "Move";
        [SerializeField]
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
        [HideInInspector]
#endif
        [Tooltip("Horizontal move axis for the old Input Manager.")]
        private string input_HorizontalAxis = "Horizontal";
        [SerializeField]
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
        [HideInInspector]
#endif
        [Tooltip("Vertical move axis for the old Input Manager.")]
        private string input_VerticalAxis = "Vertical";
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
        protected float realJumpHeight = 0;
        // The original character controller height.
        protected float originalControllerHeight = 0;
        // The original camera position.
        protected float originalCameraPosition = 0;
        // The character controller center set when the player is crouching.
        protected float controllerCrouchCenter = 0;
        // The current camera position, related to crouching.
        protected float currentCrouchCameraPosition = 0;
        // The position to set the camera to when crouching.
        protected float crouchCameraPosition = 0;
        // Just used in Input smoothing.
        protected float forwardSpeedVelocity = 0;
        // Just used in Input smoothing.
        protected float sidewaysSpeedVelocity = 0;
        // The current air time of the player.
        protected float currentAirTime = 0;
        // The current move speed multiplier.
        protected float moveSpeedMultiplier = 1;
        // The current jump height multiplier,
        protected float jumpHeightMultiplier = 1;

        // The current amount of times an air jump has been performed.
        protected int currentJumps = 0;

        // Is the player grounded?
        protected bool isGrounded = false;
        // Is the player moving at all?
        protected bool isMoving = false;
        // Does the player want to be running?
        protected bool shouldRun = false;
        // Is the player running?
        protected bool isRunning = false;
        // Did the player run at all since their last break in move input?
        private bool didRunSinceLastBreakInMovement;
        // Is the player jumping?
        protected bool isJumping = false;
        // Is the player falling?
        protected bool isFalling = false;
        // Does the player want to be crouching?
        protected bool shouldCrouch = false;
        // Is the player crouching?
        protected bool isCrouching = false;
        // Can the player stand up while crouching?
        protected bool canStandUp = false;
        // Was the player previously grounded?
        protected bool previouslyGrounded = false;
        // Was the player previously crouched?
        protected bool previouslyCrouched = false;
        // Was the player previously running?
        protected bool previouslyRunning = false;
        // Determines if the player should jump.
        protected bool shouldJump = false;

        // Input values for movement on the X and Z axis, automatically dampened for smoothing.
        protected Vector2 movementInput = Vector2.zero;
        // Whether or not the player registered movement input this frame. This can be false while
        // m_MovementInput is non-zero due to the smoothing applied to m_MovementInput.
        protected bool hasUserInput = false;

        // The original character controller center.
        protected Vector3 originalControllerCenter = Vector3.zero;
        // The direction the player is moving in.
        protected Vector3 moveDirection = Vector3.zero;
        // The current ground velocity.
        protected Vector3 groundVelocity = Vector3.zero;
        // The velocity while the player is in the air.
        protected Vector3 airVelocity = Vector3.zero;
        // The position the player was at when jumping.
        protected Vector3 jumpPosition = Vector3.zero;
        // The impact of the applied force.
        protected Vector3 forceImpact = Vector3.zero;

        protected string moveInput;
        protected string jumpInput;
        protected string runInput;
        protected string crouchInput;

        // The move speed that will be used when moving. Can be changed and it will be reflected in movement.
        protected MovementSpeeds moveSpeed = new MovementSpeeds();

        /// <summary> Determines if the player can move at all. </summary>
        public bool CanMoveAround { get { return canMoveAround; } set { canMoveAround = value; } }

        /// <summary> The speeds when walking. </summary>
        public MovementSpeeds WalkingSpeeds { get { return walkingSpeeds; } set { walkingSpeeds = value; if (!isRunning) { moveSpeed = value; } } }

        /// <summary> Multiplies the current move speed. </summary>
        public float MoveSpeedMultiplier { get { return moveSpeedMultiplier; } set { moveSpeedMultiplier = value; } }
        /// <summary> Multiplies the current jump height. </summary>
        public float JumpHeightMultiplier { get { return jumpHeightMultiplier; } set { jumpHeightMultiplier = value; CalculateJumpHeight(jumpHeight * value); } }

        /// <summary> Determines if the player can run. </summary>
        public bool CanRun { get { return canRun; } set { canRun = value; } }
        /// <summary> Configuration of running as a toggle. </summary>
        public RunToggleMode RunToggleMode { get { return runToggleMode; } set { runToggleMode = value; } }
        /// <summary> The speeds when running. </summary>
        public MovementSpeeds RunSpeeds { get { return runSpeeds; } set { runSpeeds = value; if (isRunning) { moveSpeed = value; } } }
        /// <summary> Everything related to stamina (limited running). </summary>
        public StaminaClass Stamina { get { return stamina; } set { stamina = value; } }

        /// <summary> Determines if the player can jump. </summary>
        public bool CanJump { get { return canJump; } set { canJump = value; } }
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
        /// <summary> How fast the lerp for the head is when crouching/standing up. </summary>
        public float CrouchHeadLerp { get { return crouchHeadLerp; } set { crouchHeadLerp = value; } }

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
        /// <summary> Everything related to moving platforms. </summary>
        public MovingPlatformsClass MovingPlatforms { get { return movingPlatforms; } set { movingPlatforms = value; } }

        /// <summary> Move action for the new Input System. </summary>
        public string MoveInput { get { return moveInput; } set { input_Move = value; moveInput = string.IsNullOrEmpty(rootActionMap) ? value : rootActionMap + "/" + value; } }
        /// <summary> Horizontal move axis for the old Input Manager. </summary>
        public string HorizontalAxis { get { return input_HorizontalAxis; } set { input_HorizontalAxis = value; } }
        /// <summary> Vertical move axis for the old Input Manager. </summary>
        public string VerticalAxis { get { return input_VerticalAxis; } set { input_VerticalAxis = value; } }
        /// <summary> Jump input action. </summary>
        public string JumpInput { get { return jumpInput; } set { input_Jump = value; jumpInput = string.IsNullOrEmpty(rootActionMap) ? value : rootActionMap + "/" + value; } }
        /// <summary> Run input action. </summary>
        public string RunInput { get { return runInput; } set { input_Run = value; runInput = string.IsNullOrEmpty(rootActionMap) ? value : rootActionMap + "/" + value; } }
        /// <summary> Crouch input action. </summary>
        public string CrouchInput { get { return crouchInput; } set { input_Crouch = value; crouchInput = string.IsNullOrEmpty(rootActionMap) ? value : rootActionMap + "/" + value; } }

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
            // Calculate max on all the speeds to make sure it's correct when spawning the player.
            walkingSpeeds.CalculateMax();
            runSpeeds.CalculateMax();
            crouchSpeeds.CalculateMax();

            // Initialize the stamina module.
            stamina.Initialize(PlayerController, PlayerInput);
            // Initialize the moving platforms module.
            movingPlatforms.Initialize(PlayerController, PlayerInput);

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

            // Set up input actions to stop concatenation in update.
            MoveInput = input_Move;
            JumpInput = input_Jump;
            RunInput = input_Run;
            CrouchInput = input_Crouch;
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
            // Check using a sphere at the player's feet.
            return Physics.CheckSphere(new Vector3(PlayerTransform.position.x, PlayerTransform.position.y + CharacterController.radius - 0.1f, PlayerTransform.position.z), CharacterController.radius, groundLayer, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// Updates the movement input values and also returns the current user input.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetInput()
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            Vector2 input = GetVector2Input(moveInput);
            float horizontal = canMoveAround ? input.x : 0;
            float vertical = canMoveAround ? input.y : 0;
#else
            float horizontal = canMoveAround ? GetAxisRaw(input_HorizontalAxis) : 0;
            float vertical = canMoveAround ? GetAxisRaw(input_VerticalAxis) : 0;
#endif

            hasUserInput = horizontal != 0 || vertical != 0;

            if (!hasUserInput)
            {
                didRunSinceLastBreakInMovement = false;
            }

            // Take the X input and smooth it.
            movementInput.x = Mathf.SmoothDamp(movementInput.x, horizontal, ref forwardSpeedVelocity, acceleration);
            // Take the Y input and smooth it.
            movementInput.y = Mathf.SmoothDamp(movementInput.y, vertical, ref sidewaysSpeedVelocity, acceleration);

            // Normalize the input so the player doesn't run faster when running diagonally.
            if (movementInput.sqrMagnitude > 1)
            {
                movementInput.Normalize();
            }

            // Return the input.
            return movementInput;
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public override void OnUpdate(float deltaTime)
        {
            // Call update on the stamina module.
            stamina.OnUpdate(deltaTime);
            // Call update on the moving platforms module.
            movingPlatforms.OnUpdate(deltaTime);

            // Check the grounded state.
            isGrounded = CheckGrounded();
            // Update the input.
            GetInput();

            // Do movement.
            BasicMovement(deltaTime);
            // Do crouching.
            Crouching(deltaTime);
            // Do running.
            Running();
            // Do force update.
            ForceUpdate(deltaTime);

            // Move the player using the character controller.
            CharacterController.Move(moveDirection * deltaTime);
        }

        /// <summary>
        /// Handles the basic movement, like walking and jumping.
        /// </summary>
        protected virtual void BasicMovement(float deltaTime)
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
                // The player was previously grounded.
                previouslyGrounded = true;

                // If ground stick is enabled and the player isn't jumping, apply the ground stick effect.
                moveDirection.y = enableGroundStick ? -groundStick : 0;
            }

            // Make sure the player is moving in the right direction.
            HandleMovementDirection();
            // Tell the player it should jump if the jump button is pressed, the player can jump, and if the player can move around.
            if (canJump && canMoveAround && GetButtonDown(jumpInput))
            {
                // Check if the player should jump.
                shouldJump = ShouldJump();
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
        protected virtual bool ShouldJump()
        {
            if (isGrounded && !isJumping)
            {
                return true;
            }
            else if (airJumpsAmount > 0 && currentJumps < airJumpsAmount)
            {
                return true;
            }
            else if (airJump && isFalling && currentAirTime > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Controls the movement direction and applying the correct speeds.
        /// </summary>
        protected virtual void HandleMovementDirection()
        {
            if (isGrounded)
            {
                // Get the move direction from the movement input X and Y (on the Z axis).
                moveDirection = new Vector3(movementInput.x, moveDirection.y, movementInput.y);
                // If movement input Y is above 0, we're moving forward, so apply forward move speed.
                // Else if below 0, we're moving backwards, so apply backwards move speed.
                if (movementInput.y > 0)
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
                float airControl = 1 - this.airControl;
                // Set the air velocity based on the ground velocity multiplied with the air control.
                airVelocity = new Vector3(groundVelocity.x * airControl, moveDirection.y, groundVelocity.z * airControl);
                // Apply the same movement speeds as when grounded.
                if (movementInput.y > 0)
                {
                    airVelocity.z += ((moveSpeed.ForwardSpeed * moveSpeedMultiplier) * this.airControl) * movementInput.y;
                }
                else
                {
                    airVelocity.z += ((moveSpeed.BackwardsSpeed * moveSpeedMultiplier) * this.airControl) * movementInput.y;
                }

                // Sideways movement speed.
                airVelocity.x += ((moveSpeed.SidewaysSpeed * moveSpeedMultiplier) * this.airControl) * movementInput.x;

                // Set the move direction to the air velocity.
                moveDirection = airVelocity;
            }

            // Make sure we're moving in the direction the transform is facing.
            moveDirection = PlayerTransform.TransformDirection(moveDirection);
        }

        /// <summary>
        /// Makes the player jump.
        /// </summary>
        protected virtual void Jump()
        {
            // Set 'isJumping' to true so the player tells everything we're jumping.
            isJumping = true;
            // The player should no longer jump.
            shouldJump = false;
            // Reset the current air time.
            currentAirTime = 0;

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
                moveDirection = new Vector3(movementInput.x, moveDirection.y, movementInput.y);
                // If movement input Y is above 0, we're moving forward, so apply forward move speed.
                // Else if below 0, we're moving backwards, so apply backwards move speed.
                if (movementInput.y > 0)
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
        protected virtual void Running()
        {
            // If the player can't run, just stop here.
            if (!canRun)
            {
                return;
            }

            // Set 'isRunning' to true if the player velocity is above the walking speed max.
            isRunning = new Vector2(CharacterController.velocity.x, CharacterController.velocity.z).magnitude > walkingSpeeds.Max + 0.5f;

            bool runButtonPressed = GetButtonDown(runInput);
            bool runButtonDown = GetButton(runInput);

            switch (runToggleMode)
            {
                case RunToggleMode.Off:
                {
                    shouldRun = runButtonDown;
                    break;
                }
                case RunToggleMode.Permanent:
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
                else if (stamina.CurrentStamina <= 0)
                {
                    moveSpeed = walkingSpeeds;
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
        protected virtual void Crouching(float deltaTime)
        {
            // Only run the code if we can crouch. If we can't, always set 'isCrouching' to false.
            if (canCrouch)
            {
                switch (crouchToggleMode)
                {
                    case CrouchToggleMode.Off:
                    {
                        shouldCrouch = GetButton(crouchInput);
                        break;
                    }
                    case CrouchToggleMode.Permanent:
                    {
                        bool crouchButtonPressed = GetButtonDown(crouchInput);
                        if (crouchButtonPressed)
                        {
                            shouldCrouch = !shouldCrouch;
                        }

                        break;
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

                // Lerp the current crouch camera position to either the crouch camera position or the original camera position.
                currentCrouchCameraPosition = Mathf.Lerp(currentCrouchCameraPosition, isCrouching ? crouchCameraPosition : originalCameraPosition, crouchHeadLerp * deltaTime);
                // Set the camera head position to the current crouch camera position.
                PlayerController.Camera.CameraHead.localPosition = new Vector3(PlayerController.Camera.CameraHead.localPosition.x, currentCrouchCameraPosition, PlayerController.Camera.CameraHead.localPosition.z);
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
        protected virtual bool CheckCanStandUp()
        {
            // Check if we can stand up using a capsule from the player bottom to the player top.
            return !Physics.CheckCapsule(PlayerTransform.position + Vector3.up * CharacterController.radius, PlayerTransform.position + (Vector3.up * originalControllerHeight) - (Vector3.up * CharacterController.radius), CharacterController.radius, groundLayer, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// Do updates related to force.
        /// </summary>
        protected virtual void ForceUpdate(float deltaTime)
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
        public virtual void AddForce(Vector3 direction, float force)
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

#if UNITY_EDITOR
        public override void OnValidate()
        {
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

                MoveInput = input_Move;
                JumpInput = input_Jump;
                RunInput = input_Run;
                CrouchInput = input_Crouch;
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
