#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons.Editor
{
    [CustomEditor(typeof(GoldPlayerWeapon), true)]
    public class GoldPlayerWeaponEditor : UnityEditor.Editor
    {
        private SerializedProperty m_WeaponName;
        private SerializedProperty m_Damage;
        private SerializedProperty m_IsMelee;
        private SerializedProperty m_MeleeAttackTime;
        private SerializedProperty m_InfiniteClip;
        private SerializedProperty m_MaxClip;
        private SerializedProperty m_InfiniteAmmo;
        private SerializedProperty m_MaxAmmo;
        private SerializedProperty m_FireDelay;
        private SerializedProperty m_AutoReloadEmptyClip;
        private SerializedProperty m_CanReloadInBackground;
        private SerializedProperty m_ReloadTime;
        private SerializedProperty m_EquipTime;
        private SerializedProperty m_PrimaryTriggerType;
        private SerializedProperty m_SecondaryTriggerType;

        private SerializedProperty m_ProjectileType;
        private SerializedProperty m_ProjectileLength;
        private SerializedProperty m_ShootOrigin;
        private SerializedProperty m_PoolPrefabs;
        private SerializedProperty m_InitialPrefabPool;
        private SerializedProperty m_ProjectilePrefab;
        private SerializedProperty m_ProjectileMoveSpeed;
        private SerializedProperty m_ProjectileLifetime;
        private SerializedProperty m_BulletsPerShot;
        private SerializedProperty m_BulletSpread;
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

        private SerializedProperty m_IdleAnimation;
        private SerializedProperty m_ShootAnimation;
        private SerializedProperty m_ReloadAnimation;
        private SerializedProperty m_EquipAnimation;

        private readonly Color r_HeaderBackgroundDark = new Color(0.1f, 0.1f, 0.1f, 0.2f);
        private readonly Color r_HeaderBackgroundLight = new Color(1f, 1f, 1f, 0.2f);
        private Color HeaderBackground { get { return EditorGUIUtility.isProSkin ? r_HeaderBackgroundDark : r_HeaderBackgroundLight; } }

        private bool BasicSettingsExpanded { get { return m_WeaponName.isExpanded; } set { m_WeaponName.isExpanded = value; } }
        private bool ProjectileSettingsExpanded { get { return m_ProjectileType.isExpanded; } set { m_ProjectileType.isExpanded = value; } }
        private bool RecoilSettingsExpanded { get { return m_EnableRecoil.isExpanded; } set { m_EnableRecoil.isExpanded = value; } }
        private bool AudioSettingsExpanded { get { return m_RecoilTime.isExpanded; } set { m_RecoilTime.isExpanded = value; } }
        private bool AnimationSettingsExpanded { get { return m_RecoilAmount.isExpanded; } set { m_RecoilAmount.isExpanded = value; } }

        // Get all the serialized properties from the target script.
        private void OnEnable()
        {
            m_WeaponName = serializedObject.FindProperty("m_WeaponName");
            m_Damage = serializedObject.FindProperty("m_Damage");
            m_IsMelee = serializedObject.FindProperty("m_IsMelee");
            m_MeleeAttackTime = serializedObject.FindProperty("m_MeleeAttackTime");
            m_InfiniteClip = serializedObject.FindProperty("m_InfiniteClip");
            m_MaxClip = serializedObject.FindProperty("m_MaxClip");
            m_InfiniteAmmo = serializedObject.FindProperty("m_InfiniteAmmo");
            m_MaxAmmo = serializedObject.FindProperty("m_MaxAmmo");
            m_FireDelay = serializedObject.FindProperty("m_FireDelay");
            m_AutoReloadEmptyClip = serializedObject.FindProperty("m_AutoReloadEmptyClip");
            m_CanReloadInBackground = serializedObject.FindProperty("m_CanReloadInBackground");
            m_ReloadTime = serializedObject.FindProperty("m_ReloadTime");
            m_EquipTime = serializedObject.FindProperty("m_EquipTime");
            m_PrimaryTriggerType = serializedObject.FindProperty("m_PrimaryTriggerType");
            m_SecondaryTriggerType = serializedObject.FindProperty("m_SecondaryTriggerType");

            m_ProjectileType = serializedObject.FindProperty("m_ProjectileType");
            m_ProjectileLength = serializedObject.FindProperty("m_ProjectileLength");
            m_ShootOrigin = serializedObject.FindProperty("m_ShootOrigin");
            m_PoolPrefabs = serializedObject.FindProperty("m_PoolPrefabs");
            m_InitialPrefabPool = serializedObject.FindProperty("m_InitialPrefabPool");
            m_ProjectilePrefab = serializedObject.FindProperty("m_ProjectilePrefab");
            m_ProjectileMoveSpeed = serializedObject.FindProperty("m_ProjectileMoveSpeed");
            m_ProjectileLifetime = serializedObject.FindProperty("m_ProjectileLifetime");
            m_BulletsPerShot = serializedObject.FindProperty("m_BulletsPerShot");
            m_BulletSpread = serializedObject.FindProperty("m_BulletSpread");
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

            m_IdleAnimation = serializedObject.FindProperty("m_IdleAnimation");
            m_ShootAnimation = serializedObject.FindProperty("m_ShootAnimation");
            m_ReloadAnimation = serializedObject.FindProperty("m_ReloadAnimation");
            m_EquipAnimation = serializedObject.FindProperty("m_EquipAnimation");
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

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawBasicSettings()
        {
            EditorGUILayout.PropertyField(m_WeaponName, true);
            EditorGUILayout.PropertyField(m_Damage, true);
            EditorGUILayout.PropertyField(m_IsMelee, true);
            if (m_IsMelee.boolValue)
            {
                EditorGUILayout.PropertyField(m_MeleeAttackTime, true);
                EditorGUILayout.PropertyField(m_FireDelay, true);
            }
            else
            {
                EditorGUILayout.PropertyField(m_InfiniteClip, true);
                GUI.enabled = !m_InfiniteClip.boolValue;
                EditorGUILayout.PropertyField(m_MaxClip, true);
                GUI.enabled = true;
                EditorGUILayout.PropertyField(m_InfiniteAmmo, true);
                GUI.enabled = !m_InfiniteAmmo.boolValue;
                EditorGUILayout.PropertyField(m_MaxAmmo, true);
                GUI.enabled = true;
                EditorGUILayout.PropertyField(m_FireDelay, true);
                EditorGUILayout.PropertyField(m_AutoReloadEmptyClip, true);
                EditorGUILayout.PropertyField(m_CanReloadInBackground, true);
                EditorGUILayout.PropertyField(m_ReloadTime, true);
            }

            EditorGUILayout.PropertyField(m_EquipTime, true);
            EditorGUILayout.PropertyField(m_PrimaryTriggerType, true);
            EditorGUILayout.PropertyField(m_SecondaryTriggerType, true);
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
                    EditorGUILayout.PropertyField(m_ProjectileLifetime);
                    EditorGUILayout.PropertyField(m_PoolPrefabs, true);
                    GUI.enabled = m_PoolPrefabs.boolValue;
                    EditorGUILayout.PropertyField(m_InitialPrefabPool, true);
                    GUI.enabled = true;
                }
                EditorGUILayout.PropertyField(m_BulletsPerShot);
                EditorGUILayout.PropertyField(m_BulletSpread);
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
            EditorGUILayout.PropertyField(m_IdleAnimation, true);
            EditorGUILayout.PropertyField(m_ShootAnimation, true);
            EditorGUILayout.PropertyField(m_ReloadAnimation, true);
            EditorGUILayout.PropertyField(m_EquipAnimation, true);
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
