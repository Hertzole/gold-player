#if GOLD_PLAYER_DISABLE_INTERACTION
#define OBSOLETE
#endif

#if UNITY_2019_1_OR_NEWER
//#define USE_UI_ELEMENTS
#endif

using UnityEditor;
using UnityEngine;
#if USE_UI_ELEMENTS
using UnityEngine.UIElements;
#if !OBSOLETE
using UnityEditor.UIElements;
#endif
#endif
using static Hertzole.GoldPlayer.Editor.GoldPlayerUIHelper;

namespace Hertzole.GoldPlayer.Editor
{
#pragma warning disable CS0618 // Type or member is obsolete
    [CustomEditor(typeof(GoldPlayerInteractable))]
    internal class GoldPlayerInteractableEditor : UnityEditor.Editor
    {
#if !OBSOLETE
        private SerializedProperty isInteractable;
        private SerializedProperty isHidden;
        private SerializedProperty useCustomMessage;
        private SerializedProperty customMessage;
        private SerializedProperty limitedInteractions;
        private SerializedProperty maxInteractions;
        private SerializedProperty onInteract;
        private SerializedProperty onReachedMaxInteraction;

        private GoldPlayerInteractable interactable;
#endif

#if USE_UI_ELEMENTS
        private VisualElement useCustomMessageElement;
        private VisualElement customMessageElement;
        private VisualElement maxInteractionsElement;
#endif

        private void OnEnable()
        {
#if !OBSOLETE
            isInteractable = serializedObject.FindProperty("isInteractable");
            isHidden = serializedObject.FindProperty("isHidden");
            useCustomMessage = serializedObject.FindProperty("useCustomMessage");
            customMessage = serializedObject.FindProperty("customMessage");
            limitedInteractions = serializedObject.FindProperty("limitedInteractions");
            maxInteractions = serializedObject.FindProperty("maxInteractions");
            onInteract = serializedObject.FindProperty("onInteract");
            onReachedMaxInteraction = serializedObject.FindProperty("onReachedMaxInteractions");

            interactable = (GoldPlayerInteractable)target;
#endif
        }

        public override void OnInspectorGUI()
        {
#if !OBSOLETE
            serializedObject.Update();

            EditorGUILayout.PropertyField(isInteractable);
            EditorGUILayout.PropertyField(isHidden);

            DrawElementsConditional(!isHidden.boolValue, () =>
            {
                EditorGUILayout.PropertyField(useCustomMessage);
                DrawElementsConditional(useCustomMessage, () => EditorGUILayout.PropertyField(customMessage));
            });

            EditorGUILayout.PropertyField(limitedInteractions);
            DrawElementsConditional(limitedInteractions, () => EditorGUILayout.PropertyField(maxInteractions));

            bool oldEnabled = GUI.enabled;
            if (Application.isPlaying && limitedInteractions.boolValue)
            {
                GUI.enabled = false;
                EditorGUILayout.LabelField("Interactions: " + interactable.Interactions);
            }
            GUI.enabled = oldEnabled;

            EditorGUILayout.PropertyField(onInteract);
            DrawElementsConditional(limitedInteractions, () => EditorGUILayout.PropertyField(onReachedMaxInteraction));

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
            root.Add(new PropertyField(isInteractable));
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
