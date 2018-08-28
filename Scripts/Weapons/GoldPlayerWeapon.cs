using Hertzole.HertzLib;
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
        public enum ProjectileTypeEnum { Raycast = 0, Projectile = 1 }
        public enum AnimationTypeEnum { None = 0, CodeDriven = 1, Animation = 2 }

        [System.Serializable]
        public struct WeaponAnimationInfo
        {
            [SerializeField]
            private AnimationTypeEnum m_AnimationType;
            public AnimationTypeEnum AnimationType { get { return m_AnimationType; } set { m_AnimationType = value; } }
            [SerializeField]
            private float m_AnimationSpeed;
            public float AnimationSpeed { get { return m_AnimationSpeed; } set { m_AnimationSpeed = value; } }
            [SerializeField]
            private AnimationCurve m_Curve;
            public AnimationCurve Curve { get { return m_Curve; } set { m_Curve = value; } }
            [SerializeField]
            private AnimationClip m_Clip;
            public AnimationClip Clip { get { return m_Clip; } set { m_Clip = value; } }

            public WeaponAnimationInfo(AnimationTypeEnum animationType, float animationSpeed)
            {
                m_AnimationType = animationType;
                m_AnimationSpeed = animationSpeed;
                m_Clip = null;
                m_Curve = new AnimationCurve();
            }
        }

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
        private float m_ReloadTime = 0.8f;
        public float ReloadTIme { get { return m_ReloadTime; } set { m_ReloadTime = value; } }
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

#if UNITY_EDITOR
        [Header("Animations")]
#endif
        [SerializeField]
        private WeaponAnimationInfo m_IdleAnimation = new WeaponAnimationInfo(AnimationTypeEnum.None, 0f);
        public WeaponAnimationInfo IdleAnimation { get { return m_IdleAnimation; } set { m_IdleAnimation = value; } }
        [SerializeField]
        private WeaponAnimationInfo m_ShootAnimation = new WeaponAnimationInfo(AnimationTypeEnum.None, 0f);
        public WeaponAnimationInfo ShootAnimation { get { return m_ShootAnimation; } set { m_ShootAnimation = value; } }
        [SerializeField]
        private WeaponAnimationInfo m_ReloadAnimation = new WeaponAnimationInfo(AnimationTypeEnum.CodeDriven, 1.2f);
        public WeaponAnimationInfo ReloadAnimation { get { return m_ReloadAnimation; } set { m_ReloadAnimation = value; } }
        [SerializeField]
        private WeaponAnimationInfo m_EquipAnimation = new WeaponAnimationInfo(AnimationTypeEnum.CodeDriven, 2.5f);
        public WeaponAnimationInfo EquipAnimation { get { return m_EquipAnimation; } set { m_EquipAnimation = value; } }
        [SerializeField]
        private WeaponAnimationInfo m_PutAwayAnimation = new WeaponAnimationInfo(AnimationTypeEnum.CodeDriven, 2.5f);
        public WeaponAnimationInfo PutAwayAnimation { get { return m_PutAwayAnimation; } set { m_PutAwayAnimation = value; } }

        protected float m_NextFire = 0F;
        protected float m_OriginalAngle = 0f;
        protected float m_RecoilAngle = 0f;
        protected float m_RecoilRotationVelocity = 0f;

        protected bool m_PlayingEquipAnimation = false;
        protected bool m_PlayingPutAwayAnimation = false;

        private string m_IdleAnimationName;
        private string m_ShootAnimationName;
        private string m_ReloadAnimationName;
        private string m_EquipAnimationName;
        private string m_PutAwayAnimationName;

        protected Vector3 m_OriginalPosition = Vector3.zero;
        protected Vector3 m_RecoilSmoothVelocity = Vector3.zero;

        protected Animation m_Animation;

        private Coroutine m_EquipAnimationRoutine;

        public bool IsReloading { get; protected set; }

        protected LayerMask m_HitLayer;

        public virtual void Initialize(LayerMask hitLayer)
        {
            m_HitLayer = hitLayer;

            m_OriginalPosition = transform.localPosition;
            m_OriginalAngle = transform.localEulerAngles.x;

            SetupAnimation();
        }

        protected virtual void SetupAnimation()
        {
            AddAnimationClip(m_IdleAnimation, "idle", ref m_IdleAnimationName);
            AddAnimationClip(m_ShootAnimation, "shoot", ref m_ShootAnimationName);
            AddAnimationClip(m_ReloadAnimation, "reload", ref m_ReloadAnimationName);
            AddAnimationClip(m_EquipAnimation, "equip", ref m_EquipAnimationName);
            AddAnimationClip(m_PutAwayAnimation, "put_away", ref m_PutAwayAnimationName);
        }

        private void AddAnimationClip(WeaponAnimationInfo info, string animationName, ref string animationNameString)
        {
            if (info.AnimationType == AnimationTypeEnum.Animation)
            {
                MakeSureAnimationComponentExists();
                animationNameString = "reload";
                m_Animation.AddClip(info.Clip, animationNameString);
            }
        }

        private void MakeSureAnimationComponentExists()
        {
            if (m_Animation == null)
                m_Animation = GetComponent<Animation>();

            if (m_Animation == null)
                throw new System.NullReferenceException("There's no Animation component attached to " + gameObject.name + "!");
        }

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

        public virtual void OnEquip()
        {
            PlayEquipAnimation();
        }

        protected virtual void PlayEquipAnimation()
        {
            //switch (m_EquipAnimation.AnimationType)
            //{
            //    case AnimationTypeEnum.None:
            //        break;
            //    case AnimationTypeEnum.CodeDriven:
            //    case AnimationTypeEnum.Animation:
            //        m_EquipAnimationRoutine = StartCoroutine(AnimationEquip());
            //        break;
            //    default:
            //        throw new System.NotImplementedException("No support for animation type '" + m_EquipAnimation + "'!");
            //}
        }

        //protected virtual IEnumerator AnimationEquip()
        //{
        //    m_PlayingEquipAnimation = true;

        //    if (m_EquipAnimation.AnimationType == AnimationTypeEnum.Animation)
        //    {
        //        m_Animation.Play(m_EquipAnimationName);
        //        while (m_Animation.IsPlaying(m_EquipAnimationName))
        //        {
        //            yield return null;
        //        }
        //    }
        //    else if (m_EquipAnimation.AnimationType == AnimationTypeEnum.CodeDriven)
        //    {
        //        float curveTime = 0;
        //        float curveAmount = m_EquipAnimation.Curve.Evaluate(curveTime);
        //        transform.localPosition = m_OriginalPosition - new Vector3(0, 3, 0);


        //        while (curveAmount < 1.0f)
        //        {
        //            Debug.Log("Equip code driven");
        //            curveTime += Time.deltaTime * m_EquipAnimation.AnimationSpeed;
        //            curveAmount = m_EquipAnimation.Curve.Evaluate(curveTime);
        //            transform.localPosition = new Vector3(transform.localPosition.x, m_OriginalPosition.y * curveAmount, transform.localPosition.z);
        //            yield return null;
        //        }
        //    }

        //    m_PlayingEquipAnimation = false;

        //    Debug.Log("Done with equip animation");
        //    m_EquipAnimationRoutine = null;
        //}

        public virtual void OnPutAway() { }

        public virtual void PrimaryAttack()
        {
            if (IsReloading)
                return;

            if (Time.time >= m_NextFire)
            {
                m_NextFire = Time.time + m_FireDelay;
                Shoot();
            }
        }

        protected virtual void Shoot()
        {
            ApplyRecoil();
        }

        public virtual void SecondaryAttack() { }

        public virtual void Reload()
        {
            if (IsReloading)
                return;
        }

#if HERTZLIB_UPDATE_MANAGER
        public virtual void OnUpdate()
#else
        protected virtual void Update()
#endif
        {
            RecoilUpdate();
        }

        protected virtual void ApplyRecoil()
        {
            transform.localPosition -= Vector3.forward * m_KickbackAmount;
            m_RecoilAngle += m_RecoilAmount;
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
