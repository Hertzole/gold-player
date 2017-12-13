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
        private const string SELECTED_TAB_PREFS = "HERTZ_APP_SELECTED_TAB";

        private SerializedProperty m_Camera;
        private SerializedProperty m_Movement;

        private void OnEnable()
        {
            m_CurrentTab = EditorPrefs.GetInt(SELECTED_TAB_PREFS, 0);
            m_Camera = serializedObject.FindProperty("m_Camera");
            m_Movement = serializedObject.FindProperty("m_Movement");
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
            serializedObject.ApplyModifiedProperties();
        }

        private void DoCameraGUI()
        {
            EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
            SerializedProperty it = m_Camera.Copy ();
            while (it.NextVisible(true)) {
                if(!it.propertyPath.StartsWith("m_Movement") && it.depth < 2) EditorGUILayout.PropertyField(it, false);
            }
        }

        private void DoMovementGUI()
        {
            //EditorGUILayout.LabelField("Walking", EditorStyles.boldLabel);
            //EditorGUILayout.PropertyField(m_Movement.FindPropertyRelative("m_WalkingSpeeds"), true);

            //EditorGUILayout.Space();
            //EditorGUILayout.LabelField("Running", EditorStyles.boldLabel);
            //EditorGUILayout.PropertyField(m_Movement.FindPropertyRelative("m_CanRun"));
            //GUI.enabled = m_Movement.FindPropertyRelative("m_CanRun").boolValue;
            //EditorGUILayout.PropertyField(m_Movement.FindPropertyRelative("m_RunSpeeds"), true);
            //GUI.enabled = true;
            SerializedProperty it = m_Movement.Copy ();
            while (it.NextVisible(true)) {
                if(!it.propertyPath.StartsWith("m_Camera") && it.depth < 2) EditorGUILayout.PropertyField(it, false);
            }
            
        }
    }
}
#endif
