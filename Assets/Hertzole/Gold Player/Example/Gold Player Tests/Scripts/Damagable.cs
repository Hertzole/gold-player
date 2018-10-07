using Hertzole.GoldPlayer.Weapons;
using UnityEngine;

public class Damagable : MonoBehaviour, IDamageable
{
    [SerializeField]
    private ParticleSystem m_Blood;

    private RaycastHit m_Hit;

    private void OnEnable()
    {
        GoldPlayerWeapon.OnHitDamagableGlobal += OnHit;
    }

    private void OnDisable()
    {
        GoldPlayerWeapon.OnHitDamagableGlobal -= OnHit;
    }

    private void OnHit(RaycastHit hit, int damage)
    {
        m_Hit = hit;
    }

    public void TakeDamage(int amount)
    {
        m_Blood.transform.position = m_Hit.point;
        m_Blood.transform.rotation = Quaternion.FromToRotation(Vector3.forward, m_Hit.normal);
        m_Blood.Emit(1);
    }
}
