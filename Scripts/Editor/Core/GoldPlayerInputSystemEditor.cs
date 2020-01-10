using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hertzole.GoldPlayer.Editor
{
    [CustomEditor(typeof(GoldPlayerInputSystem))]
    public class GoldPlayerInputSystemEditor : UnityEditor.Editor
    {
        private SerializedProperty actionAsset;
        private SerializedProperty autoEnableInput;
        private SerializedProperty autoDisableInput;
        private SerializedProperty actions;

        private ReorderableList actionList;

        private InputActionAsset previousAsset;
        private InputActionReference[] availableActions;
        private string[] availableActionsNames;

        private bool hasActionAsset;

        private void OnEnable()
        {
            actionAsset = serializedObject.FindProperty("inputAsset");
            autoEnableInput = serializedObject.FindProperty("autoEnableInput");
            autoDisableInput = serializedObject.FindProperty("autoDisableInput");
            actions = serializedObject.FindProperty("actions");

            previousAsset = actionAsset.objectReferenceValue as InputActionAsset;
            hasActionAsset = previousAsset != null;

            CreateActionList();

            PopulateActions();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(actionAsset);
            if (EditorGUI.EndChangeCheck())
            {
                InputActionAsset actions = actionAsset.objectReferenceValue as InputActionAsset;

                hasActionAsset = actions != null;

                serializedObject.ApplyModifiedProperties();
                PopulateActions();
                serializedObject.Update();

                if (actions != previousAsset)
                {
                    ReassignActions();
                    previousAsset = actions;
                }
            }

            EditorGUILayout.PropertyField(autoEnableInput);
            EditorGUILayout.PropertyField(autoDisableInput);

            EditorGUILayout.Space();

            actionList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void CreateActionList()
        {
            actionList = new ReorderableList(serializedObject, actions)
            {
                drawHeaderCallback = (Rect rect) =>
                {
                    Rect nameRect = new Rect(rect.x + 14, rect.y, rect.width / 2 - 18, EditorGUIUtility.singleLineHeight);
                    Rect actionRect = new Rect(rect.x + (rect.width / 2 + 12), rect.y, rect.width / 2 - 12, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(nameRect, "Action Name");
                    EditorGUI.LabelField(actionRect, "Target Action");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty element = actions.GetArrayElementAtIndex(index);

                    Rect nameRect = new Rect(rect.x, rect.y + 2, rect.width / 2 - 4, EditorGUIUtility.singleLineHeight);
                    Rect actionRect = new Rect(rect.x + (rect.width / 2 + 4), rect.y + 2, rect.width / 2 - 4, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(nameRect, actions.GetArrayElementAtIndex(index).FindPropertyRelative("actionName"), GUIContent.none);

                    if (hasActionAsset)
                    {
                        int popupIndex = Array.IndexOf(availableActions, element.FindPropertyRelative("action").objectReferenceValue) + 1;
                        EditorGUI.BeginChangeCheck();
                        popupIndex = EditorGUI.Popup(actionRect, string.Empty, popupIndex, availableActionsNames);

                        if (EditorGUI.EndChangeCheck())
                        {
                            element.FindPropertyRelative("action").objectReferenceValue = popupIndex > 0 ? availableActions[popupIndex - 1] : null;
                        }
                    }
                    else
                    {
                        EditorGUI.LabelField(actionRect, "No input asset!", EditorStyles.boldLabel);
                    }
                },
                elementHeight = EditorGUIUtility.singleLineHeight + 4
            };
        }

        private void PopulateActions()
        {
            availableActions = GetAllActionsFromAsset(actionAsset.objectReferenceValue as InputActionAsset);
            // Ugly hack: GenericMenu interprets "/" as a submenu path. But luckily, "/" is not the only slash we have in Unicode.
            availableActionsNames = new[] { "None" }.Concat(availableActions?.Select(x => x.name.Replace("/", "\uFF0F")) ?? new string[0]).ToArray();
        }

        private void ReassignActions()
        {
            InputActionAsset asset = actionAsset.objectReferenceValue as InputActionAsset;

            for (int i = 0; i < actions.arraySize; i++)
            {
                SerializedProperty element = actions.GetArrayElementAtIndex(i);
                if (asset == null || asset != previousAsset)
                {
                    element.FindPropertyRelative("action").objectReferenceValue = null;
                }

                if (availableActions != null)
                {
                    foreach (InputActionReference action in availableActions)
                    {
                        if (string.Compare(action.action.name, element.FindPropertyRelative("actionName").stringValue, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            element.FindPropertyRelative("action").objectReferenceValue = action;
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private InputActionReference[] GetAllActionsFromAsset(InputActionAsset actions)
        {
            if (actions != null)
            {
                string path = AssetDatabase.GetAssetPath(actions);
                UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                return assets.Where(asset => asset is InputActionReference).Cast<InputActionReference>().OrderBy(x => x.name).ToArray();
            }

            return null;
        }
    }
}
