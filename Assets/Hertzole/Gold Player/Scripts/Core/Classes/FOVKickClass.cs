using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    //DOCUMENT: FOVKickClass
    [System.Serializable]
    public class FOVKickClass : PlayerModule
    {
        [SerializeField]
        private bool m_EnableFOVKick = true;
        [SerializeField]
        private FOVKickWhen m_KickWhen = FOVKickWhen.MoveSpeedAboveRunSpeed;
        [SerializeField]
        private float m_KickAmount = 20f;
        [SerializeField]
        private float m_LerpTimeTo = 10;
        [SerializeField]
        private float m_LerpTimeFrom = 2.5f;

        [Space]

        [SerializeField]
        private Camera m_TargetCamera = null;

        protected float m_OriginalFOV = 0;
        protected float m_NewFOV = 0;

        private bool m_HasBeenInitialized = false;

        public bool EnableFOVKick { get { return m_EnableFOVKick; } set { m_EnableFOVKick = value; } }
        public FOVKickWhen KickWhen { get { return m_KickWhen; } set { m_KickWhen = value; } }
        public float KickAmount { get { return m_KickAmount; } set { m_KickAmount = value; } }
        public float LerpTimeTo { get { return m_LerpTimeTo; } set { m_LerpTimeTo = value; } }
        public float LerpTimeFrom { get { return m_LerpTimeFrom; } set { m_LerpTimeFrom = value; } }
        public Camera TargetCamera { get { return m_TargetCamera; } set { m_TargetCamera = value; } }

        protected override void OnInit()
        {
            if (m_HasBeenInitialized)
                return;

            if (m_EnableFOVKick && !m_TargetCamera)
            {
                Debug.LogError("There's no Target Camera set!");
                return;
            }

            m_HasBeenInitialized = true;

            m_OriginalFOV = m_TargetCamera.fieldOfView;
            m_NewFOV = m_TargetCamera.fieldOfView + m_KickAmount;
        }

        public override void OnUpdate()
        {
            HandleFOV();
        }

        protected virtual void HandleFOV()
        {
            if (!m_EnableFOVKick)
                return;

            if (!m_HasBeenInitialized)
            {
                Debug.LogError("You need to call 'Initialize()' on your FOV kick before using it!");
                return;
            }

            if (m_KickWhen == FOVKickWhen.MoveSpeedAboveRunSpeed)
            {
                DoFOV(PlayerController.Movement.IsRunning);
            }
            else if (m_KickWhen == FOVKickWhen.MoveSpeedAboveRunSpeedAndRunning)
            {
                DoFOV(GetButton("Run") && PlayerController.Movement.IsRunning);
            }
        }

        protected virtual void DoFOV(bool activate)
        {
            if (!m_EnableFOVKick)
                return;

            if (activate)
                m_TargetCamera.fieldOfView = Mathf.Lerp(m_TargetCamera.fieldOfView, m_NewFOV, m_LerpTimeTo * Time.deltaTime);
            else
                m_TargetCamera.fieldOfView = Mathf.Lerp(m_TargetCamera.fieldOfView, m_OriginalFOV, m_LerpTimeFrom * Time.deltaTime);
        }
    }
}
