using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
    [Serializable]
    public struct MovementSpeeds
    {
        [SerializeField]
        [Tooltip("The speed when moving forward.")]
        [FormerlySerializedAs("m_ForwardSpeed")]
        private float forwardSpeed;
        [SerializeField]
        [Tooltip("The speed when moving sideways.")]
        [FormerlySerializedAs("m_SidewaysSpeed")]
        private float sidewaysSpeed;
        [SerializeField]
        [Tooltip("The speed when moving backwards.")]
        [FormerlySerializedAs("m_BackwardsSpeed")]
        private float backwardsSpeed;

        /// <summary> The speed when moving forward. </summary>
        public float ForwardSpeed { get { return forwardSpeed; } set { forwardSpeed = value; CalculateMax(); } }
        /// <summary> The speed when moving sideways. </summary>
        public float SidewaysSpeed { get { return sidewaysSpeed; } set { sidewaysSpeed = value; CalculateMax(); } }
        /// <summary> The speed when moving backwards. </summary>
        public float BackwardsSpeed { get { return backwardsSpeed; } set { backwardsSpeed = value; CalculateMax(); } }
        /// <summary> The max speed out of all values. </summary>
        public float Max { get; private set; }

        public MovementSpeeds(float forwardSpeed, float sidewaysSpeed, float backwardsSpeed)
        {
            this.forwardSpeed = forwardSpeed;
            this.sidewaysSpeed = sidewaysSpeed;
            this.backwardsSpeed = backwardsSpeed;

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
            float previousMax = forwardSpeed;
            if (sidewaysSpeed > previousMax)
            {
                previousMax = sidewaysSpeed;
            }

            if (backwardsSpeed > previousMax)
            {
                previousMax = backwardsSpeed;
            }

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
