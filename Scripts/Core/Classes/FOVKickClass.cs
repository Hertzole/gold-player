using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    /// <summary>
    /// Used to apply field of view (FOV) kick to a target camera.
    /// </summary>
    [System.Serializable]
    public class FOVKickClass : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if FOV kick should be enabled.")]
        private bool m_EnableFOVKick = true;
        [SerializeField]
        [Tooltip("Sets whenever the FOV kick should kick in.")]
        private RunAction m_KickWhen = RunAction.FasterThanRunSpeed;
        [SerializeField]
        [Tooltip("Sets how much the FOV will kick.")]
        private float m_KickAmount = 15f;
        [SerializeField]
        [Tooltip("Sets how fast the FOV will move to the new FOV.")]
        private float m_LerpTimeTo = 4f;
        [SerializeField]
        [Tooltip("Sets how fast the FOV will move back to the original FOV.")]
        private float m_LerpTimeFrom = 2.5f;

        [Space]

        [SerializeField]
        [Tooltip("The camera that the FOV kick should be applied to.")]
        private Camera m_TargetCamera = null;

        // The original field of view.
        protected float m_OriginalFOV = 0;
        // The new field of view. Created using the original field of view and adding the kick amount.
        protected float m_NewFOV = 0;

        // Simple check to see if the module has been initialized.
        private bool m_HasBeenInitialized = false;

        /// <summary> Determines if FOV kick should be enabled. </summary>
        public bool EnableFOVKick { get { return m_EnableFOVKick; } set { m_EnableFOVKick = value; } }
        /// <summary> Sets whenever the FOV kick should kick in. </summary>
        public RunAction KickWhen { get { return m_KickWhen; } set { m_KickWhen = value; UpdateNewFOV(); } }
        /// <summary> Sets how much the FOV will kick. </summary>
        public float KickAmount { get { return m_KickAmount; } set { m_KickAmount = value; } }
        /// <summary> Sets how fast the FOV will move to the new FOV. </summary>
        public float LerpTimeTo { get { return m_LerpTimeTo; } set { m_LerpTimeTo = value; } }
        /// <summary> Sets how fast the FOV will move back to the original FOV. </summary>
        public float LerpTimeFrom { get { return m_LerpTimeFrom; } set { m_LerpTimeFrom = value; } }
        /// <summary> The camera that the FOV kick should be applied to. </summary>
        public Camera TargetCamera { get { return m_TargetCamera; } set { m_TargetCamera = value; } }

        protected override void OnInitialize()
        {
            // If FOV kick is enabled and there's no target camera, complain.
            if (m_EnableFOVKick && !m_TargetCamera)
            {
                throw new System.NullReferenceException("There's no Target Camera set!");
            }

            // Set hasBeenInitialized to true.
            m_HasBeenInitialized = true;

            // Only call code if it's enabled.
            if (m_EnableFOVKick)
            {
                // Get the original FOV from the target camera.
                m_OriginalFOV = m_TargetCamera.fieldOfView;
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
            if (m_TargetCamera == null)
                return;

            // Create the new FOV by taking the original FOV and adding kick amount.
            m_NewFOV = m_TargetCamera.fieldOfView + m_KickAmount;
        }

        public override void OnUpdate()
        {
            HandleFOV();
        }

        /// <summary>
        /// Runs the logic that applies the FOV kick in the right situations.
        /// </summary>
        protected virtual void HandleFOV()
        {
            // If FOV kick is disabled, stop here.
            if (!m_EnableFOVKick)
                return;

            // If "Initialize" hasn't been called yet, complain and stop here.
            if (!m_HasBeenInitialized)
            {
                Debug.LogError("You need to call 'Initialize()' on your FOV kick before using it!");
                return;
            }

            // If the kick should be enabled only when move speed is above walk speed, do the FOV kick when 'isRunning' is true.
            // Else do it when 'isRunning' is true and the run button is being held down.
            if (m_KickWhen == RunAction.FasterThanRunSpeed)
            {
                // Do FOV kick if 'isRunning' is true.
                DoFOV(PlayerController.Movement.IsRunning);
            }
            else if (m_KickWhen == RunAction.FasterThanRunSpeedAndPressingRun)
            {
                // Do FOV kick if 'isRunning' is true and the run button is being held down.
                DoFOV(GetButton(GoldPlayerConstants.RUN_BUTTON_NAME, GoldPlayerConstants.RUN_DEFAULT_KEY) && PlayerController.Movement.IsRunning);
            }
        }

        /// <summary>
        /// DOes the FOV kick.
        /// </summary>
        /// <param name="activate">Determines if the kick is active or not.</param>
        protected virtual void DoFOV(bool activate)
        {
            // If FOV kick is disabled, stop here.
            if (!m_EnableFOVKick)
                return;

            // If active is true, lerp the target camera field of view to the new FOV.
            // Else lerp it to the original FOV.
            if (activate)
                m_TargetCamera.fieldOfView = Mathf.Lerp(m_TargetCamera.fieldOfView, m_NewFOV, m_LerpTimeTo * Time.deltaTime);
            else
                m_TargetCamera.fieldOfView = Mathf.Lerp(m_TargetCamera.fieldOfView, m_OriginalFOV, m_LerpTimeFrom * Time.deltaTime);
        }

#if UNITY_EDITOR
        public override void OnValidate()
        {
            if (m_TargetCamera != null && Application.isPlaying)
            {
                // Create the new FOV by taking the original FOV and adding kick amount.
                m_NewFOV = m_TargetCamera.fieldOfView + m_KickAmount;
            }
        }
#endif
    }
}
