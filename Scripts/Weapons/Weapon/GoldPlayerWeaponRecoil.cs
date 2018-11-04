using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    public partial class GoldPlayerWeapon
    {
        [SerializeField]
        private bool m_EnableRecoil = true;
        public bool EnableRecoil { get { return m_EnableRecoil; } set { m_EnableRecoil = value; } }
        [SerializeField]
        private Transform m_RecoilTarget = null;
        public Transform RecoilTarget { get { return m_RecoilTarget; } set { m_RecoilTarget = value; } }
        [SerializeField]
        private float m_RecoilAmount = 5f;
        public float RecoilAmount { get { return m_RecoilAmount; } set { m_RecoilAmount = value; } }
        [SerializeField]
        private float m_KickbackAmount = 0.1f;
        public float KickbackAmount { get { return m_KickbackAmount; } set { m_KickbackAmount = value; } }
        [SerializeField]
        private float m_RecoilTime = 0.1f;
        public float RecoilTime { get { return m_RecoilTime; } set { m_RecoilTime = value; } }

        protected float m_OriginalRecoilAngle = 0f;
        protected float m_RecoilAngle = 0f;
        protected float m_RecoilRotationVelocity = 0f;

        protected Vector3 m_OriginalRecoilPosition = Vector3.zero;
        protected Vector3 m_OriginalRecoilRotation = Vector3.zero;
        protected Vector3 m_RecoilSmoothVelocity = Vector3.zero;

        private void InitializeRecoil()
        {
            if (m_EnableRecoil)
            {
                if (!m_RecoilTarget)
                    throw new System.NullReferenceException("There's no Recoil Target on '" + gameObject.name + "' but recoil is enabled!");

                m_OriginalRecoilPosition = m_RecoilTarget.localPosition;
                m_OriginalRecoilRotation = m_RecoilTarget.localEulerAngles;
                m_OriginalRecoilAngle = m_RecoilTarget.localEulerAngles.x;
            }
        }

        private void RecoilUnequip()
        {
            m_RecoilRotationVelocity = 0f;
            m_RecoilSmoothVelocity = Vector3.zero;

            if (m_EnableRecoil)
            {
                m_RecoilTarget.localPosition = m_OriginalRecoilPosition;
                m_RecoilTarget.localEulerAngles = m_OriginalRecoilRotation;
            }
        }

        private void RecoilUpdate()
        {
            DoRecoil();
        }

        protected virtual void ApplyRecoil()
        {
            if (m_EnableRecoil && m_RecoilTarget && !m_PlayingEquipAnimation)
            {
                m_RecoilTarget.localPosition -= Vector3.forward * m_KickbackAmount;
                m_RecoilAngle += m_RecoilAmount;
            }
        }

        protected virtual void DoRecoil()
        {
            if (m_EnableRecoil && m_RecoilTarget && !m_PlayingEquipAnimation)
            {
                m_RecoilTarget.localPosition = Vector3.SmoothDamp(m_RecoilTarget.localPosition, m_OriginalRecoilPosition, ref m_RecoilSmoothVelocity, m_RecoilTime);
                m_RecoilAngle = Mathf.SmoothDamp(m_RecoilAngle, m_OriginalRecoilAngle, ref m_RecoilRotationVelocity, m_RecoilTime);
                m_RecoilTarget.localEulerAngles = Vector3.left * m_RecoilAngle;
            }
        }
    }
}
