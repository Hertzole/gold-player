using Hertzole.GoldPlayer.Core;
using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    [CustomPropertyDrawer(typeof(AudioItem))]
    public class AudioItemEditor : PropertyDrawer
    {
        // The full complete rect.
        private Rect m_FullRect;
        // The rect for the current field.
        private Rect m_FieldRect;
        // Shortcut for EditorGUIUtility.singleLineHeight.
        private readonly float lineHeight = EditorGUIUtility.singleLineHeight;
        // Shortcut for EditorGUIUtility.standardVerticalSpacing.
        private readonly float padding = EditorGUIUtility.standardVerticalSpacing;

        // Check to see if the property height should be from the GUI.
        private bool m_DoGUI = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Set 'doGUI' to true as we want to bae it of the GUI.
            m_DoGUI = true;
            // Begin the property GUI.
            EditorGUI.BeginProperty(position, label, property);
            // Set the full rect to the provided position.
            m_FullRect = position;
            // Set the full rect height to the line height.
            m_FullRect.height = lineHeight;
            // Set the field rect to the provided position.
            m_FieldRect = position;
            // Set the field rect height to the line height.
            m_FieldRect.height = lineHeight;
            // The property foldout.
            EditorGUI.PropertyField(m_FieldRect, property, false);
            // Only draw the rest if the property is expanded.
            if (property.isExpanded)
            {
                //Indent the GUI one step.
                EditorGUI.indentLevel++;
                // Add to the rect.
                AddToRect();
                // The 'Enabled' field.
                EditorGUI.PropertyField(m_FieldRect, property.FindPropertyRelative("m_Enabled"));
                // Add to the rect.
                AddToRect();
                // The 'Random Pitch' field.
                EditorGUI.PropertyField(m_FieldRect, property.FindPropertyRelative("m_RandomPitch"));
                // Add to the rect.
                AddToRect();
                // If random pitch is enabled, draw min max fields.
                // Else just draw one field.
                if (property.FindPropertyRelative("m_RandomPitch").boolValue)
                {
                    // The 'Pitch' label.
                    EditorGUI.LabelField(new Rect(m_FieldRect.x, m_FieldRect.y, EditorGUIUtility.labelWidth, m_FieldRect.height), "Pitch", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Normal });
                    // Remove the indent to stop some weird GUI behaviour.
                    EditorGUI.indentLevel--;
                    // Draw 'Min' label.
                    EditorGUI.LabelField(new Rect(m_FieldRect.x + EditorGUIUtility.labelWidth, m_FieldRect.y, 30, m_FieldRect.height), new GUIContent("Min", property.FindPropertyRelative("m_MinPitch").tooltip), new GUIStyle(GUI.skin.label) { fontStyle = property.FindPropertyRelative("m_MinPitch").prefabOverride ? FontStyle.Bold : FontStyle.Normal });
                    // The 'Min Pitch' field.
                    EditorGUI.PropertyField(new Rect(m_FieldRect.x + EditorGUIUtility.labelWidth + 30, m_FieldRect.y, ((m_FieldRect.width - EditorGUIUtility.labelWidth) / 2) - 32, m_FieldRect.height), property.FindPropertyRelative("m_MinPitch"), GUIContent.none);
                    // Draw 'Max' label.
                    EditorGUI.LabelField(new Rect(m_FieldRect.x + EditorGUIUtility.labelWidth + ((m_FieldRect.width - EditorGUIUtility.labelWidth) / 2) + 2, m_FieldRect.y, 30, m_FieldRect.height), new GUIContent("Max", property.FindPropertyRelative("m_MaxPitch").tooltip), new GUIStyle(GUI.skin.label) { fontStyle = property.FindPropertyRelative("m_MaxPitch").prefabOverride ? FontStyle.Bold : FontStyle.Normal });
                    // The 'Max Pitch' field.
                    EditorGUI.PropertyField(new Rect(m_FieldRect.x + EditorGUIUtility.labelWidth + ((m_FieldRect.width - EditorGUIUtility.labelWidth) / 2) + 33, m_FieldRect.y, ((m_FieldRect.width - EditorGUIUtility.labelWidth) / 2) - 33, m_FieldRect.height), property.FindPropertyRelative("m_MaxPitch"), GUIContent.none);
                    // Add the indent again.
                    EditorGUI.indentLevel++;
                }
                else
                {
                    // The 'Pitch' field.
                    EditorGUI.PropertyField(m_FieldRect, property.FindPropertyRelative("m_Pitch"));
                }
                // Add to the rect.
                AddToRect();
                // The 'Change Volume' field.
                EditorGUI.PropertyField(m_FieldRect, property.FindPropertyRelative("m_ChangeVolume"));
                // If change volume is true, draw the volume field.
                if (property.FindPropertyRelative("m_ChangeVolume").boolValue)
                {
                    // Add to the rect.
                    AddToRect();
                    // The volume slider field.
                    EditorGUI.PropertyField(m_FieldRect, property.FindPropertyRelative("m_Volume"));
                }
                // Add to the rect.
                AddToRect();
                // The audio clips array.
                EditorGUI.PropertyField(m_FieldRect, property.FindPropertyRelative("m_AudioClips"), true);
                // If the audio clips array is expanded, add to the rect to make sure everything is shown.
                if (property.FindPropertyRelative("m_AudioClips").isExpanded)
                {
                    // Add a rect for the size field.
                    AddToRect();
                    // For every clip, add a size for every field.
                    for (int i = 0; i < property.FindPropertyRelative("m_AudioClips").arraySize; i++)
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
            m_FullRect.height += lineHeight + padding;
            m_FieldRect.y += lineHeight + padding;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!m_DoGUI)
                m_FullRect = CalculateFullRectHeight(property);
            return m_FullRect.height;
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
                if (property.FindPropertyRelative("m_ChangeVolume").boolValue)
                    rect.height += lineHeight + padding;
                rect.height += lineHeight + padding;
                if (property.FindPropertyRelative("m_AudioClips").isExpanded)
                {
                    rect.height += lineHeight + padding;
                    for (int i = 0; i < property.FindPropertyRelative("m_AudioClips").arraySize; i++)
                    {
                        rect.height += lineHeight + padding;
                    }
                }
            }
            return rect;
        }
    }
}
