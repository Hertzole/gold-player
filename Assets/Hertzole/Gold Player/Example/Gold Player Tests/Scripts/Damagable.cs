//using Hertzole.GoldPlayer.Weapons;
//using UnityEngine;

//public class Damagable : MonoBehaviour, IDamageable
//{
//    [SerializeField]
//    private ParticleSystem m_Blood = null;

//    public void TakeDamage(int amount, RaycastHit hit)
//    {
//        m_Blood.transform.position = hit.point;
//        m_Blood.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
//        m_Blood.Emit(1);
//    }
//}
