using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    [CustomPropertyDrawer(typeof(MovementSpeeds))]
    public class MovementSpeedsEditor : PropertyDrawer
    {
        private static readonly GUIContent forwardSpeedLabel = new GUIContent("F", "The speed when moving forward.");
        private static readonly GUIContent backwardSpeedLabel = new GUIContent("B", "The speed when moving backward.");
        private static readonly GUIContent strafeSpeedLabel = new GUIContent("S", "The speed when strafing.");
        
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

            EditorGUI.PropertyField(position, property.FindPropertyRelative("forwardSpeed"), forwardSpeedLabel);

            position = new Rect(0, position.y, width, EditorGUIUtility.singleLineHeight)
            {
                x = position.x + width + 5,
            };

            EditorGUI.PropertyField(position, property.FindPropertyRelative("sidewaysSpeed"), strafeSpeedLabel);

            position = new Rect(0, position.y, width, EditorGUIUtility.singleLineHeight)
            {
                x = position.x + width + 5,
            };

            EditorGUI.PropertyField(position, property.FindPropertyRelative("backwardsSpeed"), backwardSpeedLabel);

            EditorGUIUtility.labelWidth = oWidth;
        }
    }
}
