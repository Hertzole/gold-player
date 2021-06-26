using System;
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
        private RunAction kickWhen = RunAction.IsRunning;
        [SerializeField]
        [Tooltip("Sets how much the FOV will kick.")]
        [FormerlySerializedAs("m_KickAmount")]
        private float kickAmount = 15f;
        [SerializeField]
        [Tooltip("Sets how fast the FOV will move to the new FOV.")]
        [FormerlySerializedAs("m_LerpTimeTo")]
        private float lerpTimeTo = 0.25f;
        [SerializeField] 
        private AnimationCurve lerpToCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField]
        [Tooltip("Sets how fast the FOV will move back to the original FOV.")]
        [FormerlySerializedAs("m_LerpTimeFrom")]
        private float lerpTimeFrom = 1f;
        [SerializeField] 
        private AnimationCurve lerpFromCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // The original field of view.
        internal float originalFOV;
        // The new field of view. Created using the original field of view and adding the kick amount.
        internal float newFOV;
        // Timer used for lerping.
        private float lerpTimer;
        private float lerpStartFieldOfView;

        // Simple check to see if the module has been initialized.
        private bool hasBeenInitialized;
        // Check used to determine if it was already kicking or not.
        private bool wasKicking;

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

        /// <summary> The new field of view with the original field of view with kick amount added. </summary>
        public float TargetFieldOfView { get { return newFOV; } }
        public float FieldOfViewDifference { get; private set; }
        
        /// <summary> True if the field of view kick is active. </summary>
        public bool IsKicking { get; private set; }
        
        // The parent player camera module.
        private PlayerCamera Camera { get { return PlayerController.Camera; } }
        
        #region Obsolete
#if UNITY_EDITOR
        /// <summary> The camera that the FOV kick should be applied to. </summary>
        [Obsolete("Use 'TargetCamera' in PlayerCamera instead.", true)]
        public Camera TargetCamera
        {
            [UnityEngine.TestTools.ExcludeFromCoverage] get { return null; } 
            [UnityEngine.TestTools.ExcludeFromCoverage] set { }
        }
#if GOLD_PLAYER_CINEMACHINE
        /// <summary> Allows you to use Cinemachine virtual camera instead of a direct reference to a camera. </summary>
        [Obsolete("Use 'UseCinemachine' in PlayerCamera instead.", true)]
        public bool UseCinemachine
        {
            [UnityEngine.TestTools.ExcludeFromCoverage] get { return false; } 
            [UnityEngine.TestTools.ExcludeFromCoverage] set { }
        }
        /// <summary> The virtual camera that the FOV kick should be applied to. </summary>
        [Obsolete("Use 'TargetVirtualCamera' in PlayerCamera instead.", true)]
        public Cinemachine.CinemachineVirtualCamera TargetVirtualCamera
        {
            [UnityEngine.TestTools.ExcludeFromCoverage] get { return null; } 
            [UnityEngine.TestTools.ExcludeFromCoverage] set { }
        }
#endif
#endif
        #endregion

        protected override void OnInitialize()
        {
            // If FOV kick is enabled and there's no target camera, complain.
            if (enableFOVKick && Camera.IsCameraNull)
            {
                Debug.LogError("There's no camera set on field of view kick!", PlayerTransform.gameObject);
                return;
            }

            // Set hasBeenInitialized to true.
            hasBeenInitialized = true;

            lerpTimer = lerpTimeFrom;

            // Only call code if it's enabled.
            if (enableFOVKick)
            {
                // Get the original FOV from the target camera.
                originalFOV = Camera.CameraFieldOfView;
                // Update the new FOV.
                UpdateNewFOV();
            }
        }

#if UNITY_EDITOR
        internal void INTERNAL__UpdateNewFOV()
        {
            UpdateNewFOV();
        }
#endif

        /// <summary>
        /// Updates the target FOV.
        /// </summary>
        private void UpdateNewFOV()
        {
            // If there's no target camera, stop here.
            if (Camera.IsCameraNull)
            {
                return;
            }

            // Create the new FOV by taking the original FOV and adding kick amount.
            newFOV = Camera.CameraFieldOfView + kickAmount;
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            HandleFOV(unscaledTime ? unscaledDeltaTime : deltaTime);
        }

#if UNITY_EDITOR
        internal void INTERNAL__ForceHandleFOV()
        {
            HandleFOV(Time.deltaTime);
        }
#endif

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

            // We need to set it to false here or else the player will just kick by standing still.
            bool kick = kickWhen != RunAction.None;

            // Only check if we need to kick if we're still kicking.
            if (kick)
            {
                // Check if the IsRunning flag is set and if the player isn't running, then we're not kicking.
                if ((kickWhen & RunAction.IsRunning) == RunAction.IsRunning && !PlayerController.Movement.IsRunning)
                {
                    kick = false;
                }
            }

            if (kick)
            {
                // Check if the PressingRun flag is set and if the button isn't pressed, then we're not kicking.
                if ((kickWhen & RunAction.PressingRun) == RunAction.PressingRun && !GetButton(PlayerController.Movement.RunInput))
                {
                    kick = false;
                }
            }

            DoFOV(kick, deltaTime);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Used for tests.
        /// </summary>
        /// <param name="activate"></param>
        internal void ForceFOV(bool activate)
        {
            DoFOV(activate, Time.deltaTime);
        }
#endif

        /// <summary>
        /// Does the FOV kick.
        /// </summary>
        /// <param name="activate">Determines if the kick is active or not.</param>
        /// <param name="deltaTime">The delta time.</param>
        protected virtual void DoFOV(bool activate, float deltaTime)
        {
            // If FOV kick is disabled or if there's no camera, stop here.
            if (!enableFOVKick || Camera.IsCameraNull)
            {
                return;
            }

            // If it was kicking but no longer, stop kicking.
            // Else if it wasn't kicking but is now, start kicking.
            if (wasKicking && !activate)
            {
                lerpTimer = 0;
                wasKicking = false;
                lerpStartFieldOfView = Camera.CameraFieldOfView;
                IsKicking = false;
            }
            else if (!wasKicking && activate)
            {
                lerpTimer = 0;
                wasKicking = true;
                lerpStartFieldOfView = Camera.CameraFieldOfView;
                IsKicking = true;
            }

            float targetFOV;

            if (activate)
            {
                // As long as the lerpTimeTo is above 0 and the lerp timer is less than the time to lerp, lerp the target FOV value.
                // Else just set it directly.
                if (lerpTimeTo > 0 && lerpTimer < lerpTimeTo)
                {
                    // Lerp to the target field of view.
                    targetFOV = Mathf.Lerp(lerpStartFieldOfView, newFOV, lerpToCurve.Evaluate(lerpTimer / lerpTimeTo));
                    lerpTimer += deltaTime;
                }
                else
                {
                    targetFOV = newFOV;
                }
            }
            else
            {
                // As long as the lerpTimeFrom is above 0 and the lerp timer is less than the time to lerp, lerp the target FOV value.
                // Else just set it directly.
                if (lerpTimeFrom > 0 && lerpTimer < lerpTimeFrom)
                {
                    // Lerp to the original field of view.
                    targetFOV = Mathf.Lerp(lerpStartFieldOfView, originalFOV, lerpFromCurve.Evaluate(lerpTimer / lerpTimeFrom));
                    lerpTimer += deltaTime;
                }
                else
                {
                    targetFOV = originalFOV;
                }
            }

            FieldOfViewDifference = targetFOV - originalFOV;

            Camera.CameraFieldOfView = targetFOV;
        }

#if UNITY_EDITOR
        public override void OnValidate()
        {
            if (Application.isPlaying && enableFOVKick && PlayerController != null && Camera != null)
            {
                if (!Camera.IsCameraNull)
                {
                    // Create the new FOV by taking the original FOV and adding kick amount.
                    newFOV = Camera.CameraFieldOfView + kickAmount;
                }
            }
        }
#endif
    }
}
