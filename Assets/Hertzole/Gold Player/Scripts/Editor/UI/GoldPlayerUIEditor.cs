#if GOLD_PLAYER_DISABLE_UI
#define OBSOLETE
#endif

#if UNITY_2018_1_OR_NEWER || GOLD_PLAYER_TMP
#define USE_TMP
#endif

#if !UNITY_2019_2_OR_NEWER || (UNITY_2019_2_OR_NEWER && GOLD_PLAYER_UGUI)
#define USE_GUI
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
#if USE_GUI
        private SerializedProperty autoFindPlayer;
        private SerializedProperty player;
        private SerializedProperty sprintingBarType;
        private SerializedProperty sprintingBarImage;
        private SerializedProperty sprintingBarSlider;
        private SerializedProperty sprintingLabel;
        private SerializedProperty sprintingLabelDisplay;
        private SerializedProperty autoFindInteraction;
        private SerializedProperty playerInteraction;
        private SerializedProperty interactionBox;
        private SerializedProperty interactionLabel;
#if USE_TMP
        private SerializedProperty sprintingLabelPro;
        private SerializedProperty interactionLabelPro;
#endif

        // Get all the serialized properties from the target script.
        private void OnEnable()
        {
            autoFindPlayer = serializedObject.FindProperty("autoFindPlayer");
            player = serializedObject.FindProperty("player");
            sprintingBarType = serializedObject.FindProperty("sprintingBarType");
            sprintingBarImage = serializedObject.FindProperty("sprintingBarImage");
            sprintingBarSlider = serializedObject.FindProperty("sprintingBarSlider");
            sprintingLabel = serializedObject.FindProperty("sprintingLabel");
            sprintingLabelDisplay = serializedObject.FindProperty("sprintingLabelDisplay");
            autoFindInteraction = serializedObject.FindProperty("autoFindInteraction");
            playerInteraction = serializedObject.FindProperty("playerInteraction");
            interactionBox = serializedObject.FindProperty("interactionBox");
            interactionLabel = serializedObject.FindProperty("interactionLabel");
#if USE_TMP
            sprintingLabelPro = serializedObject.FindProperty("sprintingLabelPro");
            interactionLabelPro = serializedObject.FindProperty("interactionLabelPro");
#endif
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
#if USE_TMP
            EditorGUILayout.PropertyField(sprintingLabelPro, true);
#endif
            EditorGUILayout.PropertyField(sprintingLabelDisplay, true);
            EditorGUILayout.PropertyField(autoFindInteraction, true);
            EditorGUILayout.PropertyField(playerInteraction, true);
            EditorGUILayout.PropertyField(interactionBox, true);
            EditorGUILayout.PropertyField(interactionLabel, true);
#if USE_TMP
            EditorGUILayout.PropertyField(interactionLabelPro, true);
#endif

            serializedObject.ApplyModifiedProperties();
#else
            if (GUILayout.Button("Remove Component"))
            {
                Undo.DestroyObjectImmediate((GoldPlayerUI)target);
            }
#endif
        }
#endif // USE_GUI
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
