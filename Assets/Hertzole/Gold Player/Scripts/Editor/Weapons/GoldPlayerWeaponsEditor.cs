#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons.Editor
{
    [CustomEditor(typeof(GoldPlayerWeapons))]
    public class GoldPlayerWeaponsEditor : UnityEditor.Editor
    {
        private SerializedProperty m_AvailableWeapons;
        private SerializedProperty m_MyWeaponIndexes;
        private SerializedProperty m_HitLayer;

        private SerializedProperty m_CanChangeWeapon;
        private SerializedProperty m_CanScrollThrough;
        private SerializedProperty m_LoopScroll;
        private SerializedProperty m_InvertScroll;
        private SerializedProperty m_EnableScrollDelay;
        private SerializedProperty m_ScrollDelay;
        private SerializedProperty m_CanUseNumberKeys;
        private SerializedProperty m_CanChangeWhenReloading;

        private SerializedProperty m_BulletDecals;
        private SerializedProperty m_DecalHitLayers;
        private SerializedProperty m_IgnoreRigidbodies;

        private GoldPlayerWeapons m_Weapons;

        private ReorderableList m_MyWeaponsList;

        // Get all the serialized properties from the target script.
        private void OnEnable()
        {
            m_Weapons = (GoldPlayerWeapons)target;

            m_AvailableWeapons = serializedObject.FindProperty("m_AvailableWeapons");
            m_MyWeaponIndexes = serializedObject.FindProperty("m_MyWeaponIndexes");
            m_HitLayer = serializedObject.FindProperty("m_HitLayer");
            m_CanChangeWeapon = serializedObject.FindProperty("m_CanChangeWeapon");
            m_CanScrollThrough = serializedObject.FindProperty("m_CanScrollThrough");
            m_LoopScroll = serializedObject.FindProperty("m_LoopScroll");
            m_InvertScroll = serializedObject.FindProperty("m_InvertScroll");
            m_EnableScrollDelay = serializedObject.FindProperty("m_EnableScrollDelay");
            m_ScrollDelay = serializedObject.FindProperty("m_ScrollDelay");
            m_CanUseNumberKeys = serializedObject.FindProperty("m_CanUseNumberKeys");
            m_CanChangeWhenReloading = serializedObject.FindProperty("m_CanChangeWhenReloading");
            m_BulletDecals = serializedObject.FindProperty("m_BulletDecals");
            m_DecalHitLayers = serializedObject.FindProperty("m_DecalHitLayers");
            m_IgnoreRigidbodies = serializedObject.FindProperty("m_IgnoreRigidbodies");

            m_MyWeaponsList = new ReorderableList(serializedObject, m_MyWeaponIndexes, true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "My Weapons");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (m_AvailableWeapons.GetArrayElementAtIndex(index).objectReferenceValue != null)
                        EditorGUI.LabelField(rect, m_AvailableWeapons.GetArrayElementAtIndex(m_MyWeaponIndexes.GetArrayElementAtIndex(index).intValue).objectReferenceValue.name);
                    else
                        EditorGUI.LabelField(rect, "-Removed- REMOVE ME!");
                },
                onAddDropdownCallback = (Rect buttonRect, ReorderableList list) =>
                {
                    GenericMenu menu = new GenericMenu();
                    if (m_AvailableWeapons.arraySize > 0)
                    {
                        int index = 0;
                        for (int i = 0; i < m_AvailableWeapons.arraySize; i++)
                        {
                            index = i;
                            if (!m_Weapons.MyWeaponIndexes.Contains(i) && m_AvailableWeapons.GetArrayElementAtIndex(i).objectReferenceValue != null)
                            {
                                menu.AddItem(new GUIContent(m_AvailableWeapons.GetArrayElementAtIndex(i).objectReferenceValue.name), false,
                                    OnClickAddMyWeapon, index);

                            }
                        }
                    }
                    if (menu.GetItemCount() == 0)
                        menu.AddDisabledItem(new GUIContent("No available weapons"), false);
                    menu.ShowAsContext();
                }
            };
        }

        private void OnClickAddMyWeapon(object target)
        {
            int weaponIndex = (int)target;
            int index = m_MyWeaponsList.serializedProperty.arraySize;
            m_MyWeaponsList.serializedProperty.arraySize++;
            m_MyWeaponsList.index = index;
            SerializedProperty element = m_MyWeaponsList.serializedProperty.GetArrayElementAtIndex(index);
            element.intValue = weaponIndex;
            serializedObject.ApplyModifiedProperties();
        }

        // Draw all the GUI in the inspector.
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_AvailableWeapons, true);
            m_MyWeaponsList.DoLayoutList();
            EditorGUILayout.PropertyField(m_HitLayer, true);
            EditorGUILayout.PropertyField(m_CanChangeWeapon, true);
            EditorGUILayout.PropertyField(m_CanScrollThrough, true);
            EditorGUILayout.PropertyField(m_LoopScroll, true);
            EditorGUILayout.PropertyField(m_InvertScroll, true);
            EditorGUILayout.PropertyField(m_EnableScrollDelay, true);
            EditorGUILayout.PropertyField(m_ScrollDelay, true);
            EditorGUILayout.PropertyField(m_CanUseNumberKeys, true);
            EditorGUILayout.PropertyField(m_CanChangeWhenReloading, true);
            EditorGUILayout.PropertyField(m_BulletDecals, true);
            EditorGUILayout.PropertyField(m_DecalHitLayers, true);
            EditorGUILayout.PropertyField(m_IgnoreRigidbodies, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
