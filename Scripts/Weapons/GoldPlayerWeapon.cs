using Hertzole.HertzLib;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    [AddComponentMenu("Gold Player/Weapons/Gold Player Weapon")]
    public class GoldPlayerWeapon : MonoBehaviour
    {
        public enum TriggerTypeEnum { Manual, Automatic }

#if UNITY_EDITOR
        [Header("Basic Information")]
#endif
        [SerializeField]
        private string m_WeaponName = "New Weapon";
        public string WeaponName { get { return m_WeaponName; } set { m_WeaponName = value; } }
        [SerializeField]
        private RandomInt m_Damage = new RandomInt(9, 11);
        public RandomInt Damage { get { return m_Damage; } set { m_Damage = value; } }
        [SerializeField]
        private int m_MaxClip = 16;
        public int MaxClip { get { return m_MaxClip; } set { m_MaxClip = value; } }
        [SerializeField]
        private int m_MaxAmmo = 64;
        public int MaxAmmo { get { return m_MaxAmmo; } set { m_MaxAmmo = value; } }
        [SerializeField]
        private float m_FireDelay = 0.2f;
        public float FireDelay { get { return m_FireDelay; } set { m_FireDelay = value; } }
        [SerializeField]
        private TriggerTypeEnum m_TriggerType = TriggerTypeEnum.Automatic;
        public TriggerTypeEnum TriggerType { get { return m_TriggerType; } set { m_TriggerType = value; } }

#if UNITY_EDITOR
        [Header("Recoil Settings")]
#endif
        [SerializeField]
        private bool m_EnableRecoil = true;
        public bool EnableRecoil { get { return m_EnableRecoil; } set { m_EnableRecoil = value; } }
        [SerializeField]
        private float m_RecoilAmount = 5f;
        public float RecoilAmount { get { return m_RecoilAmount; } set { m_RecoilAmount = value; } }
        [SerializeField]
        private float m_KickbackAmount = 0.1f;
        public float KickbackAmount { get { return m_KickbackAmount; } set { m_KickbackAmount = value; } }
        [SerializeField]
        private float m_RecoilTime = 0.1f;
        public float RecoilTime { get { return m_RecoilTime; } set { m_RecoilTime = value; } }

#if UNITY_EDITOR
        [Header("Sound Settings")]
#endif
        [SerializeField]
        private AudioClip m_ShootSound;
        public AudioClip ShootSound { get { return m_ShootSound; } set { m_ShootSound = value; } }
        [SerializeField]
        private AudioClip m_DryShootSound;
        public AudioClip DryShootSound { get { return m_DryShootSound; } set { m_DryShootSound = value; } }

        protected LayerMask m_HitLayer;

        public virtual void Initialize(LayerMask hitLayer)
        {
            m_HitLayer = hitLayer;
        }

        public virtual void OnEquip() { }

        public virtual void OnPutAway() { }
    }
}
