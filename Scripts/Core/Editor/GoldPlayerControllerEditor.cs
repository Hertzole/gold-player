#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    [CustomEditor(typeof(GoldPlayerController))]
    public class GoldPlayerControllerEditor : UnityEditor.Editor
    {
        private int m_CurrentTab = 0;

        private string[] m_Tabs = new string[] { "Camera", "Movement", "Head Bob", "Audio" };
        private const string SELECTED_TAB_PREFS = "HERTZ_GOLD_PLAYER_SELECTED_TAB";

        private SerializedProperty m_Camera;
        private SerializedProperty m_Movement;
        private SerializedProperty m_HeadBob;

        private void OnEnable()
        {
            m_CurrentTab = EditorPrefs.GetInt(SELECTED_TAB_PREFS, 0);
            m_Camera = serializedObject.FindProperty("m_Camera");
            m_Movement = serializedObject.FindProperty("m_Movement");
            m_HeadBob = serializedObject.FindProperty("m_HeadBob");
        }

        private void OnDisable()
        {
            EditorPrefs.SetInt(SELECTED_TAB_PREFS, m_CurrentTab);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            m_CurrentTab = GUILayout.Toolbar(m_CurrentTab, m_Tabs);

            if (m_CurrentTab == 0) // Camera
            {
                DoCameraGUI();
            }
            else if (m_CurrentTab == 1) // Movement
            {
                DoMovementGUI();
            }
            else if (m_CurrentTab == 2) // Head bob
            {
                DoHeadBobGUI();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DoCameraGUI()
        {
            SerializedProperty it = m_Camera.Copy();
            while (it.NextVisible(true))
            {
                if ((!it.propertyPath.StartsWith("m_Movement") && !it.propertyPath.StartsWith("m_HeadBob")) && it.depth < 2) EditorGUILayout.PropertyField(it, true);
            }
        }

        private void DoMovementGUI()
        {
            SerializedProperty it = m_Movement.Copy();
            while (it.NextVisible(true))
            {
                if ((!it.propertyPath.StartsWith("m_Camera") && !it.propertyPath.StartsWith("m_HeadBob")) && it.depth < 2) EditorGUILayout.PropertyField(it, true);
            }
        }

        private void DoHeadBobGUI()
        {
            SerializedProperty it = m_HeadBob.Copy();
            while (it.NextVisible(true))
            {
                if ((!it.propertyPath.StartsWith("m_Camera") && !it.propertyPath.StartsWith("m_Movement")) && it.depth < 2) EditorGUILayout.PropertyField(it, true);
            }
        }
    }
}
#endif
