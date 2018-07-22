#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Interaction.Editor
{
    [CustomEditor(typeof(GoldPlayerInteractable))]
    internal class GoldPlayerInteractableEditor : UnityEditor.Editor
    {
        private SerializedProperty m_CanInteract;
        private SerializedProperty m_IsHidden;
        private SerializedProperty m_UseCustomMessage;
        private SerializedProperty m_CustomMessage;
        private SerializedProperty m_OnInteract;

        private void OnEnable()
        {
            m_CanInteract = serializedObject.FindProperty("m_CanInteract");
            m_IsHidden = serializedObject.FindProperty("m_IsHidden");
            m_UseCustomMessage = serializedObject.FindProperty("m_UseCustomMessage");
            m_CustomMessage = serializedObject.FindProperty("m_CustomMessage");
            m_OnInteract = serializedObject.FindProperty("m_OnInteract");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_CanInteract);
            EditorGUILayout.PropertyField(m_IsHidden);

            EditorGUILayout.PropertyField(m_UseCustomMessage);
            GUI.enabled = m_UseCustomMessage.boolValue;
            EditorGUILayout.PropertyField(m_CustomMessage);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(m_OnInteract);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
