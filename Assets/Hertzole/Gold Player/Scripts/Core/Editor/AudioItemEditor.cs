using Hertzole.GoldPlayer.Core;
using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    //DOCUMENT: AudioItemEditor
    [CustomPropertyDrawer(typeof(AudioItem))]
    public class AudioItemEditor : PropertyDrawer
    {
        private Rect m_FullRect;
        private Rect m_FieldRect;
        private readonly float lineHeight = EditorGUIUtility.singleLineHeight;
        private readonly float padding = EditorGUIUtility.standardVerticalSpacing;

        private float m_MinPitch;
        private float m_MaxPitch;

        private bool m_DoGUI = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            m_DoGUI = true;
            EditorGUI.BeginProperty(position, label, property);
            m_FullRect = position;
            m_FullRect.height = lineHeight;
            m_FieldRect = position;
            m_FieldRect.height = lineHeight;
            EditorGUI.PropertyField(m_FieldRect, property, false);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                AddToRect();
                EditorGUI.PropertyField(m_FieldRect, property.FindPropertyRelative("m_Enabled"));
                AddToRect();
                EditorGUI.PropertyField(m_FieldRect, property.FindPropertyRelative("m_RandomPitch"));
                AddToRect();
                if (property.FindPropertyRelative("m_RandomPitch").boolValue)
                {
                    EditorGUI.LabelField(new Rect(m_FieldRect.x, m_FieldRect.y, EditorGUIUtility.labelWidth, m_FieldRect.height), "Pitch", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Normal });
                    EditorGUI.indentLevel--;
                    EditorGUI.LabelField(new Rect(m_FieldRect.x + EditorGUIUtility.labelWidth, m_FieldRect.y, 30, m_FieldRect.height), new GUIContent("Min", property.FindPropertyRelative("m_MinPitch").tooltip), new GUIStyle(GUI.skin.label) { fontStyle = property.FindPropertyRelative("m_MinPitch").prefabOverride ? FontStyle.Bold : FontStyle.Normal });
                    EditorGUI.PropertyField(new Rect(m_FieldRect.x + EditorGUIUtility.labelWidth + 30, m_FieldRect.y, ((m_FieldRect.width - EditorGUIUtility.labelWidth) / 2) - 32, m_FieldRect.height), property.FindPropertyRelative("m_MinPitch"), GUIContent.none);
                    EditorGUI.LabelField(new Rect(m_FieldRect.x + EditorGUIUtility.labelWidth + ((m_FieldRect.width - EditorGUIUtility.labelWidth) / 2) + 2, m_FieldRect.y, 30, m_FieldRect.height), new GUIContent("Max", property.FindPropertyRelative("m_MaxPitch").tooltip), new GUIStyle(GUI.skin.label) { fontStyle = property.FindPropertyRelative("m_MaxPitch").prefabOverride ? FontStyle.Bold : FontStyle.Normal });
                    EditorGUI.PropertyField(new Rect(m_FieldRect.x + EditorGUIUtility.labelWidth + ((m_FieldRect.width - EditorGUIUtility.labelWidth) / 2) + 33, m_FieldRect.y, ((m_FieldRect.width - EditorGUIUtility.labelWidth) / 2) - 33, m_FieldRect.height), property.FindPropertyRelative("m_MaxPitch"), GUIContent.none);
                    EditorGUI.indentLevel++;
                }
                else
                {
                    EditorGUI.PropertyField(m_FieldRect, property.FindPropertyRelative("m_Pitch"));
                }
                AddToRect();
                EditorGUI.PropertyField(m_FieldRect, property.FindPropertyRelative("m_ChangeVolume"));
                if (property.FindPropertyRelative("m_ChangeVolume").boolValue)
                {
                    AddToRect();
                    EditorGUI.Slider(m_FieldRect, property.FindPropertyRelative("m_Volume"), 0f, 1f);
                }
                AddToRect();
                EditorGUI.PropertyField(m_FieldRect, property.FindPropertyRelative("m_AudioClips"), true);
                if (property.FindPropertyRelative("m_AudioClips").isExpanded)
                {
                    AddToRect();
                    for (int i = 0; i < property.FindPropertyRelative("m_AudioClips").arraySize; i++)
                    {
                        AddToRect();
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
        }

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
