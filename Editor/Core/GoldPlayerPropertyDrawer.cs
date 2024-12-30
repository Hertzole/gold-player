using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    public abstract class GoldPlayerPropertyDrawer : PropertyDrawer
    {
        protected static EditorGUIAdaption GUIAdaption { get { return GoldPlayerProjectSettings.Instance.GUIAdapation; } }

        protected const float SPACE_HEIGHT = 6f;

        protected static Rect NextRow(Rect position)
        {
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return position;
        }

        protected static Rect DrawSpace(Rect position)
        {
            position.y += SPACE_HEIGHT;
            return position;
        }

        protected static Rect DrawNextProperty(Rect position, SerializedProperty property)
        {
            position = NextRow(position);
            EditorGUI.PropertyField(position, property);
            return position;
        }

        protected static float GetFieldHeight()
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
