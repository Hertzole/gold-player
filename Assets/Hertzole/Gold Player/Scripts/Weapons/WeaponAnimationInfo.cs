using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    public enum WeaponAnimationType { None = 0, CodeDriven = 1 }

    [System.Serializable]
    public struct WeaponAnimationInfo
    {
        [SerializeField]
        private WeaponAnimationType m_AnimationType;
        public WeaponAnimationType AnimationType { get { return m_AnimationType; } set { m_AnimationType = value; } }
        [SerializeField]
        private AnimationCurve m_Curve;
        public AnimationCurve Curve { get { return m_Curve; } set { m_Curve = value; } }

        public WeaponAnimationInfo(WeaponAnimationType animationType)
        {
            m_AnimationType = animationType;
            m_Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        }
    }
}
