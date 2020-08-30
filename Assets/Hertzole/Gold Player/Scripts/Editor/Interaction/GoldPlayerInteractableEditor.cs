#if GOLD_PLAYER_DISABLE_INTERACTION
#define OBSOLETE
#endif

#if UNITY_2019_1_OR_NEWER
#define USE_UI_ELEMENTS
#endif

using UnityEditor;
using UnityEngine;
#if USE_UI_ELEMENTS
using UnityEngine.UIElements;
#if !OBSOLETE
using UnityEditor.UIElements;
#endif
#endif

namespace Hertzole.GoldPlayer.Editor
{
#pragma warning disable CS0618 // Type or member is obsolete
    [CustomEditor(typeof(GoldPlayerInteractable))]
    internal class GoldPlayerInteractableEditor : UnityEditor.Editor
    {
        private SerializedProperty canInteract;
        private SerializedProperty isHidden;
        private SerializedProperty useCustomMessage;
        private SerializedProperty customMessage;
        private SerializedProperty onInteract;

#if USE_UI_ELEMENTS
        private VisualElement useCustomMessageElement;
        private VisualElement customMessageElement;
#endif

        private void OnEnable()
        {
#if !OBSOLETE
            canInteract = serializedObject.FindProperty("canInteract");
            isHidden = serializedObject.FindProperty("isHidden");
            useCustomMessage = serializedObject.FindProperty("useCustomMessage");
            customMessage = serializedObject.FindProperty("customMessage");
            onInteract = serializedObject.FindProperty("onInteract");
#endif
        }

        public override void OnInspectorGUI()
        {
#if !OBSOLETE
            serializedObject.Update();

            EditorGUILayout.PropertyField(canInteract);
            EditorGUILayout.PropertyField(isHidden);

            EditorGUILayout.PropertyField(useCustomMessage);
            GUI.enabled = useCustomMessage.boolValue;
            EditorGUILayout.PropertyField(customMessage);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(onInteract);

            serializedObject.ApplyModifiedProperties();
#else
            if (GUILayout.Button("Remove Component"))
            {
                Undo.DestroyObjectImmediate((GoldPlayerInteractable)target);
            }
#endif

        }
#if USE_UI_ELEMENTS
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

#if !OBSOLETE
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
#else
            Button removeButton = new Button(() => { Undo.DestroyObjectImmediate((GoldPlayerInteractable)target); })
            {
                text = "Remove Component"
            };
            root.Add(removeButton);
#endif

            return root;
        }
#endif
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
