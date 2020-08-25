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

        public static VisualElement GetHeaderLabel(string text)
        {
            VisualElement labelHolder = new VisualElement();
            labelHolder.AddToClassList("unity-property-field");
            labelHolder.AddToClassList("unity-base-field");

            labelHolder.style.marginTop = 4;

            Label label = new Label(text);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            labelHolder.Add(label);

            return labelHolder;
        }

        public static VisualElement GetHelpBox(string message, MessageType type)
        {
            return new IMGUIContainer(() =>
            {
                EditorGUILayout.HelpBox(message, type);
            });
        }
    }
}
#endif
