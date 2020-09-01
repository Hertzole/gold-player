using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
    /// <summary>
    /// Used to apply field of view (FOV) kick to a target camera.
    /// </summary>
    [System.Serializable]
    public class FOVKickClass : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if FOV kick should be enabled.")]
        [FormerlySerializedAs("m_EnableFOVKick")]
        private bool enableFOVKick = true;
        [SerializeField]
        [Tooltip("If true, FOV kick will use unscaled delta time.")]
        private bool unscaledTime = false;
        [SerializeField]
        [Tooltip("Sets whenever the FOV kick should kick in.")]
        [FormerlySerializedAs("m_KickWhen")]
        private RunAction kickWhen = RunAction.FasterThanRunSpeed;
        [SerializeField]
        [Tooltip("Sets how much the FOV will kick.")]
        [FormerlySerializedAs("m_KickAmount")]
        private float kickAmount = 15f;
        [SerializeField]
        [Tooltip("Sets how fast the FOV will move to the new FOV.")]
        [FormerlySerializedAs("m_LerpTimeTo")]
        private float lerpTimeTo = 4f;
        [SerializeField]
        [Tooltip("Sets how fast the FOV will move back to the original FOV.")]
        [FormerlySerializedAs("m_LerpTimeFrom")]
        private float lerpTimeFrom = 2.5f;

#if GOLD_PLAYER_CINEMACHINE
        [SerializeField]
        [Tooltip("Allows you to use Cinemachine virtual camera instead of a direct reference to a camera.")]
        private bool useCinemachine = false;
        [SerializeField]
        [Tooltip("The virtual camera that the FOV kick should be applied to.")]
        private Cinemachine.CinemachineVirtualCamera targetVirtualCamera = null;
#endif
        [SerializeField]
        [Tooltip("The camera that the FOV kick should be applied to.")]
        [FormerlySerializedAs("m_TargetCamera")]
        private Camera targetCamera = null;

        // The original field of view.
        protected float originalFOV = 0;
        // The new field of view. Created using the original field of view and adding the kick amount.
        protected float newFOV = 0;

        // Simple check to see if the module has been initialized.
        private bool hasBeenInitialized = false;

        /// <summary> Determines if FOV kick should be enabled. </summary>
        public bool EnableFOVKick { get { return enableFOVKick; } set { enableFOVKick = value; } }
        /// <summary> If true, FOV kick will use unscaled delta time. </summary>
        public bool UnscaledTime { get { return unscaledTime; } set { unscaledTime = value; } }
        /// <summary> Sets whenever the FOV kick should kick in. </summary>
        public RunAction KickWhen { get { return kickWhen; } set { kickWhen = value; UpdateNewFOV(); } }
        /// <summary> Sets how much the FOV will kick. </summary>
        public float KickAmount { get { return kickAmount; } set { kickAmount = value; } }
        /// <summary> Sets how fast the FOV will move to the new FOV. </summary>
        public float LerpTimeTo { get { return lerpTimeTo; } set { lerpTimeTo = value; } }
        /// <summary> Sets how fast the FOV will move back to the original FOV. </summary>
        public float LerpTimeFrom { get { return lerpTimeFrom; } set { lerpTimeFrom = value; } }
        /// <summary> The camera that the FOV kick should be applied to. </summary>
        public Camera TargetCamera { get { return targetCamera; } set { targetCamera = value; } }

#if GOLD_PLAYER_CINEMACHINE
        /// <summary> Allows you to use Cinemachine virtual camera instead of a direct reference to a camera. </summary>
        public bool UseCinemachine { get { return useCinemachine; } set { useCinemachine = value; } }
        /// <summary> The virtual camera that the FOV kick should be applied to. </summary>
        public Cinemachine.CinemachineVirtualCamera TargetVirtualCamera { get { return targetVirtualCamera; } set { targetVirtualCamera = value; } }
#endif

        protected override void OnInitialize()
        {
            // If FOV kick is enabled and there's no target camera, complain.
            if (enableFOVKick && !targetCamera)
            {
                throw new System.NullReferenceException("There's no Target Camera set!");
            }

            // Set hasBeenInitialized to true.
            hasBeenInitialized = true;

            // Only call code if it's enabled.
            if (enableFOVKick)
            {
#if GOLD_PLAYER_CINEMACHINE
                // If use cinemachine, assign the original FOV to the FOV of the virtual camera.
                if (useCinemachine)
                {
                    if (targetVirtualCamera != null)
                    {
                        originalFOV = targetVirtualCamera.m_Lens.FieldOfView;
                    }
#if DEBUG // Put in debug define to discard it in release builds.
                    else
                    {
                        Debug.LogWarning("There's no virtual camera assigned on the field of view kick on " + PlayerTransform.gameObject.name + ".", PlayerTransform.gameObject);
                    }
#endif
                }
                else
#endif
                {
                    if (targetCamera != null)
                    {
                        // Get the original FOV from the target camera.
                        originalFOV = targetCamera.fieldOfView;
                    }
#if DEBUG // Put in debug define to discard it in release builds.
                    else
                    {
                        Debug.LogWarning("There's no target camera assigned on the field of view kick on " + PlayerTransform.gameObject.name + ".", PlayerTransform.gameObject);
                    }
#endif
                }
                // Update the new FOV.
                UpdateNewFOV();
            }
        }

        /// <summary>
        /// Updates the target FOV.
        /// </summary>
        private void UpdateNewFOV()
        {
            // If there's no target camera, stop here.
            if (targetCamera == null)
            {
                return;
            }

#if GOLD_PLAYER_CINEMACHINE
            if (useCinemachine)
            {
                newFOV = targetVirtualCamera.m_Lens.FieldOfView + kickAmount;
            }
            else
            {
                newFOV = targetCamera.fieldOfView + kickAmount;
            }
#else
            // Create the new FOV by taking the original FOV and adding kick amount.
            newFOV = targetCamera.fieldOfView + kickAmount;
#endif
        }

        public override void OnUpdate(float deltaTime)
        {
            if (unscaledTime)
            {
                deltaTime = Time.unscaledDeltaTime;
            }

            HandleFOV(deltaTime);
        }

        /// <summary>
        /// Runs the logic that applies the FOV kick in the right situations.
        /// </summary>
        protected virtual void HandleFOV(float deltaTime)
        {
            // If FOV kick is disabled, stop here.
            if (!enableFOVKick)
            {
                return;
            }

            // If "Initialize" hasn't been called yet, complain and stop here.
            if (!hasBeenInitialized)
            {
                Debug.LogError("You need to call 'Initialize()' on your FOV kick before using it!");
                return;
            }

            // If the kick should be enabled only when move speed is above walk speed, do the FOV kick when 'isRunning' is true.
            // Else do it when 'isRunning' is true and the run button is being held down.
            if (kickWhen == RunAction.FasterThanRunSpeed)
            {
                // Do FOV kick if 'isRunning' is true.
                DoFOV(PlayerController.Movement.IsRunning, deltaTime);
            }
            else if (kickWhen == RunAction.FasterThanRunSpeedAndPressingRun)
            {
                // Do FOV kick if 'isRunning' is true and the run button is being held down.
                DoFOV(GetButton(PlayerController.Movement.RunInput) && PlayerController.Movement.IsRunning, deltaTime);
            }
        }

        /// <summary>
        /// DOes the FOV kick.
        /// </summary>
        /// <param name="activate">Determines if the kick is active or not.</param>
        protected virtual void DoFOV(bool activate, float deltaTime)
        {
            // If FOV kick is disabled, stop here.
            if (!enableFOVKick)
            {
                return;
            }

            // If active is true, lerp the target camera field of view to the new FOV.
            // Else lerp it to the original FOV.
            float targetFOV = Mathf.Lerp(targetCamera.fieldOfView, activate ? newFOV : originalFOV, (activate ? lerpTimeTo : lerpTimeFrom) * deltaTime);
#if GOLD_PLAYER_CINEMACHINE
            if (useCinemachine)
            {
                targetVirtualCamera.m_Lens.FieldOfView = targetFOV;
            }
            else
            {
                targetCamera.fieldOfView = targetFOV;
            }
#else
            targetCamera.fieldOfView = targetFOV;
#endif
        }

#if UNITY_EDITOR
        public override void OnValidate()
        {
            if (Application.isPlaying)
            {
#if GOLD_PLAYER_CINEMACHINE
                if (useCinemachine && targetVirtualCamera != null)
                {
                    newFOV = targetVirtualCamera.m_Lens.FieldOfView + kickAmount;
                }
                else if (targetCamera != null)
                {
                    newFOV = targetCamera.fieldOfView + kickAmount;
                }
#else
                // Create the new FOV by taking the original FOV and adding kick amount.
                if (targetCamera != null)
                {
                    newFOV = targetCamera.fieldOfView + kickAmount;
                }
#endif
            }
        }
#endif
    }
}
