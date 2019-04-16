#if UNITY_EDITOR
using UnityEditor;
#if UNITY_2019_2_OR_NEWER
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Hertzole.GoldPlayer.Editor;
#else
using UnityEngine;
#endif

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

#if UNITY_2019_2_OR_NEWER
        private VisualElement useCustomMessageElement;
        private VisualElement customMessageElement;
#endif

        private void OnEnable()
        {
            m_CanInteract = serializedObject.FindProperty("m_CanInteract");
            m_IsHidden = serializedObject.FindProperty("m_IsHidden");
            m_UseCustomMessage = serializedObject.FindProperty("m_UseCustomMessage");
            m_CustomMessage = serializedObject.FindProperty("m_CustomMessage");
            m_OnInteract = serializedObject.FindProperty("m_OnInteract");
        }

#if !UNITY_2019_2_OR_NEWER
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
#else
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            root.Add(new PropertyField(m_CanInteract));
            root.Add(new PropertyField(m_IsHidden));

            root.Add(GoldPlayerUIHelper.GetSpace());

            useCustomMessageElement = new PropertyField(m_UseCustomMessage);
            customMessageElement = new PropertyField(m_CustomMessage);

            useCustomMessageElement.RegisterCallback<ChangeEvent<bool>>((evt) => { customMessageElement.SetEnabled(evt.newValue); });

            customMessageElement.SetEnabled(m_UseCustomMessage.boolValue);

            root.Add(useCustomMessageElement);
            root.Add(customMessageElement);

            root.Add(new PropertyField(m_OnInteract));

            return root;
        }
    }
#endif
}
#endif
