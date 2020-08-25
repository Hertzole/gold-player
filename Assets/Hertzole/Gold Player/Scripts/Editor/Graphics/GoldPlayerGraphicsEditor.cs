#if !GOLD_PLAYER_DISABLE_GRAPHICS
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditor.EditorGUIUtility;

namespace Hertzole.GoldPlayer.Editor
{
    [CustomEditor(typeof(GoldPlayerGraphics))]
    public class GoldPlayerGraphicsEditor : UnityEditor.Editor
    {
        private SerializedProperty owner;
        private SerializedProperty objects;

        private ReorderableList list;

        private void OnEnable()
        {
            owner = serializedObject.FindProperty("owner");
            objects = serializedObject.FindProperty("objects");
            list = new ReorderableList(serializedObject, objects, true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, objects.displayName); },
                drawElementCallback = DrawElementInList,
                elementHeightCallback = CalculateElementHeight
            };
        }

        private float CalculateElementHeight(int index)
        {
            SerializedProperty element = objects.GetArrayElementAtIndex(index);
            if (element.isExpanded)
            {
                return (singleLineHeight + standardVerticalSpacing) * 5;
            }
            else
            {
                return singleLineHeight + standardVerticalSpacing;
            }
        }

        private void DrawElementInList(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = objects.GetArrayElementAtIndex(index);
            SerializedProperty target = element.FindPropertyRelative("target");

            rect.x += 12;
            rect.width -= 12;
            rect.height = singleLineHeight;

            element.isExpanded = EditorGUI.Foldout(rect, element.isExpanded, target.objectReferenceValue == null ? element.displayName : target.objectReferenceValue.name, true);
            if (element.isExpanded)
            {
                rect.x += 8;
                rect.y += standardVerticalSpacing + singleLineHeight;
                rect.width -= 8;
                EditorGUI.PropertyField(rect, target);
                rect.y += standardVerticalSpacing + singleLineHeight;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("isParent"));
                rect.y += standardVerticalSpacing + singleLineHeight;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("whenMyGraphics"));
                rect.y += standardVerticalSpacing + singleLineHeight;
                EditorGUI.PropertyField(rect, element.FindPropertyRelative("whenOtherGraphics"));
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(owner);
            EditorGUILayout.Space();
            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
