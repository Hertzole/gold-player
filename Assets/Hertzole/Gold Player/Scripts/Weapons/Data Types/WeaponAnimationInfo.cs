using System;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    public enum AnimatorParamaterType { None = 0, Float = 1, Int = 2, Bool = 3, Trigger = 4 }

    [Serializable]
    public struct WeaponAnimationInfo
    {
        [SerializeField]
        private bool m_Enabled;
        public bool Enabled { get { return m_Enabled; } set { m_Enabled = value; } }
        [SerializeField]
        private AnimationCurve m_Curve;
        public AnimationCurve Curve { get { return m_Curve; } set { m_Curve = value; } }
        [SerializeField]
        private AnimatorParamaterType m_ParameterType;
        public AnimatorParamaterType ParameterType { get { return m_ParameterType; } set { m_ParameterType = value; } }
        [SerializeField]
        private string m_ParameterName;
        public string ParamaterName { get { return m_ParameterName; } set { m_ParameterName = value; } }

        public WeaponAnimationInfo(AnimatorParamaterType paramaterType, string parameterName)
        {
            m_Enabled = true;
            m_ParameterType = paramaterType;
            m_Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            m_ParameterName = parameterName;
        }
    }
}
