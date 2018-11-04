using System.Collections;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{

    public partial class GoldPlayerWeapon
    {
        public enum AnimationTypeEnum { None = 0, CodeDriven = 1, Animator = 2 }

        [SerializeField]
        private AnimationTypeEnum m_AnimationType = AnimationTypeEnum.CodeDriven;
        public AnimationTypeEnum AnimationType { get { return m_AnimationType; } set { m_AnimationType = value; } }
        [SerializeField]
        private Animator m_AnimationTarget = null;
        public Animator AnimationTarget { get { return m_AnimationTarget; } set { m_AnimationTarget = value; } }
        [SerializeField]
        private WeaponAnimationInfo m_IdleAnimation = new WeaponAnimationInfo(AnimatorParamaterType.None, "Idle");
        public WeaponAnimationInfo IdleAnimation { get { return m_IdleAnimation; } set { m_IdleAnimation = value; } }
        [SerializeField]
        private WeaponAnimationInfo m_EquipAnimation = new WeaponAnimationInfo(AnimatorParamaterType.Trigger, "Equip");
        public WeaponAnimationInfo EquipAnimation { get { return m_EquipAnimation; } set { m_EquipAnimation = value; } }
        [SerializeField]
        private WeaponAnimationInfo m_ReloadAnimation = new WeaponAnimationInfo(AnimatorParamaterType.Bool, "Reloading");
        public WeaponAnimationInfo ReloadAnimation { get { return m_ReloadAnimation; } set { m_ReloadAnimation = value; } }
        [SerializeField]
        private WeaponAnimationInfo m_ShootAnimation = new WeaponAnimationInfo(AnimatorParamaterType.Trigger, "Shoot");
        public WeaponAnimationInfo ShootAnimation { get { return m_ShootAnimation; } set { m_ShootAnimation = value; } }

        protected bool m_PlayingEquipAnimation = false;
        protected bool m_PlayingReloadAnimation = false;

        protected Vector3 m_OriginalPosition;

        protected Coroutine m_EquipAnimationRoutine = null;
        protected Coroutine m_ReloadAnimationRoutine = null;
        protected Coroutine m_ShootAnimationRoutine = null;

        private void InitializeAnimations()
        {
            m_OriginalPosition = transform.localPosition;

            if (m_AnimationType == AnimationTypeEnum.Animator && !m_AnimationTarget)
                throw new System.NullReferenceException("There's no Animator Target assigned on '" + gameObject.name + "'!");
        }

        protected void OnEquipAnimation()
        {
            switch (m_AnimationType)
            {
                case AnimationTypeEnum.None:
                    break;
                case AnimationTypeEnum.CodeDriven:
                case AnimationTypeEnum.Animator:
                    m_EquipAnimationRoutine = StartCoroutine(AnimationEquipRoutine());
                    break;
                default:
                    throw new System.NotImplementedException("No support for animation type '" + m_AnimationType + "'!");
            }
        }

        protected virtual IEnumerator AnimationEquipRoutine()
        {
            m_PlayingEquipAnimation = true;
            if (m_AnimationType == AnimationTypeEnum.CodeDriven && m_EquipAnimation.Enabled)
            {
                Vector3 startPosition = m_OriginalPosition - new Vector3(0, 1, 0);
                transform.localPosition = startPosition;

                float currentEquipTime = 0;
                while (currentEquipTime < m_EquipTime)
                {
                    currentEquipTime += Time.deltaTime;
                    if (currentEquipTime > m_EquipTime)
                        currentEquipTime = m_EquipTime;

                    float perc = currentEquipTime / m_EquipTime;
                    transform.localPosition = Vector3.Lerp(startPosition, m_OriginalPosition, m_EquipAnimation.Curve.Evaluate(perc));
                    yield return null;
                }
            }
            else
            {
                SetTriggerOnTarget(m_EquipAnimation, 1, 1, true, true);
                float currentEquipTime = 0;
                while (currentEquipTime < m_EquipTime)
                {
                    currentEquipTime += Time.deltaTime;
                    if (currentEquipTime > m_EquipTime)
                        currentEquipTime = m_EquipTime;

                    yield return null;
                }
                SetTriggerOnTarget(m_EquipAnimation, 0, 0, false, false);
            }

            m_PlayingEquipAnimation = false;
            m_EquipAnimationRoutine = null;
        }

        protected void DoReloadAnimation()
        {
            switch (m_AnimationType)
            {
                case AnimationTypeEnum.None:
                    break;
                case AnimationTypeEnum.CodeDriven:
                case AnimationTypeEnum.Animator:
                    m_ReloadAnimationRoutine = StartCoroutine(ReloadAnimationRoutine());
                    break;
                default:
                    throw new System.NotImplementedException("No support for animation type '" + m_AnimationType + "'!");
            }
        }

        protected virtual IEnumerator ReloadAnimationRoutine()
        {
            m_PlayingReloadAnimation = true;
            if (m_AnimationType == AnimationTypeEnum.CodeDriven && m_ReloadAnimation.Enabled)
            {
                float reloadSpeed = 1f / m_ReloadTime;
                float reloadPercent = 0;
                Vector3 initialRot = transform.localEulerAngles;

                while (reloadPercent < 1)
                {
                    reloadPercent += Time.deltaTime * reloadSpeed;

                    float interpolation = (-Mathf.Pow(reloadPercent, 2) + reloadPercent) * 4;
                    float reloadAngle = Mathf.Lerp(0, 30, m_ReloadAnimation.Curve.Evaluate(interpolation));
                    transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;
                    yield return null;
                }
            }
            else
            {
                SetTriggerOnTarget(m_ReloadAnimation, 1, 1, true, true);
                float currentReloadTime = 0;
                while (currentReloadTime < m_ReloadTime)
                {
                    currentReloadTime += Time.deltaTime;
                    if (currentReloadTime > m_ReloadTime)
                        currentReloadTime = m_ReloadTime;

                    yield return null;
                }
                SetTriggerOnTarget(m_ReloadAnimation, 0, 0, false, false);
            }

            m_PlayingReloadAnimation = false;
            m_ReloadAnimationRoutine = null;
        }

        protected void DoShootAnimation()
        {
            switch (m_AnimationType)
            {
                case AnimationTypeEnum.None:
                    break;
                case AnimationTypeEnum.CodeDriven:
                    if (m_ShootAnimation.Enabled)
                        Debug.LogWarning("There's no support for code driven shoot animations. You should probably use the recoil feature instead!");
                    break;
                case AnimationTypeEnum.Animator:
                    m_ShootAnimationRoutine = StartCoroutine(ShootAnimationRoutine());
                    break;
                default:
                    throw new System.NotImplementedException("No support for animation type '" + m_AnimationType + "'!");
            }
        }

        protected virtual IEnumerator ShootAnimationRoutine()
        {
            SetTriggerOnTarget(m_ShootAnimation, 1, 1, true, true);
            float currentShootTime = 0;
            while (currentShootTime < m_FireDelay)
            {
                currentShootTime += Time.deltaTime;
                if (currentShootTime > m_FireDelay)
                    currentShootTime = m_FireDelay;

                yield return null;
            }
            SetTriggerOnTarget(m_ShootAnimation, 0, 0, false, false);

            m_ShootAnimationRoutine = null;
        }

        protected void SetTriggerOnTarget(WeaponAnimationInfo info, float floatValue, int intValue, bool boolValue, bool shouldTrigger)
        {
            if (m_AnimationTarget && info.Enabled)
            {
                switch (info.ParameterType)
                {
                    case AnimatorParamaterType.None:
                        break;
                    case AnimatorParamaterType.Float:
                        m_AnimationTarget.SetFloat(info.ParamaterName, floatValue);
                        break;
                    case AnimatorParamaterType.Int:
                        m_AnimationTarget.SetInteger(info.ParamaterName, intValue);
                        break;
                    case AnimatorParamaterType.Bool:
                        m_AnimationTarget.SetBool(info.ParamaterName, boolValue);
                        break;
                    case AnimatorParamaterType.Trigger:
                        if (shouldTrigger)
                            m_AnimationTarget.SetTrigger(info.ParamaterName);
                        break;
                    default:
                        throw new System.NotImplementedException("No support for parameter type '" + info.ParameterType + "'!");
                }
            }
        }
    }
}
