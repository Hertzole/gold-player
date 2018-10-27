using Hertzole.GoldPlayer.Core;
using System.Collections.Generic;
using UnityEngine;
#if HERTZLIB_UPDATE_MANAGER
using Hertzole.HertzLib;
#endif

namespace Hertzole.GoldPlayer.Weapons
{
    [AddComponentMenu("Gold Player/Weapons/Gold Player Weapon Manager")]
#if HERTZLIB_UPDATE_MANAGER
    public class GoldPlayerWeapons : PlayerBehaviour, IUpdate
#else
    public class GoldPlayerWeapons : PlayerBehaviour
#endif
    {
        [SerializeField]
        private GoldPlayerWeapon[] m_AvailableWeapons;
        public GoldPlayerWeapon[] AvailableWeapons { get { return m_AvailableWeapons; } set { m_AvailableWeapons = value; } }
        [SerializeField]
        private LayerMask m_HitLayer;
        public LayerMask HitLayer { get { return m_HitLayer; } set { m_HitLayer = value; } }

#if UNITY_EDITOR
        [Header("Change Weapon Settings")]
#endif
        [SerializeField]
        private bool m_CanChangeWeapon = true;
        public bool CanChangeWeapon { get { return m_CanChangeWeapon; } set { m_CanChangeWeapon = value; } }
        [SerializeField]
        private bool m_CanScrollThrough = true;
        public bool CanScrollThrough { get { return m_CanScrollThrough; } set { m_CanScrollThrough = value; } }
        [SerializeField]
        private bool m_LoopScroll = true;
        public bool LoopScroll { get { return m_LoopScroll; } set { m_LoopScroll = value; } }
        [SerializeField]
        private bool m_InvertScroll = false;
        public bool InvertScroll { get { return m_InvertScroll; } set { m_InvertScroll = value; } }
        [SerializeField]
        private bool m_EnableScrollDelay = false;
        public bool EnableScrollDelay { get { return m_EnableScrollDelay; } set { m_EnableScrollDelay = value; } }
        [SerializeField]
        private float m_ScrollDelay = 0.1f;
        public float ScrollDelay { get { return m_ScrollDelay; } set { m_ScrollDelay = value; } }
        [SerializeField]
        private bool m_CanUseNumberKeys = true;
        public bool CanUseNumberKeys { get { return m_CanUseNumberKeys; } set { m_CanUseNumberKeys = value; } }
        [SerializeField]
        private bool m_CanChangeWhenReloading = true;
        public bool CanChangeWhenReloading { get { return m_CanChangeWhenReloading; } set { m_CanChangeWhenReloading = value; } }

#if UNITY_EDITOR
        [Header("Cosmetic Changes")]
#endif
        [SerializeField]
        private ParticleSystem m_BulletDecals = null;
        public ParticleSystem BulletDecals { get { return m_BulletDecals; } set { m_BulletDecals = value; } }

        protected int m_NewWeaponIndex = -1;
        protected int m_CurrentWeaponIndex = -1;
        protected int m_DecalParticleDataIndex = 0;

        protected bool m_DoPrimaryAttack = false;
        protected bool m_DoSecondaryAttack = false;

        protected float m_NextScroll = 0;

        private BulletDecalData[] m_DecalData;
        private ParticleSystem.Particle[] m_DecalParticles;

        private List<int> m_MyWeapons = new List<int>();

        protected GoldPlayerWeapon m_PreviousWeapon = null;

        public GoldPlayerWeapon CurrentWeapon
        {
            get
            {
                if (m_MyWeapons == null || m_MyWeapons.Count == 0)
                    return null;

                return m_CurrentWeaponIndex < 0 || m_CurrentWeaponIndex > m_MyWeapons.Count - 1 ? null : m_AvailableWeapons[m_MyWeapons[m_CurrentWeaponIndex]];
            }
        }

        public delegate void WeaponChangeEvent(GoldPlayerWeapon previousWeapon, GoldPlayerWeapon newWeapon);
        public event WeaponChangeEvent OnWeaponChanged;

#if HERTZLIB_UPDATE_MANAGER
        protected virtual void OnEnable()
        {
            UpdateManager.AddUpdate(this);
        }

        protected virtual OnDisable()
        {
            UpdateManager.RemoveUpdate(this);
        }
#endif

        protected virtual void Start()
        {
            for (int i = 0; i < m_AvailableWeapons.Length; i++)
            {
                m_AvailableWeapons[i].gameObject.SetActive(false);
                m_AvailableWeapons[i].Initialize(this, m_HitLayer);
            }

            //TODO: Add a way in the editor to set what weapons are included.
            AddWeapon(0);
            AddWeapon(1);
            AddWeapon(2);

            ChangeWeapon(0);

            SetupBulletDecals();
        }

        protected virtual void SetupBulletDecals()
        {
            if (m_BulletDecals)
            {
                m_DecalParticles = new ParticleSystem.Particle[m_BulletDecals.main.maxParticles];
                m_DecalData = new BulletDecalData[m_BulletDecals.main.maxParticles];
                for (int i = 0; i < m_BulletDecals.main.maxParticles; i++)
                {
                    m_DecalData[i] = new BulletDecalData();
                }
            }
        }

#if HERTZLIB_UPDATE_MANAGER
        public virtual void OnUpdate()
#else
        protected virtual void Update()
#endif
        {
            HandleWeaponChanging();
            HandlePrimaryAttacking();
            HandleSecondaryAttacking();
            HandleReloading();
        }

        protected virtual void HandleWeaponChanging()
        {
            if (!m_CanChangeWeapon)
                return;

            if (m_CanUseNumberKeys)
            {
                if (GetButtonDown("Change Weapon 1", KeyCode.Alpha1))
                    ChangeWeapon(0);
                if (GetButtonDown("Change Weapon 2", KeyCode.Alpha2))
                    ChangeWeapon(1);
                if (GetButtonDown("Change Weapon 3", KeyCode.Alpha3))
                    ChangeWeapon(2);
                if (GetButtonDown("Change Weapon 4", KeyCode.Alpha4))
                    ChangeWeapon(3);
                if (GetButtonDown("Change Weapon 5", KeyCode.Alpha5))
                    ChangeWeapon(4);
                if (GetButtonDown("Change Weapon 6", KeyCode.Alpha6))
                    ChangeWeapon(5);
                if (GetButtonDown("Change Weapon 7", KeyCode.Alpha7))
                    ChangeWeapon(6);
                if (GetButtonDown("Change Weapon 8", KeyCode.Alpha8))
                    ChangeWeapon(7);
                if (GetButtonDown("Change Weapon 9", KeyCode.Alpha9))
                    ChangeWeapon(8);
            }

            if (m_CanScrollThrough)
            {
                if (GetAxis("Change Weapon Scroll", "Mouse ScrollWheel") >= 0.1f)
                {
                    if (m_EnableScrollDelay && Time.time < m_NextScroll)
                        return;

                    if (!m_InvertScroll)
                        GoUpWeaponList();
                    else
                        GoDownWeaponList();

                    if (m_EnableScrollDelay)
                        m_NextScroll = Time.time + m_ScrollDelay;
                }

                if (GetAxis("Change Weapon Scroll", "Mouse ScrollWheel") <= -0.1f)
                {
                    if (m_EnableScrollDelay && Time.time < m_NextScroll)
                        return;

                    if (!m_InvertScroll)
                        GoDownWeaponList();
                    else
                        GoUpWeaponList();

                    if (m_EnableScrollDelay)
                        m_NextScroll = Time.time + m_ScrollDelay;
                }
            }
        }

        protected virtual void HandlePrimaryAttacking()
        {
            if (CurrentWeapon == null)
                return;

            switch (CurrentWeapon.PrimaryTriggerType)
            {
                case GoldPlayerWeapon.TriggerTypeEnum.Manual:
                    m_DoPrimaryAttack = GetButtonDown("Primary Attack");
                    break;
                case GoldPlayerWeapon.TriggerTypeEnum.Automatic:
                    m_DoPrimaryAttack = GetButton("Primary Attack");
                    break;
                default:
                    throw new System.NotImplementedException("No support for " + CurrentWeapon.PrimaryTriggerType + " trigger type!");
            }

            if (m_DoPrimaryAttack)
                CurrentWeapon.PrimaryAttack();
        }

        protected virtual void HandleSecondaryAttacking()
        {
            if (CurrentWeapon == null)
                return;

            switch (CurrentWeapon.SecondaryTriggerType)
            {
                case GoldPlayerWeapon.TriggerTypeEnum.Manual:
                    m_DoSecondaryAttack = GetButtonDown("Secondary Attack");
                    break;
                case GoldPlayerWeapon.TriggerTypeEnum.Automatic:
                    m_DoSecondaryAttack = GetButton("Secondary Attack");
                    break;
                default:
                    throw new System.NotImplementedException("No support for " + CurrentWeapon.PrimaryTriggerType + " trigger type!");
            }

            if (m_DoSecondaryAttack)
                CurrentWeapon.SecondaryAttack();
        }

        protected virtual void HandleReloading()
        {
            if (CurrentWeapon == null)
                return;

            if (GetButtonDown("Reload"))
            {
                CurrentWeapon.Reload();
            }
        }

        protected void GoUpWeaponList()
        {
            if (m_MyWeapons == null || m_MyWeapons.Count == 0)
                return;

            m_NewWeaponIndex = m_CurrentWeaponIndex - 1;
            if (m_NewWeaponIndex < 0)
            {
                if (m_LoopScroll)
                    m_NewWeaponIndex = m_MyWeapons.Count - 1;
            }

            ChangeWeapon(m_NewWeaponIndex);
        }

        protected void GoDownWeaponList()
        {
            if (m_MyWeapons == null || m_MyWeapons.Count == 0)
                return;

            m_NewWeaponIndex = m_CurrentWeaponIndex + 1;
            if (m_NewWeaponIndex > m_MyWeapons.Count - 1)
            {
                if (m_LoopScroll)
                    m_NewWeaponIndex = 0;
            }

            ChangeWeapon(m_NewWeaponIndex);
        }

        public virtual void AddWeapon(GoldPlayerWeapon weapon)
        {
            for (int i = 0; i < m_AvailableWeapons.Length; i++)
            {
                if (m_AvailableWeapons[i] == weapon)
                {
                    AddWeapon(i);
                    return;
                }
            }

            throw new System.ArgumentException(weapon.gameObject.name + " has not been added to the list of available weapons!");
        }

        public virtual void AddWeapon(int weaponIndex)
        {
            if (m_MyWeapons.Contains(weaponIndex))
            {
                Debug.LogWarning("Weapon with index " + weaponIndex + " already exists in the player weapon inventory!");
                return;
            }

            m_MyWeapons.Add(weaponIndex);
        }

        public virtual void RemoveWeapon(GoldPlayerWeapon weapon)
        {
            int weaponToRemoveIndex = -1;
            for (int i = 0; i < m_AvailableWeapons.Length; i++)
            {
                if (m_AvailableWeapons[i] == weapon)
                {
                    weaponToRemoveIndex = i;
                    break;
                }
            }

            if (weaponToRemoveIndex < 0)
            {
                throw new System.ArgumentException(weapon + " has not been added to the available weapons list!");
            }

            RemoveWeapon(weaponToRemoveIndex);
        }

        public virtual void RemoveWeapon(int weaponIndex)
        {
            if (!m_MyWeapons.Contains(weaponIndex))
            {
                Debug.LogWarning("No weapon with the index " + weaponIndex + " exists in the player weapon inventory!");
                return;
            }
        }

        public virtual void ChangeWeapon(int index)
        {
            if (m_AvailableWeapons == null || m_AvailableWeapons.Length == 0 || m_CurrentWeaponIndex == index)
                return;

            if (!m_CanChangeWhenReloading && CurrentWeapon != null && CurrentWeapon.IsReloading)
                return;

            if (index < 0)
                index = 0;
            else if (index > m_AvailableWeapons.Length - 1)
            {
                index = m_AvailableWeapons.Length - 1;
                if (index == m_CurrentWeaponIndex)
                    return;
            }

            if (CurrentWeapon != null)
            {
                CurrentWeapon.gameObject.SetActive(false);
                CurrentWeapon.OnPutAway();
            }

            m_PreviousWeapon = CurrentWeapon;

            m_CurrentWeaponIndex = index;
            if (CurrentWeapon != null)
            {
                CurrentWeapon.gameObject.SetActive(true);
                CurrentWeapon.OnEquip();
            }

#if NET_4_6 || UNITY_2018_3_OR_NEWER
            OnWeaponChanged?.Invoke(m_PreviousWeapon, CurrentWeapon);
#else
            if (OnWeaponChanged != null)
                OnWeaponChanged.Invoke(m_PreviousWeapon, CurrentWeapon);
#endif
        }

        public virtual void DoBulletDecal(RaycastHit hit)
        {
            if (m_BulletDecals)
            {
                if (m_DecalParticleDataIndex >= m_BulletDecals.main.maxParticles)
                    m_DecalParticleDataIndex = 0;

                m_DecalData[m_DecalParticleDataIndex].position = hit.point;
                Vector3 particleRotationEuler = Quaternion.LookRotation(hit.normal).eulerAngles;
                particleRotationEuler.z = Random.Range(0f, 360f);
                m_DecalData[m_DecalParticleDataIndex].rotation = particleRotationEuler;

                if (m_BulletDecals.main.startSize.mode == ParticleSystemCurveMode.Constant)
                    m_DecalData[m_DecalParticleDataIndex].size = m_BulletDecals.main.startSize.constant;
                else if (m_BulletDecals.main.startSize.mode == ParticleSystemCurveMode.TwoConstants)
                    m_DecalData[m_DecalParticleDataIndex].size = Random.Range(m_BulletDecals.main.startSize.constantMin, m_BulletDecals.main.startSize.constantMax);

                if (m_BulletDecals.main.startColor.mode == ParticleSystemGradientMode.Color)
                    m_DecalData[m_DecalParticleDataIndex].color = m_BulletDecals.main.startColor.color;
                else if (m_BulletDecals.main.startColor.mode == ParticleSystemGradientMode.TwoColors) // FIX: This is the incorrect color.
                    m_DecalData[m_DecalParticleDataIndex].color = RandomColor(m_BulletDecals.main.startColor.colorMin, m_BulletDecals.main.startColor.colorMax);

                m_DecalParticleDataIndex++;

                for (int i = 0; i < m_DecalParticles.Length; i++)
                {
                    m_DecalParticles[i].position = m_DecalData[i].position;
                    m_DecalParticles[i].rotation3D = m_DecalData[i].rotation;
                    m_DecalParticles[i].startSize = m_DecalData[i].size;
                    m_DecalParticles[i].startColor = m_DecalData[i].color;
                }

                m_BulletDecals.SetParticles(m_DecalParticles, m_DecalParticles.Length);
            }
        }

        protected Color RandomColor()
        {
            return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        protected Color RandomColor(Color minColor, Color maxColor)
        {
            return new Color(Random.Range(minColor.r, maxColor.r), Random.Range(minColor.g, maxColor.g), Random.Range(minColor.b, maxColor.b), Random.Range(minColor.a, maxColor.a));
        }
    }
}
