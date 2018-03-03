using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    /*  This head bob code is based on Unity's standard assets player released
     *  some time in 2014(?), when it was actually good. Something happened
     *  and they turned real bad. The head bob they made was actually really
     *  good. So props to Unity!
     */

    [System.Serializable]
    public class BobClass
    {
        [SerializeField]
        [Tooltip("Determines if the bob effect should be enabled.")]
        private bool m_EnableBob = true;

        [Space]

        [SerializeField]
        [Tooltip("Sets how frequent the bob happens.")]
        private float m_BobFrequency = 1.5f;
        [SerializeField]
        [Tooltip("The height of the bob.")]
        private float m_BobHeight = 0.3f;
        [SerializeField]
        [Tooltip("How much the target will sway from side to side.")]
        private float m_SwayAngle = 0.5f;
        [SerializeField]
        [Tooltip("How much the target will move to the sides.")]
        private float m_SideMovement = 0.05f;
        [SerializeField]
        [Tooltip("Adds extra movement to the bob height.")]
        private float m_HeightMultiplier = 0.3f;
        [SerializeField]
        [Tooltip("Multiplies the bob frequency speed.")]
        private float m_StrideMultiplier = 0.3f;

        [Space]

        [SerializeField]
        [Tooltip("How much the target will move when landing.")]
        private float m_LandMove = 0.4f;
        [SerializeField]
        [Tooltip("How much the target will tilt when landing.")]
        private float m_LandTilt = 20f;
        [SerializeField]
        [Tooltip("How much the target will tilt when strafing.")]
        private float m_StrafeTilt = 3f;

        [Space]

        [SerializeField]
        [Tooltip("The object to bob.")]
        private Transform m_BobTarget = null;

        private Vector3 m_PreviousVelocity = Vector3.zero;
        private Vector3 m_OriginalHeadLocalPosition = Vector3.zero;

        protected float m_BobCycle = 0f;
        protected float m_BobFade = 0f;
        protected float m_SpringPos = 0f;
        protected float m_SpringVelocity = 0f;
        protected float m_SpringElastic = 1.1f;
        protected float m_SpringDampen = 0.8f;
        protected float m_SpringVelocityThreshold = 0.05f;
        protected float m_SpringPositionThreshold = 0.05f;
        protected float m_ZTilt = 0;
        protected float m_ZTiltVelocity = 0;

        /// <summary> Determines if the bob effect should be enabled. </summary>
        public bool EnableBob { get { return m_EnableBob; } set { m_EnableBob = value; } }
        /// <summary> Sets how frequent the bob happens. </summary>
        public float BobFrequency { get { return m_BobFrequency; } set { m_BobFrequency = value; } }
        /// <summary> The height of the bob. </summary>
        public float BobHeight { get { return m_BobHeight; } set { m_BobHeight = value; } }
        /// <summary> How much the target will sway from side to side. </summary>
        public float SwayAngle { get { return m_SwayAngle; } set { m_SwayAngle = value; } }
        /// <summary> How much the target will move to the sides. </summary>
        public float SideMovement { get { return m_SideMovement; } set { m_SideMovement = value; } }
        /// <summary> Adds extra movement to the bob height. </summary>
        public float HeightMultiplier { get { return m_HeightMultiplier; } set { m_HeightMultiplier = value; } }
        /// <summary> Multiplies the bob frequency speed. </summary>
        public float StrideMultiplier { get { return m_StrideMultiplier; } set { m_StrideMultiplier = value; } }
        /// <summary> How much the target will move when landing. </summary>
        public float LandMove { get { return m_LandMove; } set { m_LandMove = value; } }
        /// <summary> How much the target will tilt when landing. </summary>
        public float LandTilt { get { return m_LandTilt; } set { m_LandTilt = value; } }
        /// <summary> How much the target will tilt when strafing. </summary>
        public float StrafeTilt { get { return m_StrafeTilt; } set { m_StrafeTilt = value; } }
        /// <summary> The object to bob. </summary>
        public Transform BobTarget { get { return m_BobTarget; } set { m_BobTarget = value; } }

        public float BobCycle { get { return m_BobCycle; } }

        public void Initialize()
        {
            if (m_BobTarget == null && m_EnableBob)
            {
                Debug.LogError("No Bob Target set!");
                return;
            }
            else if (!m_EnableBob && m_BobTarget == null)
                return;

            m_OriginalHeadLocalPosition = m_BobTarget.localPosition;
        }

        public void DoBob(Vector3 velocity)
        {
            DoBob(velocity, 0);
        }

        public void DoBob(Vector3 velocity, float zTiltAxis)
        {
            if (!m_EnableBob || m_BobTarget == null)
                return;

            Vector3 velocityChange = velocity - m_PreviousVelocity;
            m_PreviousVelocity = velocity;

            // Vertical head position "spring simulation" for jumping/landing impacts.
            // Input to spring from change in character Y velocity.
            m_SpringVelocity -= velocityChange.y;
            // Elastic spring force towards zero position.
            m_SpringVelocity -= m_SpringPos * m_SpringElastic;
            // Damping towards zero velocity.
            m_SpringVelocity *= m_SpringDampen;
            // Output to head Y position.
            m_SpringPos += m_SpringVelocity * Time.deltaTime;
            // Clamp spring distance.
            m_SpringPos = Mathf.Clamp(m_SpringPos, -.3f, .3f);

            if (Mathf.Abs(m_SpringVelocity) < m_SpringVelocityThreshold && Mathf.Abs(m_SpringPos) < m_SpringPositionThreshold)
            {
                m_SpringVelocity = 0;
                m_SpringPos = 0;
            }

            float flatVelocity = new Vector3(velocity.x, 0, velocity.z).magnitude;
            float strideLengthen = 1 + (flatVelocity * m_StrideMultiplier);
            m_BobCycle += (flatVelocity / strideLengthen) * (Time.deltaTime / m_BobFrequency);

            float bobFactor = Mathf.Sin(m_BobCycle * Mathf.PI * 2);
            float bobSwayFactor = Mathf.Sin(m_BobCycle * Mathf.PI * 2 + Mathf.PI * .5f);
            bobFactor = 1 - (bobFactor * .5f + 1);
            bobFactor *= bobFactor;

            if (new Vector3(velocity.x, 0, velocity.z).magnitude < 0.1f)
                m_BobFade = Mathf.Lerp(m_BobFade, 0, Time.deltaTime);
            else
                m_BobFade = Mathf.Lerp(m_BobFade, 1, Time.deltaTime);

            float speedHeightFactor = 1 + (flatVelocity * m_HeightMultiplier);

            m_ZTilt = Mathf.SmoothDamp(m_ZTilt, -zTiltAxis, ref m_ZTiltVelocity, 0.2f);

            float xPos = -m_SideMovement * bobSwayFactor;
            float yPos = m_SpringPos * m_LandMove + bobFactor * m_BobHeight * m_BobFade * speedHeightFactor;
            float xTilt = -m_SpringPos * m_LandTilt;
            float zTilt = bobSwayFactor * m_SwayAngle * m_BobFade + m_ZTilt * m_StrafeTilt;

            m_BobTarget.localPosition = m_OriginalHeadLocalPosition + new Vector3(xPos, yPos, m_BobTarget.localPosition.z);
            m_BobTarget.localRotation = Quaternion.Euler(xTilt, m_BobTarget.localRotation.y, m_BobTarget.localRotation.z + zTilt);
        }
    }
}
