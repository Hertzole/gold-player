using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
    [Serializable]
    public struct MovementSpeeds : IEquatable<MovementSpeeds>
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

#if UNITY_EDITOR || GOLD_PLAYER_DISABLE_OPTIMIZATIONS
        /// <summary> The max speed out of all values. </summary>
        public float Max { get; private set; }
#else
        [System.NonSerialized]
        public float Max;
#endif

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

        public override bool Equals(object obj)
        {
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
            return obj is MovementSpeeds speeds && Equals(speeds);
#else
            return obj is MovementSpeeds && Equals((MovementSpeeds)obj);
#endif
        }

        public bool Equals(MovementSpeeds other)
        {
            return forwardSpeed == other.forwardSpeed && sidewaysSpeed == other.sidewaysSpeed && backwardsSpeed == other.backwardsSpeed;
        }

        public override int GetHashCode()
        {
            int hashCode = -949775398;
            hashCode = hashCode * -1521134295 + forwardSpeed.GetHashCode();
            hashCode = hashCode * -1521134295 + sidewaysSpeed.GetHashCode();
            hashCode = hashCode * -1521134295 + backwardsSpeed.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(MovementSpeeds left, MovementSpeeds right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MovementSpeeds left, MovementSpeeds right)
        {
            return !(left == right);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Only to be called in the Unity editor!
        /// </summary>
        [UnityEngine.TestTools.ExcludeFromCoverage]
        public void OnValidate()
        {
            if (Application.isPlaying)
            {
                CalculateMax();
            }
        }
#endif
    }
}
