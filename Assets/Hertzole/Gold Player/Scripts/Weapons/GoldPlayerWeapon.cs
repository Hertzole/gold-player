using Hertzole.GoldPlayer.Core;
using Hertzole.HertzLib;
using System.Collections;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    [AddComponentMenu("Gold Player/Weapons/Gold Player Weapon")]
#if HERTZLIB_UPDATE_MANAGER
    public class GoldPlayerWeapon : MonoBehaviour, IUpdate
#else
    public class GoldPlayerWeapon : MonoBehaviour
#endif
    {
        public enum TriggerTypeEnum { Manual = 0, Automatic = 1 }
        public enum ProjectileTypeEnum { Raycast = 0, Prefab = 1 }

        // Basic info
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
        private float m_MeleeAttackTime = 0f;
        public float MeleeAttackTime { get { return m_MeleeAttackTime; } set { m_MeleeAttackTime = value; } }
        [SerializeField]
        private bool m_InfiniteClip = false;
        public bool InfiniteClip { get { return m_InfiniteClip; } set { m_InfiniteClip = value; } }
        [SerializeField]
        private int m_MaxClip = 16;
        public int MaxClip { get { return m_MaxClip; } set { m_MaxClip = value; } }
        [SerializeField]
        private bool m_InfiniteAmmo = false;
        public bool InfiniteAmmo { get { return m_InfiniteAmmo; } set { m_InfiniteAmmo = value; } }
        [SerializeField]
        private int m_MaxAmmo = 64;
        public int MaxAmmo { get { return m_MaxAmmo; } set { m_MaxAmmo = value; } }
        [SerializeField]
        private float m_FireDelay = 0.2f;
        public float FireDelay { get { return m_FireDelay; } set { m_FireDelay = value; } }
        [SerializeField]
        private bool m_AutoReloadEmptyClip = true;
        public bool AutoReloadEmptyClip { get { return m_AutoReloadEmptyClip; } set { m_AutoReloadEmptyClip = value; } }
        [SerializeField]
        private bool m_CanReloadInBackground = false;
        public bool CanReloadInBackground { get { return m_CanReloadInBackground; } set { m_CanReloadInBackground = value; } }
        [SerializeField]
        private float m_ReloadTime = 0.8f;
        public float ReloadTIme { get { return m_ReloadTime; } set { m_ReloadTime = value; } }
        [SerializeField]
        private float m_EquipTime = 0.2f;
        public float EquipTime { get { return m_EquipTime; } set { m_EquipTime = value; } }
        [SerializeField]
        private TriggerTypeEnum m_PrimaryTriggerType = TriggerTypeEnum.Automatic;
        public TriggerTypeEnum PrimaryTriggerType { get { return m_PrimaryTriggerType; } set { m_PrimaryTriggerType = value; } }
        [SerializeField]
        private TriggerTypeEnum m_SecondaryTriggerType = TriggerTypeEnum.Manual;
        public TriggerTypeEnum SecondaryTriggerType { get { return m_SecondaryTriggerType; } set { m_SecondaryTriggerType = value; } }

        // Projectile info
        [SerializeField]
        private ProjectileTypeEnum m_ProjectileType = ProjectileTypeEnum.Raycast;
        public ProjectileTypeEnum ProjectileType { get { return m_ProjectileType; } set { m_ProjectileType = value; } }
        [SerializeField]
        private float m_ProjectileLength = 1000f;
        public float ProjectileLength { get { return m_ProjectileLength; } set { m_ProjectileLength = value; } }
        [SerializeField]
        private Transform m_RaycastOrigin = null;
        public Transform RaycastOrigin { get { return m_RaycastOrigin; } set { m_RaycastOrigin = value; } }
        [SerializeField]
        private GoldPlayerProjectile m_ProjectilePrefab;
        public GoldPlayerProjectile ProjectilePrefab { get { return m_ProjectilePrefab; } set { m_ProjectilePrefab = value; } }

        // Recoil settings
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

        // Sound settings
        [SerializeField]
        private AudioItem m_EquipSound;
        public AudioItem EquipSound { get { return m_EquipSound; } set { m_EquipSound = value; } }
        [SerializeField]
        private AudioItem m_PrimaryAttackSound;
        public AudioItem PrimaryAttackSound { get { return m_PrimaryAttackSound; } set { m_PrimaryAttackSound = value; } }
        [SerializeField]
        private AudioItem m_DryShootSound;
        public AudioItem DryShootSound { get { return m_DryShootSound; } set { m_DryShootSound = value; } }
        [SerializeField]
        private AudioItem m_ReloadSound;
        public AudioItem ReloadSound { get { return m_ReloadSound; } set { m_ReloadSound = value; } }
        [SerializeField]
        private AudioSource m_EquipAudioSource;
        public AudioSource EquipAudioSource { get { return m_EquipAudioSource; } set { m_EquipAudioSource = value; } }
        [SerializeField]
        private AudioSource m_PrimaryAttackAudioSource;
        public AudioSource PrimaryAttackAudioSource { get { return m_PrimaryAttackAudioSource; } set { m_PrimaryAttackAudioSource = value; } }
        [SerializeField]
        private AudioSource m_DryShootAudioSource;
        public AudioSource DryShootAudioSource { get { return m_DryShootAudioSource; } set { m_DryShootAudioSource = value; } }
        [SerializeField]
        private AudioSource m_ReloadAudioSource;
        public AudioSource ReloadAudioSource { get { return m_ReloadAudioSource; } set { m_ReloadAudioSource = value; } }

        // Animations
        [SerializeField]
        private WeaponAnimationInfo m_IdleAnimation = new WeaponAnimationInfo(WeaponAnimationType.None);
        public WeaponAnimationInfo IdleAnimation { get { return m_IdleAnimation; } set { m_IdleAnimation = value; } }
        [SerializeField]
        private WeaponAnimationInfo m_ShootAnimation = new WeaponAnimationInfo(WeaponAnimationType.None);
        public WeaponAnimationInfo ShootAnimation { get { return m_ShootAnimation; } set { m_ShootAnimation = value; } }
        [SerializeField]
        private WeaponAnimationInfo m_ReloadAnimation = new WeaponAnimationInfo(WeaponAnimationType.CodeDriven);
        public WeaponAnimationInfo ReloadAnimation { get { return m_ReloadAnimation; } set { m_ReloadAnimation = value; } }
        [SerializeField]
        private WeaponAnimationInfo m_EquipAnimation = new WeaponAnimationInfo(WeaponAnimationType.CodeDriven);
        public WeaponAnimationInfo EquipAnimation { get { return m_EquipAnimation; } set { m_EquipAnimation = value; } }

        protected float m_NextFire = 0F;
        protected float m_OriginalAngle = 0f;
        protected float m_RecoilAngle = 0f;
        protected float m_RecoilRotationVelocity = 0f;
        protected float m_FinishReloadTime = 0f;

        private int m_CurrentClip = 0;
        public int CurrentClip
        {
            get { return m_CurrentClip; }
            protected set
            {
                m_CurrentClip = value;
#if NET_4_6 || UNITY_2018_3_OR_NEWER
                OnAmmoChanged?.Invoke(m_CurrentClip, m_CurrentAmmo);
#else
                if (OnAmmoChanged != null)
                    OnAmmoChanged.Invoke(m_CurrentClip, m_CurrentAmmo);
#endif
            }
        }

        private int m_CurrentAmmo = 0;
        public int CurrentAmmo
        {
            get { return m_CurrentAmmo; }
            protected set
            {
                m_CurrentAmmo = value;
#if NET_4_6 || UNITY_2018_3_OR_NEWER
                OnAmmoChanged?.Invoke(m_CurrentClip, m_CurrentAmmo);
#else
                if (OnAmmoChanged != null)
                    OnAmmoChanged.Invoke(m_CurrentClip, m_CurrentAmmo);
#endif
            }
        }

        protected bool m_PlayingEquipAnimation = false;
        protected bool m_PlayingPutAwayAnimation = false;

        private string m_IdleAnimationName;
        private string m_ShootAnimationName;
        private string m_ReloadAnimationName;
        private string m_EquipAnimationName;
        private string m_PutAwayAnimationName;

        protected Vector3 m_OriginalPosition = Vector3.zero;
        protected Vector3 m_OriginalRotation = Vector3.zero;
        protected Vector3 m_RecoilSmoothVelocity = Vector3.zero;
        protected Vector3 m_EquipVelocity = Vector3.zero;

        protected Animator m_Animator;

        private RaycastHit m_RaycastHit;

        private Transform m_PreviousHit;
        private IDamageable m_Damageable;

        private Coroutine m_EquipAnimationRoutine;

        public bool IsReloading { get; protected set; }

        protected LayerMask m_HitLayer;

        public delegate void AmmoEvent(int clip, int ammo);
        public event AmmoEvent OnAmmoChanged;
        public event System.Action OnStartReloading;
        public event System.Action OnFinishReload;

        public virtual void Initialize(LayerMask hitLayer)
        {
            m_HitLayer = hitLayer;

            m_OriginalPosition = transform.localPosition;
            m_OriginalAngle = transform.localEulerAngles.x;
            m_OriginalRotation = transform.localEulerAngles;

            CurrentClip = m_MaxClip;
            CurrentAmmo = m_MaxAmmo;

            ValidateAnimations();
            ValidateWeapon();
        }

        protected virtual void ValidateAnimations()
        {
            //if (m_IdleAnimation.AnimationType == AnimationTypeEnum.Animator || m_ShootAnimation.AnimationType == AnimationTypeEnum.Animator ||
            //    m_ReloadAnimation.AnimationType == AnimationTypeEnum.Animator || m_EquipAnimation.AnimationType == AnimationTypeEnum.Animator ||
            //    m_PutAwayAnimation.AnimationType == AnimationTypeEnum.Animator)
            //{
            //    MakeSureAnimatorComponentExists();
            //}
        }

        protected virtual void ValidateWeapon()
        {
            if (m_ProjectileType == ProjectileTypeEnum.Raycast && m_RaycastOrigin == null)
            {
                throw new System.NullReferenceException("There's no Raycast Origin assigned to weapon '" + gameObject.name + "'!");
            }

            if (m_ProjectileType == ProjectileTypeEnum.Prefab && m_ProjectilePrefab == null)
            {
                throw new System.NullReferenceException("There's no Projectile Prefab assigned to weapon '" + gameObject.name + "'!");
            }
        }

        private void MakeSureAnimatorComponentExists()
        {
            if (m_Animator == null)
                m_Animator = GetComponent<Animator>();

            if (m_Animator == null)
                throw new System.NullReferenceException("There's no Animator component attached to " + gameObject.name + "! It requires one to play animations.");
        }

        protected virtual void OnEnable()
        {
#if HERTZLIB_UPDATE_MANAGER
            UpdateManager.AddUpdate(this);
#endif
            if (IsReloading && m_CanReloadInBackground && Time.time >= m_FinishReloadTime)
            {
                FinishReloading();
            }
            else if (IsReloading && !m_CanReloadInBackground)
            {
                m_FinishReloadTime = Time.time + m_ReloadTime + m_EquipTime;
#if NET_4_6 || UNITY_2018_3_OR_NEWER
                OnStartReloading?.Invoke();
#else
                if (OnStartReloading != null)
                    OnStartReloading.Invoke();
#endif
            }
        }

#if HERTZLIB_UPDATE_MANAGER
        protected virtual void OnDisable()
        {
            UpdateManager.RemoveUpdate(this);
        }
#endif

        public virtual void OnEquip()
        {
            if (m_EquipAudioSource)
                m_EquipSound.Play(m_EquipAudioSource);

            PlayEquipAnimation();
        }

        public virtual void OnPutAway()
        {
            m_RecoilRotationVelocity = 0f;
            m_RecoilSmoothVelocity = Vector3.zero;

            transform.localPosition = m_OriginalPosition;
            transform.localEulerAngles = m_OriginalRotation;

            if (m_EquipAnimationRoutine != null)
                StopCoroutine(m_EquipAnimationRoutine);
        }

        protected virtual void PlayEquipAnimation()
        {
            switch (m_EquipAnimation.AnimationType)
            {
                case WeaponAnimationType.None:
                    break;
                case WeaponAnimationType.CodeDriven:
                    m_EquipAnimationRoutine = StartCoroutine(AnimationEquip());
                    break;
                default:
                    throw new System.NotImplementedException("No support for animation type '" + m_EquipAnimation + "'!");
            }
        }

        protected virtual IEnumerator AnimationEquip()
        {
            m_PlayingEquipAnimation = true;
            transform.localPosition = m_OriginalPosition - new Vector3(0, 1, 0);

            float equipTime = 0;
            while (equipTime < m_EquipTime)
            {
                equipTime += Time.deltaTime;
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, m_OriginalPosition, ref m_EquipVelocity, m_EquipTime / 3);
                yield return null;
            }

            m_PlayingEquipAnimation = false;
            m_EquipAnimationRoutine = null;
        }

        public virtual void PrimaryAttack()
        {
            if (IsReloading)
                return;

            if (Time.time >= m_NextFire)
            {
                m_NextFire = Time.time + m_FireDelay;
                DoPrimaryAttack();
            }
        }

        protected virtual void DoPrimaryAttack()
        {
            ApplyRecoil();
            PlayPrimaryAttackSound();
            DoProjectile();

            if (!m_InfiniteClip && m_CurrentClip > 0)
                CurrentClip--;
            else if (!m_InfiniteClip && m_CurrentClip == 0 && m_AutoReloadEmptyClip && !IsReloading)
                Reload();
        }

        public virtual void SecondaryAttack() { }

        public virtual void Reload()
        {
            if (IsReloading || m_CurrentClip == m_MaxClip || m_CurrentAmmo == 0)
                return;

            IsReloading = true;
            m_FinishReloadTime = Time.time + m_ReloadTime;
#if NET_4_6 || UNITY_2018_3_OR_NEWER
            OnStartReloading?.Invoke();
#else
                if (OnStartReloading != null)
                    OnStartReloading.Invoke();
#endif
        }

        protected virtual void FinishReloading()
        {
            IsReloading = false;
            if (m_MaxAmmo == -1)
            {
                m_CurrentClip = m_MaxClip;
            }
            else
            {
                if (m_CurrentClip >= m_MaxClip)
                {
                    int toReload = m_MaxClip - m_CurrentClip;
                    m_CurrentClip += toReload;
                    m_CurrentAmmo -= toReload;
                }
                else
                {
                    int toReload = m_MaxClip - m_CurrentClip;
                    if (toReload > m_CurrentAmmo)
                        toReload = m_CurrentAmmo;
                    m_CurrentClip += toReload;
                    m_CurrentAmmo -= toReload;
                }
            }

#if NET_4_6 || UNITY_2018_3_OR_NEWER
            OnAmmoChanged?.Invoke(m_CurrentClip, m_CurrentAmmo);
#else
                if (OnAmmoChanged != null)
                    OnAmmoChanged.Invoke(m_CurrentClip, m_CurrentAmmo);
#endif
#if NET_4_6 || UNITY_2018_3_OR_NEWER
            OnFinishReload?.Invoke();
#else
                if (OnFinishReload != null)
                    OnFinishReload.Invoke();
#endif
        }

        protected virtual void ReloadUpdate()
        {
            if (IsReloading && Time.time >= m_FinishReloadTime)
            {
                FinishReloading();
            }
        }

#if HERTZLIB_UPDATE_MANAGER
        public virtual void OnUpdate()
#else
        protected virtual void Update()
#endif
        {
            RecoilUpdate();
            ReloadUpdate();
        }

        protected virtual void PlayPrimaryAttackSound()
        {
            if (m_CurrentClip > 0)
            {
                if (m_PrimaryAttackAudioSource)
                    m_PrimaryAttackSound.Play(m_PrimaryAttackAudioSource);
            }
            else
            {
                if (m_DryShootAudioSource)
                    m_DryShootSound.Play(m_DryShootAudioSource);
            }
        }

        private void DoProjectile()
        {
            if (m_CurrentClip > 0)
            {
                switch (m_ProjectileType)
                {
                    case ProjectileTypeEnum.Raycast:
                        DoRaycastProjectile();
                        break;
                    case ProjectileTypeEnum.Prefab:
                        DoPrefabProjectile();
                        break;
                    default:
                        throw new System.NotSupportedException("No support for '" + m_ProjectileType + "' projectile type!");
                }
            }
        }

        protected virtual void DoRaycastProjectile()
        {
            Physics.Raycast(m_RaycastOrigin.position, m_RaycastOrigin.forward, out m_RaycastHit, m_ProjectileLength, m_HitLayer, QueryTriggerInteraction.Ignore);
            if (m_RaycastHit.transform != null)
            {
                if (m_PreviousHit != m_RaycastHit.transform)
                {
                    m_PreviousHit = m_RaycastHit.transform;
#if NET_4_6 || UNITY_2018_3_OR_NEWER
                    m_Damageable = m_RaycastHit.transform?.GetComponent<IDamageable>();
#else
                    m_Damageable = m_RaycastHit.transform != null ? m_RaycastHit.transform.GetComponent<IDamageable>() : null;
#endif
                }

                if (m_Damageable != null)
                    m_Damageable.TakeDamage(m_Damage);
            }
        }

        protected virtual void DoPrefabProjectile()
        {

        }

        protected virtual void ApplyRecoil()
        {
            if (!m_PlayingEquipAnimation && m_CurrentClip > 0)
            {
                transform.localPosition -= Vector3.forward * m_KickbackAmount;
                m_RecoilAngle += m_RecoilAmount;
            }
        }

        protected virtual void RecoilUpdate()
        {
            if (!m_PlayingEquipAnimation)
            {
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, m_OriginalPosition, ref m_RecoilSmoothVelocity, m_RecoilTime);
                m_RecoilAngle = Mathf.SmoothDamp(m_RecoilAngle, m_OriginalAngle, ref m_RecoilRotationVelocity, m_RecoilTime);
                transform.localEulerAngles = Vector3.left * m_RecoilAngle;
            }
        }
    }
}
