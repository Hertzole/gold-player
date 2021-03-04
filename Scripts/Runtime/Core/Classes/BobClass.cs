using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
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
        [FormerlySerializedAs("m_EnableBob")]
        internal bool enableBob = true;
        [SerializeField]
        [Tooltip("If true, bobbing will use unscaled delta time.")]
        [FormerlySerializedAs("m_UnscaledTime")]
        internal bool unscaledTime = false;

        [SerializeField]
        [Tooltip("Sets how frequent the bob happens.")]
        [FormerlySerializedAs("m_BobFrequency")]
        internal float bobFrequency = 1.5f;
        [SerializeField]
        [Tooltip("The height of the bob.")]
        [FormerlySerializedAs("m_BobHeight")]
        internal float bobHeight = 0.3f;
        [SerializeField]
        [Tooltip("How much the target will sway from side to side.")]
        [FormerlySerializedAs("m_SwayAngle")]
        internal float swayAngle = 0.5f;
        [SerializeField]
        [Tooltip("How much the target will move to the sides.")]
        [FormerlySerializedAs("m_SideMovement")]
        internal float sideMovement = 0.05f;
        [SerializeField]
        [Tooltip("Adds extra movement to the bob height.")]
        [FormerlySerializedAs("m_HeightMultiplier")]
        internal float heightMultiplier = 0.3f;
        [SerializeField]
        [Tooltip("Multiplies the bob frequency speed.")]
        [FormerlySerializedAs("m_StrideMultiplier")]
        internal float strideMultiplier = 0.3f;

        [SerializeField]
        [Tooltip("How much the target will move when landing.")]
        [FormerlySerializedAs("m_LandMove")]
        internal float landMove = 0.4f;
        [SerializeField]
        [Tooltip("How much the target will tilt when landing.")]
        [FormerlySerializedAs("m_LandTilt")]
        internal float landTilt = 20f;

        [SerializeField]
        [Tooltip("If enabled, the target will tilt when strafing.")]
        internal bool enableStrafeTilting = true;
        [SerializeField]
        [Tooltip("How much the target will tilt when strafing.")]
        [FormerlySerializedAs("m_StrafeTilt")]
        internal float strafeTilt = 3f;

        [SerializeField]
        [Tooltip("The object to bob.")]
        [FormerlySerializedAs("m_BobTarget")]
        internal Transform bobTarget = null;

        private Vector3 previousVelocity = Vector3.zero;
        private Vector3 originalHeadLocalPosition = Vector3.zero;

        internal float bobCycle = 0f;
        internal float bobCycleBackup = 0;
        internal float bobFade = 0f;
        internal float springPos = 0f;
        internal float springVelocity = 0f;
        internal float springElastic = 1.1f;
        internal float springDampen = 0.8f;
        internal float springVelocityThreshold = 0.05f;
        internal float springPositionThreshold = 0.05f;
        internal float zTilt = 0;
        internal float zTiltVelocity = 0;

        /// <summary> Determines if the bob effect should be enabled. </summary>
        public bool EnableBob { get { return enableBob; } set { enableBob = value; } }
        /// <summary> If true, bobbing will use unscaled delta time. </summary>
        public bool UnscaledTime { get { return unscaledTime; } set { unscaledTime = value; } }
        /// <summary> Sets how frequent the bob happens. </summary>
        public float BobFrequency { get { return bobFrequency; } set { bobFrequency = value; } }
        /// <summary> The height of the bob. </summary>
        public float BobHeight { get { return bobHeight; } set { bobHeight = value; } }
        /// <summary> How much the target will sway from side to side. </summary>
        public float SwayAngle { get { return swayAngle; } set { swayAngle = value; } }
        /// <summary> How much the target will move to the sides. </summary>
        public float SideMovement { get { return sideMovement; } set { sideMovement = value; } }
        /// <summary> Adds extra movement to the bob height. </summary>
        public float HeightMultiplier { get { return heightMultiplier; } set { heightMultiplier = value; } }
        /// <summary> Multiplies the bob frequency speed. </summary>
        public float StrideMultiplier { get { return strideMultiplier; } set { strideMultiplier = value; } }
        /// <summary> How much the target will move when landing. </summary>
        public float LandMove { get { return landMove; } set { landMove = value; } }
        /// <summary> How much the target will tilt when landing. </summary>
        public float LandTilt { get { return landTilt; } set { landTilt = value; } }
        /// <summary> If enabled, the target will tilt when strafing. </summary>
        public bool EnableStrafeTilting { get { return enableStrafeTilting; } set { enableStrafeTilting = value; } }
        /// <summary> How much the target will tilt when strafing. </summary>
        public float StrafeTilt { get { return strafeTilt; } set { strafeTilt = value; } }
        /// <summary> The object to bob. </summary>
        public Transform BobTarget { get { return bobTarget; } set { bobTarget = value; } }

        public float BobCycle { get { return bobCycle; } }

#if UNITY_EDITOR
        internal Vector3 OriginalHeadLocalPosition { get { return originalHeadLocalPosition; } set { originalHeadLocalPosition = value; } }
#endif

        public void Initialize()
        {
            if (enableBob)
            {
                if (!bobTarget)
                {
                    Debug.LogError("No Bob Target set!");
                    originalHeadLocalPosition = Vector3.zero;
                }
                else
                {
                    originalHeadLocalPosition = bobTarget.localPosition;
                }
            }
        }

        public void DoBob(Vector3 velocity, float deltaTime, float zTiltAxis = 0)
        {
            Vector3 velocityChange = velocity - previousVelocity;
            previousVelocity = velocity;

            // Cache unscaled delta time to minimize calls to native engine code.
            if (unscaledTime)
            {
                deltaTime = Time.unscaledDeltaTime;
            }

            // Make sure to reset the values so bobbing can recover from an unstable state.
            if (springVelocity.IsNaN())
            {
                springVelocity = 0;
            }

            if (springPos.IsNaN())
            {
                springPos = 0;
            }

            // Vertical head position "spring simulation" for jumping/landing impacts.
            // Input to spring from change in character Y velocity.
            springVelocity -= velocityChange.y;
            // Elastic spring force towards zero position.
            springVelocity -= springPos * springElastic;
            // Damping towards zero velocity.
            springVelocity *= springDampen;
            // Output to head Y position.
            springPos += springVelocity * deltaTime;
            // Clamp spring distance.
            springPos = Mathf.Clamp(springPos, -0.3f, 0.3f);

            if (Mathf.Abs(springVelocity) < springVelocityThreshold && Mathf.Abs(springPos) < springPositionThreshold)
            {
                springVelocity = 0;
                springPos = 0;
            }

            if (bobCycle.IsNaN())
            {
                bobCycle = bobCycleBackup;
            }

            float flatVelocity = new Vector3(velocity.x, 0, velocity.z).magnitude;
            float strideLengthen = 1 + (flatVelocity * strideMultiplier);
            bobCycle += (flatVelocity / strideLengthen) * (deltaTime / bobFrequency);
            if (!bobCycle.IsNaN())
            {
                bobCycleBackup = bobCycle;
            }

            // Stop here instead because if head bob is disabled it can mess up the step sounds cycle.
            if (!enableBob || bobTarget == null)
            {
                return;
            }

            float bobFactor = Mathf.Sin(bobCycle * Mathf.PI * 2);
            float bobSwayFactor = Mathf.Sin(bobCycle * Mathf.PI * 2 + Mathf.PI * .5f);
            bobFactor = 1 - (bobFactor * .5f + 1);
            bobFactor *= bobFactor;

            float fadeTarget = new Vector3(velocity.x, 0, velocity.z).magnitude < 0.1f ? 0 : 1;
            bobFade = Mathf.Lerp(bobFade, fadeTarget, deltaTime);

            float speedHeightFactor = 1 + (flatVelocity * heightMultiplier);

            // If strafe tilting isn't enabled, just set it to 0 to stop the effect.
            if (!enableStrafeTilting)
            {
                zTiltAxis = 0;
            }

            this.zTilt = Mathf.SmoothDamp(this.zTilt, -zTiltAxis, ref zTiltVelocity, 0.2f, Mathf.Infinity, deltaTime);

            float xPos = -sideMovement * bobSwayFactor;
            float yPos = springPos * landMove + bobFactor * bobHeight * bobFade * speedHeightFactor;
            float xTilt = -springPos * landTilt;
            float zTilt = bobSwayFactor * swayAngle * bobFade + this.zTilt * strafeTilt;

            Vector3 targetPosition = originalHeadLocalPosition + new Vector3(xPos, yPos, 0);

            if (!targetPosition.IsNaN())
            {
                bobTarget.localPosition = targetPosition;
            }

            if (xTilt.IsNaN())
            {
                xTilt = 0;
            }

            if (zTilt.IsNaN())
            {
                zTilt = 0;
            }

            Quaternion targetRotation = Quaternion.Euler(xTilt, bobTarget.localRotation.y, bobTarget.localRotation.z + zTilt);

            if (!targetRotation.IsNaN())
            {
                bobTarget.localRotation = targetRotation;
            }
        }
    }
}
