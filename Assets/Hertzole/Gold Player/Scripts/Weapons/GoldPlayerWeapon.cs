using Hertzole.HertzLib;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    [AddComponentMenu("Gold Player/Weapons/Gold Player Weapon")]
    public class GoldPlayerWeapon : MonoBehaviour
    {
        public enum TriggerTypeEnum { Manual, Automatic }
        public enum ProjectileTypeEnum { Raycast, Projectile }

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
        private TriggerTypeEnum m_PrimaryTriggerType = TriggerTypeEnum.Automatic;
        public TriggerTypeEnum PrimaryTriggerType { get { return m_PrimaryTriggerType; } set { m_PrimaryTriggerType = value; } }
        [SerializeField]
        private TriggerTypeEnum m_SecondaryTriggerType = TriggerTypeEnum.Manual;
        public TriggerTypeEnum SecondaryTriggerType { get { return m_SecondaryTriggerType; } set { m_SecondaryTriggerType = value; } }

#if UNITY_EDITOR
        [Header("Projectile Settings")]
#endif
        [SerializeField]
        private ProjectileTypeEnum m_ProjectileType = ProjectileTypeEnum.Raycast;
        public ProjectileTypeEnum ProjectileType { get { return m_ProjectileType; } set { m_ProjectileType = value; } }
        [SerializeField]
        private float m_ProjectileLength = 1000f;
        public float ProjectileLength { get { return m_ProjectileLength; } set { m_ProjectileLength = value; } }
        [SerializeField]
        private GoldPlayerProjectile m_ProjectilePrefab;
        public GoldPlayerProjectile ProjectilePrefab { get { return m_ProjectilePrefab; } set { m_ProjectilePrefab = value; } }

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
        [SerializeField]
        private AudioClip m_ReloadSound;
        public AudioClip ReloadSound { get { return m_ReloadSound; } set { m_ReloadSound = value; } }
        [SerializeField]
        private AudioSource m_ShootAudioSource;
        public AudioSource ShootAudioSource { get { return m_ShootAudioSource; } set { m_ShootAudioSource = value; } }
        [SerializeField]
        private AudioSource m_DryShootAudioSource;
        public AudioSource DryShootAudioSource { get { return m_DryShootAudioSource; } set { m_DryShootAudioSource = value; } }
        [SerializeField]
        private AudioSource m_ReloadAudioSource;
        public AudioSource ReloadAudioSource { get { return m_ReloadAudioSource; } set { m_ReloadAudioSource = value; } }

        public bool IsReloading { get; protected set; }

        protected LayerMask m_HitLayer;

        public virtual void Initialize(LayerMask hitLayer)
        {
            m_HitLayer = hitLayer;
        }

        public virtual void OnEquip() { }

        public virtual void OnPutAway() { }

        public virtual void PrimaryAttack()
        {
            if (IsReloading)
                return;
        }

        public virtual void SecondaryAttack() { }

        public virtual void Reload()
        {
            if (IsReloading)
                return;
        }
    }
}
