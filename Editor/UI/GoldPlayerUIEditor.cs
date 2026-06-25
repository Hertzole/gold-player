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
        private SerializedProperty staminaBarType;
        private SerializedProperty staminaBarImage;
        private SerializedProperty staminaBarSlider;
        private SerializedProperty staminaLabel;
        private SerializedProperty staminaLabelDisplay;
        private SerializedProperty staminaLabelChangeRequired;
        private SerializedProperty staminaPercentageFormat;
        private SerializedProperty staminaDirectValueFormat;
        private SerializedProperty staminaDirectMaxFormat;
#if !GOLD_PLAYER_DISABLE_INTERACTION
        private SerializedProperty autoFindInteraction;
        private SerializedProperty playerInteraction;
        private SerializedProperty interactionBox;
        private SerializedProperty interactionLabel;
#endif
#if USE_TMP
        private SerializedProperty staminaLabelPro;
#if !GOLD_PLAYER_DISABLE_INTERACTION
        private SerializedProperty interactionLabelPro;
#endif
#endif

        // Get all the serialized properties from the target script.
        private void OnEnable()
        {
            autoFindPlayer = serializedObject.FindProperty("autoFindPlayer");
            player = serializedObject.FindProperty("player");
            staminaBarType = serializedObject.FindProperty("staminaBarType");
            staminaBarImage = serializedObject.FindProperty("staminaBarImage");
            staminaBarSlider = serializedObject.FindProperty("staminaBarSlider");
            staminaLabel = serializedObject.FindProperty("staminaLabel");
#if USE_TMP
            staminaLabelPro = serializedObject.FindProperty("staminaLabelPro");
#endif
            staminaLabelDisplay = serializedObject.FindProperty("staminaLabelDisplay");
            staminaLabelChangeRequired = serializedObject.FindProperty("staminaLabelChangeRequired");
            staminaPercentageFormat = serializedObject.FindProperty("staminaPercentageFormat");
            staminaDirectValueFormat = serializedObject.FindProperty("staminaDirectValueFormat");
            staminaDirectMaxFormat = serializedObject.FindProperty("staminaDirectMaxFormat");
#if !GOLD_PLAYER_DISABLE_INTERACTION
            autoFindInteraction = serializedObject.FindProperty("autoFindInteraction");
            playerInteraction = serializedObject.FindProperty("playerInteraction");
            interactionBox = serializedObject.FindProperty("interactionBox");
            interactionLabel = serializedObject.FindProperty("interactionLabel");
#if USE_TMP
            interactionLabelPro = serializedObject.FindProperty("interactionLabelPro");
#endif
#endif
        }

        // Draw all the GUI in the inspector.
        public override void OnInspectorGUI()
        {
#if !OBSOLETE
            serializedObject.Update();

            EditorGUILayout.PropertyField(autoFindPlayer, true);
            EditorGUILayout.PropertyField(player, true);
            EditorGUILayout.PropertyField(staminaBarType, true);
            switch (staminaBarType.enumValueIndex)
            {
                case 0:
                    EditorGUILayout.PropertyField(staminaBarSlider, true);
                    break;
                case 1:
                    EditorGUILayout.PropertyField(staminaBarImage, true);
                    break;
            }
            EditorGUILayout.PropertyField(staminaLabel, true);
#if USE_TMP
            EditorGUILayout.PropertyField(staminaLabelPro, true);
#endif
	        EditorGUILayout.PropertyField(staminaLabelChangeRequired, true);
            EditorGUILayout.PropertyField(staminaLabelDisplay, true);
            switch (staminaLabelDisplay.enumValueIndex)
            {
                case 0:
                    EditorGUILayout.PropertyField(staminaDirectValueFormat);
                    EditorGUILayout.PropertyField(staminaDirectMaxFormat);
                    break;
                case 1:
                    EditorGUILayout.PropertyField(staminaPercentageFormat);
                    break;
                default:
                    break;
            }
#if !GOLD_PLAYER_DISABLE_INTERACTION
            EditorGUILayout.PropertyField(autoFindInteraction, true);
            EditorGUILayout.PropertyField(playerInteraction, true);
            EditorGUILayout.PropertyField(interactionBox, true);
            EditorGUILayout.PropertyField(interactionLabel, true);
#if USE_TMP
            EditorGUILayout.PropertyField(interactionLabelPro, true);
#endif
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
