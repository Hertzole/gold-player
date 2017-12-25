using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    //DOCUMENT: StaminaClass
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

        private float m_CurrentStamina;
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
            m_CurrentStamina = m_MaxStamina;
            m_CurrentRegenWait = m_RegenWait;
        }

        public override void OnUpdate()
        {
            HandleStamina();
        }

        protected virtual void HandleStamina()
        {
            if (!PlayerController.Movement.CanRun)
                return;

            if (!m_EnableStamina)
                return;

            if (m_DrainStaminaWhen == RunAction.MoveSpeedAboveRunSpeed)
            {
                if (PlayerController.Movement.IsRunning)
                    DrainStamina();
                else if (!GetButton(Constants.RUN_BUTTON_NAME, Constants.RUN_DEFAULT_KEY))
                    RegenStamina();
            }
            else if (m_DrainStaminaWhen == RunAction.MoveSpeedAboveRunSpeedAndRunning)
            {
                if (PlayerController.Movement.IsRunning && GetButton(Constants.RUN_BUTTON_NAME, Constants.RUN_DEFAULT_KEY))
                    DrainStamina();
                else if (!GetButton(Constants.RUN_BUTTON_NAME, Constants.RUN_DEFAULT_KEY))
                    RegenStamina();
            }

            ClampValues();
        }

        protected virtual void DrainStamina()
        {
            if (m_CurrentStamina > 0)
                m_CurrentStamina -= m_DrainRate * Time.deltaTime;

            m_CurrentRegenWait = 0;
        }

        protected virtual void RegenStamina()
        {
            if (m_CurrentRegenWait < m_RegenWait)
                m_CurrentRegenWait += 1 * Time.deltaTime;

            if (m_CurrentRegenWait >= m_RegenWait && m_CurrentStamina < m_MaxStamina)
                m_CurrentStamina += m_RegenRate * Time.deltaTime;
        }

        protected virtual void ClampValues()
        {
            if (m_CurrentStamina < 0)
                m_CurrentStamina = 0;

            if (m_CurrentStamina > m_MaxStamina)
                m_CurrentStamina = m_MaxStamina;

            if (m_CurrentRegenWait > m_RegenWait)
                m_CurrentRegenWait = m_RegenWait;

            if (m_CurrentRegenWait < 0)
                m_CurrentRegenWait = 0;
        }
    }
}
