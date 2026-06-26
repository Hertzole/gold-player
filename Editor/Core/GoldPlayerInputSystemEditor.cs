#pragma warning disable CS0618 // Type or member is obsolete

#if !ENABLE_INPUT_SYSTEM || !GOLD_PLAYER_NEW_INPUT
#define OBSOLETE
#endif
using UnityEditor;
using UnityEngine;
#if !OBSOLETE
using System;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEditorInternal;
using System.IO;
#endif

namespace Hertzole.GoldPlayer.Editor
{
    [CustomEditor(typeof(GoldPlayerInputSystem))]
    public class GoldPlayerInputSystemEditor : UnityEditor.Editor
    {
#if !OBSOLETE
        private SerializedProperty playerInput;
        private SerializedProperty actionAsset;
        private SerializedProperty autoEnableInput;
        private SerializedProperty autoDisableInput;
        private SerializedProperty actions;

        private ReorderableList actionList;

        private InputActionAsset previousAsset;
        private InputActionReference[] availableActions;
        private string[] availableActionsNames;

        private void OnEnable()
        {
            GetProperties();

            previousAsset = actionAsset.objectReferenceValue as InputActionAsset;

            CreateActionList();

            PopulateActions();
        }

        private void GetProperties()
        {
            playerInput = serializedObject.FindProperty("playerInput");
            actionAsset = serializedObject.FindProperty("inputAsset");
            autoEnableInput = serializedObject.FindProperty("autoEnableInput");
            autoDisableInput = serializedObject.FindProperty("autoDisableInput");
            actions = serializedObject.FindProperty("actions");
        }
#endif

        public override void OnInspectorGUI()
        {
#if !OBSOLETE
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(playerInput);
            if (playerInput.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(actionAsset);
            }
            if (EditorGUI.EndChangeCheck())
            {
                InputActionAsset actions = actionAsset.objectReferenceValue as InputActionAsset;

                serializedObject.ApplyModifiedProperties();
                PopulateActions();
                serializedObject.Update();

                if (actions != previousAsset)
                {
                    ReassignActions();
                    previousAsset = actions;
                }
            }

            DoCreateActions();

            EditorGUILayout.PropertyField(autoEnableInput);
            EditorGUILayout.PropertyField(autoDisableInput);

            EditorGUILayout.Space();

            actionList.DoLayoutList();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Auto Assign"))
            {
                bool allNull = true;

                for (int i = 0; i < actions.arraySize; i++)
                {
                    if (actions.GetArrayElementAtIndex(i).FindPropertyRelative("action").objectReferenceValue != null)
                    {
                        allNull = false;
                        break;
                    }
                }

                if (allNull || (!allNull && EditorUtility.DisplayDialog("Notice", "Your assigned input actions will be overwritten. Are you sure you want to do this?", "Yes", "No")))
                {
                    ReassignActions();
                }
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
#else
            if(GUILayout.Button("Replace with Gold Player Input"))
            {
                GameObject go = ((GoldPlayerInputSystem)target).gameObject;

                Undo.DestroyObjectImmediate(go.GetComponent<GoldPlayerInputSystem>());
                Undo.AddComponent<GoldPlayerInput>(go);
            }
#endif
        }

#if !OBSOLETE
        private void DoCreateActions()
        {
            if (actionAsset.objectReferenceValue == null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Create Actions", GUILayout.Width(140)))
                {
                    string fileName = EditorUtility.SaveFilePanel("Create Input Actions Asset", "Assets", Application.productName, InputActionAsset.Extension);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (!fileName.StartsWith(Application.dataPath))
                        {
                            Debug.LogError("Path must be located in Assets/ folder.");
                            EditorGUILayout.EndHorizontal();
                            return;
                        }

                        if (!fileName.EndsWith("." + InputActionAsset.Extension))
                        {
                            fileName += "." + InputActionAsset.Extension;
                        }

                        InputActionAsset actions = CreateInstance<InputActionAsset>();

                        actions.name = Path.GetFileNameWithoutExtension(fileName);

                        InputActionMap playerMap = new InputActionMap("Player");
                        actions.AddActionMap(playerMap);

                        playerMap.AddAction("Move", InputActionType.Value, expectedControlLayout: "Vector2");
                        playerMap.AddAction("Look", InputActionType.Value, expectedControlLayout: "Vector2");
                        playerMap.AddAction("Zoom", InputActionType.Button);
                        playerMap.AddAction("Jump", InputActionType.Button);
                        playerMap.AddAction("Crouch", InputActionType.Button);
                        playerMap.AddAction("Run", InputActionType.Button);
#if !GOLD_PLAYER_DISABLE_INTERACTION
                        playerMap.AddAction("Interact", InputActionType.Button);
#endif

                        playerMap.AddBinding(new InputBinding("2DVector", "Move", null, null, null, "WASD") { isComposite = true });
                        playerMap.AddBinding(new InputBinding("<Keyboard>/w", "Move", "Keyboard", null, null, "up") { isPartOfComposite = true });
                        playerMap.AddBinding(new InputBinding("<Keyboard>/s", "Move", "Keyboard", null, null, "down") { isPartOfComposite = true });
                        playerMap.AddBinding(new InputBinding("<Keyboard>/a", "Move", "Keyboard", null, null, "left") { isPartOfComposite = true });
                        playerMap.AddBinding(new InputBinding("<Keyboard>/d", "Move", "Keyboard", null, null, "right") { isPartOfComposite = true });
                        playerMap.AddBinding(new InputBinding("<Gamepad>/leftStick", "Move", "Gamepad", null, null, null));

                        playerMap.AddBinding(new InputBinding("<Mouse>/delta", "Look", "Keyboard", "ScaleVector2(x=0.1,y=0.1)", null, null));
                        playerMap.AddBinding(new InputBinding("<Gamepad>/rightStick", "Look", "Keyboard", "ScaleVector2(x=2,y=2)", null, null));
                        playerMap.AddBinding(new InputBinding("<Keyboard>/z", "Zoom", "Keyboard"));
                        playerMap.AddBinding(new InputBinding("<Gamepad>/leftShoulder", "Zoom", "Gamepad"));

                        playerMap.AddBinding(new InputBinding("<Keyboard>/space", "Jump", "Keyboard", null, null, null));
                        playerMap.AddBinding(new InputBinding("<Gamepad>/buttonSouth", "Jump", "Gamepad", null, null, null));

                        playerMap.AddBinding(new InputBinding("<Keyboard>/c", "Crouch", "Keyboard", null, null, null));
                        playerMap.AddBinding(new InputBinding("<Gamepad>/leftStickPress", "Crouch", "Gamepad", null, null, null));

                        playerMap.AddBinding(new InputBinding("<Keyboard>/leftShift", "Run", "Keyboard", null, null, null));
                        playerMap.AddBinding(new InputBinding("<Gamepad>/rightTrigger", "Run", "Gamepad", null, null, null));

#if !GOLD_PLAYER_DISABLE_INTERACTION
                        playerMap.AddBinding(new InputBinding("<Keyboard>/e", "Interact", "Keyboard", null, null, null));
                        playerMap.AddBinding(new InputBinding("<Gamepad>/buttonWest", "Interact", "Gamepad", null, null, null));
#endif

#if GOLD_PLAYER_UGUI
                        InputActionMap uiMap = new InputActionMap("UI");
                        actions.AddActionMap(uiMap);

                        uiMap.AddAction("Navigate", InputActionType.Value, expectedControlLayout: "Vector2");
                        uiMap.AddAction("Submit", InputActionType.Button);
                        uiMap.AddAction("Cancel", InputActionType.Button);
                        uiMap.AddAction("Point", InputActionType.Button);
                        uiMap.AddAction("Click", InputActionType.Button);
                        uiMap.AddAction("ScrollWheel", InputActionType.Value, expectedControlLayout: "Vector2");
                        uiMap.AddAction("MiddleClick", InputActionType.Button);
                        uiMap.AddAction("RightClick", InputActionType.Button);

                        uiMap.AddBinding(new InputBinding("2DVector", "Navigate", null, null, null, "Gamepad") { isComposite = true });
                        uiMap.AddBinding(new InputBinding("<Gamepad>/leftStick/up", "Navigate", ";Gamepad", null, null, "up") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Gamepad>/rightStick/up", "Navigate", ";Gamepad", null, null, "up") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Gamepad>/leftStick/down", "Navigate", ";Gamepad", null, null, "down") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Gamepad>/rightStick/down", "Navigate", ";Gamepad", null, null, "down") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Gamepad>/leftStick/left", "Navigate", ";Gamepad", null, null, "left") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Gamepad>/rightStick/left", "Navigate", ";Gamepad", null, null, "left") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Gamepad>/leftStick/right", "Navigate", ";Gamepad", null, null, "right") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Gamepad>/rightStick/right", "Navigate", ";Gamepad", null, null, "right") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Gamepad>/dpad", "Navigate", ";Gamepad", null, null, null));
                        uiMap.AddBinding(new InputBinding("2DVector", "Navigate", null, null, null, "Joystick") { isComposite = true });
                        uiMap.AddBinding(new InputBinding("<Joystick>/stick/up", "Navigate", "Joystick", null, null, "up") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Joystick>/stick/down", "Navigate", "Joystick", null, null, "down") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Joystick>/stick/left", "Navigate", "Joystick", null, null, "left") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Joystick>/stick/right", "Navigate", "Joystick", null, null, "right") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("2DVector", "Navigate", null, null, null, "Keyboard") { isComposite = true });
                        uiMap.AddBinding(new InputBinding("<Keyboard>/w", "Navigate", "Keyboard", null, null, "up") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Keyboard>/s", "Navigate", "Keyboard", null, null, "down") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Keyboard>/a", "Navigate", "Keyboard", null, null, "left") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Keyboard>/d", "Navigate", "Keyboard", null, null, "right") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Keyboard>/upArrow", "Navigate", "Keyboard", null, null, "up") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Keyboard>/downArrow", "Navigate", "Keyboard", null, null, "down") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Keyboard>/leftArrow", "Navigate", "Keyboard", null, null, "left") { isPartOfComposite = true });
                        uiMap.AddBinding(new InputBinding("<Keyboard>/rightArrow", "Navigate", "Keyboard", null, null, "right") { isPartOfComposite = true });

                        uiMap.AddBinding(new InputBinding("*/{Submit}", "Submit"));

                        uiMap.AddBinding(new InputBinding("*/{Cancel}", "Cancel"));

                        uiMap.AddBinding(new InputBinding("<Mouse>/position", "Point", "Keyboard"));
                        uiMap.AddBinding(new InputBinding("<Pen>/position", "Point", "Keyboard"));
                        uiMap.AddBinding(new InputBinding("<Touchscreen>/touch*/position", "Point", "Touch"));

                        uiMap.AddBinding(new InputBinding("<Mouse>/leftButton", "Click", ";Keyboard"));
                        uiMap.AddBinding(new InputBinding("<Pen>/tip", "Click", ";Keyboard"));
                        uiMap.AddBinding(new InputBinding("<Touchscreen>/touch*/press", "Click", "Touch"));
                        uiMap.AddBinding(new InputBinding("<XRController>/trigger", "Click", "XR"));

                        uiMap.AddBinding(new InputBinding("<Mouse>/scroll", "ScrollWheel", ";Keyboard"));

                        uiMap.AddBinding(new InputBinding("<Mouse>/middleButton", "MiddleClick", ";Keyboard"));

                        uiMap.AddBinding(new InputBinding("<Mouse>/rightButton", "RightClick", ";Keyboard"));
#endif

                        InputControlScheme.DeviceRequirement[] keyboardDevice = new InputControlScheme.DeviceRequirement[]
                        {
                            new InputControlScheme.DeviceRequirement()
                            {
                                controlPath = "<Keyboard>",
                                isOptional = false,
                                isOR = false
                            },
                            new InputControlScheme.DeviceRequirement()
                            {
                                controlPath = "<Mouse>",
                                isOptional = true,
                                isOR = false
                            }
                        };
                        InputControlScheme.DeviceRequirement[] gamepadDevice = new InputControlScheme.DeviceRequirement[]
                        {
                            new InputControlScheme.DeviceRequirement()
                            {
                                controlPath = "<Gamepad>",
                                isOptional = false,
                                isOR = false
                            }
                        };
                        InputControlScheme.DeviceRequirement[] touchDevice = new InputControlScheme.DeviceRequirement[]
                        {
                            new InputControlScheme.DeviceRequirement()
                            {
                                controlPath = "<Touchscreen>",
                                isOptional = false,
                                isOR = false
                            }
                        };
                        InputControlScheme.DeviceRequirement[] joystickDevice = new InputControlScheme.DeviceRequirement[]
                        {
                            new InputControlScheme.DeviceRequirement()
                            {
                                controlPath = "<Joystick>",
                                isOptional = false,
                                isOR = false
                            }
                        };

                        actions.AddControlScheme(new InputControlScheme("Keyboard", keyboardDevice, "Keyboard"));
                        actions.AddControlScheme(new InputControlScheme("Gamepad", gamepadDevice, "Gamepad"));
                        actions.AddControlScheme(new InputControlScheme("Touch", touchDevice, "Touch"));
                        actions.AddControlScheme(new InputControlScheme("Joystick", joystickDevice, "Joystick"));

                        File.WriteAllText(fileName, actions.ToJson());
                        AssetDatabase.Refresh();

                        EditorApplication.delayCall += () =>
                        {
                            // Need to get the properties again or else we will get a "property is disposed" error.
                            GetProperties();

                            string relativePath = "Assets/" + fileName.Substring(Application.dataPath.Length + 1);
                            InputActionAsset loadedActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(relativePath);
                            actionAsset.objectReferenceValue = loadedActions;
                            serializedObject.ApplyModifiedProperties();
                            serializedObject.Update();
                            PopulateActions();
                            serializedObject.Update();
                            ReassignActions();
                        };
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
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
                    if (availableActions == null)
                    {
                        PopulateActions();
                    }

                    SerializedProperty element = actions.GetArrayElementAtIndex(index);

                    Rect nameRect = new Rect(rect.x, rect.y + 2, rect.width / 2 - 4, EditorGUIUtility.singleLineHeight);
                    Rect actionRect = new Rect(rect.x + (rect.width / 2 + 4), rect.y + 2, rect.width / 2 - 4, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(nameRect, actions.GetArrayElementAtIndex(index).FindPropertyRelative("actionName"), GUIContent.none);

                    if (actionAsset.objectReferenceValue != null)
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
#endif
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
