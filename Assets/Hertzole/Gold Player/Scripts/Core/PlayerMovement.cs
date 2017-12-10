using UnityEngine;

//TODO: Add tooltips
namespace Hertzole.GoldPlayer.Core
{
    [System.Serializable]
    public struct MovementSpeeds
    {
        [SerializeField]
        private float m_ForwardSpeed;
        public float ForwardSpeed { get { return m_ForwardSpeed; } }
        [SerializeField]
        private float m_SidewaysSpeed;
        public float SidewaysSpeed { get { return m_SidewaysSpeed; } }
        [SerializeField]
        private float m_BackwardsSpeed;
        public float BackwardsSpeed { get { return m_BackwardsSpeed; } }

        public MovementSpeeds(float forwardSpeed, float sidewaysSpeed, float backwardsSpeed)
        {
            m_ForwardSpeed = forwardSpeed;
            m_SidewaysSpeed = sidewaysSpeed;
            m_BackwardsSpeed = backwardsSpeed;
        }
    }

    [System.Serializable]
    public class PlayerMovement : PlayerModule
    {
        [SerializeField]
        private MovementSpeeds m_WalkingSpeeds = new MovementSpeeds(4f, 3.5f, 2.5f);
        public MovementSpeeds WalkingSpeeds { get { return m_WalkingSpeeds; } set { m_WalkingSpeeds = value; /*TODO: If walking, set move speed to value */ } }
        [SerializeField]
        private bool m_CanRun = true;
        public bool CanRun { get { return m_CanRun; } set { m_CanRun = value; } }
        [SerializeField]
        private MovementSpeeds m_RunSpeeds = new MovementSpeeds(7f, 5.5f, 5f);
        public MovementSpeeds RunSpeeds { get { return m_RunSpeeds; } set { m_RunSpeeds = value; /*TODO: If running, set move speed to value */ } }

        private MovementSpeeds m_MoveSpeed = new MovementSpeeds();
    }
}
