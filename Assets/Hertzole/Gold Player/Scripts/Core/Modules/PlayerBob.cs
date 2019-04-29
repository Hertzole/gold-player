using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer.Core
{
    [System.Serializable]
    public class PlayerBob : PlayerModule
    {
        [SerializeField]
        [FormerlySerializedAs("m_BobClass")]
        private BobClass bobClass = new BobClass();

        /// <summary> Determines if the bob effect should be enabled. </summary>
        public bool EnableBob { get { return bobClass.EnableBob; } set { bobClass.EnableBob = value; } }
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
        /// <summary> How much the target will tilt when strafing. </summary>
        public float StrafeTilt { get { return bobClass.StrafeTilt; } set { bobClass.StrafeTilt = value; } }
        /// <summary> The object to bob. </summary>
        public Transform BobTarget { get { return bobClass.BobTarget; } set { bobClass.BobTarget = value; } }

        public float BobCycle { get { return bobClass.BobCycle; } }

        protected override void OnInitialize()
        {
            if (bobClass.BobTarget == null && bobClass.EnableBob)
            {
                Debug.LogError("No Bob Target set on '" + PlayerController.gameObject.name + "'!");
                return;
            }

            bobClass.Initialize();
        }

        public override void OnUpdate()
        {
            BobHandler();
        }

        protected virtual void BobHandler()
        {
            bobClass.DoBob(CharacterController.velocity, GetAxisRaw(GoldPlayerConstants.HORIZONTAL_AXIS));
        }
    }
}
