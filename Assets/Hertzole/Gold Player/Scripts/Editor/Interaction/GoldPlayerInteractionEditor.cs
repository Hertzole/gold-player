#if GOLD_PLAYER_DISABLE_INTERACTION
#define OBSOLETE
#endif

using UnityEditor;
#if OBSOLETE
using UnityEngine;
#endif

namespace Hertzole.GoldPlayer.Editor
{
#pragma warning disable CS0618 // Type or member is obsolete
    [CustomEditor(typeof(GoldPlayerInteraction))]
    public class GoldPlayerInteractionEditor : UnityEditor.Editor
    {
#if !OBSOLETE
        private SerializedProperty cameraHead;
        private SerializedProperty interactionRange;
        private SerializedProperty interactionLayer;
        private SerializedProperty ignoreTriggers;
        private SerializedProperty interactMessage;
        private SerializedProperty interactInput;

        private void OnEnable()
        {
            cameraHead = serializedObject.FindProperty("cameraHead");
            interactionRange = serializedObject.FindProperty("interactionRange");
            interactionLayer = serializedObject.FindProperty("interactionLayer");
            ignoreTriggers = serializedObject.FindProperty("ignoreTriggers");
            interactMessage = serializedObject.FindProperty("interactMessage");
            interactInput = serializedObject.FindProperty("interactInput");
        }
#endif

        public override void OnInspectorGUI()
        {
#if !OBSOLETE
            serializedObject.Update();

            EditorGUILayout.PropertyField(cameraHead);
            EditorGUILayout.PropertyField(interactionRange);
            EditorGUILayout.PropertyField(interactionLayer);
            EditorGUILayout.PropertyField(ignoreTriggers);
            EditorGUILayout.PropertyField(interactMessage);
            EditorGUILayout.PropertyField(interactInput);

            serializedObject.ApplyModifiedProperties();
#else
            if (GUILayout.Button("Remove Component"))
            {
                Undo.DestroyObjectImmediate((GoldPlayerInteraction)target);
            }
#endif
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
