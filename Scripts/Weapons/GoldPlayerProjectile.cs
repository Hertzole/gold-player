using UnityEngine;
#if HERTZLIB_UPDATE_MANAGER
using Hertzole.HertzLib;
#endif

namespace Hertzole.GoldPlayer.Weapons
{
#if HERTZLIB_UPDATE_MANAGER
    public class GoldPlayerProjectile : MonoBehaviour, IUpdate
#else
    public class GoldPlayerProjectile : MonoBehaviour
#endif
    {
        [SerializeField]
        private bool m_MoveProjectile = true;
        public bool MoveProjectile { get { return m_MoveProjectile; } set { m_MoveProjectile = value; } }
        [SerializeField]
        private bool m_HandleHitsMyself = false;
        public bool HandleHitsMyself { get { return m_HandleHitsMyself; } set { m_HandleHitsMyself = value; } }
        [SerializeField]
        private QueryTriggerInteraction m_TriggerInteraction = QueryTriggerInteraction.Ignore;
        public QueryTriggerInteraction TriggerInteraction { get { return m_TriggerInteraction; } set { m_TriggerInteraction = value; } }

        protected float m_MoveSpeed = 0;
        protected float m_LifeTime = 0;
        private float m_MoveDistance = 0;

        public int Damage { get; private set; }

        private Ray m_Ray;
        private RaycastHit m_Hit;

        protected GoldPlayerWeapon m_Weapon;

        protected LayerMask m_HitLayer;

        public delegate void ProjectileHitEvent(GoldPlayerProjectile projectile, RaycastHit hit);
        public event ProjectileHitEvent OnProjectileHit;

        public void Initialize(float moveSpeed, float lifeTime, LayerMask hitLayer, int damage, GoldPlayerWeapon weapon)
        {
            m_MoveSpeed = moveSpeed;
            m_LifeTime = Time.time + lifeTime;

            Damage = damage;

            m_Weapon = weapon;
            m_HitLayer = hitLayer;

            if (weapon == null)
                throw new System.ArgumentNullException("weapon", "GoldPlayerWeapon reference passed to projectile was null!");
        }

        private void OnEnable()
        {
#if HERTZLIB_UPDATE_MANAGER
            UpdateManager.AddUpdate(this);
#endif

            OnEnabled();
        }

        protected virtual void OnEnabled() { }

        private void OnDisable()
        {
#if HERTZLIB_UPDATE_MANAGER
            UpdateManager.RemoveUpdate(this);
#endif

            OnDisabled();
        }

        protected virtual void OnDisabled() { }

        // Update is called once per frame
#if HERTZLIB_UPDATE_MANAGER
        public void OnUpdate()
#else
        private void Update()
#endif
        {
            DoCollisionChecking();
            DoMovement();
            DoLifeTime();

            DoUpdate();
        }

        protected virtual void DoUpdate() { }

        protected virtual void DoCollisionChecking()
        {
            m_MoveDistance = m_MoveSpeed * Time.deltaTime;
            m_Ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(m_Ray, out m_Hit, m_MoveDistance, m_HitLayer, m_TriggerInteraction))
            {
                OnHitObject(m_Hit);
            }
        }

        protected virtual void DoMovement()
        {
            if (m_MoveProjectile)
                transform.Translate(Vector3.forward * m_MoveDistance);
        }

        protected virtual void DoLifeTime()
        {
            if (Time.time >= m_LifeTime)
                DestroyProjectile();
        }

        protected virtual void OnHitObject(RaycastHit hit)
        {
            CallHitEvent(hit);
            DestroyProjectile();
        }

        public void DestroyProjectile()
        {
            m_Weapon.DestroyProjectile(this);
        }

        protected void CallHitEvent(RaycastHit hit)
        {
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
            OnProjectileHit?.Invoke(this, hit);
#else
            if (OnProjectileHit != null)
                OnProjectileHit.Invoke(this, hit);
#endif
        }
    }
}
