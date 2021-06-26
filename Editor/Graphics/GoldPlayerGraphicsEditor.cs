#if GOLD_PLAYER_DISABLE_GRAPHICS
#define OBSOLETE
#endif

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
#pragma warning disable CS0618 // Type or member is obsolete
    [CustomEditor(typeof(GoldPlayerGraphics))]
    public class GoldPlayerGraphicsEditor : UnityEditor.Editor
    {
        private SerializedProperty owner;
        private SerializedProperty objects;

        private ReorderableList list;

        private float singleLineHeight { get { return EditorGUIUtility.singleLineHeight; } }
        private float standardVerticalSpacing { get { return EditorGUIUtility.standardVerticalSpacing; } }

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
#if !OBSOLETE
            serializedObject.Update();

            EditorGUILayout.PropertyField(owner);
            EditorGUILayout.Space();
            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
#else
            if (GUILayout.Button("Remove Component"))
            {
                Undo.DestroyObjectImmediate((GoldPlayerGraphics)target);
            }
#endif
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
