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
        private QueryTriggerInteraction m_TriggerInteraction = QueryTriggerInteraction.Ignore;
        public QueryTriggerInteraction TriggerInteraction { get { return m_TriggerInteraction; } set { m_TriggerInteraction = value; } }

        protected float m_MoveSpeed = 0;
        protected float m_LifeTime = 0;
        private float m_MoveDistance = 0;

        protected int m_Damage = 0;

        private Ray m_Ray;
        private RaycastHit m_Hit;

        protected GoldPlayerWeapon m_Weapon;

        protected LayerMask m_HitLayer;

        public void Initialize(float moveSpeed, float lifeTime, LayerMask hitLayer, int damage, GoldPlayerWeapon weapon)
        {
            m_MoveSpeed = moveSpeed;
            m_LifeTime = lifeTime;

            m_Damage = damage;

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
            transform.Translate(Vector3.forward * m_MoveDistance);

            DoUpdate();
        }

        protected virtual void DoUpdate() { }

        protected void DoCollisionChecking()
        {
            m_MoveDistance = m_MoveSpeed * Time.deltaTime;
            m_Ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(m_Ray, out m_Hit, m_MoveDistance, m_HitLayer, m_TriggerInteraction))
            {
                OnHitObject(m_Hit);
            }
        }

        protected virtual void OnHitObject(RaycastHit hit)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(m_Damage, hit);
            DestroyProjectile();
        }

        public void DestroyProjectile()
        {
            m_Weapon.DestroyProjectile(this);
        }
    }
}
