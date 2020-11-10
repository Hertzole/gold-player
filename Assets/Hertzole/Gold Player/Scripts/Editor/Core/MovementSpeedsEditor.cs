using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    [CustomPropertyDrawer(typeof(MovementSpeeds))]
    public class MovementSpeedsEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PrefixLabel(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), label);

            float width = ((EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 34) / 3) - 4;
            position = new Rect(0, position.y, 0, EditorGUIUtility.singleLineHeight)
            {
                x = position.x + EditorGUIUtility.labelWidth,
                width = width
            };

            float oWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 12;

            EditorGUI.PropertyField(position, property.FindPropertyRelative("forwardSpeed"), new GUIContent("F"));

            position = new Rect(0, position.y, width, EditorGUIUtility.singleLineHeight)
            {
                x = position.x + width + 5,
            };

            EditorGUI.PropertyField(position, property.FindPropertyRelative("sidewaysSpeed"), new GUIContent("S"));

            position = new Rect(0, position.y, width, EditorGUIUtility.singleLineHeight)
            {
                x = position.x + width + 5,
            };

            EditorGUI.PropertyField(position, property.FindPropertyRelative("backwardsSpeed"), new GUIContent("B"));

            EditorGUIUtility.labelWidth = oWidth;
        }
    }
}
