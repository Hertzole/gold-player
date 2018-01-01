using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    //TODO: Move bob code into own class and call that using PlayerBob module.
    [System.Serializable]
    public class PlayerBob : PlayerModule
    {
        [SerializeField]
        private BobClass m_BobClass = new BobClass();

        /// <summary> Determines if the bob effect should be enabled. </summary>
        public bool EnableBob { get { return m_BobClass.EnableBob; } set { m_BobClass.EnableBob = value; } }
        /// <summary> Sets how frequent the bob happens. </summary>
        public float BobFrequency { get { return m_BobClass.BobFrequency; } set { m_BobClass.BobFrequency = value; } }
        /// <summary> The height of the bob. </summary>
        public float BobHeight { get { return m_BobClass.BobHeight; } set { m_BobClass.BobHeight = value; } }
        /// <summary> How much the target will sway from side to side. </summary>
        public float SwayAngle { get { return m_BobClass.SwayAngle; } set { m_BobClass.SwayAngle = value; } }
        /// <summary> How much the target will move to the sides. </summary>
        public float SideMovement { get { return m_BobClass.SideMovement; } set { m_BobClass.SideMovement = value; } }
        /// <summary> Adds extra movement to the bob height. </summary>
        public float HeightMultiplier { get { return m_BobClass.HeightMultiplier; } set { m_BobClass.HeightMultiplier = value; } }
        /// <summary> Multiplies the bob frequency speed. </summary>
        public float StrideMultiplier { get { return m_BobClass.StrideMultiplier; } set { m_BobClass.StrideMultiplier = value; } }
        /// <summary> How much the target will move when landing. </summary>
        public float LandMove { get { return m_BobClass.LandMove; } set { m_BobClass.LandMove = value; } }
        /// <summary> How much the target will tilt when landing. </summary>
        public float LandTilt { get { return m_BobClass.LandTilt; } set { m_BobClass.LandTilt = value; } }
        /// <summary> How much the target will tilt when strafing. </summary>
        public float StrafeTilt { get { return m_BobClass.StrafeTilt; } set { m_BobClass.StrafeTilt = value; } }
        /// <summary> The object to bob. </summary>
        public Transform BobTarget { get { return m_BobClass.BobTarget; } set { m_BobClass.BobTarget = value; } }

        public float BobCycle { get { return m_BobClass.BobCycle; } }

        protected override void OnInit()
        {
            if (m_BobClass.BobTarget == null && m_BobClass.EnableBob)
            {
                Debug.LogError("No Bob Target set on '" + PlayerController.gameObject.name + "'!");
                return;
            }

            m_BobClass.Initialize();
        }

        public override void OnUpdate()
        {
            BobHandler();
        }

        protected virtual void BobHandler()
        {
            m_BobClass.DoBob(CharacterController.velocity, GetAxisRaw(GoldPlayerConstants.HORIZONTAL_AXIS));
        }
    }
}
