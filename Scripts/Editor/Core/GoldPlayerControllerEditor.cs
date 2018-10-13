#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    [CustomEditor(typeof(GoldPlayerController))]
    internal class GoldPlayerControllerEditor : UnityEditor.Editor
    {
        private int m_CurrentTab = 0;

        private readonly string[] m_Tabs = new string[] { "Camera", "Movement", "Head Bob", "Audio" };
        private const string SELECTED_TAB_PREFS = "HERTZ_GOLD_PLAYER_SELECTED_TAB";

        private CharacterController m_CharacterController;

        private SerializedProperty m_Camera;
        private SerializedProperty m_Movement;
        private SerializedProperty m_HeadBob;
        private SerializedProperty m_Audio;

        private void OnEnable()
        {
            m_CurrentTab = EditorPrefs.GetInt(SELECTED_TAB_PREFS, 0);

            if (m_CurrentTab < 0)
                m_CurrentTab = 0;
            if (m_CurrentTab > 3)
                m_CurrentTab = 3;

            m_Camera = serializedObject.FindProperty("m_Camera");
            m_Movement = serializedObject.FindProperty("m_Movement");
            m_HeadBob = serializedObject.FindProperty("m_HeadBob");
            m_Audio = serializedObject.FindProperty("m_Audio");

            m_CharacterController = ((GoldPlayerController)target).GetComponent<CharacterController>();
        }

        public override void OnInspectorGUI()
        {
            if (m_CharacterController.center.y != m_CharacterController.height / 2)
                EditorGUILayout.HelpBox("The Character Controller Y center must be half of the height. Set your Y center to " + m_CharacterController.height / 2 + "!", MessageType.Warning);

            serializedObject.Update();
            int newTab = GUILayout.Toolbar(m_CurrentTab, m_Tabs);
            if (newTab != m_CurrentTab)
            {
                m_CurrentTab = newTab;
                EditorPrefs.SetInt(SELECTED_TAB_PREFS, m_CurrentTab);
            }

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
            else if (m_CurrentTab == 3) // Audio
            {
                DoAudioGUI();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DoCameraGUI()
        {
            SerializedProperty it = m_Camera.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(m_Camera.name) && it.depth < 2)
                    EditorGUILayout.PropertyField(it, true);
            }
        }

        private void DoMovementGUI()
        {
            SerializedProperty it = m_Movement.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(m_Movement.name) && it.depth < 2)
                {
                    EditorGUILayout.PropertyField(it, true);
                    if (it.name.Equals("m_CrouchHeight") && it.floatValue < 0.8f)
                        EditorGUILayout.HelpBox("The Crouch Height should not be less than 0.8 because it breaks the character controller!", MessageType.Warning);
                }
            }
        }

        private void DoHeadBobGUI()
        {
            SerializedProperty it = m_HeadBob.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(m_HeadBob.name) && !it.propertyPath.StartsWith(m_HeadBob.name + ".m_BobClass") && it.depth < 2)
                    EditorGUILayout.PropertyField(it, true);

                if (it.propertyPath.StartsWith(m_HeadBob.name + ".m_BobClass") && it.depth >= 2)
                    EditorGUILayout.PropertyField(it, true);
            }
        }

        private void DoAudioGUI()
        {
            SerializedProperty it = m_Audio.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(m_Audio.name) && it.depth < 2)
                    EditorGUILayout.PropertyField(it, true);
            }
        }
    }
}
#endif
