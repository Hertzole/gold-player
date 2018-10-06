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
        [FormerlySerializedAs("m_MaxiumumX")]
        private float m_MaximumX = 90f;

        [Space]

        [SerializeField]
        [Tooltip("Settings related to field of view kick.")]
        private FOVKickClass m_FOVKick = new FOVKickClass();

        [Space]

        [SerializeField]
        [Tooltip("The camera head that should be moved around.")]
        private Transform m_CameraHead = null;

        // Determines if a camera shake should be preformed.
        private bool m_DoShake = false;
        // Was the camera previously shaking?
        private bool m_PreviouslyShaking = false;

        // Sets how strong the camera shake is.
        private float m_ShakeFrequency = 0;
        // Also sets how strong the camera shake.
        private float m_ShakeMagnitude = 0;
        // The original magnitude of the camera shake.
        private float m_ShakeMagnitudeFull = 0;
        // How last the camera shake lasts.
        private float m_ShakeDuration = 0;
        // The timer used to reach the duration.
        private float m_ShakeTimer = 0;

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
        // The rotation the head should be facing.
        private Quaternion m_TargetHeadRotation = Quaternion.identity;

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
        public float MaximumX { get { return m_MaximumX; } set { m_MaximumX = value; } }
        [System.Obsolete("Typo property. Use 'MaximumX' instead.")]
        public float MaxiumumX { get { return m_MaximumX; } set { m_MaximumX = value; } }
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

        /// <summary> Is the camera currently shaking from a camera shake? </summary>
        public bool IsCameraShaking { get { return m_DoShake; } }

        /// <summary> Fires when the camera shake begins. </summary>
        public event GoldPlayerDelegates.PlayerEvent OnBeginCameraShake;
        /// <summary> Fires when the camera shake ends. </summary>
        public event GoldPlayerDelegates.PlayerEvent OnEndCameraShake;

        protected override void OnInitialize()
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
            FOVKick.Initialize(PlayerController, PlayerInput);
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
            FOVKick.OnUpdate();
            ShakeHandler();

            // Update the camera head rotation.
            m_CameraHead.localRotation = m_TargetHeadRotation;
        }

        /// <summary>
        /// Does all the mouse work for looking around.
        /// </summary>
        protected virtual void MouseHandler()
        {
            // If the camera head field is null, stop here.
            if (m_CameraHead == null)
                return;

            // Make sure to lock the cursor when pressing the mouse button, but only if ShouldLockCursor is true.
            if (Input.GetMouseButtonDown(0) && m_ShouldLockCursor)
                LockCursor(true);

            // If the player can look around, get the input. 
            // Else just set the input to zero.
            if (m_CanLookAround)
            {
                // Set the input.
                m_MouseInput = new Vector2(m_InvertXAxis ? -GetAxis(GoldPlayerConstants.MOUSE_X) : GetAxis(GoldPlayerConstants.MOUSE_X), m_InvertYAxis ? -GetAxis(GoldPlayerConstants.MOUSE_Y) : GetAxis(GoldPlayerConstants.MOUSE_Y)) * m_MouseSensitivity;
            }
            else
            {
                // Can't look around, just set input to zero.
                m_MouseInput = Vector2.zero;
            }

            // Apply the input and mouse sensitivity.
            m_TargetHeadAngles.x += m_MouseInput.y * m_MouseSensitivity * Time.deltaTime;
            m_TargetBodyAngles.y += m_MouseInput.x * m_MouseSensitivity * Time.deltaTime;

            // Clamp the head angle.
            m_TargetHeadAngles.x = Mathf.Clamp(m_TargetHeadAngles.x, m_MinimumX, m_MaximumX);

            // Smooth the movement.
            m_FollowHeadAngles = Vector3.SmoothDamp(m_FollowHeadAngles, m_TargetHeadAngles, ref m_FollowHeadVelocity, m_MouseDamping);
            m_FollowBodyAngles = Vector3.SmoothDamp(m_FollowBodyAngles, m_TargetBodyAngles, ref m_FollowBodyVelocity, m_MouseDamping);

            // Set the rotation on the camera head and player.
            m_TargetHeadRotation = m_OriginalHeadRotation * Quaternion.Euler(-m_FollowHeadAngles.x, m_CameraHead.rotation.y, m_CameraHead.rotation.z);
            PlayerTransform.rotation = PlayerTransform.rotation * Quaternion.Euler(-m_FollowBodyAngles.x, m_FollowBodyAngles.y, 0);

            // Reset the target body angles so we can set the transform rotation from other places.
            m_TargetBodyAngles = Vector3.zero;
        }

        /// <summary>
        /// Handles all the camera shake code.
        /// </summary>
        protected virtual void ShakeHandler()
        {
            // Only run the code if doShake is true.
            if (m_DoShake)
            {
                // Apply the shake effect to the target head rotation.
                m_TargetHeadRotation *= Quaternion.Euler(PerlinShake(m_ShakeFrequency, m_ShakeMagnitude));

                // Increase the shake timer.
                m_ShakeTimer += Time.deltaTime;
                // Stop shaking whenever the shake timer is above the shake duration.
                if (m_ShakeTimer >= m_ShakeDuration)
                    m_DoShake = false;

                // Calculate the percentage of the lerp.
                float shakePercentage = m_ShakeTimer / m_ShakeDuration;
                // Decrease the magnitude over time.
                m_ShakeMagnitude = Mathf.Lerp(m_ShakeMagnitudeFull, 0f, shakePercentage);

                // The camera was previously shaking.
                m_PreviouslyShaking = true;
            }
            else
            {
                // If the camera is no longer shaking, but it just were, fire the OnEndCameraShake event.
                if (m_PreviouslyShaking)
                {
#if NET_4_6 || UNITY_2018_3_OR_NEWER
                    OnEndCameraShake?.Invoke();
#else
                    if (OnEndCameraShake != null)
                        OnEndCameraShake.Invoke();
#endif
                }

                // The player was previously not shaking.
                m_PreviouslyShaking = false;
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
            m_DoShake = true;
            m_ShakeFrequency = frequency;
            m_ShakeMagnitude = magnitude;
            m_ShakeMagnitudeFull = magnitude;
            m_ShakeDuration = duration;
            m_ShakeTimer = 0;

            // Fire the OnBeginCameraShake event.
#if NET_4_6 || UNITY_2018_3_OR_NEWER
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
            m_DoShake = false;
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
            result = result * magnitude;
            // Return the result.
            return result;
        }

#if UNITY_EDITOR
        public override void OnValidate()
        {
            m_FOVKick.OnValidate();
        }
#endif
    }
}
