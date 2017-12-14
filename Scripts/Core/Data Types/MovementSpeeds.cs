using System;
using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    [Serializable]
    public struct MovementSpeeds
    {
        [SerializeField]
        [Tooltip("The speed when moving forward.")]
        private float m_ForwardSpeed;
        public float ForwardSpeed { get { return m_ForwardSpeed; } }
        [SerializeField]
        [Tooltip("The speed when moving sideways.")]
        private float m_SidewaysSpeed;
        public float SidewaysSpeed { get { return m_SidewaysSpeed; } }
        [SerializeField]
        [Tooltip("The speed when moving backwards.")]
        private float m_BackwardsSpeed;
        public float BackwardsSpeed { get { return m_BackwardsSpeed; } }

        public MovementSpeeds(float forwardSpeed, float sidewaysSpeed, float backwardsSpeed)
        {
            m_ForwardSpeed = forwardSpeed;
            m_SidewaysSpeed = sidewaysSpeed;
            m_BackwardsSpeed = backwardsSpeed;
        }

        public float Max()
        {
            if (m_ForwardSpeed > m_SidewaysSpeed && m_ForwardSpeed > m_BackwardsSpeed)
                return m_ForwardSpeed;
            else if (m_SidewaysSpeed > m_ForwardSpeed && m_SidewaysSpeed > m_BackwardsSpeed)
                return m_SidewaysSpeed;
            else if (m_BackwardsSpeed > m_ForwardSpeed && m_BackwardsSpeed > m_SidewaysSpeed)
                return m_BackwardsSpeed;

            Debug.LogWarning("Unknown max");
            return 0;
        }
    }
}
