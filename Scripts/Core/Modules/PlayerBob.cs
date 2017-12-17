using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    [System.Serializable]
    public class PlayerBob : PlayerModule
    {
        [SerializeField]
        private bool m_EnableBob = true;
        public bool EnableBob { get { return m_EnableBob; } set { m_EnableBob = value; } }

        [Space]

        [SerializeField]
        private float m_BobFrequency = 1.5f;
        public float BobFrequency { get { return m_BobFrequency; } set { m_BobFrequency = value; } }
        [SerializeField]
        private float m_BobHeight = 0.3f;
        public float BobHeight { get { return m_BobHeight; } set { m_BobHeight = value; } }
        [SerializeField]
        private float m_SwayAngle = 0.5f;
        public float SwayAngle { get { return m_SwayAngle; } set { m_SwayAngle = value; } }
        [SerializeField]
        private float m_SideMovement;
        public float SideMovement { get { return m_SideMovement; } set { m_SideMovement = value; } }
        [SerializeField]
        private float m_HeightSpeed = 0.3f;
        public float HeightSpeed { get { return m_HeightSpeed; } set { m_HeightSpeed = value; } }
        [SerializeField]
        private float m_StrideSpeed = 0.3f;
        public float StrideSpeed { get { return m_StrideSpeed; } set { m_StrideSpeed = value; } }

        [Space]

        [SerializeField]
        private float m_LandMove = 0.4f;
        public float LandMove { get { return m_LandMove; } set { m_LandMove = value; } }
        [SerializeField]
        private float m_LandTilt = 20f;
        public float LandTilt { get { return m_LandTilt; } set { m_LandTilt = value; } }
        [SerializeField]
        private float m_StrafeTilt = 3f;
        public float StrafeTilt { get { return m_StrafeTilt; } set { m_StrafeTilt = value; } }

        [Space]

        [SerializeField]
        private Transform m_BobTarget = null;
        public Transform BobTarget { get { return m_BobTarget; } set { m_BobTarget = value; } }

        private Vector3 m_PreviousVelocity = Vector3.zero;
        private Vector3 m_OriginalHeadLocalPosition = Vector3.zero;

        private float m_BobCycle = 0f;
        private float m_BobFade = 0f;
        private float m_SpringPos = 0f;
        private float m_SpringVelocity = 0f;
        private float m_SpringElastic = 1.1f;
        private float m_SpringDampen = 0.8f;
        private float m_SpringVelocityThreshold = 0.05f;
        private float m_SpringPositionThreshold = 0.05f;
        private float m_ZTilt = 0;
        private float m_ZTiltVelocity = 0;

        protected override void OnInit()
        {
            if (!m_EnableBob)
                return;

            if (m_BobTarget == null)
            {
                Debug.LogError("No Bob Target set on '" + PlayerController.gameObject.name + "'!");
                return;
            }

            m_OriginalHeadLocalPosition = m_BobTarget.localPosition;
        }

        public override void OnUpdate()
        {
            BobHandler();
        }

        protected virtual void BobHandler()
        {
            if (!m_EnableBob || m_BobTarget == null)
                return;

            Vector3 velocityChange = CharacterController.velocity - m_PreviousVelocity;
            m_PreviousVelocity = CharacterController.velocity;

            // vertical head position "spring simulation" for jumping/landing impacts
            m_SpringVelocity -= velocityChange.y;                         // input to spring from change in character Y velocity
            m_SpringVelocity -= m_SpringPos * m_SpringElastic;                    // elastic spring force towards zero position
            m_SpringVelocity *= m_SpringDampen;                             // damping towards zero velocity
            m_SpringPos += m_SpringVelocity * Time.deltaTime;               // output to head Y position
            m_SpringPos = Mathf.Clamp(m_SpringPos, -.3f, .3f);			// clamp spring distance

            if (Mathf.Abs(m_SpringVelocity) < m_SpringVelocityThreshold && Mathf.Abs(m_SpringPos) < m_SpringPositionThreshold)
            {
                m_SpringVelocity = 0;
                m_SpringPos = 0;
            }

            float flatVelocity = new Vector3(CharacterController.velocity.x, 0, CharacterController.velocity.z).magnitude;
            float strideLengthen = 1 + (flatVelocity * m_StrideSpeed);
            m_BobCycle += (flatVelocity / strideLengthen) * (Time.deltaTime / m_BobFrequency);

            float bobFactor = Mathf.Sin(m_BobCycle * Mathf.PI * 2);
            float bobSwayFactor = Mathf.Sin(m_BobCycle * Mathf.PI * 2 + Mathf.PI * .5f);
            bobFactor = 1 - (bobFactor * .5f + 1);
            bobFactor *= bobFactor;

            if (new Vector3(CharacterController.velocity.x, 0, CharacterController.velocity.z).magnitude < 0.1f)
                m_BobFade = Mathf.Lerp(m_BobFade, 0, Time.deltaTime);
            else
                m_BobFade = Mathf.Lerp(m_BobFade, 1, Time.deltaTime);

            float speedHeightFactor = 1 + (flatVelocity * m_HeightSpeed);

            m_ZTilt = Mathf.SmoothDamp(m_ZTilt, -GetAxisRaw("Horizontal"), ref m_ZTiltVelocity, 0.2f);

            float xPos = -m_SideMovement * bobSwayFactor;
            float yPos = m_SpringPos * m_LandMove + bobFactor * m_BobHeight * m_BobFade * speedHeightFactor;
            float xTilt = -m_SpringPos * m_LandTilt;
            float zTilt = bobSwayFactor * m_SwayAngle * m_BobFade + m_ZTilt * m_StrafeTilt;

            m_BobTarget.localPosition = m_OriginalHeadLocalPosition + new Vector3(xPos, yPos, m_BobTarget.localPosition.z);
            m_BobTarget.localRotation = Quaternion.Euler(xTilt, m_BobTarget.localRotation.y, m_BobTarget.localRotation.z + zTilt);
        }
    }
}
