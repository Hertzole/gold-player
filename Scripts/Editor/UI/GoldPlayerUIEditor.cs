#if GOLD_PLAYER_DISABLE_UI
#define OBSOLETE
#endif

using UnityEditor;
#if OBSOLETE
using UnityEngine;
#endif

namespace Hertzole.GoldPlayer.Editor
{
#pragma warning disable CS0618 // Type or member is obsolete
    [CustomEditor(typeof(GoldPlayerUI))]
    public class GoldPlayerUIEditor : UnityEditor.Editor
    {
        private SerializedProperty autoFindPlayer;
        private SerializedProperty player;
        private SerializedProperty sprintingBarType;
        private SerializedProperty sprintingBarImage;
        private SerializedProperty sprintingBarSlider;
        private SerializedProperty sprintingLabel;
        private SerializedProperty sprintingLabelPro;
        private SerializedProperty sprintingLabelDisplay;
        private SerializedProperty autoFindInteraction;
        private SerializedProperty playerInteraction;
        private SerializedProperty interactionBox;
        private SerializedProperty interactionLabel;
        private SerializedProperty interactionLabelPro;

        // Get all the serialized properties from the target script.
        private void OnEnable()
        {
            autoFindPlayer = serializedObject.FindProperty("autoFindPlayer");
            player = serializedObject.FindProperty("player");
            sprintingBarType = serializedObject.FindProperty("sprintingBarType");
            sprintingBarImage = serializedObject.FindProperty("sprintingBarImage");
            sprintingBarSlider = serializedObject.FindProperty("sprintingBarSlider");
            sprintingLabel = serializedObject.FindProperty("sprintingLabel");
            sprintingLabelPro = serializedObject.FindProperty("sprintingLabelPro");
            sprintingLabelDisplay = serializedObject.FindProperty("sprintingLabelDisplay");
            autoFindInteraction = serializedObject.FindProperty("autoFindInteraction");
            playerInteraction = serializedObject.FindProperty("playerInteraction");
            interactionBox = serializedObject.FindProperty("interactionBox");
            interactionLabel = serializedObject.FindProperty("interactionLabel");
            interactionLabelPro = serializedObject.FindProperty("interactionLabelPro");
        }

        // Draw all the GUI in the inspector.
        public override void OnInspectorGUI()
        {
#if !OBSOLETE
            serializedObject.Update();

            EditorGUILayout.PropertyField(autoFindPlayer, true);
            EditorGUILayout.PropertyField(player, true);
            EditorGUILayout.PropertyField(sprintingBarType, true);
            EditorGUILayout.PropertyField(sprintingBarImage, true);
            EditorGUILayout.PropertyField(sprintingBarSlider, true);
            EditorGUILayout.PropertyField(sprintingLabel, true);
            EditorGUILayout.PropertyField(sprintingLabelPro, true);
            EditorGUILayout.PropertyField(sprintingLabelDisplay, true);
            EditorGUILayout.PropertyField(autoFindInteraction, true);
            EditorGUILayout.PropertyField(playerInteraction, true);
            EditorGUILayout.PropertyField(interactionBox, true);
            EditorGUILayout.PropertyField(interactionLabel, true);
            EditorGUILayout.PropertyField(interactionLabelPro, true);
            
            serializedObject.ApplyModifiedProperties();
#else
            if (GUILayout.Button("Remove Component"))
            {
                Undo.DestroyObjectImmediate((GoldPlayerUI)target);
            }
#endif
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
