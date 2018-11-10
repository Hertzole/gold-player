#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons.Editor
{
    [CustomEditor(typeof(GoldPlayerWeapon), true)]
    [CanEditMultipleObjects]
    public class GoldPlayerWeaponEditor : UnityEditor.Editor
    {
        private SerializedProperty m_WeaponName;
        private SerializedProperty m_Damage;
        private SerializedProperty m_IsMelee;
        private SerializedProperty m_MeleeAttackTime;
        private SerializedProperty m_EquipTime;
        private SerializedProperty m_PrimaryAttackTrigger;
        //private SerializedProperty m_SecondaryTriggerType;

        private SerializedProperty m_AmmoType;
        private SerializedProperty m_InfiniteClip;
        private SerializedProperty m_MaxClip;
        private SerializedProperty m_InfiniteAmmo;
        private SerializedProperty m_MaxAmmo;
        private SerializedProperty m_FireDelay;
        private SerializedProperty m_AutoReloadEmptyClip;
        private SerializedProperty m_CanReloadInBackground;
        private SerializedProperty m_ReloadTime;
        private SerializedProperty m_ReloadType;
        private SerializedProperty m_MaxCharge;
        private SerializedProperty m_ChargeDecreaseRate;
        private SerializedProperty m_AutoRecharge;
        private SerializedProperty m_ChargeRegenerateRate;
        private SerializedProperty m_RechargeWaitTime;
        private SerializedProperty m_CanOverheat;
        private SerializedProperty m_OverheatTime;

        private SerializedProperty m_ProjectileType;
        private SerializedProperty m_ProjectileLength;
        private SerializedProperty m_ShootOrigin;
        private SerializedProperty m_PoolPrefabs;
        private SerializedProperty m_InitialPrefabPool;
        private SerializedProperty m_ProjectilePrefab;
        private SerializedProperty m_ProjectileMoveSpeed;
        private SerializedProperty m_ProjectileLifeTime;

        private SerializedProperty m_SpreadType;
        private SerializedProperty m_BulletsPerShot;
        private SerializedProperty m_BulletSpread;
        private SerializedProperty m_BulletPoints;
        private SerializedProperty m_ApplyRigidbodyForce;
        private SerializedProperty m_RigidbodyForce;
        private SerializedProperty m_ForceType;

        private SerializedProperty m_EnableRecoil;
        private SerializedProperty m_RecoilTarget;
        private SerializedProperty m_RecoilAmount;
        private SerializedProperty m_KickbackAmount;
        private SerializedProperty m_RecoilTime;

        private SerializedProperty m_EquipSound;
        private SerializedProperty m_PrimaryAttackSound;
        private SerializedProperty m_DryShootSound;
        private SerializedProperty m_ReloadSound;

        private SerializedProperty m_EquipAudioSource;
        private SerializedProperty m_PrimaryAttackAudioSource;
        private SerializedProperty m_DryShootAudioSource;
        private SerializedProperty m_ReloadAudioSource;

        private SerializedProperty m_AnimationType;
        private SerializedProperty m_AnimatorTarget;
        private SerializedProperty m_AnimationTarget;
        private SerializedProperty m_IdleAnimation;
        private SerializedProperty m_ShootAnimation;
        private SerializedProperty m_ReloadAnimation;
        private SerializedProperty m_EquipAnimation;

        private SerializedProperty m_MuzzleFlashObject;
        private SerializedProperty m_ObjectFlashTime;
        private SerializedProperty m_MuzzleFlashParticles;
        private SerializedProperty m_MuzzleFlashParticlesEmitAmount;
        private SerializedProperty m_LineEffect;
        private SerializedProperty m_LineFlashTime;
        private SerializedProperty m_ShellEjectParticles;
        private SerializedProperty m_ShellEjectAmount;
        private SerializedProperty m_ShellEjectPoint;
        private SerializedProperty m_RigidbodyShell;
        private SerializedProperty m_MaxRigidbodyShells;
        private SerializedProperty m_ShellForce;

        private readonly Color r_HeaderBackgroundDark = new Color(0.1f, 0.1f, 0.1f, 0.2f);
        private readonly Color r_HeaderBackgroundLight = new Color(0.35f, 0.35f, 0.35f, 0.2f);
        private Color HeaderBackground { get { return EditorGUIUtility.isProSkin ? r_HeaderBackgroundDark : r_HeaderBackgroundLight; } }

        private bool BasicSettingsExpanded { get { return m_WeaponName.isExpanded; } set { m_WeaponName.isExpanded = value; } }
        private bool AmmoSettingsExpanded { get { return m_InfiniteClip.isExpanded; } set { m_InfiniteClip.isExpanded = value; } }
        private bool ProjectileSettingsExpanded { get { return m_ProjectileType.isExpanded; } set { m_ProjectileType.isExpanded = value; } }
        private bool RecoilSettingsExpanded { get { return m_EnableRecoil.isExpanded; } set { m_EnableRecoil.isExpanded = value; } }
        private bool AudioSettingsExpanded { get { return m_RecoilTime.isExpanded; } set { m_RecoilTime.isExpanded = value; } }
        private bool AnimationSettingsExpanded { get { return m_RecoilAmount.isExpanded; } set { m_RecoilAmount.isExpanded = value; } }
        private bool CosmeticSettingsExpanded { get { return m_MuzzleFlashObject.isExpanded; } set { m_MuzzleFlashObject.isExpanded = value; } }

        // Get all the serialized properties from the target script.
        private void OnEnable()
        {
            m_WeaponName = serializedObject.FindProperty("m_WeaponName");
            m_Damage = serializedObject.FindProperty("m_Damage");
            m_IsMelee = serializedObject.FindProperty("m_IsMelee");
            m_MeleeAttackTime = serializedObject.FindProperty("m_MeleeAttackTime");
            m_EquipTime = serializedObject.FindProperty("m_EquipTime");
            m_PrimaryAttackTrigger = serializedObject.FindProperty("m_PrimaryAttackTrigger");
            //m_SecondaryTriggerType = serializedObject.FindProperty("m_SecondaryTriggerType");

            m_AmmoType = serializedObject.FindProperty("m_AmmoType");
            m_InfiniteClip = serializedObject.FindProperty("m_InfiniteClip");
            m_MaxClip = serializedObject.FindProperty("m_MaxClip");
            m_InfiniteAmmo = serializedObject.FindProperty("m_InfiniteAmmo");
            m_MaxAmmo = serializedObject.FindProperty("m_MaxAmmo");
            m_FireDelay = serializedObject.FindProperty("m_FireDelay");
            m_AutoReloadEmptyClip = serializedObject.FindProperty("m_AutoReloadEmptyClip");
            m_CanReloadInBackground = serializedObject.FindProperty("m_CanReloadInBackground");
            m_ReloadTime = serializedObject.FindProperty("m_ReloadTime");
            m_ReloadType = serializedObject.FindProperty("m_ReloadType");
            m_MaxCharge = serializedObject.FindProperty("m_MaxCharge");
            m_ChargeDecreaseRate = serializedObject.FindProperty("m_ChargeDecreaseRate");
            m_AutoRecharge = serializedObject.FindProperty("m_AutoRecharge");
            m_ChargeRegenerateRate = serializedObject.FindProperty("m_ChargeRegenerateRate");
            m_RechargeWaitTime = serializedObject.FindProperty("m_RechargeWaitTime");
            m_CanOverheat = serializedObject.FindProperty("m_CanOverheat");
            m_OverheatTime = serializedObject.FindProperty("m_OverheatTime");

            m_ProjectileType = serializedObject.FindProperty("m_ProjectileType");
            m_ProjectileLength = serializedObject.FindProperty("m_ProjectileLength");
            m_ShootOrigin = serializedObject.FindProperty("m_ShootOrigin");
            m_PoolPrefabs = serializedObject.FindProperty("m_PoolPrefabs");
            m_InitialPrefabPool = serializedObject.FindProperty("m_InitialPrefabPool");
            m_ProjectilePrefab = serializedObject.FindProperty("m_ProjectilePrefab");
            m_ProjectileMoveSpeed = serializedObject.FindProperty("m_ProjectileMoveSpeed");
            m_ProjectileLifeTime = serializedObject.FindProperty("m_ProjectileLifeTime");

            m_SpreadType = serializedObject.FindProperty("m_SpreadType");
            m_BulletsPerShot = serializedObject.FindProperty("m_BulletsPerShot");
            m_BulletSpread = serializedObject.FindProperty("m_BulletSpread");
            m_BulletPoints = serializedObject.FindProperty("m_BulletPoints");
            m_ApplyRigidbodyForce = serializedObject.FindProperty("m_ApplyRigidbodyForce");
            m_RigidbodyForce = serializedObject.FindProperty("m_RigidbodyForce");
            m_ForceType = serializedObject.FindProperty("m_ForceType");

            m_EnableRecoil = serializedObject.FindProperty("m_EnableRecoil");
            m_RecoilTarget = serializedObject.FindProperty("m_RecoilTarget");
            m_RecoilAmount = serializedObject.FindProperty("m_RecoilAmount");
            m_KickbackAmount = serializedObject.FindProperty("m_KickbackAmount");
            m_RecoilTime = serializedObject.FindProperty("m_RecoilTime");

            m_EquipSound = serializedObject.FindProperty("m_EquipSound");
            m_PrimaryAttackSound = serializedObject.FindProperty("m_PrimaryAttackSound");
            m_DryShootSound = serializedObject.FindProperty("m_DryShootSound");
            m_ReloadSound = serializedObject.FindProperty("m_ReloadSound");

            m_EquipAudioSource = serializedObject.FindProperty("m_EquipAudioSource");
            m_PrimaryAttackAudioSource = serializedObject.FindProperty("m_PrimaryAttackAudioSource");
            m_DryShootAudioSource = serializedObject.FindProperty("m_DryShootAudioSource");
            m_ReloadAudioSource = serializedObject.FindProperty("m_ReloadAudioSource");

            m_AnimationType = serializedObject.FindProperty("m_AnimationType");
            m_AnimatorTarget = serializedObject.FindProperty("m_AnimatorTarget");
            m_AnimationTarget = serializedObject.FindProperty("m_AnimationTarget");
            m_IdleAnimation = serializedObject.FindProperty("m_IdleAnimation");
            m_ShootAnimation = serializedObject.FindProperty("m_ShootAnimation");
            m_ReloadAnimation = serializedObject.FindProperty("m_ReloadAnimation");
            m_EquipAnimation = serializedObject.FindProperty("m_EquipAnimation");

            m_MuzzleFlashObject = serializedObject.FindProperty("m_MuzzleFlashObject");
            m_ObjectFlashTime = serializedObject.FindProperty("m_ObjectFlashTime");
            m_MuzzleFlashParticles = serializedObject.FindProperty("m_MuzzleFlashParticles");
            m_MuzzleFlashParticlesEmitAmount = serializedObject.FindProperty("m_ParticleEmitAmount");
            m_LineEffect = serializedObject.FindProperty("m_LineEffect");
            m_LineFlashTime = serializedObject.FindProperty("m_LineFlashTime");
            m_ShellEjectParticles = serializedObject.FindProperty("m_ShellEjectParticle");
            m_ShellEjectAmount = serializedObject.FindProperty("m_ShellEjectAmount");
            m_ShellEjectPoint = serializedObject.FindProperty("m_ShellEjectPoint");
            m_RigidbodyShell = serializedObject.FindProperty("m_RigidbodyShell");
            m_MaxRigidbodyShells = serializedObject.FindProperty("m_MaxRigidbodyShells");
            m_ShellForce = serializedObject.FindProperty("m_ShellForce");
        }

        // Draw all the GUI in the inspector.
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            BasicSettingsExpanded = DrawHeader("Basic Settings", BasicSettingsExpanded);

            if (BasicSettingsExpanded)
            {
                DrawBasicSettings();
            }

            EditorGUILayout.Space();

            AmmoSettingsExpanded = DrawHeader("Ammo & Reload Settings", AmmoSettingsExpanded);

            if (AmmoSettingsExpanded)
            {
                DrawAmmoSettings();
            }

            EditorGUILayout.Space();

            ProjectileSettingsExpanded = DrawHeader(m_IsMelee.boolValue ? "Raycast Settings" : "Projectile Settings", ProjectileSettingsExpanded);

            if (ProjectileSettingsExpanded)
            {
                DrawProjectileSettings();
            }

            EditorGUILayout.Space();

            RecoilSettingsExpanded = DrawHeader("Recoil Settings", RecoilSettingsExpanded);

            if (RecoilSettingsExpanded)
            {
                DrawRecoilSettings();
            }

            EditorGUILayout.Space();

            AudioSettingsExpanded = DrawHeader("Audio Settings", AudioSettingsExpanded);

            if (AudioSettingsExpanded)
            {
                DrawAudioSettings();
            }

            EditorGUILayout.Space();

            AnimationSettingsExpanded = DrawHeader("Animation Settings", AnimationSettingsExpanded);

            if (AnimationSettingsExpanded)
            {
                DrawAnimationSettings();
            }

            EditorGUILayout.Space();

            CosmeticSettingsExpanded = DrawHeader("Effect Settings", CosmeticSettingsExpanded);

            if (CosmeticSettingsExpanded)
            {
                DrawEffectsSettings();
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawBasicSettings()
        {
            EditorGUILayout.PropertyField(m_WeaponName, true);
            EditorGUILayout.PropertyField(m_Damage, true);
            EditorGUILayout.PropertyField(m_IsMelee, true);
            if (m_IsMelee.boolValue)
                EditorGUILayout.PropertyField(m_MeleeAttackTime, true);

            EditorGUILayout.PropertyField(m_FireDelay, true);
            EditorGUILayout.PropertyField(m_EquipTime, true);
            EditorGUILayout.PropertyField(m_PrimaryAttackTrigger, true);
            //EditorGUILayout.PropertyField(m_SecondaryTriggerType, true);
        }

        protected virtual void DrawAmmoSettings()
        {
            EditorGUILayout.PropertyField(m_AmmoType, true);
            if (m_AmmoType.enumValueIndex == 0) // Ammo and Clip
            {
                EditorGUILayout.PropertyField(m_InfiniteClip, true);
                GUI.enabled = !m_InfiniteClip.boolValue;
                EditorGUILayout.PropertyField(m_MaxClip, true);
                GUI.enabled = true;
                EditorGUILayout.PropertyField(m_InfiniteAmmo, true);
                GUI.enabled = !m_InfiniteAmmo.boolValue;
                EditorGUILayout.PropertyField(m_MaxAmmo, true);
                GUI.enabled = true;
                EditorGUILayout.PropertyField(m_AutoReloadEmptyClip, true);
                EditorGUILayout.PropertyField(m_CanReloadInBackground, true);
                EditorGUILayout.PropertyField(m_ReloadTime, true);
                EditorGUILayout.PropertyField(m_ReloadType, true);
            }
            else if (m_AmmoType.enumValueIndex == 1) // One Clip
            {
                EditorGUILayout.PropertyField(m_InfiniteClip, true);
                GUI.enabled = !m_InfiniteClip.boolValue;
                EditorGUILayout.PropertyField(m_MaxClip, true);
                GUI.enabled = true;
            }
            else if (m_AmmoType.enumValueIndex == 2) // Charge
            {
                EditorGUILayout.PropertyField(m_MaxCharge, true);
                EditorGUILayout.PropertyField(m_ChargeDecreaseRate, true);
                EditorGUILayout.PropertyField(m_AutoRecharge, true);
                GUI.enabled = m_AutoRecharge.boolValue;
                EditorGUILayout.PropertyField(m_ChargeRegenerateRate, true);
                EditorGUILayout.PropertyField(m_RechargeWaitTime, true);
                EditorGUILayout.PropertyField(m_CanOverheat, true);
                GUI.enabled = m_CanOverheat.boolValue && m_AutoRecharge.boolValue;
                EditorGUILayout.PropertyField(m_OverheatTime, true);
                GUI.enabled = true;
            }
        }

        protected virtual void DrawProjectileSettings()
        {
            if (!m_IsMelee.boolValue)
            {
                EditorGUILayout.PropertyField(m_ProjectileType, true);
                if (m_ProjectileType.enumValueIndex == 0)
                {
                    EditorGUILayout.PropertyField(m_ShootOrigin, true);
                    EditorGUILayout.PropertyField(m_ProjectileLength, true);
                }
                else if (m_ProjectileType.enumValueIndex == 1)
                {
                    EditorGUILayout.PropertyField(m_ShootOrigin, true);
                    EditorGUILayout.PropertyField(m_ProjectilePrefab, true);
                    EditorGUILayout.PropertyField(m_ProjectileMoveSpeed, true);
                    EditorGUILayout.PropertyField(m_ProjectileLifeTime);
                    EditorGUILayout.PropertyField(m_PoolPrefabs, true);
                    GUI.enabled = m_PoolPrefabs.boolValue;
                    EditorGUILayout.PropertyField(m_InitialPrefabPool, true);
                    GUI.enabled = true;
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(m_SpreadType, true);
                EditorGUILayout.PropertyField(m_BulletsPerShot, true);
                if (m_SpreadType.enumValueIndex == 1) // Random spread.
                    EditorGUILayout.PropertyField(m_BulletSpread, true);
                else if (m_SpreadType.enumValueIndex == 2) // Fixed spread.
                    EditorGUILayout.PropertyField(m_BulletPoints, true);
                EditorGUILayout.PropertyField(m_ApplyRigidbodyForce, true);
                GUI.enabled = m_ApplyRigidbodyForce.boolValue;
                EditorGUILayout.PropertyField(m_RigidbodyForce, true);
                EditorGUILayout.PropertyField(m_ForceType, true);
                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.PropertyField(m_ShootOrigin, new GUIContent("Raycast Origin", m_ShootOrigin.tooltip), true);
                EditorGUILayout.PropertyField(m_ProjectileLength, true);
                EditorGUILayout.PropertyField(m_ApplyRigidbodyForce, true);
                GUI.enabled = m_ApplyRigidbodyForce.boolValue;
                EditorGUILayout.PropertyField(m_RigidbodyForce, true);
                EditorGUILayout.PropertyField(m_ForceType, true);
                GUI.enabled = true;
            }
        }

        protected virtual void DrawRecoilSettings()
        {
            GUI.enabled = !m_IsMelee.boolValue;
            EditorGUILayout.PropertyField(m_EnableRecoil, true);
            GUI.enabled = m_EnableRecoil.boolValue && !m_IsMelee.boolValue;
            EditorGUILayout.PropertyField(m_RecoilTarget, true);
            EditorGUILayout.PropertyField(m_RecoilAmount, true);
            EditorGUILayout.PropertyField(m_KickbackAmount, true);
            EditorGUILayout.PropertyField(m_RecoilTime, true);
            GUI.enabled = true;
        }

        protected virtual void DrawAudioSettings()
        {
            EditorGUILayout.PropertyField(m_EquipSound, true);
            EditorGUILayout.PropertyField(m_PrimaryAttackSound, true);
            EditorGUILayout.PropertyField(m_DryShootSound, true);
            EditorGUILayout.PropertyField(m_ReloadSound, true);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_EquipAudioSource, true);
            EditorGUILayout.PropertyField(m_PrimaryAttackAudioSource, true);
            EditorGUILayout.PropertyField(m_DryShootAudioSource, true);
            EditorGUILayout.PropertyField(m_ReloadAudioSource, true);
        }

        protected virtual void DrawAnimationSettings()
        {
            EditorGUILayout.PropertyField(m_AnimationType, true);
            if (m_AnimationType.enumValueIndex != 0) // None
            {
                switch (m_AnimationType.enumValueIndex)
                {
                    case 1: // Code driven.
                        EditorGUILayout.PropertyField(m_AnimationTarget, true);
                        break;
                    case 2: // Animator.
                        EditorGUILayout.PropertyField(m_AnimatorTarget, true);
                        break;
                }

                DrawAnimationInfo(m_IdleAnimation);
                DrawAnimationInfo(m_ShootAnimation);
                DrawAnimationInfo(m_ReloadAnimation);
                DrawAnimationInfo(m_EquipAnimation);
            }
        }

        protected virtual void DrawEffectsSettings()
        {
            EditorGUILayout.PropertyField(m_MuzzleFlashObject, true);
            if (m_MuzzleFlashObject.objectReferenceValue != null)
                EditorGUILayout.PropertyField(m_ObjectFlashTime, true);

            EditorGUILayout.PropertyField(m_MuzzleFlashParticles, true);
            if (m_MuzzleFlashParticles.objectReferenceValue != null)
                EditorGUILayout.PropertyField(m_MuzzleFlashParticlesEmitAmount, true);

            if (m_ProjectileType.enumValueIndex == 0) // Raycast.
            {
                EditorGUILayout.PropertyField(m_LineEffect, true);
                if (m_LineEffect.objectReferenceValue != null)
                    EditorGUILayout.PropertyField(m_LineFlashTime, true);
            }

            EditorGUILayout.PropertyField(m_ShellEjectParticles);
            if (m_ShellEjectParticles.objectReferenceValue != null)
                EditorGUILayout.PropertyField(m_ShellEjectAmount);

            EditorGUILayout.PropertyField(m_ShellEjectPoint, true);
            EditorGUILayout.PropertyField(m_RigidbodyShell, true);
            if (m_RigidbodyShell.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(m_MaxRigidbodyShells, true);
                EditorGUILayout.PropertyField(m_ShellForce, true);
            }
        }

        protected void DrawAnimationInfo(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property, false);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(property.FindPropertyRelative("m_Enabled"));
                GUI.enabled = property.FindPropertyRelative("m_Enabled").boolValue;
                if (m_AnimationType.enumValueIndex == 1) // Code driven.
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("m_Curve"));
                }
                else if (m_AnimationType.enumValueIndex == 2) // Animator.
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("m_ParameterType"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("m_ParameterName"));
                }
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }
        }

        // Borrowed from Unity's post processing stack.
        // https://github.com/Unity-Technologies/PostProcessing/blob/5b295f9ba82c132b62d2920a0748b59bf9facaef/PostProcessing/Editor/Utils/EditorUtilities.cs#L158
        protected bool DrawHeader(string title, bool state)
        {
            Rect backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            Rect labelRect = backgroundRect;
            labelRect.xMin += 16f;
            labelRect.xMax -= 20f;

            Rect foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            // Background rect should be full-width
            backgroundRect.xMin = 0f;
            backgroundRect.width += 4f;

            // Background
            EditorGUI.DrawRect(backgroundRect, HeaderBackground);

            // Title
            EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            // Foldout
            state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

            Event e = Event.current;
            if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 0)
            {
                state = !state;
                e.Use();
            }

            return state;
        }
    }
}
#endif
