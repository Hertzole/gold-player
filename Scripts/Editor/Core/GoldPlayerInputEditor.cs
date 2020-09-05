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
        private SerializedProperty inputs;

        private ReorderableList list;

        private void OnEnable()
        {
            useKeyCodes = serializedObject.FindProperty("useKeyCodes");
            inputs = serializedObject.FindProperty("inputs");

            list = new ReorderableList(serializedObject, inputs, true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) =>
                {
                    float oWidth = rect.width - 16;
                    rect.x += 16;
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, oWidth / 3, rect.height), new GUIContent("Button Name", "The name components uses to get input."));
                    EditorGUI.LabelField(new Rect(rect.x + oWidth / 3, rect.y, oWidth / 3, rect.height), new GUIContent("Input Name", "The name of the input in the input manager."));
                    EditorGUI.LabelField(new Rect(rect.x + (oWidth / 3) * 2, rect.y, oWidth / 3, rect.height), new GUIContent("Button Name", "The key code for the input."));
                },
                drawElementCallback = DrawElement
            };
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            float oWidth = rect.width;
            rect.height = EditorGUIUtility.singleLineHeight;
            SerializedProperty element = inputs.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, oWidth / 3 - 4, rect.height), element.FindPropertyRelative("buttonName"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + oWidth / 3, rect.y, oWidth / 3 - 4, rect.height), element.FindPropertyRelative("inputName"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + (oWidth / 3) * 2, rect.y, oWidth / 3, rect.height), element.FindPropertyRelative("key"), GUIContent.none);
        }

        public override void OnInspectorGUI()
        {
#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
            if (GUILayout.Button("Replace with Gold Player Input System"))
            {
                GameObject go = ((GoldPlayerInput)target).gameObject;

                Undo.DestroyObjectImmediate(go.GetComponent<GoldPlayerInput>());
                Undo.AddComponent<GoldPlayerInputSystem>(go);
            }
#else
            serializedObject.Update();

            EditorGUILayout.PropertyField(useKeyCodes);

            EditorGUILayout.Space();

            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
#endif
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
