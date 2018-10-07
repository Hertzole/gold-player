using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    [SelectionBase]
#if HERTZLIB_UPDATE_MANAGER
    public class GoldPlayerProjectile : MonoBehaviour, IUpdate
#else
    public class GoldPlayerProjectile : MonoBehaviour
#endif
    {
        [SerializeField]
        private float m_HitDetectRange = 1f;
        public float HitDetectRange { get { return m_HitDetectRange; } set { m_HitDetectRange = value; } }
        [SerializeField]
        private Vector3 m_HitDetectOffset;
        public Vector3 HitDetectOffset { get { return m_HitDetectOffset; } set { m_HitDetectOffset = value; } }

        protected float m_ExpireTime = 0f;
        protected float m_MoveSpeed = 0f;
        protected float m_RigidbodyForce = 0f;

        protected int m_Damage = 0;

        protected bool m_Destroyed = false;
        protected bool m_ApplyForceToRigidbody = false;

        protected ForceMode m_RigidbodyForceMode = ForceMode.Impulse;

        private RaycastHit m_Hit;

        private LayerMask m_HitLayer;

        private GoldPlayerWeapon m_MyWeapon;

        public void Initialize(GoldPlayerWeapon myWeapon, int damage, LayerMask hitLayer)
        {
            m_MyWeapon = myWeapon;

            m_ExpireTime = Time.time + m_MyWeapon.ProjectileLifetime;
            m_MoveSpeed = m_MyWeapon.ProjectileMoveSpeed;
            m_RigidbodyForce = m_MyWeapon.RigidbodyForce;

            m_Damage = damage;

            m_Destroyed = false;
            m_ApplyForceToRigidbody = m_MyWeapon.ApplyRigidbodyForce;

            m_RigidbodyForceMode = m_MyWeapon.ForceType;

            m_HitLayer = hitLayer;

            OnIntiailized();
        }

        protected virtual void OnIntiailized() { }

#if HERTZLIB_UPDATE_MANAGER
        private void OnEnable()
        {
            UpdateManager.AddUpdate(this);
        }

        private void OnDisable()
        {
            UpdateManager.RemoveUpdate(this);
        }
#endif
#if HERTZLIB_UPDATE_MANAGER
        public virtual void OnUpdate()
#else
        public virtual void Update()
#endif
        {
            DoMovement();
            DoHitDetection();
            DoLifeChecking();
        }

        protected virtual void DoMovement()
        {
            transform.Translate(Vector3.up * Time.deltaTime * m_MoveSpeed, Space.Self);
        }

        protected virtual void DoHitDetection()
        {
            Vector3 offset = transform.TransformDirection(m_HitDetectOffset);
            if (Physics.Raycast(transform.position + offset, transform.position + transform.up + offset, out m_Hit, m_HitDetectRange, m_HitLayer, QueryTriggerInteraction.Ignore) && !m_Destroyed)
            {
                m_Destroyed = true;
                OnImpact(m_Hit);
                m_MyWeapon.DestroyProjectile(this);
            }
        }

        protected virtual void DoLifeChecking()
        {
            if (Time.time >= m_ExpireTime)
            {
                m_Destroyed = true;
                m_MyWeapon.DestroyProjectile(this);
            }
        }

        protected virtual void OnImpact(RaycastHit hit)
        {
            if (hit.transform != null)
            {
                IDamageable damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.TakeDamage(m_Damage);

                if (m_ApplyForceToRigidbody)
                {
                    Rigidbody rig = hit.transform.GetComponent<Rigidbody>();
                    if (rig != null)
                        rig.AddForceAtPosition(transform.up * m_RigidbodyForce, hit.point, m_RigidbodyForceMode);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Vector3 offset = transform.TransformDirection(m_HitDetectOffset);
            Gizmos.DrawLine(transform.position + offset, transform.position + transform.up * m_HitDetectRange + offset);
            Gizmos.DrawCube(transform.position + transform.up * m_HitDetectRange + offset, Vector3.one * 0.02f);
        }
#endif
    }
}
