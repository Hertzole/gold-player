//using Hertzole.GoldPlayer.Weapons;
//using UnityEngine;

//namespace Hertzole.GoldPlayer.Example
//{
//    public class ExplosionProjectile : GoldPlayerProjectile
//    {
//        [SerializeField]
//        private float m_ExplosionRadius = 10f;
//        [SerializeField]
//        private float m_ExplosionForce = 5f;

//        private Collider[] m_OverlapColliders;

//        protected override void OnHitObject(RaycastHit hit)
//        {
//            m_OverlapColliders = Physics.OverlapSphere(hit.point, m_ExplosionRadius);
//            for (int i = 0; i < m_OverlapColliders.Length; i++)
//            {
//                Rigidbody rig = m_OverlapColliders[i].GetComponent<Rigidbody>();
//                if (rig != null)
//                    rig.AddExplosionForce(m_ExplosionForce, hit.point, m_ExplosionRadius, 1, ForceMode.Impulse);

//                //GoldPlayerController player = m_OverlapColliders[i].GetComponent<GoldPlayerController>();
//                //if (player != null)
//                //    player.Movement.AddExplosionForce(m_ExplosionForce, hit.point);
//            }
//            DestroyProjectile();
//        }
//    }
//}
