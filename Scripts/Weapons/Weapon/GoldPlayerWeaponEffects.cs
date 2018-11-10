using Hertzole.HertzLib;
using System.Collections.Generic;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    public partial class GoldPlayerWeapon
    {
#if UNITY_EDITOR
        [Header("Muzzle Flash")]
#endif
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

#if UNITY_EDITOR
        [Header("Shell Ejection")]
#endif
        [SerializeField]
        private ParticleSystem m_ShellEjectParticle = null;
        public ParticleSystem ShellEjectParticle { get { return m_ShellEjectParticle; } set { m_ShellEjectParticle = value; } }
        [SerializeField]
        private RandomInt m_ShellEjectAmount = new RandomInt(1, 1);
        public RandomInt ShellEjectAmount { get { return m_ShellEjectAmount; } set { m_ShellEjectAmount = value; } }

#if UNITY_EDITOR
        [Space]
#endif

        [SerializeField]
        private Transform m_ShellEjectPoint = null;
        public Transform ShellEjectPoint { get { return m_ShellEjectPoint; } set { m_ShellEjectPoint = value; } }
        [SerializeField]
        private Rigidbody m_RigidbodyShell = null;
        public Rigidbody RigidbodyShell { get { return m_RigidbodyShell; } set { m_RigidbodyShell = value; } }
        [SerializeField]
        private int m_MaxRigidbodyShells = 10;
        public int MaxRigidbodyShells { get { return m_MaxRigidbodyShells; } set { m_MaxRigidbodyShells = value; } }
        [SerializeField]
        private RandomFloat m_ShellForce = new RandomFloat(5f, 10f);
        public RandomFloat ShellForce { get { return m_ShellForce; } set { m_ShellForce = value; } }

        protected float m_ObjectFlashEndTime = 0;
        protected float m_LineFlashEndTime = 0;

        protected int m_RigidbodyShellIndex = 0;

        public event System.Action OnMuzzleFlash;

        protected List<Rigidbody> m_RigidbodyShellsPool = new List<Rigidbody>();

        private static Transform s_ShellPool;
        public static Transform ShellPool
        {
            get
            {
                if (s_ShellPool == null)
                    s_ShellPool = new GameObject("Gun Shell Pool").transform;

                return s_ShellPool;
            }
        }

        private void InitializeEffects()
        {
            if (m_MuzzleFlashObject != null)
                m_MuzzleFlashObject.gameObject.SetActive(false);

            if (m_LineEffect != null)
            {
                m_LineEffect.useWorldSpace = true;
                m_LineEffect.enabled = false;
            }

            if (m_RigidbodyShell != null && m_ShellEjectPoint == null)
                throw new System.NullReferenceException("There's no Shell Ejection Point attached on '" + gameObject.name + "'!");

            CreateShellsPool();
        }

        protected void CreateShellsPool()
        {
            if (m_RigidbodyShell != null)
            {
                for (int i = 0; i < m_MaxRigidbodyShells; i++)
                {
                    Rigidbody shell = Instantiate(m_RigidbodyShell, ShellPool);
                    shell.gameObject.SetActive(false);
                    m_RigidbodyShellsPool.Add(shell);
                }
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

            DoMuzzleFlash();
            DoShellEjection();
        }

        protected virtual void DoMuzzleFlash()
        {
            if (m_MuzzleFlashObject != null)
            {
                m_MuzzleFlashObject.gameObject.SetActive(true);
                m_ObjectFlashEndTime = Time.time + m_ObjectFlashTime;
            }

            if (m_MuzzleFlashParticles != null)
            {
                m_MuzzleFlashParticles.Emit(m_ParticleEmitAmount);
            }

            if (m_LineEffect != null && m_ProjectileType == ProjectileTypeEnum.Raycast && m_SpreadType != BulletSpreadTypeEnum.FixedSpread)
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

        protected virtual void DoShellEjection()
        {
            if (m_ShellEjectParticle != null)
            {
                m_ShellEjectParticle.Emit(m_ShellEjectAmount);
            }

            if (m_RigidbodyShell != null)
            {
                if (m_RigidbodyShellIndex > m_RigidbodyShellsPool.Count - 1)
                    m_RigidbodyShellIndex = 0;

                Rigidbody shell = m_RigidbodyShellsPool[m_RigidbodyShellIndex];
                shell.transform.position = m_ShellEjectPoint.position;
                shell.transform.eulerAngles = m_ShellEjectPoint.eulerAngles;
                shell.gameObject.SetActive(true);
                shell.AddForce(m_ShellEjectPoint.forward * m_ShellForce.Value, ForceMode.Impulse);
                shell.AddTorque(Random.insideUnitSphere * m_ShellForce.Value);
                m_RigidbodyShellIndex++;
            }
        }
    }
}
