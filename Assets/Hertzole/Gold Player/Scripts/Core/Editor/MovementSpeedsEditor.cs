using Hertzole.GoldPlayer.Core;
using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    [CustomPropertyDrawer(typeof(MovementSpeeds))]
    public class MovementSpeedsEditor : PropertyDrawer
    {
        public static readonly float gap = EditorGUIUtility.singleLineHeight + 2;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {           
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.Foldout(position, true, label); // TODO: Allow user to actually use the folout :)
            position.height = EditorGUIUtility.singleLineHeight;
            position.y += gap;
            
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;
            
            EditorGUI.PropertyField(position, property.FindPropertyRelative("m_ForwardSpeed"));
            position.y += gap;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("m_SidewaysSpeed"));
            position.y += gap;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("m_BackwardsSpeed"));
            position.y += gap;
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + gap * 3;
        }
    }
}