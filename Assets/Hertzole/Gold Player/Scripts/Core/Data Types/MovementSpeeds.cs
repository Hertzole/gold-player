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
        [SerializeField]
        [Tooltip("The speed when moving sideways.")]
        private float m_SidewaysSpeed;
        [SerializeField]
        [Tooltip("The speed when moving backwards.")]
        private float m_BackwardsSpeed;

        /// <summary> The speed when moving forward. </summary>
        public float ForwardSpeed { get { return m_ForwardSpeed; } set { m_ForwardSpeed = value; CalculateMax(); } }
        /// <summary> The speed when moving sideways. </summary>
        public float SidewaysSpeed { get { return m_SidewaysSpeed; } set { m_SidewaysSpeed = value; CalculateMax(); } }
        /// <summary> The speed when moving backwards. </summary>
        public float BackwardsSpeed { get { return m_BackwardsSpeed; } set { m_BackwardsSpeed = value; CalculateMax(); } }
        /// <summary> The max speed out of all values. </summary>
        public float Max { get; private set; }

        public MovementSpeeds(float forwardSpeed, float sidewaysSpeed, float backwardsSpeed)
        {
            m_ForwardSpeed = forwardSpeed;
            m_SidewaysSpeed = sidewaysSpeed;
            m_BackwardsSpeed = backwardsSpeed;

            // It doesn't allow you to use functions unless you set the value first.
            Max = 0;
            CalculateMax();
        }

        /// <summary>
        /// Calculates the max value out of all the speeds.
        /// </summary>
        /// <returns>The max value.</returns>
        public void CalculateMax()
        {
            float previousMax = m_ForwardSpeed;
            if (m_SidewaysSpeed > previousMax)
                previousMax = m_SidewaysSpeed;
            if (m_BackwardsSpeed > previousMax)
                previousMax = m_BackwardsSpeed;

            Max = previousMax;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Only to be called in the Unity editor!
        /// </summary>
        public void OnValidate()
        {
            CalculateMax();
        }
#endif
    }
}
