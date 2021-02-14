#if UNITY_EDITOR
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
    [CustomPropertyDrawer(typeof(AudioItem))]
    internal class AudioItemEditor : PropertyDrawer
    {
        // The full complete rect.
        private Rect fullRect;
        // The rect for the current field.
        private Rect fieldRect;
        // Shortcut for EditorGUIUtility.singleLineHeight.
        private readonly float lineHeight = EditorGUIUtility.singleLineHeight;
        // Shortcut for EditorGUIUtility.standardVerticalSpacing.
        private readonly float padding = EditorGUIUtility.standardVerticalSpacing;

        // Check to see if the property height should be from the GUI.
        private bool doGUI = false;

#if USE_UI_ELEMENTS
        private VisualElement elements;
        private VisualElement randomPitchElements;

        private VisualElement enabled;
        private VisualElement randomPitch;
        private VisualElement pitch;
        private VisualElement changeVolume;
        private VisualElement volume;
        private VisualElement audioClips;
#endif

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Set 'doGUI' to true as we want to bae it of the GUI.
            doGUI = true;
            // Begin the property GUI.
            EditorGUI.BeginProperty(position, label, property);
            // Set the full rect to the provided position.
            fullRect = position;
            // Set the full rect height to the line height.
            fullRect.height = lineHeight;
            // Set the field rect to the provided position.
            fieldRect = position;
            // Set the field rect height to the line height.
            fieldRect.height = lineHeight;
            // The property foldout.
            EditorGUI.PropertyField(fieldRect, property, label, false);
            // Only draw the rest if the property is expanded.
            if (property.isExpanded)
            {
                //Indent the GUI one step.
                EditorGUI.indentLevel++;
                // Add to the rect.
                AddToRect();
                // The 'Enabled' field.
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("enabled"));

                GoldPlayerUIHelper.DrawElementsConditional(property.FindPropertyRelative("enabled"), () =>
                {
                    // Add to the rect.
                    AddToRect();
                    // The 'Random Pitch' field.
                    EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("randomPitch"));
                    // Add to the rect.
                    AddToRect();
                    // If random pitch is enabled, draw min max fields.
                    // Else just draw one field.
                    if (property.FindPropertyRelative("randomPitch").boolValue)
                    {
                        GoldPlayerUIHelper.DrawCustomVector2Field(fieldRect,
                            property.FindPropertyRelative("minPitch"),
                            property.FindPropertyRelative("maxPitch"),
                            30, new GUIContent("Pitch"), true, new GUIContent("Min"), new GUIContent("Max"));
                    }
                    else
                    {
                        // The 'Pitch' field.
                        EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("pitch"));
                    }
                    // Add to the rect.
                    AddToRect();
                    // The 'Change Volume' field.
                    EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("changeVolume"));
                    // If change volume is true, draw the volume field.
                    GoldPlayerUIHelper.DrawElementsConditional(property.FindPropertyRelative("changeVolume"), () =>
                    {
                        // Add to the rect.
                        AddToRect();
                        // The volume slider field.
                        EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("volume"));
                    });
                    // Add to the rect.
                    AddToRect();
                    // The audio clips array.
                    EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("audioClips"), true);
                    // If the audio clips array is expanded, add to the rect to make sure everything is shown.
                    if (property.FindPropertyRelative("audioClips").isExpanded)
                    {
                        // Add a rect for the size field.
                        AddToRect();
                        // For every clip, add a size for every field.
                        for (int i = 0; i < property.FindPropertyRelative("audioClips").arraySize; i++)
                        {
                            AddToRect();
                        }
                    }
                });

                // Remove the indent.
                EditorGUI.indentLevel--;
            }
            // End the property GUI.
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Adds to the full rect and field rect.
        /// </summary>
        private void AddToRect()
        {
            fullRect.height += lineHeight + padding;
            fieldRect.y += lineHeight + padding;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!doGUI)
            {
                fullRect = CalculateFullRectHeight(property);
            }

            return fullRect.height;
        }

        /// <summary>
        /// Calculates the full height of the property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private Rect CalculateFullRectHeight(SerializedProperty property)
        {
            Rect rect = new Rect(0, 0, 0, lineHeight);
            if (property.isExpanded)
            {
                rect.height += (lineHeight + padding) * 5;
                if (property.FindPropertyRelative("changeVolume").boolValue)
                {
                    rect.height += lineHeight + padding;
                }

                if (property.FindPropertyRelative("audioClips").isExpanded)
                {
                    rect.height += lineHeight + padding;
                    for (int i = 0; i < property.FindPropertyRelative("audioClips").arraySize; i++)
                    {
                        rect.height += lineHeight + padding;
                    }
                }
            }
            return rect;
        }

#if USE_UI_ELEMENTS
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            Foldout foldout = new Foldout { text = property.displayName, value = property.isExpanded };
            foldout.RegisterValueChangedCallback((evt) => { property.isExpanded = foldout.value; });

            elements = new VisualElement();
            foldout.contentContainer.Add(elements);

            enabled = new PropertyField(property.FindPropertyRelative("enabled"));
            randomPitch = new PropertyField(property.FindPropertyRelative("randomPitch"));
            pitch = new PropertyField(property.FindPropertyRelative("pitch"));

            randomPitchElements = new VisualElement();
            randomPitchElements.AddToClassList("unity-property-field");

            VisualElement basePitchFields = new VisualElement();
            basePitchFields.AddToClassList("unity-base-field");
            basePitchFields.AddToClassList("unity-base-text-field");
            basePitchFields.AddToClassList("unity-float-field");

            Label pitchLabel = new Label("Pitch");
            pitchLabel.AddToClassList("unity-text-element");
            pitchLabel.AddToClassList("unity-label");
            pitchLabel.AddToClassList("unity-base-field__label");
            pitchLabel.AddToClassList("unity-base-text-field__label");
            pitchLabel.AddToClassList("unity-float-field__label");
            pitchLabel.AddToClassList("unity-property-field__label");

            FloatField minPitchField = new FloatField
            {
                bindingPath = property.FindPropertyRelative("minPitch").propertyPath,
            };
            minPitchField.style.flexGrow = 1;
            minPitchField.AddToClassList("unity-property-field__input");

            FloatField maxPitchField = new FloatField
            {
                bindingPath = property.FindPropertyRelative("maxPitch").propertyPath,
            };
            maxPitchField.style.flexGrow = 1;
            maxPitchField.AddToClassList("unity-property-field__input");

            VisualElement pitchFields = new VisualElement()
            {
                name = "pitch-fields"
            };
            pitchFields.style.flexDirection = FlexDirection.Row;
            pitchFields.style.flexGrow = 1;

            Label minLabel = new Label("Min");
            minLabel.AddToClassList("unity-text-element");
            minLabel.AddToClassList("unity-label");
            minLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

            pitchFields.Add(minLabel);
            pitchFields.Add(minPitchField);

            Label maxLabel = new Label("Max");
            maxLabel.AddToClassList("unity-text-element");
            maxLabel.AddToClassList("unity-label");
            maxLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

            pitchFields.Add(maxLabel);
            pitchFields.Add(maxPitchField);

            basePitchFields.Add(pitchLabel);
            basePitchFields.Add(pitchFields);

            randomPitchElements.Add(basePitchFields);

            changeVolume = new PropertyField(property.FindPropertyRelative("changeVolume"));
            volume = new PropertyField(property.FindPropertyRelative("volume"));
            audioClips = new PropertyField(property.FindPropertyRelative("audioClips"));

            elements.Add(enabled);
            elements.Add(randomPitch);
            elements.Add(pitch);
            elements.Add(randomPitchElements);
            elements.Add(changeVolume);
            elements.Add(volume);
            elements.Add(audioClips);

            enabled.RegisterCallback<ChangeEvent<bool>>(x => ToggleEnabled(x.newValue));
            randomPitch.RegisterCallback<ChangeEvent<bool>>(x => ToggleRandomPitch(x.newValue));
            changeVolume.RegisterCallback<ChangeEvent<bool>>(x => ToggleVolume(x.newValue));

            ToggleEnabled(property.FindPropertyRelative("enabled").boolValue);
            ToggleRandomPitch(property.FindPropertyRelative("randomPitch").boolValue);
            ToggleVolume(property.FindPropertyRelative("changeVolume").boolValue);

            root.Add(foldout);

            return root;
        }

        private void ToggleEnabled(bool toggle)
        {
            randomPitch.SetEnabled(toggle);
            pitch.SetEnabled(toggle);
            randomPitchElements.SetEnabled(toggle);
            changeVolume.SetEnabled(toggle);
            volume.SetEnabled(toggle);
            audioClips.SetEnabled(toggle);
        }

        private void ToggleRandomPitch(bool randomPitch)
        {
            randomPitchElements.style.display = randomPitch ? DisplayStyle.Flex : DisplayStyle.None;
            pitch.style.display = randomPitch ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void ToggleVolume(bool changeVolume)
        {
            volume.style.display = changeVolume ? DisplayStyle.Flex : DisplayStyle.None;
        }
#endif
    }
}
#endif
