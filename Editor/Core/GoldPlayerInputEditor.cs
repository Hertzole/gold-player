#pragma warning disable CS0618 // Type or member is obsolete
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    [CustomEditor(typeof(GoldPlayerInput))]
    public class GoldPlayerInputEditor : UnityEditor.Editor
    {
        private SerializedProperty useKeyCodes;
        private SerializedProperty autoEnableInput;
        private SerializedProperty autoDisableInput;
        private SerializedProperty inputs;

        private ReorderableList list;

        private float FieldHeight { get { return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; } }
        
        private static readonly GUIContent inputsContent = new GUIContent("Inputs");
        private static readonly GUIContent buttonNameContent = new GUIContent("Name");
        private static readonly GUIContent buttonTypeContent = new GUIContent("Type");
        private static readonly GUIContent inputNameContent = new GUIContent("Input Name");
        private static readonly GUIContent vector2InputNameContent = new GUIContent("Vector2 Input Name");
        private static readonly GUIContent vector2InputNameXContent = new GUIContent("X", "The input action that will serve the X axis.");
        private static readonly GUIContent vector2InputNameYContent = new GUIContent("Y", "The input action that will serve the Y axis.");

        private void OnEnable()
        {
            useKeyCodes = serializedObject.FindProperty("useKeyCodes");
            autoEnableInput = serializedObject.FindProperty("autoEnableInput");
            autoDisableInput = serializedObject.FindProperty("autoDisableInput");
            inputs = serializedObject.FindProperty("inputs");

            list = new ReorderableList(serializedObject, inputs, true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, inputsContent);

                },
                drawElementCallback = DrawElement,
                elementHeightCallback = CalculateHeight
            };
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            SerializedProperty element = inputs.GetArrayElementAtIndex(index);
            SerializedProperty type = element.FindPropertyRelative("type");
            EditorGUI.PropertyField(rect, element.FindPropertyRelative("buttonName"), buttonNameContent);
            rect.y += FieldHeight;
            EditorGUI.PropertyField(rect, type, buttonTypeContent);
            rect.y += FieldHeight;
            if (type.enumValueIndex != 2)
            {
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("inputName"), inputNameContent);
            }
            else
            {
                GoldPlayerUIHelper.DrawCustomVector2Field(rect,
                    element.FindPropertyRelative("inputName"),
                    element.FindPropertyRelative("inputNameSecondary"),
                    10, vector2InputNameContent, false,
                    vector2InputNameXContent,
                    vector2InputNameYContent);
            }

            rect.y += FieldHeight;

            if (type.enumValueIndex == 0 && useKeyCodes.boolValue)
            {
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("key"));
            }
        }

        private float CalculateHeight(int index)
        {
            SerializedProperty item = inputs.GetArrayElementAtIndex(index);
            float height = FieldHeight * 3 + 5;

            if (item.FindPropertyRelative("type").enumValueIndex == 0 && useKeyCodes.boolValue)
            {
                height += FieldHeight;
            }

            return height;
        }

        public override void OnInspectorGUI()
        {
#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT && !ENABLE_LEGACY_INPUT_MANAGER
            if (GUILayout.Button("Replace with Gold Player Input System"))
            {
                GameObject go = ((GoldPlayerInput)target).gameObject;

                Undo.DestroyObjectImmediate(go.GetComponent<GoldPlayerInput>());
                Undo.AddComponent<GoldPlayerInputSystem>(go);
            }
#else
            serializedObject.Update();

            EditorGUILayout.PropertyField(useKeyCodes);
            EditorGUILayout.PropertyField(autoEnableInput);
            EditorGUILayout.PropertyField(autoDisableInput);

            EditorGUILayout.Space();

            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
#endif
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
