using System.Collections.Generic;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    public partial class GoldPlayerWeapon
    {
        public enum TriggerTypeEnum { Manual = 0, Automatic = 1 }
        public enum ProjectileTypeEnum { Raycast = 0, Prefab = 1 }
        public enum BulletSpreadTypeEnum { NoSpread = 0, RandomSpread = 1, FixedSpread = 2 }

        [SerializeField]
        private float m_FireDelay = 0.2f;
        public float FireDelay { get { return m_FireDelay; } set { m_FireDelay = value; } }
        [SerializeField]
        private float m_MeleeAttackTime = 0.4f;
        public float MeleeAttackTime { get { return m_MeleeAttackTime; } set { m_MeleeAttackTime = value; } }
        [SerializeField]
        private TriggerTypeEnum m_PrimaryAttackTrigger = TriggerTypeEnum.Automatic;
        public TriggerTypeEnum PrimaryAttackTrigger { get { return m_PrimaryAttackTrigger; } set { m_PrimaryAttackTrigger = value; } }
        [SerializeField]
        private ProjectileTypeEnum m_ProjectileType = ProjectileTypeEnum.Raycast;
        public ProjectileTypeEnum ProjectileType { get { return m_ProjectileType; } set { m_ProjectileType = value; } }
        [SerializeField]
        private float m_ProjectileLength = 1000f;
        public float ProjectileLength { get { return m_ProjectileLength; } set { m_ProjectileLength = value; } }
        [SerializeField]
        private Transform m_ShootOrigin = null;
        public Transform ShootOrigin { get { return m_ShootOrigin; } set { m_ShootOrigin = value; } }
        [SerializeField]
        private bool m_PoolPrefabs = true;
        public bool PoolPrefabs { get { return m_PoolPrefabs; } set { m_PoolPrefabs = value; } }
        [SerializeField]
        private int m_InitialPrefabPool = 20;
        public int InitialPrefabPool { get { return m_InitialPrefabPool; } set { m_InitialPrefabPool = value; } }
        [SerializeField]
        private GoldPlayerProjectile m_ProjectilePrefab = null;
        public GoldPlayerProjectile ProjectilePrefab { get { return m_ProjectilePrefab; } set { m_ProjectilePrefab = value; } }
        [SerializeField]
        private float m_ProjectileMoveSpeed = 20f;
        public float ProjectileMoveSpeed { get { return m_ProjectileMoveSpeed; } set { m_ProjectileMoveSpeed = value; } }
        [SerializeField]
        private float m_ProjectileLifeTime = 5f;
        public float ProjectileLifeTime { get { return m_ProjectileLifeTime; } set { m_ProjectileLifeTime = value; } }
        [SerializeField]
        private int m_BulletsPerShot = 1;
        public int BulletsPerShot { get { return m_BulletsPerShot; } set { m_BulletsPerShot = value; } }
        [SerializeField]
        private BulletSpreadTypeEnum m_SpreadType = BulletSpreadTypeEnum.NoSpread;
        public BulletSpreadTypeEnum SpreadType { get { return m_SpreadType; } set { m_SpreadType = value; } }
        [SerializeField]
        [Range(0, 90)]
        private float m_BulletSpread = 0f;
        public float BulletSpread { get { return m_BulletSpread; } set { m_BulletSpread = value; } }
        [SerializeField]
        private Transform[] m_BulletPoints = new Transform[0];
        public Transform[] BulletPoints { get { return m_BulletPoints; } set { m_BulletPoints = value; } }
        [SerializeField]
        private bool m_ApplyRigidbodyForce = false;
        public bool ApplyRigidbodyForce { get { return m_ApplyRigidbodyForce; } set { m_ApplyRigidbodyForce = value; } }
        [SerializeField]
        private float m_RigidbodyForce = 1f;
        public float RigidbodyForce { get { return m_RigidbodyForce; } set { m_RigidbodyForce = value; } }
        [SerializeField]
        private ForceMode m_ForceType = ForceMode.Impulse;
        public ForceMode ForceType { get { return m_ForceType; } set { m_ForceType = value; } }

        protected float m_NextFire = 0;

        private RaycastHit m_RaycastHit;

        private Transform m_PreviousHit;
        private IDamageable m_HitDamageable;
        private Rigidbody m_HitRigidbody;

        protected Stack<GoldPlayerProjectile> m_PooledProjectiles = new Stack<GoldPlayerProjectile>();
        protected List<GoldPlayerProjectile> m_ActiveProjectiles = new List<GoldPlayerProjectile>();

        private static Transform s_ProjectilePool;
        public static Transform ProjectilePool
        {
            get
            {
                if (!s_ProjectilePool)
                    s_ProjectilePool = new GameObject("Projectile Pool").transform;

                return s_ProjectilePool;
            }
        }
        public delegate void HitEvent(RaycastHit hit, int damage);
        public event HitEvent OnHit;
        public event HitEvent OnHitDamagable;
        public static event HitEvent OnHitGlobal;
        public static event HitEvent OnHitDamagableGlobal;

        private void InitializeAttacking()
        {
            if (m_ProjectileType == ProjectileTypeEnum.Prefab)
            {
                if (!m_ProjectilePrefab)
                    throw new System.NullReferenceException("There's no Projectile Prefab assigned on '" + gameObject.name + "'!");

                if (m_PoolPrefabs)
                {
                    for (int i = 0; i < m_InitialPrefabPool; i++)
                    {
                        GoldPlayerProjectile projectile = Instantiate(m_ProjectilePrefab, ProjectilePool);
                        projectile.gameObject.SetActive(false);
                        m_PooledProjectiles.Push(projectile);
                    }
                }
            }
        }

        private void OnEnableAttacking()
        {
            m_NextFire = Time.time + m_EquipTime;
        }

        public virtual void PrimaryAttack()
        {
            if (IsReloading)
                return;

            if (Time.time >= m_NextFire && !m_PlayingEquipAnimation)
            {
                m_NextFire = Time.time + m_FireDelay;
                if (HasEnoughClip)
                {
                    DoPrimaryAttack();
                }
                else if (!m_InfiniteClip && m_CurrentClip == 0 && !IsReloading)
                {
                    if (m_AutoReloadEmptyClip)
                    {
                        Reload();
                    }
                    else
                    {
                        PlayDryAttackSound();
                    }
                }
            }
        }

        protected virtual void DoPrimaryAttack()
        {
            ApplyRecoil();
            DoProjectile();
            PlayPrimaryAttackSound();
            DoShootAnimation();
            DoShootEffects();

            if (!m_InfiniteClip && m_CurrentClip > 0)
                CurrentClip--;
        }

        protected void DoProjectile()
        {
            if (HasEnoughClip)
            {
                switch (m_ProjectileType)
                {
                    case ProjectileTypeEnum.Raycast:
                        DoRaycastProjectile();
                        break;
                    case ProjectileTypeEnum.Prefab:
                        DoPrefabProjectile();
                        break;
                    default:
                        throw new System.NotSupportedException("No support for '" + m_ProjectileType + "' projectile type!");
                }
            }
        }

        protected virtual void DoRaycastProjectile()
        {
            int damage = m_Damage / m_BulletsPerShot;

            for (int i = 0; i < m_BulletsPerShot; i++)
            {
                //TODO: Implement spread type.
                Vector3 rotation = new Vector3(Random.Range(-m_BulletSpread, m_BulletSpread) / 360, Random.Range(-m_BulletSpread, m_BulletSpread) / 360, Random.Range(-m_BulletSpread, m_BulletSpread) / 360);
                Physics.Raycast(m_ShootOrigin.position, m_ShootOrigin.forward + rotation, out m_RaycastHit, m_ProjectileLength, HitLayer, QueryTriggerInteraction.Ignore);
                if (m_RaycastHit.transform != null)
                {
                    if (m_PreviousHit != m_RaycastHit.transform)
                    {
                        m_PreviousHit = m_RaycastHit.transform;
                        m_HitDamageable = m_RaycastHit.transform.GetComponent<IDamageable>();
                        if (m_ApplyRigidbodyForce)
                            m_HitRigidbody = m_RaycastHit.transform.GetComponent<Rigidbody>();
                    }

                    OnRaycastHit(damage);
                }
            }
        }

        protected virtual void OnRaycastHit(int damage)
        {
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
            OnHit?.Invoke(m_RaycastHit, damage);
            OnHitGlobal?.Invoke(m_RaycastHit, damage);
#else
            if (OnHit != null)
                OnHit.Invoke(m_RaycastHit, damage);
            if (OnHitGlobal != null)
                OnHitGlobal.Invoke(m_RaycastHit, damage);
#endif

            if (m_HitDamageable != null)
            {
                m_HitDamageable.TakeDamage(damage, m_RaycastHit);
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                OnHitDamagable?.Invoke(m_RaycastHit, damage);
                OnHitDamagableGlobal?.Invoke(m_RaycastHit, damage);
#else
                if (OnHitDamagable != null)
                    OnHitDamagable.Invoke(m_RaycastHit, damage);
                if (OnHitDamagableGlobal != null)
                    OnHitDamagableGlobal.Invoke(m_RaycastHit, damage);
#endif
            }

            if (m_ApplyRigidbodyForce && m_HitRigidbody)
                m_HitRigidbody.AddForceAtPosition(transform.forward * m_RigidbodyForce, m_RaycastHit.point, m_ForceType);

            Weapons.DoBulletDecal(m_RaycastHit);
        }

        protected virtual void DoPrefabProjectile()
        {
            int damage = m_Damage / m_BulletsPerShot;

            Quaternion rotation = Quaternion.identity;
            GoldPlayerProjectile projectile = null;

            int bulletPointIndex = 0;
            for (int i = 0; i < m_BulletsPerShot; i++)
            {
                switch (m_SpreadType)
                {
                    case BulletSpreadTypeEnum.NoSpread:
                        rotation = m_ShootOrigin.rotation;
                        break;
                    case BulletSpreadTypeEnum.RandomSpread:
                        rotation = Quaternion.Euler(m_ShootOrigin.eulerAngles.x + Random.Range(-m_BulletSpread, m_BulletSpread), m_ShootOrigin.eulerAngles.y + Random.Range(-m_BulletSpread, m_BulletSpread), m_ShootOrigin.eulerAngles.z);
                        break;
                    case BulletSpreadTypeEnum.FixedSpread:
                        {
                            if (bulletPointIndex >= m_BulletPoints.Length)
                                bulletPointIndex = 0;
                            rotation = m_BulletPoints[bulletPointIndex].rotation;
                            bulletPointIndex++;
                        }
                        break;
                    default:
                        throw new System.NotImplementedException("No support for '" + m_SpreadType + "' spread type!");
                }
                projectile = GetProjectile(m_ShootOrigin.position, rotation);
                projectile.Initialize(m_ProjectileMoveSpeed, m_ProjectileLifeTime, HitLayer, damage, this);
            }
        }

        protected virtual GoldPlayerProjectile GetProjectile(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (m_PoolPrefabs)
            {
                GoldPlayerProjectile projectile = null;

                if (m_PooledProjectiles.Count > 0)
                {
                    projectile = m_PooledProjectiles.Pop();
                    projectile.transform.SetPositionAndRotation(position, rotation);
                    if (parent != null)
                        projectile.transform.SetParent(parent);
                    projectile.gameObject.SetActive(true);
                }
                else
                {
                    projectile = Instantiate(m_ProjectilePrefab, position, rotation, parent == null ? ProjectilePool : parent);
                    projectile.gameObject.SetActive(true);
                }

                m_ActiveProjectiles.Add(projectile);

                return projectile;
            }
            else
            {
                GoldPlayerProjectile projectile = Instantiate(m_ProjectilePrefab, position, rotation, parent);
                m_ActiveProjectiles.Add(projectile);
                return projectile;
            }
        }

        public virtual void DestroyProjectile(GoldPlayerProjectile projectile)
        {
            if (m_PoolPrefabs)
            {
                projectile.gameObject.SetActive(false);
                if (m_ActiveProjectiles.Contains(projectile))
                    m_ActiveProjectiles.Remove(projectile);
                m_PooledProjectiles.Push(projectile);
            }
            else
            {
                if (m_ActiveProjectiles.Contains(projectile))
                    m_ActiveProjectiles.Remove(projectile);
                Destroy(projectile.gameObject);
            }
        }
    }
}
