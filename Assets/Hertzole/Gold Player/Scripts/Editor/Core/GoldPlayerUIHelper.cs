#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hertzole.GoldPlayer.Editor
{
    public static class GoldPlayerUIHelper
    {
        public static VisualElement GetSpace(float space = 8)
        {
            return new VisualElement() { style = { height = space } };
        }

        public static Label GetHeaderLabel(string text)
        {
            return new Label(text)
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    paddingTop = 9,
                    paddingLeft = 2,
                    paddingBottom = 0
                }
            };
        }

        public static IMGUIContainer GetHelpBox(string message, MessageType type)
        {
            return new IMGUIContainer(() =>
            {
                EditorGUILayout.HelpBox(message, type);
            });
        }
    }
}
#endif