using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("m_CanLookAround")]
        private bool canLookAround = true;
        [SerializeField]
        [Tooltip("Determines if the cursor should be locked.")]
        [FormerlySerializedAs("m_ShouldLockCursor")]
        private bool shouldLockCursor = true;

        [Space]
        [SerializeField]
        [Tooltip("Determines if the X axis should be inverted.")]
        [FormerlySerializedAs("m_InvertXAxis")]
        private bool invertXAxis = false;
        [SerializeField]
        [Tooltip("Determines if the Y axis should be inverted.")]
        [FormerlySerializedAs("m_InvertYAxis")]
        private bool invertYAxis = false;

        [Space]

        [SerializeField]
        [Tooltip("How fast the camera head should move when looking around.")]
        [FormerlySerializedAs("m_MouseSensitivity")]
        private float mouseSensitivity = 10f;
        [SerializeField]
        [Tooltip("Sets how smooth the movement should be.")]
        [FormerlySerializedAs("m_MouseDamping")]
        private float mouseDamping = 0f;
        [SerializeField]
        [Tooltip("Sets how far down the player can look.")]
        [FormerlySerializedAs("m_MinimumX")]
        private float minimumX = -90f;
        [SerializeField]
        [Tooltip("Sets how far up the player can look.")]
        [FormerlySerializedAs("m_MaximumX")]
        private float maximumX = 90f;

        [Space]

        [SerializeField]
        [Tooltip("Settings related to field of view kick.")]
        [FormerlySerializedAs("m_FOVKick")]
        private FOVKickClass fieldOfViewKick = new FOVKickClass();

        [Space]

        [SerializeField]
        [Tooltip("The camera head that should be moved around.")]
        [FormerlySerializedAs("m_CameraHead")]
        private Transform cameraHead = null;

        // Determines if a camera shake should be preformed.
        private bool doShake = false;
        // Was the camera previously shaking?
        private bool previouslyShaking = false;

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

        // The original head rotation.
        private Quaternion originalHeadRotation = Quaternion.identity;
        // The rotation the head should be facing.
        private Quaternion targetHeadRotation = Quaternion.identity;

        /// <summary> Determines if the player can look around. </summary>
        public bool CanLookAround { get { return canLookAround; } set { canLookAround = value; } }
        /// <summary> Determines if the cursor should be locked. </summary>
        public bool ShouldLockCursor { get { return shouldLockCursor; } set { shouldLockCursor = value; } }
        /// <summary> Determines if the X axis should be inverted. </summary>
        public bool InvertXAxis { get { return invertXAxis; } set { invertXAxis = value; } }
        /// <summary> Determines if the Y axis should be inverted. </summary>
        public bool InvertYAxis { get { return invertYAxis; } set { invertYAxis = value; } }
        /// <summary> How fast the camera head should move when looking around. </summary>
        public float MouseSensitivity { get { return mouseSensitivity; } set { mouseSensitivity = value; } }
        /// <summary> Sets how smooth the movement should be. </summary>
        public float MouseDamping { get { return mouseDamping; } set { mouseDamping = value; } }
        /// <summary> Sets how far down the player can look. </summary>
        public float MinimumX { get { return minimumX; } set { minimumX = value; } }
        /// <summary> Sets how far up the player can look. </summary>
        public float MaximumX { get { return maximumX; } set { maximumX = value; } }
        /// <summary> Settings related to field of view kick. </summary>
        public FOVKickClass FieldOfViewKick { get { return fieldOfViewKick; } set { fieldOfViewKick = value; } }
        [System.Obsolete("Use 'FieldOfViewKick' instead.")]
        public FOVKickClass FOVKick { get { return fieldOfViewKick; } set { fieldOfViewKick = value; } }
        /// <summary> The camera head that should be moved around. </summary>
        public Transform CameraHead { get { return cameraHead; } set { cameraHead = value; } }

        /// <summary> Where the head should be looking. </summary>
        public Vector3 TargetHeadAngles { get { return targetHeadAngles; } }
        /// <summary> Where the body should be looking. </summary>
        public Vector3 TargetBodyAngles { get { return targetBodyAngles; } }
        /// <summary> Where the head should be looking, smoothed. </summary>
        public Vector3 FollowHeadAngles { get { return followHeadAngles; } }
        /// <summary> Where the body should be looking, smoothed. </summary>
        public Vector3 FollowBodyAngles { get { return followBodyAngles; } }

        /// <summary> Is the camera currently shaking from a camera shake? </summary>
        public bool IsCameraShaking { get { return doShake; } }

        /// <summary> Fires when the camera shake begins. </summary>
        public event GoldPlayerDelegates.PlayerEvent OnBeginCameraShake;
        /// <summary> Fires when the camera shake ends. </summary>
        public event GoldPlayerDelegates.PlayerEvent OnEndCameraShake;

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
            fieldOfViewKick.Initialize(PlayerController, PlayerInput);
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

        public override void OnUpdate()
        {
            MouseHandler();
            fieldOfViewKick.OnUpdate();
            ShakeHandler();

            // Update the camera head rotation.
            cameraHead.localRotation = targetHeadRotation;
        }

        /// <summary>
        /// Does all the mouse work for looking around.
        /// </summary>
        protected virtual void MouseHandler()
        {
            // If the camera head field is null, stop here.
            if (cameraHead == null)
                return;

            // Make sure to lock the cursor when pressing the mouse button, but only if ShouldLockCursor is true.
            if (Input.GetMouseButtonDown(0) && shouldLockCursor)
                LockCursor(true);

            // If the player can look around, get the input. 
            // Else just set the input to zero.
            if (canLookAround)
            {
                // Set the input.
                mouseInput = new Vector2(invertXAxis ? -GetAxis(GoldPlayerConstants.MOUSE_X) : GetAxis(GoldPlayerConstants.MOUSE_X), invertYAxis ? -GetAxis(GoldPlayerConstants.MOUSE_Y) : GetAxis(GoldPlayerConstants.MOUSE_Y)) * mouseSensitivity;
            }
            else
            {
                // Can't look around, just set input to zero.
                mouseInput = Vector2.zero;
            }

            // Apply the input and mouse sensitivity.
            targetHeadAngles.x += mouseInput.y * mouseSensitivity * Time.unscaledDeltaTime;
            targetBodyAngles.y += mouseInput.x * mouseSensitivity * Time.unscaledDeltaTime;

            // Clamp the head angle.
            targetHeadAngles.x = Mathf.Clamp(targetHeadAngles.x, minimumX, maximumX);

            // Smooth the movement.
            followHeadAngles = Vector3.SmoothDamp(followHeadAngles, targetHeadAngles, ref followHeadVelocity, mouseDamping);
            followBodyAngles = Vector3.SmoothDamp(followBodyAngles, targetBodyAngles, ref followBodyVelocity, mouseDamping);

            // Set the rotation on the camera head and player.
            targetHeadRotation = originalHeadRotation * Quaternion.Euler(-followHeadAngles.x + (-recoil), cameraHead.rotation.y, cameraHead.rotation.z);
            PlayerTransform.rotation = PlayerTransform.rotation * Quaternion.Euler(-followBodyAngles.x, followBodyAngles.y, 0);

            // If recoil is above 0, decrease it. If not, just set it to 0.
            if (recoil > 0f)
            {
                // Increase the recoil time.
                currentRecoilTime += Time.deltaTime;
                // Cap the current recoil time at the max recoil time.
                if (currentRecoilTime > recoilTime)
                    currentRecoilTime = recoilTime;

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
        protected virtual void ShakeHandler()
        {
            // Only run the code if doShake is true.
            if (doShake)
            {
                // Apply the shake effect to the target head rotation.
                targetHeadRotation *= Quaternion.Euler(PerlinShake(shakeFrequency, shakeMagnitude));

                // Increase the shake timer.
                shakeTimer += Time.deltaTime;
                // Stop shaking whenever the shake timer is above the shake duration.
                if (shakeTimer >= shakeDuration)
                    doShake = false;

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

        /// <summary>
        /// Starts a camera shake.
        /// </summary>
        /// <param name="frequency">The frequency of the camera shake.</param>
        /// <param name="magnitude">The magnitude of the camera shake.</param>
        /// <param name="duration">The duration of the camera shake.</param>
        public virtual void CameraShake(float frequency, float magnitude, float duration)
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
        public virtual void StopCameraShake()
        {
            doShake = false;
        }

        /// <summary>
        /// Applies a upward recoil to the camera and slowly moves it down.
        /// </summary>
        /// <param name="recoilAmount">The amount of recoil.</param>
        /// <param name="recoilTime">The amount of time to "fade out" takes.</param>
        public virtual void ApplyRecoil(float recoilAmount, float recoilTime)
        {
            this.recoilTime = recoilTime;
            recoil = recoilAmount;
            startRecoil = recoil;
            currentRecoilTime = 0;
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
        }
#endif
    }
}
