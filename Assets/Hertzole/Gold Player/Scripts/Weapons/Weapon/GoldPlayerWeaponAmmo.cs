using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    //TODO: Implement Ammo Type.
    public partial class GoldPlayerWeapon
    {
        public enum ReloadTypeEnum { ReloadEntireClip = 0, ReloadEachBullet = 1 }
        public enum AmmoTypeEnum { AmmoAndClip = 0, OneClip = 1, Charge = 2 }

        [SerializeField]
        private AmmoTypeEnum m_AmmoType = AmmoTypeEnum.AmmoAndClip;
        public AmmoTypeEnum AmmoType { get { return m_AmmoType; } set { m_AmmoType = value; } }
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
        private bool m_AutoReloadEmptyClip = true;
        public bool AutoReloadEmptyClip { get { return m_AutoReloadEmptyClip; } set { m_AutoReloadEmptyClip = value; } }
        [SerializeField]
        private bool m_CanReloadInBackground = false;
        public bool CanReloadInBackground { get { return m_CanReloadInBackground; } set { m_CanReloadInBackground = value; } }
        [SerializeField]
        private float m_ReloadTime = 0.8f;
        public float ReloadTime { get { return m_ReloadTime; } set { m_ReloadTime = value; } }
        [SerializeField]
        private ReloadTypeEnum m_ReloadType = ReloadTypeEnum.ReloadEntireClip;
        public ReloadTypeEnum ReloadType { get { return m_ReloadType; } set { m_ReloadType = value; } }
        [SerializeField]
        private float m_MaxCharge = 100;
        public float MaxCharge { get { return m_MaxCharge; } set { m_MaxCharge = value; } }
        [SerializeField]
        private float m_ChargeDecreaseRate = 5f;
        public float ChargeDecreaseRate { get { return m_ChargeDecreaseRate; } set { m_ChargeDecreaseRate = value; } }
        [SerializeField]
        private bool m_AutoRecharge = true;
        public bool AutoRecharge { get { return m_AutoRecharge; } set { m_AutoRecharge = value; } }
        [SerializeField]
        private float m_ChargeRegenerateRate = 4.5f;
        public float ChargeRegenerateRate { get { return m_ChargeRegenerateRate; } set { m_ChargeRegenerateRate = value; } }
        [SerializeField]
        private float m_RechargeWaitTime = 1f;
        public float RechargeWaitTime { get { return m_RechargeWaitTime; } set { m_RechargeWaitTime = value; } }
        [SerializeField]
        private bool m_CanOverheat = true;
        public bool CanOverheat { get { return m_CanOverheat; } set { m_CanOverheat = value; } }
        [SerializeField]
        private float m_OverheatTime = 3f;
        public float OverheatTime { get { return m_OverheatTime; } set { m_OverheatTime = value; } }

        protected int m_CurrentClip = 0;
        protected int m_CurrentAmmo = 0;

        public int CurrentClip
        {
            get { return m_CurrentClip; }
            protected set
            {
                m_CurrentClip = value;
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                OnAmmoChanged?.Invoke(m_CurrentClip, m_CurrentAmmo);
#else
                if (OnAmmoChanged != null)
                    OnAmmoChanged.Invoke(m_CurrentClip, m_CurrentAmmo);
#endif
            }
        }

        public int CurrentAmmo
        {
            get { return m_CurrentAmmo; }
            protected set
            {
                m_CurrentAmmo = value;
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                OnAmmoChanged?.Invoke(m_CurrentClip, m_CurrentAmmo);
#else
                if (OnAmmoChanged != null)
                    OnAmmoChanged.Invoke(m_CurrentClip, m_CurrentAmmo);
#endif
            }
        }

        protected float m_FinishReloadTime = 0;
        protected float m_CurrentCharge = 0;
        protected float m_RechargeStartTime = 0;
        protected float m_OverheatTimer = 0;

        public float CurrentCharge
        {
            get { return m_CurrentCharge; }
            set
            {
                m_CurrentCharge = value;
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                OnChargeChanged?.Invoke(m_CurrentCharge);
#else
                if (OnChargeChanged != null)
                    OnChargeChanged.Invoke(m_CurrentCharge);
#endif
            }
        }

        public bool HasEnoughClip
        {
            get
            {
                if (m_AmmoType == AmmoTypeEnum.Charge)
                    return m_CurrentCharge > 0;
                else
                    return m_InfiniteClip ? true : m_CurrentClip > 0;
            }
        }

        public bool IsReloading { get; protected set; }
        public bool IsOverheated { get; protected set; }

        public delegate void AmmoEvent(int clip, int ammo);
        public delegate void ChargeEvent(float currentCharge);
        public event AmmoEvent OnAmmoChanged;
        public event ChargeEvent OnChargeChanged;
        public event System.Action OnChargeOverheated;
        public event System.Action OnChargeStopOverheat;
        public event System.Action OnStartReloading;
        public event System.Action OnFinishReload;

        private void InitializeAmmo()
        {
            if (m_AmmoType != AmmoTypeEnum.Charge)
                m_CurrentClip = m_MaxClip;
            if (m_AmmoType == AmmoTypeEnum.AmmoAndClip)
                m_CurrentAmmo = m_MaxAmmo;
            if (m_AmmoType == AmmoTypeEnum.Charge)
                m_CurrentCharge = m_MaxCharge;
        }

        private void OnEnableAmmo()
        {
            if (m_AmmoType == AmmoTypeEnum.AmmoAndClip)
            {
                if (IsReloading && m_CanReloadInBackground && Time.time >= m_FinishReloadTime)
                {
                    FinishReloading();
                }
                else if (IsReloading && !m_CanReloadInBackground)
                {
                    m_FinishReloadTime = Time.time + m_ReloadTime + m_EquipTime;
                    DoReload();
                }
            }
        }

        private void ReloadUpdate()
        {
            //TODO: Implement reload type.
            if (m_AmmoType == AmmoTypeEnum.AmmoAndClip)
            {
                if (IsReloading && Time.time >= m_FinishReloadTime)
                    FinishReloading();
            }
        }

        public virtual void Reload()
        {
            if (m_AmmoType != AmmoTypeEnum.AmmoAndClip || IsReloading || m_CurrentClip == m_MaxClip || m_CurrentAmmo == 0)
                return;

            IsReloading = true;
            m_FinishReloadTime = Time.time + m_ReloadTime;

            DoReload();
        }

        protected void DoReload()
        {
            if (m_AmmoType != AmmoTypeEnum.AmmoAndClip)
                return;

            PlayReloadSound();
            DoReloadAnimation();

#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
            OnStartReloading?.Invoke();
#else
            if (OnStartReloading != null)
                OnStartReloading.Invoke();
#endif
        }

        protected virtual void FinishReloading()
        {
            if (m_AmmoType != AmmoTypeEnum.AmmoAndClip)
                return;

            IsReloading = false;

            if (m_CurrentAmmo == -1)
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
                    if (!m_InfiniteAmmo)
                        m_CurrentAmmo -= toReload;
                }
            }

#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
            OnAmmoChanged?.Invoke(m_CurrentClip, m_CurrentAmmo);
#else
            if (OnAmmoChanged != null)
                OnAmmoChanged.Invoke(m_CurrentClip, m_CurrentAmmo);
#endif
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
            OnFinishReload?.Invoke();
#else
            if (OnFinishReload != null)
                OnFinishReload.Invoke();
#endif
        }

        protected virtual void ChargeUpdate()
        {
            if (m_AmmoType == AmmoTypeEnum.Charge)
            {
                if (IsOverheated)
                {
                    if (Time.time >= m_OverheatTimer)
                    {
                        IsOverheated = false;
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
                        OnChargeStopOverheat?.Invoke();
#else
                        if (OnChargeStopOverheat != null)
                            OnChargeStopOverheat.Invoke();
#endif
                    }
                }
                else
                {
                    if (m_AutoRecharge && Time.time >= m_RechargeStartTime && m_CurrentCharge < m_MaxCharge)
                    {
                        if (m_CurrentCharge > m_MaxCharge)
                            CurrentCharge = m_MaxCharge;
                        else
                            CurrentCharge += m_ChargeRegenerateRate * Time.deltaTime;
                    }
                }
            }
        }

        public virtual void SetAmmo(int amount)
        {
            amount = Mathf.Clamp(amount, 0, m_MaxAmmo);
            CurrentAmmo = amount;
        }

        public virtual void SetAmmo(float percent)
        {
            percent = Mathf.Clamp01(percent);
            SetAmmo(Mathf.RoundToInt(m_MaxAmmo * percent));
        }

        public virtual void SetClip(int amount)
        {
            amount = Mathf.Clamp(amount, 0, m_MaxClip);
            CurrentClip = amount;
        }

        public virtual void SetClip(float percent)
        {
            percent = Mathf.Clamp01(percent);
            SetClip(Mathf.RoundToInt(m_MaxClip * percent));
        }

        public virtual void AddAmmo(int amount)
        {
            amount = Mathf.Clamp(amount, 0, m_MaxAmmo);
            if (m_CurrentAmmo + amount > m_MaxAmmo)
                CurrentAmmo = amount;
            else
                CurrentAmmo += amount;
        }

        public virtual void AddAmmo(float percent)
        {
            percent = Mathf.Clamp01(percent);
            AddAmmo(Mathf.RoundToInt(m_MaxAmmo * percent));
        }

        public virtual void AddClip(int amount)
        {
            amount = Mathf.Clamp(amount, 0, m_MaxClip);
            if (m_CurrentClip + amount > m_MaxClip)
                CurrentClip = amount;
            else
                CurrentClip += amount;
        }

        public virtual void AddClip(float percent)
        {
            percent = Mathf.Clamp01(percent);
            AddClip(Mathf.RoundToInt(m_MaxClip * percent));
        }

        public virtual void RemoveAmmo(int amount)
        {
            amount = Mathf.Clamp(amount, 0, m_MaxAmmo);
            if (m_CurrentAmmo - amount < 0)
                CurrentAmmo = 0;
            else
                CurrentAmmo -= amount;
        }

        public virtual void RemoveAmmo(float percent)
        {
            percent = Mathf.Clamp01(percent);
            RemoveAmmo(Mathf.RoundToInt(m_MaxAmmo * percent));
        }

        public virtual void RemoveClip(int amount)
        {
            amount = Mathf.Clamp(amount, 0, m_MaxClip);
            if (m_CurrentClip - amount < 0)
                CurrentClip = 0;
            else
                CurrentClip -= amount;
        }

        public virtual void RemoveClip(float percent)
        {
            percent = Mathf.Clamp01(percent);
            RemoveAmmo(Mathf.RoundToInt(m_MaxClip * percent));
        }
    }
}
