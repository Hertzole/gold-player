using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
    /// <summary>
    /// Used to move a player camera around.
    /// </summary>
    [System.Serializable]
    public sealed class PlayerCamera : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if the player can look around.")]
        [FormerlySerializedAs("m_CanLookAround")]
        private bool canLookAround = true;
        [SerializeField]
        [Tooltip("Determines if the cursor should be locked.")]
        [FormerlySerializedAs("m_ShouldLockCursor")]
        private bool shouldLockCursor = true;
        [SerializeField]
        [Tooltip("If true, the transform Y will not rotate and only the camera will rotate.")]
        private bool rotateCameraOnly = false;

        [SerializeField]
        [Tooltip("Determines if the X axis should be inverted.")]
        [FormerlySerializedAs("m_InvertXAxis")]
        private bool invertXAxis = false;
        [SerializeField]
        [Tooltip("Determines if the Y axis should be inverted.")]
        [FormerlySerializedAs("m_InvertYAxis")]
        private bool invertYAxis = false;

        [SerializeField]
        [Tooltip("How fast the camera head should move when looking around.")]
        [FormerlySerializedAs("mouseSensitivity")]
        private Vector2 lookSensitivity = new Vector2(2f, 2f);
        [SerializeField]
        [Tooltip("Sets how smooth the movement should be.")]
        [FormerlySerializedAs("m_MouseDamping")]
        [FormerlySerializedAs("mouseDamping")]
        private float lookDamping = 0f;
        [SerializeField]
        [Tooltip("Sets how far down the player can look.")]
        [FormerlySerializedAs("m_MinimumX")]
        private float minimumX = -90f;
        [SerializeField]
        [Tooltip("Sets how far up the player can look.")]
        [FormerlySerializedAs("m_MaximumX")]
        private float maximumX = 90f;

        [SerializeField]
        [Tooltip("Settings related to field of view kick.")]
        [FormerlySerializedAs("m_FOVKick")]
        private FOVKickClass fieldOfViewKick = new FOVKickClass();

        [SerializeField]
        [Tooltip("The camera head that should be moved around.")]
        [FormerlySerializedAs("m_CameraHead")]
        private Transform cameraHead = null;

        [SerializeField]
        [Tooltip("Look action for the new Input System.")]
        private string input_Look = "Look";

        // Determines if a camera shake should be preformed.
        private bool doShake = false;
        // Was the camera previously shaking?
        private bool previouslyShaking = false;
        // Check to see if the player is force looking.
        private bool forceLooking = false;
        // Check to see if the player is being forced to look at a target.
        private bool forceLookAtTarget = false;

        // Sets how strong the camera shake is.
        private float shakeFrequency = 0;
        // Also sets how strong the camera shake.
        private float shakeMagnitude = 0;
        // The original magnitude of the camera shake.
        private float shakeMagnitudeFull = 0;
        // How last the camera shake lasts.
        private float shakeDuration = 0;
        // The timer used to reach the duration.
        private float shakeTimer = 0;
        // The amount of current recoil.
        private float recoil = 0;
        // The amount of the time the recoil should take.
        private float recoilTime = 0;
        // The start recoil amount.
        private float startRecoil;
        // The current recoil time.
        private float currentRecoilTime = 0;
        // The smoothing intensity when force looking.
        private float lookAtStrength;
        // The rotation of the body.
        private float bodyAngle = 0;

        // The current input from the mouse.
        private Vector2 mouseInput = Vector2.zero;

        // Where the head should be looking.
        private Vector3 targetHeadAngles = Vector3.zero;
        // Where the body should be looking.
        private Vector3 targetBodyAngles = Vector3.zero;
        // Where the head should be looking, smoothed.
        private Vector3 followHeadAngles = Vector3.zero;
        // Where the body should be looking, smoothed.
        private Vector3 followBodyAngles = Vector3.zero;
        // The head smooth velocity.
        private Vector3 followHeadVelocity = Vector3.zero;
        // The body smooth velocity.
        private Vector3 followBodyVelocity = Vector3.zero;
        // The point where the player will be forced to look at.
        private Vector3 lookPoint;
        // The force look direction of the body.
        private Vector3 bodyLookDirection;
        // The force look direction of the head.
        private Vector3 headLookDirection;

        // The original head rotation.
        private Quaternion originalHeadRotation = Quaternion.identity;
        // The rotation the head should be facing.
        private Quaternion targetHeadRotation = Quaternion.identity;
        // The force look body rotation
        private Quaternion bodyLookRotation;
        // The force look head rotation.
        private Quaternion headLookRotation;

        // The target to forcibly look at.
        private Transform lookTarget;

        /// <summary> Determines if the player can look around. </summary>
        public bool CanLookAround { get { return canLookAround; } set { canLookAround = value; } }
        /// <summary> Determines if the cursor should be locked. Setting this will also (un)lock the cursor. </summary>
        public bool ShouldLockCursor { get { return shouldLockCursor; } set { shouldLockCursor = value; LockCursor(value); } }
        /// <summary> If true, the transform Y will not rotate and only the camera will rotate. </summary>
        public bool RotateCameraOnly { get { return rotateCameraOnly; } set { rotateCameraOnly = value; } }
        /// <summary> Determines if the X axis should be inverted. </summary>
        public bool InvertXAxis { get { return invertXAxis; } set { invertXAxis = value; } }
        /// <summary> Determines if the Y axis should be inverted. </summary>
        public bool InvertYAxis { get { return invertYAxis; } set { invertYAxis = value; } }
        /// <summary> How fast the camera head should move when looking around. </summary>
        public Vector2 MouseSensitivity { get { return lookSensitivity; } set { lookSensitivity = value; } }
        /// <summary> Sets how smooth the movement should be. </summary>
        public float MouseDamping { get { return lookDamping; } set { lookDamping = value; } }
        /// <summary> Sets how far down the player can look. </summary>
        public float MinimumX { get { return minimumX; } set { minimumX = value; } }
        /// <summary> Sets how far up the player can look. </summary>
        public float MaximumX { get { return maximumX; } set { maximumX = value; } }
        /// <summary> Settings related to field of view kick. </summary>
        public FOVKickClass FieldOfViewKick { get { return fieldOfViewKick; } set { fieldOfViewKick = value; } }

        /// <summary> The camera head that should be moved around. </summary>
        public Transform CameraHead { get { return cameraHead; } set { cameraHead = value; } }

        /// <summary> Look action for the new Input System. </summary>
        public string LookInput { get { return input_Look; } set { input_Look = value; } }

        /// <summary> The rotation of the body. </summary>
        public float BodyAngle { get { return bodyAngle; } set { bodyAngle = value; } }

        /// <summary> Where the head should be looking. </summary>
        public Vector3 TargetHeadAngles { get { return targetHeadAngles; } }
        /// <summary> Where the body should be looking. </summary>
        public Vector3 TargetBodyAngles { get { return targetBodyAngles; } }
        /// <summary> Where the head should be looking, smoothed. </summary>
        public Vector3 FollowHeadAngles { get { return followHeadAngles; } }
        /// <summary> Where the body should be looking, smoothed. </summary>
        public Vector3 FollowBodyAngles { get { return followBodyAngles; } }

        /// <summary> The forward direction of the player. </summary>
        public Vector3 BodyForward
        {
            get
            {
                return rotateCameraOnly ? Quaternion.Euler(0, cameraHead.localEulerAngles.y, 0) * Vector3.forward : PlayerTransform.forward;
            }
        }

        /// <summary> Is the camera currently shaking from a camera shake? </summary>
        public bool IsCameraShaking { get { return doShake; } }

        /// <summary> Fires when the camera shake begins. </summary>
        public event GoldPlayerDelegates.PlayerEvent OnBeginCameraShake;
        /// <summary> Fires when the camera shake ends. </summary>
        public event GoldPlayerDelegates.PlayerEvent OnEndCameraShake;

        #region Obsolete
#if UNITY_EDITOR
        [System.Obsolete("Use 'FieldOfViewKick' instead. This will be removed on build.", true)]
        public FOVKickClass FOVKick { get { return fieldOfViewKick; } set { fieldOfViewKick = value; } }
        /// <summary> Mouse X axis for the old Input Manager. </summary>
        [System.Obsolete("Use 'LookInput' instead along with GetVector2. This will be removed on build.", true)]
        public string MouseX { get { return null; } set { } }
        /// <summary> Mouse Y axis for the old Input Manager. </summary>
        [System.Obsolete("Use 'LookInput' instead along with GetVector2. This will be removed on build.", true)]
        public string MouseY { get { return null; } set { } }
#endif
        #endregion

        protected override void OnInitialize()
        {
            // If the camera head is null, complain.
            if (cameraHead == null)
            {
                Debug.LogError("'" + PlayerController.gameObject.name + "' needs to have Camera Head assigned in the Camera settings!");
                return;
            }

            // Lock the cursor, if it should.
            LockCursor(shouldLockCursor);

            // Set the original head rotation to the one on the camera head.
            originalHeadRotation = cameraHead.localRotation;

            // Initialize the FOV kick module.
            fieldOfViewKick.Initialize(PlayerInput);
        }

        /// <summary>
        /// Locks/unlocks the cursor and makes it invisible/visible.
        /// </summary>
        /// <param name="lockCursor">Should the cursor be locked?</param>
        public void LockCursor(bool lockCursor)
        {
            // Set the cursor lock state to either locked or none, depending on the lock cursor parameter.
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            // Hide/Show the cursor based on the lock cursor parameter.
            Cursor.visible = !lockCursor;
        }

        public override void OnUpdate(float deltaTime)
        {
            ForceLookHandler();
            MouseHandler(deltaTime);
            fieldOfViewKick.OnUpdate(deltaTime);
            ShakeHandler(deltaTime);

            // Update the camera head rotation.
            cameraHead.localRotation = targetHeadRotation;
        }

        /// <summary>
        /// Does all the mouse work for looking around.
        /// </summary>
        private void MouseHandler(float deltaTime)
        {
            // If the camera head field is null, stop here.
            if (cameraHead == null)
            {
                return;
            }

            // Make sure to lock the cursor when pressing the mouse button, but only if ShouldLockCursor is true.
#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
            if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame && shouldLockCursor)
            {
                LockCursor(true);
            }
#else
            if (Input.GetMouseButtonDown(0) && shouldLockCursor)
            {
                LockCursor(true);
            }
#endif

            // If the player can look around, get the input.
            // Else just set the input to zero.
            if (canLookAround && !forceLooking)
            {
                // Set the input.
                mouseInput = GetVector2Input(input_Look);
                if (invertXAxis)
                {
                    mouseInput.x = -mouseInput.x;
                }

                if (invertYAxis)
                {
                    mouseInput.y = -mouseInput.y;
                }
            }
            else
            {
                // Can't look around, just set input to zero.
                mouseInput = Vector2.zero;
            }

            if (!forceLooking)
            {
                // Apply the input and mouse sensitivity.
                targetHeadAngles.x -= mouseInput.y * lookSensitivity.y;
                targetBodyAngles.y += mouseInput.x * lookSensitivity.x;

                // Clamp the head angle.
                targetHeadAngles.x = Mathf.Clamp(targetHeadAngles.x, minimumX, maximumX);

                // Smooth the movement.
                followHeadAngles = Vector3.SmoothDamp(followHeadAngles, targetHeadAngles, ref followHeadVelocity, lookDamping, Mathf.Infinity, Time.unscaledDeltaTime);
                followBodyAngles = Vector3.SmoothDamp(followBodyAngles, targetBodyAngles, ref followBodyVelocity, lookDamping, Mathf.Infinity, Time.unscaledDeltaTime);

                if (rotateCameraOnly)
                {
                    bodyAngle += followBodyAngles.y;
                }

                // Set the rotation on the camera head and player.
                targetHeadRotation = originalHeadRotation * Quaternion.Euler(followHeadAngles.x + (-recoil), rotateCameraOnly ? bodyAngle : 0, 0);
                if (!rotateCameraOnly)
                {
                    PlayerTransform.rotation = PlayerTransform.rotation * Quaternion.Euler(-followBodyAngles.x, followBodyAngles.y, 0);
                }
            }

            // If recoil is above 0, decrease it. If not, just set it to 0.
            if (recoil > 0f)
            {
                // Increase the recoil time.
                currentRecoilTime += deltaTime;
                // Cap the current recoil time at the max recoil time.
                if (currentRecoilTime > recoilTime)
                {
                    currentRecoilTime = recoilTime;
                }

                // Calculate the percentage and lerp with it.
                float recoilPercentage = currentRecoilTime / recoilTime;
                recoil = Mathf.Lerp(startRecoil, 0, recoilPercentage);
            }
            else
            {
                recoil = 0;
            }

            // Reset the target body angles so we can set the transform rotation from other places.
            targetBodyAngles = Vector3.zero;
        }

        /// <summary>
        /// Handles all the camera shake code.
        /// </summary>
        private void ShakeHandler(float deltaTime)
        {
            // Only run the code if doShake is true.
            if (doShake)
            {
                // Apply the shake effect to the target head rotation.
                targetHeadRotation *= Quaternion.Euler(PerlinShake(shakeFrequency, shakeMagnitude));

                // Increase the shake timer.
                shakeTimer += deltaTime;
                // Stop shaking whenever the shake timer is above the shake duration.
                if (shakeTimer >= shakeDuration)
                {
                    doShake = false;
                }

                // Calculate the percentage of the lerp.
                float shakePercentage = shakeTimer / shakeDuration;
                // Decrease the magnitude over time.
                shakeMagnitude = Mathf.Lerp(shakeMagnitudeFull, 0f, shakePercentage);

                // The camera was previously shaking.
                previouslyShaking = true;
            }
            else
            {
                // If the camera is no longer shaking, but it just were, fire the OnEndCameraShake event.
                if (previouslyShaking)
                {
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                    OnEndCameraShake?.Invoke();
#else
                    if (OnEndCameraShake != null)
                        OnEndCameraShake.Invoke();
#endif
                }

                // The player was previously not shaking.
                previouslyShaking = false;
            }
        }

        private void ForceLookHandler()
        {
            // Only run if the player is looking at something.
            if (forceLooking)
            {
                // If there's a target, update the look point to that target.
                if (forceLookAtTarget)
                {
                    lookPoint = lookTarget.position;
                }

                // Get the direction of the body.
                bodyLookDirection = lookPoint - PlayerTransform.position;
                // Get the direction of the head.
                headLookDirection = lookPoint - cameraHead.position;

                // The body only needs to know the X and Z axis because it does not look up and down.
                bodyLookDirection.y = 0;

                // Get the body rotation.
                bodyLookRotation = Quaternion.LookRotation(bodyLookDirection, Vector3.up);

                // Because when the Z-axis is positive, the view will invert. So let's invert it back.
                if (bodyLookDirection.z > 0)
                {
                    headLookDirection.y = -headLookDirection.y;
                }

                // Get the head rotation.
                headLookRotation = Quaternion.AngleAxis(Vector3.SignedAngle(headLookDirection, bodyLookDirection, Vector3.right), Vector3.right);

                // If the strength is above 0, make the look smooth.
                // Else make it instant.
                if (lookAtStrength > 0)
                {
                    PlayerTransform.rotation = Quaternion.Slerp(PlayerTransform.rotation, bodyLookRotation, lookAtStrength * Time.deltaTime);
                    targetHeadRotation = Quaternion.Slerp(targetHeadRotation, headLookRotation, lookAtStrength * Time.deltaTime);
                }
                else
                {
                    PlayerTransform.rotation = bodyLookRotation;
                    targetHeadRotation = headLookRotation;
                }

                // Get the X look angle to make sure the camera doesn't snap back when we stop force looking.
                float lookAngle = cameraHead.transform.eulerAngles.x <= 90f ? -cameraHead.eulerAngles.x : 360 - cameraHead.eulerAngles.x;
                targetHeadAngles.x = -lookAngle;
            }
        }

        /// <summary>
        /// Starts a camera shake.
        /// </summary>
        /// <param name="frequency">The frequency of the camera shake.</param>
        /// <param name="magnitude">The magnitude of the camera shake.</param>
        /// <param name="duration">The duration of the camera shake.</param>
        public void CameraShake(float frequency, float magnitude, float duration)
        {
            doShake = true;
            shakeFrequency = frequency;
            shakeMagnitude = magnitude;
            shakeMagnitudeFull = magnitude;
            shakeDuration = duration;
            shakeTimer = 0;

            // Fire the OnBeginCameraShake event.
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
            OnBeginCameraShake?.Invoke();
#else
            if (OnBeginCameraShake != null)
                OnBeginCameraShake.Invoke();
#endif
        }

        /// <summary>
        /// Stops the camera shake.
        /// </summary>
        public void StopCameraShake()
        {
            doShake = false;
        }

        /// <summary>
        /// Applies a upward recoil to the camera and slowly moves it down.
        /// </summary>
        /// <param name="recoilAmount">The amount of recoil.</param>
        /// <param name="recoilTime">The amount of time to "fade out" takes.</param>
        public void ApplyRecoil(float recoilAmount, float recoilTime)
        {
            this.recoilTime = recoilTime;
            recoil = recoilAmount;
            startRecoil = recoil;
            currentRecoilTime = 0;
        }

        /// <summary>
        /// Forces the player to look at a target. With a target assigned, it will move with the target.
        /// </summary>
        /// <param name="target">The target to look at.</param>
        /// <param name="strength">The strength of the look. Set to 0 for instant.</param>
        public void ForceLook(Transform target, float strength = 10)
        {
            if (target == null)
            {
                StopForceLooking();
                return;
            }

            ForceLookInternal(strength);
            forceLookAtTarget = true;
            lookTarget = target;
            lookPoint = target.position;
        }

        /// <summary>
        /// Forces the player to look at a position.
        /// </summary>
        /// <param name="point">The position to look at.</param>
        /// <param name="strength">The strength of the look. Set to 0 for instant.</param>
        public void ForceLook(Vector3 point, float strength = 10)
        {
            ForceLookInternal(strength);
            forceLookAtTarget = false;
            lookTarget = null;
            lookPoint = point;
        }

        /// <summary>
        /// Stops the player from force looking.
        /// </summary>
        public void StopForceLooking()
        {
            forceLookAtTarget = false;
            lookTarget = null;
            forceLooking = false;
        }

        private void ForceLookInternal(float strength)
        {
            forceLooking = true;
            lookAtStrength = strength;
        }

        /// <summary>
        /// Get a random Vector3 shake based on perlin noise.
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="magnitude"></param>
        /// <returns></returns>
        private Vector3 PerlinShake(float frequency, float magnitude)
        {
            // Create the result variable.
            Vector3 result = Vector3.zero;
            // Create the seed.
            float seed = Time.time * frequency;
            // Apply perlin noise.
            result.x = Mathf.Clamp01(Mathf.PerlinNoise(seed, 0f)) - 0.5f;
            result.y = Mathf.Clamp01(Mathf.PerlinNoise(seed, seed)) - 0.5f;
            result.z = Mathf.Clamp01(Mathf.PerlinNoise(0f, seed)) - 0.5f;
            // Multiple result with magnitude.
            result *= magnitude;
            // Return the result.
            return result;
        }

#if UNITY_EDITOR
        public override void OnValidate()
        {
            fieldOfViewKick.OnValidate();

            if (fieldOfViewKick.PlayerController == null)
            {
                fieldOfViewKick.PlayerController = PlayerController;
            }
        }
#endif
    }
}
