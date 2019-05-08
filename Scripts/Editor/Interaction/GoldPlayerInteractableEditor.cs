#if UNITY_EDITOR
using UnityEditor;
#if UNITY_2019_1_OR_NEWER
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
        private SerializedProperty canInteract;
        private SerializedProperty isHidden;
        private SerializedProperty useCustomMessage;
        private SerializedProperty customMessage;
        private SerializedProperty onInteract;

#if UNITY_2019_1_OR_NEWER
        private VisualElement useCustomMessageElement;
        private VisualElement customMessageElement;
#endif

        private void OnEnable()
        {
            canInteract = serializedObject.FindProperty("canInteract");
            isHidden = serializedObject.FindProperty("isHidden");
            useCustomMessage = serializedObject.FindProperty("useCustomMessage");
            customMessage = serializedObject.FindProperty("customMessage");
            onInteract = serializedObject.FindProperty("onInteract");
        }

#if !UNITY_2019_1_OR_NEWER
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(canInteract);
            EditorGUILayout.PropertyField(isHidden);

            EditorGUILayout.PropertyField(useCustomMessage);
            GUI.enabled = useCustomMessage.boolValue;
            EditorGUILayout.PropertyField(customMessage);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(onInteract);

            serializedObject.ApplyModifiedProperties();
        }
#else
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            root.Add(new PropertyField(canInteract));
            root.Add(new PropertyField(isHidden));

            root.Add(GoldPlayerUIHelper.GetSpace());

            useCustomMessageElement = new PropertyField(useCustomMessage);
            customMessageElement = new PropertyField(customMessage);

            useCustomMessageElement.RegisterCallback<ChangeEvent<bool>>((evt) => { customMessageElement.SetEnabled(evt.newValue); });

            customMessageElement.SetEnabled(useCustomMessage.boolValue);

            root.Add(useCustomMessageElement);
            root.Add(customMessageElement);

            root.Add(new PropertyField(onInteract));

            return root;
        }
#endif
    }
}
#endif
