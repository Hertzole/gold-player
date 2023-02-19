using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    [CustomPropertyDrawer(typeof(BobClass))]
    public class BobClassEditor : GoldPlayerPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                position = DrawNextProperty(position, property.FindPropertyRelative("enableBob"));
                GoldPlayerUIHelper.DrawElementsConditional(property.FindPropertyRelative("enableBob"), () =>
                {
                    position = DrawNextProperty(position, property.FindPropertyRelative("unscaledTime"));

                    position = DrawSpace(position);

                    position = DrawNextProperty(position, property.FindPropertyRelative("bobFrequency"));
                    position = DrawNextProperty(position, property.FindPropertyRelative("bobHeight"));
                    position = DrawNextProperty(position, property.FindPropertyRelative("swayAngle"));
                    position = DrawNextProperty(position, property.FindPropertyRelative("sideMovement"));
                    position = DrawNextProperty(position, property.FindPropertyRelative("heightMultiplier"));
                    position = DrawNextProperty(position, property.FindPropertyRelative("strideMultiplier"));

                    position = DrawSpace(position);

                    position = DrawNextProperty(position, property.FindPropertyRelative("landMove"));
                    position = DrawNextProperty(position, property.FindPropertyRelative("landTilt"));

                    position = DrawSpace(position);

                    position = DrawNextProperty(position, property.FindPropertyRelative("enableStrafeTilting"));
                    GoldPlayerUIHelper.DrawElementsConditional(property.FindPropertyRelative("enableStrafeTilting"), () =>
                    {
                        position = DrawNextProperty(position, property.FindPropertyRelative("strafeTilt"));
                    });

                    position = DrawSpace(position);

                    position = DrawNextProperty(position, property.FindPropertyRelative("bobTarget"));
                });
                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                height += GetFieldHeight();

                if (GoldPlayerUIHelper.ShouldShowElements(property.FindPropertyRelative("enableBob")))
                {
                    height += (GetFieldHeight() * 11) + (SPACE_HEIGHT * 4);

                    if (GoldPlayerUIHelper.ShouldShowElements(property.FindPropertyRelative("enableStrafeTilting")))
                    {
                        height += GetFieldHeight();
                    }
                }

            }

            return height;
        }
    }
}
