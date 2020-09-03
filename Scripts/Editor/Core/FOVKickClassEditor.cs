#if UNITY_2019_1_OR_NEWER
#define USE_UI_ELEMENTS
#endif
using UnityEditor;
using UnityEngine;
#if USE_UI_ELEMENTS
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace Hertzole.GoldPlayer.Editor
{
    [CustomPropertyDrawer(typeof(FOVKickClass))]
    public class FOVKickClassEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(position, property);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("enableFOVKick"));
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("unscaledTime"));
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("kickWhen"));
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("kickAmount"));
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("lerpTimeTo"));
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, property.FindPropertyRelative("lerpTimeFrom"));

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + 10;
#if GOLD_PLAYER_CINEMACHINE
                EditorGUI.PropertyField(position, property.FindPropertyRelative("useCinemachine"));
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (property.FindPropertyRelative("useCinemachine").boolValue)
                {
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("targetVirtualCamera"));
                }
                else
                {
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("targetCamera"));
                }
#else
                EditorGUI.PropertyField(position, property.FindPropertyRelative("targetCamera"));
#endif

                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
#if GOLD_PLAYER_CINEMACHINE
                int propCount = 9;
#else
                int propCount = 8;
#endif
                return (EditorGUIUtility.singleLineHeight * propCount) + (EditorGUIUtility.standardVerticalSpacing * propCount) + 10;
            }
        }

#if USE_UI_ELEMENTS
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Foldout foldout = new Foldout();
            foldout.BindProperty(property);
            foldout.text = property.displayName;

            foldout.contentContainer.Add(new PropertyField(property.FindPropertyRelative("enableFOVKick")));
            foldout.contentContainer.Add(new PropertyField(property.FindPropertyRelative("unscaledTime")));
            foldout.contentContainer.Add(new PropertyField(property.FindPropertyRelative("kickWhen")));
            foldout.contentContainer.Add(new PropertyField(property.FindPropertyRelative("kickAmount")));
            foldout.contentContainer.Add(new PropertyField(property.FindPropertyRelative("lerpTimeTo")));
            foldout.contentContainer.Add(new PropertyField(property.FindPropertyRelative("lerpTimeFrom")));

            foldout.contentContainer.Add(GoldPlayerUIHelper.GetSpace());

            PropertyField targetCamera = new PropertyField(property.FindPropertyRelative("targetCamera"));
#if GOLD_PLAYER_CINEMACHINE
            PropertyField cineToggle = new PropertyField(property.FindPropertyRelative("useCinemachine"));
            PropertyField cineCamera = new PropertyField(property.FindPropertyRelative("targetVirtualCamera"));

            // Put the register value changed in GeometryChangedEvent because then the property will have been rebuilt.
            cineToggle.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                cineToggle.Q<Toggle>().RegisterValueChangedCallback(x =>
                {
                    targetCamera.style.display = x.newValue ? DisplayStyle.None : DisplayStyle.Flex;
                    cineCamera.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                });
            });

            bool useCinemachine = property.FindPropertyRelative("useCinemachine").boolValue;
            targetCamera.style.display = useCinemachine ? DisplayStyle.None : DisplayStyle.Flex;
            cineCamera.style.display = useCinemachine ? DisplayStyle.Flex : DisplayStyle.None;

            foldout.contentContainer.Add(cineToggle);
            foldout.contentContainer.Add(cineCamera);
#endif
            foldout.contentContainer.Add(targetCamera);


            return foldout;
        }
#endif
    }
}
