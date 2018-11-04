using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    public interface IDamageable
    {
        void TakeDamage(int damageAmount, RaycastHit hit);
    }
}
