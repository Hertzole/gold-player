using Hertzole.HertzLib;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
#if HERTZLIB_UPDATE_MANAGER
    public partial class GoldPlayerWeapon : MonoBehaviour, IUpdate
#else
    public partial class GoldPlayerWeapon : MonoBehaviour
#endif
    {
        [SerializeField]
        private string m_WeaponName = "New Weapon";
        public string WeaponName { get { return m_WeaponName; } set { m_WeaponName = value; } }
        [SerializeField]
        private RandomInt m_Damage = new RandomInt(9, 11);
        public RandomInt Damage { get { return m_Damage; } set { m_Damage = value; } }
        [SerializeField]
        private bool m_IsMelee = false;
        public bool IsMelee { get { return m_IsMelee; } set { m_IsMelee = value; } }
        [SerializeField]
        private float m_EquipTime = 0.2f;
        public float EquipTime { get { return m_EquipTime; } set { m_EquipTime = value; } }

        protected GoldPlayerWeapons Weapons { get; private set; }
        protected LayerMask HitLayer { get; private set; }

        public void Initialize(GoldPlayerWeapons weapons, LayerMask hitLayer)
        {
            Weapons = weapons;
            HitLayer = hitLayer;

            InitializeAmmo();
            InitializeRecoil();
            InitializeAttacking();
            InitializeAnimations();
            InitializeEffects();

            OnInitialized();
        }

        protected virtual void OnInitialized() { }

        private void OnEnable()
        {
#if HERTZLIB_UPDATE_MANAGER
            UpdateManager.AddUpdate(this);
#endif
            OnEnableAmmo();
            OnEnableAttacking();

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

#if HERTZLIB_UPDATE_MANAGER
        public void OnUpdate()
#else
        private void Update()
#endif
        {
            ReloadUpdate();
            ChargeUpdate();
            RecoilUpdate();
            EffectsUpdate();

            DoUpdate();
        }

        protected virtual void DoUpdate() { }

        public void Equip()
        {
            PlayEquipSound();
            OnEquipAnimation();

            OnEquip();
        }

        protected virtual void OnEquip() { }

        public void Unequip()
        {
            RecoilUnequip();

            OnUnequip();
        }

        protected virtual void OnUnequip() { }
    }
}
