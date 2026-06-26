#if UNITY_EDITOR
#if UNITY_2021_3_OR_NEWER
#define USE_UI_ELEMENTS
#endif
using System;
using UnityEngine;
using UnityEditor;
#if USE_UI_ELEMENTS
using UnityEngine.UIElements;
using UnityEditor.UIElements;
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

		private static readonly GUIContent pitchLabel = new GUIContent("Pitch");
		private static readonly GUIContent minLabel = new GUIContent("Min");
		private static readonly GUIContent maxLabel = new GUIContent("Max");

#if USE_UI_ELEMENTS
		private VisualElement elements;

		private PropertyField enabled;
		private PropertyField randomPitch;
		private PropertyField pitch;
		private MinMaxField pitchMinMax;
		private PropertyField changeVolume;
		private PropertyField volume;
		private PropertyField audioClips;
#endif

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
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
							30, pitchLabel, true, minLabel, maxLabel);
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
        ///     Adds to the full rect and field rect.
        /// </summary>
        private void AddToRect()
		{
			fullRect.height += lineHeight + padding;
			fieldRect.y += lineHeight + padding;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			fullRect = CalculateFullRectHeight(property);

			return fullRect.height;
		}

        /// <summary>
        ///     Calculates the full height of the property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private Rect CalculateFullRectHeight(SerializedProperty property)
		{
			Rect rect = new Rect(0, 0, 0, lineHeight);
			if (property.isExpanded)
			{
				rect.height += lineHeight + padding;
				if (!property.FindPropertyRelative("enabled").boolValue && GoldPlayerProjectSettings.Instance.GUIAdapation == EditorGUIAdaption.HideUnused)
				{
					return rect;
				}

				rect.height += (lineHeight + padding) * 3;

				if (property.FindPropertyRelative("changeVolume").boolValue)
				{
					rect.height += lineHeight + padding;
				}

				rect.height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("audioClips")) + padding;
			}

			return rect;
		}

#if USE_UI_ELEMENTS
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();

			Foldout foldout = new Foldout { text = property.displayName, value = property.isExpanded };
			foldout.RegisterValueChangedCallback(evt => { property.isExpanded = foldout.value; });

			elements = new VisualElement();
			foldout.contentContainer.Add(elements);

			enabled = new PropertyField(property.FindPropertyRelative("enabled"));
			randomPitch = new PropertyField(property.FindPropertyRelative("randomPitch"));
			pitch = new PropertyField(property.FindPropertyRelative("pitch"));
			pitchMinMax = new MinMaxField("Random Pitch", property.FindPropertyRelative("minPitch"), property.FindPropertyRelative("maxPitch"));
			pitchMinMax.AddToClassList(MinMaxField.alignedFieldUssClassName);

			changeVolume = new PropertyField(property.FindPropertyRelative("changeVolume"));
			volume = new PropertyField(property.FindPropertyRelative("volume"));
			audioClips = new PropertyField(property.FindPropertyRelative("audioClips"));

			elements.Add(enabled);
			elements.Add(randomPitch);
			elements.Add(pitch);
			elements.Add(pitchMinMax);
			elements.Add(changeVolume);
			elements.Add(volume);
			elements.Add(audioClips);

			enabled.RegisterValueChangeCallback(evt => ToggleEnabled(evt.changedProperty.boolValue, property));
			randomPitch.RegisterValueChangeCallback(evt => ToggleRandomPitch(evt.changedProperty.boolValue, property));
			changeVolume.RegisterValueChangeCallback(evt => ToggleVolume(evt.changedProperty.boolValue, property));

			ToggleEnabled(property.FindPropertyRelative("enabled").boolValue, property);
			ToggleRandomPitch(property.FindPropertyRelative("randomPitch").boolValue, property);
			ToggleVolume(property.FindPropertyRelative("changeVolume").boolValue, property);

			root.Add(foldout);

			return root;
		}

		private void ToggleEnabled(bool toggle, SerializedProperty property)
		{
			bool randomPitchValue = property.FindPropertyRelative("randomPitch").boolValue;
			bool changeVolumeValue = property.FindPropertyRelative("changeVolume").boolValue;

			switch (GoldPlayerProjectSettings.Instance.GUIAdapation)
			{
				case EditorGUIAdaption.AlwaysShow:
					randomPitch.SetEnabled(true);
					pitch.SetEnabled(true);
					pitchMinMax.SetEnabled(true);
					changeVolume.SetEnabled(true);
					volume.SetEnabled(true);
					audioClips.SetEnabled(true);

					SetVisible(randomPitch, true);
					SetVisible(pitch, !randomPitchValue);
					SetVisible(pitchMinMax, randomPitchValue);
					SetVisible(changeVolume, true);
					SetVisible(volume, changeVolumeValue);
					SetVisible(audioClips, true);
					break;
				case EditorGUIAdaption.HideUnused:
					SetVisible(randomPitch, toggle);
					SetVisible(pitch, toggle && !randomPitchValue);
					SetVisible(pitchMinMax, toggle && randomPitchValue);
					SetVisible(changeVolume, toggle);
					SetVisible(volume, toggle && changeVolumeValue);
					SetVisible(audioClips, toggle);
					break;
				case EditorGUIAdaption.DisableUnused:
					randomPitch.SetEnabled(toggle);
					pitch.SetEnabled(toggle && !randomPitchValue);
					SetVisible(pitch, !randomPitchValue);
					pitchMinMax.SetEnabled(toggle && randomPitchValue);
					SetVisible(pitchMinMax, randomPitchValue);
					changeVolume.SetEnabled(toggle);
					volume.SetEnabled(toggle && randomPitchValue);
					SetVisible(volume, randomPitchValue);
					audioClips.SetEnabled(toggle);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void ToggleRandomPitch(bool randomPitchValue, SerializedProperty property)
		{
			if (property.FindPropertyRelative("enabled").boolValue)
			{
				SetVisible(pitchMinMax, randomPitchValue);
				pitchMinMax.SetEnabled(true);
				SetVisible(pitch, !randomPitchValue);
				pitch.SetEnabled(true);
			}
		}

		private void ToggleVolume(bool changeVolumeValue, SerializedProperty property)
		{
			if (property.FindPropertyRelative("enabled").boolValue)
			{
				SetVisible(volume, changeVolumeValue);
				volume.SetEnabled(true);
			}
		}

		private static void SetVisible(VisualElement element, bool visible)
		{
			element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
		}
#endif
	}
}
#endif