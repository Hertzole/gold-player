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
        private Transform m_BobTarget;
        public Transform BobTarget { get { return m_BobTarget; } set { m_BobTarget = value; } }
    }
}
