using Hertzole.HertzLib;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    public partial class GoldPlayerWeapon
    {
        [SerializeField]
        private GameObject m_MuzzleFlashObject = null;
        public GameObject MuzzleFlashObject { get { return m_MuzzleFlashObject; } set { m_MuzzleFlashObject = value; } }
        [SerializeField]
        private float m_ObjectFlashTime = 0.05f;
        public float ObjectFlashTime { get { return m_ObjectFlashTime; } set { m_ObjectFlashTime = value; } }
        [SerializeField]
        private ParticleSystem m_MuzzleFlashParticles = null;
        public ParticleSystem MuzzleFlashParticles { get { return m_MuzzleFlashParticles; } set { m_MuzzleFlashParticles = value; } }
        [SerializeField]
        private RandomInt m_ParticleEmitAmount = new RandomInt(8, 14);
        public RandomInt ParticleEmitAmount { get { return m_ParticleEmitAmount; } set { m_ParticleEmitAmount = value; } }
        [SerializeField]
        private LineRenderer m_LineEffect = null;
        public LineRenderer LineEffect { get { return m_LineEffect; } set { m_LineEffect = value; } }
        [SerializeField]
        private float m_LineFlashTime = 0.05f;
        public float LineFlashTime { get { return m_LineFlashTime; } set { m_LineFlashTime = value; } }

        protected float m_ObjectFlashEndTime = 0;
        protected float m_LineFlashEndTime = 0;

        public event System.Action OnMuzzleFlash;

        private void InitializeEffects()
        {
            if (m_MuzzleFlashObject != null)
                m_MuzzleFlashObject.gameObject.SetActive(false);

            if (m_LineEffect != null)
            {
                m_LineEffect.useWorldSpace = true;
                m_LineEffect.enabled = false;
            }
        }

        private void EffectsUpdate()
        {
            if (m_MuzzleFlashObject != null && Time.time >= m_ObjectFlashEndTime)
                m_MuzzleFlashObject.gameObject.SetActive(false);

            if (m_LineEffect != null && m_ProjectileType == ProjectileTypeEnum.Raycast && Time.time >= m_LineFlashEndTime)
                m_LineEffect.enabled = false;
        }

        protected virtual void DoShootEffects()
        {
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
            OnMuzzleFlash?.Invoke();
#else
            if (OnMuzzleFlash != null)
                OnMuzzleFlash.Invoke();
#endif

            if (m_MuzzleFlashObject != null)
            {
                m_MuzzleFlashObject.gameObject.SetActive(true);
                m_ObjectFlashEndTime = Time.time + m_ObjectFlashTime;
            }

            if (m_MuzzleFlashParticles != null)
            {
                m_MuzzleFlashParticles.Emit(m_ParticleEmitAmount);
            }

            if (m_LineEffect != null && m_ProjectileType == ProjectileTypeEnum.Raycast)
            {
                m_LineEffect.SetPosition(0, m_LineEffect.transform.position);
                if (m_RaycastHit.transform != null)
                    m_LineEffect.SetPosition(1, m_RaycastHit.point);
                else
                    m_LineEffect.SetPosition(1, m_ShootOrigin.forward * m_ProjectileLength);

                m_LineEffect.enabled = true;
                m_LineFlashEndTime = Time.time + m_LineFlashTime;
            }
        }
    }
}
