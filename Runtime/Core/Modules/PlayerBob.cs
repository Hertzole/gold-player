using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
    [System.Serializable]
    public sealed class PlayerBob : PlayerModule
    {
        [SerializeField]
        [FormerlySerializedAs("m_BobClass")]
        private BobClass bobClass = new BobClass();

        /// <summary> Determines if the bob effect should be enabled. </summary>
        public bool EnableBob { get { return bobClass.EnableBob; } set { bobClass.EnableBob = value; } }
        /// <summary> If true, bobbing will use unscaled delta time. </summary>
        public bool UnscaledTime { get { return bobClass.UnscaledTime; } set { bobClass.UnscaledTime = value; } }
        /// <summary> Sets how frequent the bob happens. </summary>
        public float BobFrequency { get { return bobClass.BobFrequency; } set { bobClass.BobFrequency = value; } }
        /// <summary> The height of the bob. </summary>
        public float BobHeight { get { return bobClass.BobHeight; } set { bobClass.BobHeight = value; } }
        /// <summary> How much the target will sway from side to side. </summary>
        public float SwayAngle { get { return bobClass.SwayAngle; } set { bobClass.SwayAngle = value; } }
        /// <summary> How much the target will move to the sides. </summary>
        public float SideMovement { get { return bobClass.SideMovement; } set { bobClass.SideMovement = value; } }
        /// <summary> Adds extra movement to the bob height. </summary>
        public float HeightMultiplier { get { return bobClass.HeightMultiplier; } set { bobClass.HeightMultiplier = value; } }
        /// <summary> Multiplies the bob frequency speed. </summary>
        public float StrideMultiplier { get { return bobClass.StrideMultiplier; } set { bobClass.StrideMultiplier = value; } }
        /// <summary> How much the target will move when landing. </summary>
        public float LandMove { get { return bobClass.LandMove; } set { bobClass.LandMove = value; } }
        /// <summary> How much the target will tilt when landing. </summary>
        public float LandTilt { get { return bobClass.LandTilt; } set { bobClass.LandTilt = value; } }
        /// <summary> If enabled, the target will tilt when strafing. </summary>
        public bool EnableStrafeTilting { get { return bobClass.EnableStrafeTilting; } set { bobClass.EnableStrafeTilting = value; } }
        /// <summary> How much the target will tilt when strafing. </summary>
        public float StrafeTilt { get { return bobClass.StrafeTilt; } set { bobClass.StrafeTilt = value; } }
        /// <summary> The object to bob. </summary>
        public Transform BobTarget { get { return bobClass.BobTarget; } set { bobClass.BobTarget = value; } }

        public float BobCycle { get { return bobClass.BobCycle; } }

#if UNITY_EDITOR
        internal BobClass BobClass { get { return bobClass; } }
#endif

        protected override void OnInitialize()
        {
            if (bobClass.EnableBob && bobClass.BobTarget == null)
            {
                Debug.LogError("No Bob Target set on '" + PlayerController.gameObject.name + "'!");
                return;
            }

            bobClass.Initialize();
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            BobHandler(deltaTime);
        }

        private void BobHandler(float deltaTime)
        {
            bobClass.DoBob(PlayerController.Velocity, deltaTime, PlayerController.Movement.MovementInput.x);
        }
    }
}
