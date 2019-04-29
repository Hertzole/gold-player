#if UNITY_EDITOR
using Hertzole.GoldPlayer.Core;
using UnityEditor;
#if UNITY_2019_2_OR_NEWER
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#else
using UnityEngine;
#endif

namespace Hertzole.GoldPlayer.Editor
{
    [CustomPropertyDrawer(typeof(AudioItem))]
    internal class AudioItemEditor : PropertyDrawer
    {
#if !UNITY_2019_2_OR_NEWER
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
#else
        private VisualElement elements;
        private VisualElement randomPitchElements;

        private VisualElement enabled;
        private VisualElement randomPitch;
        private VisualElement pitch;
        private VisualElement minPitch;
        private VisualElement maxPitch;
        private VisualElement changeVolume;
        private VisualElement volume;
        private VisualElement audioClips;
#endif

#if !UNITY_2019_2_OR_NEWER
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
            EditorGUI.PropertyField(fieldRect, property, false);
            // Only draw the rest if the property is expanded.
            if (property.isExpanded)
            {
                //Indent the GUI one step.
                EditorGUI.indentLevel++;
                // Add to the rect.
                AddToRect();
                // The 'Enabled' field.
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("enabled"));
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
                    // The 'Pitch' label.
                    EditorGUI.LabelField(new Rect(fieldRect.x, fieldRect.y, EditorGUIUtility.labelWidth, fieldRect.height), "Pitch", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Normal });
                    // Remove the indent to stop some weird GUI behaviour.
                    EditorGUI.indentLevel--;
                    // Draw 'Min' label.
                    EditorGUI.LabelField(new Rect(fieldRect.x + EditorGUIUtility.labelWidth, fieldRect.y, 30, fieldRect.height), new GUIContent("Min", property.FindPropertyRelative("minPitch").tooltip), new GUIStyle(GUI.skin.label) { fontStyle = property.FindPropertyRelative("minPitch").prefabOverride ? FontStyle.Bold : FontStyle.Normal });
                    // The 'Min Pitch' field.
                    EditorGUI.PropertyField(new Rect(fieldRect.x + EditorGUIUtility.labelWidth + 30, fieldRect.y, ((fieldRect.width - EditorGUIUtility.labelWidth) / 2) - 32, fieldRect.height), property.FindPropertyRelative("minPitch"), GUIContent.none);
                    // Draw 'Max' label.
                    EditorGUI.LabelField(new Rect(fieldRect.x + EditorGUIUtility.labelWidth + ((fieldRect.width - EditorGUIUtility.labelWidth) / 2) + 2, fieldRect.y, 30, fieldRect.height), new GUIContent("Max", property.FindPropertyRelative("maxPitch").tooltip), new GUIStyle(GUI.skin.label) { fontStyle = property.FindPropertyRelative("maxPitch").prefabOverride ? FontStyle.Bold : FontStyle.Normal });
                    // The 'Max Pitch' field.
                    EditorGUI.PropertyField(new Rect(fieldRect.x + EditorGUIUtility.labelWidth + ((fieldRect.width - EditorGUIUtility.labelWidth) / 2) + 33, fieldRect.y, ((fieldRect.width - EditorGUIUtility.labelWidth) / 2) - 33, fieldRect.height), property.FindPropertyRelative("maxPitch"), GUIContent.none);
                    // Add the indent again.
                    EditorGUI.indentLevel++;
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
                if (property.FindPropertyRelative("changeVolume").boolValue)
                {
                    // Add to the rect.
                    AddToRect();
                    // The volume slider field.
                    EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("volume"));
                }
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
                fullRect = CalculateFullRectHeight(property);
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
                rect.height += lineHeight + padding;
                rect.height += lineHeight + padding;
                rect.height += lineHeight + padding;
                rect.height += lineHeight + padding;
                if (property.FindPropertyRelative("changeVolume").boolValue)
                    rect.height += lineHeight + padding;
                rect.height += lineHeight + padding;
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
#else
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            Foldout foldout = new Foldout { text = property.displayName, value = property.isExpanded, };
            //TODO: Fix isExpanded being set when any bool is changed.
            foldout.RegisterValueChangedCallback((evt) => { property.isExpanded = evt.newValue; });

            elements = new VisualElement();
            foldout.contentContainer.Add(elements);

            enabled = new PropertyField(property.FindPropertyRelative("enabled"));
            randomPitch = new PropertyField(property.FindPropertyRelative("randomPitch"));
            pitch = new PropertyField(property.FindPropertyRelative("pitch"));
            minPitch = new PropertyField(property.FindPropertyRelative("minPitch"));
            maxPitch = new PropertyField(property.FindPropertyRelative("maxPitch"));

            //TODO: Make one liner.
            randomPitchElements = new VisualElement();
            randomPitchElements.Add(minPitch);
            randomPitchElements.Add(maxPitch);

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
            minPitch.SetEnabled(toggle);
            maxPitch.SetEnabled(toggle);
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
