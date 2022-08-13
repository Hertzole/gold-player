#if GOLD_PLAYER_DISABLE_OBJECT_BOB
#define OBSOLETE
#endif
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
#pragma warning disable CS0618 // Type or member is obsolete
    [CustomEditor(typeof(GoldPlayerObjectBob))]
    public class GoldPlayerObjectBobEditor : UnityEditor.Editor
    {
        private ReorderableList list;

        private SerializedProperty targetsProp;

        private void OnEnable()
        {
            targetsProp = serializedObject.FindProperty("targets");

            list = new ReorderableList(serializedObject, targetsProp, true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, new GUIContent(targetsProp.displayName, targetsProp.tooltip)); },
                drawElementCallback = DrawElement,
                elementHeightCallback = CalculateHeight,
                onAddCallback = OnAdd
            };
        }

        private void OnAdd(ReorderableList list)
        {
            int index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("enableBob").boolValue = true;
            element.FindPropertyRelative("unscaledTime").boolValue = false;
            element.FindPropertyRelative("bobFrequency").floatValue = 1.5f;
            element.FindPropertyRelative("bobHeight").floatValue = 0.05f;
            element.FindPropertyRelative("swayAngle").floatValue = 0.5f;
            element.FindPropertyRelative("sideMovement").floatValue = 0.05f;
            element.FindPropertyRelative("heightMultiplier").floatValue = 0.3f;
            element.FindPropertyRelative("strideMultiplier").floatValue = 0.3f;
            element.FindPropertyRelative("landMove").floatValue = 0.3f;
            element.FindPropertyRelative("landTilt").floatValue = 10;
            element.FindPropertyRelative("enableStrafeTilting").boolValue = true;
            element.FindPropertyRelative("strafeTilt").floatValue = 3;
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = targetsProp.GetArrayElementAtIndex(index);
            rect.x += 12;
            rect.width -= 12;
            rect.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty bobTarget = element.FindPropertyRelative("bobTarget");

            EditorGUI.PropertyField(rect, element, new GUIContent(bobTarget.objectReferenceValue == null ? element.displayName : bobTarget.objectReferenceValue.name), true);
        }

        private float CalculateHeight(int index)
        {
            SerializedProperty element = targetsProp.GetArrayElementAtIndex(index);
            if (element.isExpanded)
            {
                // Line height + spacing * 14 elements + (8) spacing * 4 (spacing attributes) + some more spacing.
                return ((EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 14) + (8 * 4) + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        public override void OnInspectorGUI()
        {
#if !OBSOLETE
            serializedObject.Update();

            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
#else
            if (GUILayout.Button("Remove Component"))
            {
                Undo.DestroyObjectImmediate((GoldPlayerObjectBob)target);
            }
#endif
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
