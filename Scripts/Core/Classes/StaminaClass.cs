using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    /// <summary>
    /// Used to calculate stamina/limited running.
    /// </summary>
    [System.Serializable]
    public class StaminaClass : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if stamina should be enabled.")]
        private bool m_EnableStamina = false;
        [SerializeField]
        [Tooltip("Sets when the stamina should be drained.")]
        private RunAction m_DrainStaminaWhen = RunAction.MoveSpeedAboveRunSpeedAndRunning;
        [SerializeField]
        [Tooltip("The maximum amount of stamina.")]
        private float m_MaxStamina = 10f;
        [SerializeField]
        [Tooltip("How much stamina will be drained per second.")]
        private float m_DrainRate = 1f;
        [SerializeField]
        [Tooltip("How much stamina will regenerate per second.")]
        private float m_RegenRate = 0.8f;
        [SerializeField]
        [Tooltip("How long it will wait before starting to regenerate stamina.")]
        private float m_RegenWait = 1f;

        // The amount current stamina.
        private float m_CurrentStamina;
        // The current regen wait time.
        private float m_CurrentRegenWait;

        /// <summary> Determines if stamina should be enabled. </summary>
        public bool EnableStamina { get { return m_EnableStamina; } set { m_EnableStamina = value; } }
        /// <summary> Sets when the stamina should be drained. </summary>
        public RunAction DrainStaminaWhen { get { return m_DrainStaminaWhen; } set { m_DrainStaminaWhen = value; } }
        /// <summary> The maximum amount of stamina. </summary>
        public float MaxStamina { get { return m_MaxStamina; } set { m_MaxStamina = value; } }
        /// <summary> How much stamina will be drained per second. </summary>
        public float DrainRate { get { return m_DrainRate; } set { m_DrainRate = value; } }
        /// <summary> "How much stamina will regenerate per second. </summary>
        public float RegenRate { get { return m_RegenRate; } set { m_RegenRate = value; } }
        /// <summary>How long it will wait before starting to regenerate stamina. </summary>
        public float RegenWait { get { return m_RegenWait; } set { m_RegenWait = value; } }

        /// <summary> The current amount of stamina. </summary>
        public float CurrentStamina { get { return m_CurrentStamina; } set { m_CurrentStamina = value; } }
        /// <summary> The current regen wait time. </summary>
        public float CurrentRegenWait { get { return m_CurrentRegenWait; } set { m_CurrentRegenWait = value; } }

        protected override void OnInit()
        {
            // Set the current stamina to the max stamina. This way we always start with a full stamina bar.
            m_CurrentStamina = m_MaxStamina;
            // Set the current regen wait to the regen wait. This way we will always start at a full regen time.
            m_CurrentRegenWait = m_RegenWait;
        }

        public override void OnUpdate()
        {
            // Do the stamina logic.
            HandleStamina();
        }

        /// <summary>
        /// Handles all the stamina logic.
        /// </summary>
        protected virtual void HandleStamina()
        {
            // There's no point in doing stamina logic if we can't run. Stop here if running is disabled.
            if (!PlayerController.Movement.CanRun)
                return;

            // Stop here if stamina is disabled.
            if (!m_EnableStamina)
                return;

            // If we should drain stamina when move speed is above walk speed, drain stamina when 'isRunning' is true.
            // Else drain it when 'isRunning' is true and the run button is being held down.
            if (m_DrainStaminaWhen == RunAction.MoveSpeedAboveRunSpeed)
            {
                // If 'isRunning' is true, drain the stamina.
                // Else if the run button is not being held down, regen the stamina.
                if (PlayerController.Movement.IsRunning)
                    DrainStamina();
                else if (!GetButton(GoldPlayerConstants.RUN_BUTTON_NAME, GoldPlayerConstants.RUN_DEFAULT_KEY))
                    RegenStamina();
            }
            else if (m_DrainStaminaWhen == RunAction.MoveSpeedAboveRunSpeedAndRunning)
            {
                // If 'isRunning' is true and the run button is being held down, drain the stamina.
                // Else if the run button is not being held down, regen the stamina.
                if (PlayerController.Movement.IsRunning && GetButton(GoldPlayerConstants.RUN_BUTTON_NAME, GoldPlayerConstants.RUN_DEFAULT_KEY))
                    DrainStamina();
                else if (!GetButton(GoldPlayerConstants.RUN_BUTTON_NAME, GoldPlayerConstants.RUN_DEFAULT_KEY))
                    RegenStamina();
            }

            // Clamps the values so they stay within range.
            ClampValues();
        }

        /// <summary>
        /// Drains the stamina.
        /// </summary>
        protected virtual void DrainStamina()
        {
            // Only drain the stamina is the current stamina is above 0.
            if (m_CurrentStamina > 0)
                m_CurrentStamina -= m_DrainRate * Time.deltaTime;

            // Set the current regen wait to 0.
            m_CurrentRegenWait = 0;
        }

        /// <summary>
        /// Does the stamina regeneration logic.
        /// </summary>
        protected virtual void RegenStamina()
        {
            // If the current regen wait is less than the regen wait, increase the current regen wait.
            if (m_CurrentRegenWait < m_RegenWait)
                m_CurrentRegenWait += 1 * Time.deltaTime;

            // If the current regen wait is the same as regen wait and current stamina is less than max stamina,
            // increase the current stamina with regen rate.
            if (m_CurrentRegenWait >= m_RegenWait && m_CurrentStamina < m_MaxStamina)
                m_CurrentStamina += m_RegenRate * Time.deltaTime;
        }

        /// <summary>
        /// Clamps current stamina and current regen wait.
        /// </summary>
        protected virtual void ClampValues()
        {
            // Make sure current stamina doesn't go below 0.
            if (m_CurrentStamina < 0)
                m_CurrentStamina = 0;

            // Make sure current stamina doesn't go above max stamina.
            if (m_CurrentStamina > m_MaxStamina)
                m_CurrentStamina = m_MaxStamina;

            // Make sure current regen wait doesn't go above regen wait.
            if (m_CurrentRegenWait > m_RegenWait)
                m_CurrentRegenWait = m_RegenWait;

            // Make sure current regen wait doesn't go below 0.
            if (m_CurrentRegenWait < 0)
                m_CurrentRegenWait = 0;
        }
    }
}
