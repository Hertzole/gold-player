#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace Hertzole.GoldPlayer.Editor
{
    [CustomEditor(typeof(GoldPlayerController))]
    internal class GoldPlayerControllerEditor : UnityEditor.Editor
    {
        private int currentTab = 0;

        private readonly string[] tabs = new string[] { "Camera", "Movement", "Head Bob", "Audio", "Input" };
        private const string SELECTED_TAB_PREFS = "HERTZ_GOLD_PLAYER_SELECTED_TAB";

        private GoldPlayerController goldPlayer;
        private CharacterController characterController;

        private SerializedProperty camera;
        private SerializedProperty movement;
        private SerializedProperty headBob;
        private SerializedProperty audio;

#if UNITY_2019_1_OR_NEWER
        private VisualElement root;

        private VisualElement cameraElements;
        private VisualElement movementElements;
        private VisualElement headBobElements;
        private VisualElement audioElements;
        private VisualElement inputElements;

        private VisualElement controllerWarning;
        private VisualElement crouchHeightWarning;
        private VisualElement groundLayerWarning;
#endif

        private void OnEnable()
        {
            currentTab = EditorPrefs.GetInt(SELECTED_TAB_PREFS, 0);

            if (currentTab < 0)
            {
                currentTab = 0;
            }

            if (currentTab > 4)
            {
                currentTab = 4;
            }

            camera = serializedObject.FindProperty("camera");
            movement = serializedObject.FindProperty("movement");
            headBob = serializedObject.FindProperty("headBob");
            audio = serializedObject.FindProperty("audio");

            goldPlayer = (GoldPlayerController)target;
            characterController = goldPlayer.GetComponent<CharacterController>();
        }

#if !UNITY_2019_1_OR_NEWER
        public override void OnInspectorGUI()
        {
            if (characterController.center.y != characterController.height / 2)
            {
                EditorGUILayout.HelpBox("The Character Controller Y center must be half of the height. Set your Y center to " + characterController.height / 2 + "!", MessageType.Warning);
            }

            serializedObject.Update();
            int newTab = GUILayout.Toolbar(currentTab, tabs);
            if (newTab != currentTab)
            {
                currentTab = newTab;
                EditorPrefs.SetInt(SELECTED_TAB_PREFS, currentTab);
            }

            if (currentTab == 0) // Camera
            {
                DoCameraGUI();
            }
            else if (currentTab == 1) // Movement
            {
                DoMovementGUI();
            }
            else if (currentTab == 2) // Head bob
            {
                DoHeadBobGUI();
            }
            else if (currentTab == 3) // Audio
            {
                DoAudioGUI();
            }
            else if (currentTab == 4)
            {
                DoInputGUI();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DoCameraGUI()
        {
            SerializedProperty it = camera.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(camera.name) && it.depth < 2)
                {
                    if (it.name.StartsWith("input_"))
                    {
                        continue;
                    }

                    EditorGUILayout.PropertyField(it, true);
                }
            }
        }

        private void DoMovementGUI()
        {
            SerializedProperty it = movement.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(movement.name) && it.depth < 2)
                {
                    if (it.name.StartsWith("input_"))
                    {
                        continue;
                    }

                    EditorGUILayout.PropertyField(it, true);
                    if (it.name.Equals("crouchHeight") && it.floatValue < 0.8f)
                    {
                        EditorGUILayout.HelpBox("The Crouch Height should not be less than 0.8 because it breaks the character controller!", MessageType.Warning);
                    }

                    if (it.name.Equals("groundLayer") && it.intValue == (it.intValue | (1 << goldPlayer.gameObject.layer)))
                    {
                        EditorGUILayout.HelpBox("The player layer should not be included as a Ground Layer!", MessageType.Warning);
                    }
                }
            }
        }

        private void DoHeadBobGUI()
        {
            SerializedProperty it = headBob.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(headBob.name) && !it.propertyPath.StartsWith(headBob.name + ".bobClass") && it.depth < 2)
                {
                    EditorGUILayout.PropertyField(it, true);
                }

                if (it.propertyPath.StartsWith(headBob.name + ".bobClass") && it.depth >= 2)
                {
                    EditorGUILayout.PropertyField(it, true);
                }
            }
        }

        private void DoAudioGUI()
        {
            SerializedProperty it = audio.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(audio.name) && it.depth < 2)
                {
                    EditorGUILayout.PropertyField(it, true);
                }
            }
        }

        private void DoInputGUI()
        {
            EditorGUILayout.LabelField("Camera", EditorStyles.boldLabel);

            SerializedProperty cam = camera.Copy();
            while (cam.NextVisible(true))
            {
                if (cam.propertyPath.StartsWith(camera.name) && cam.depth < 2)
                {
                    if (cam.name.StartsWith("input_"))
                    {
                        EditorGUILayout.PropertyField(cam, new GUIContent(cam.displayName.Substring(6), cam.tooltip), true);
                    }
                }
            }

            EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);

            SerializedProperty move = movement.Copy();
            while (move.NextVisible(true))
            {
                if (move.propertyPath.StartsWith(movement.name) && move.depth < 2)
                {
                    if (move.name.StartsWith("input_"))
                    {
                        EditorGUILayout.PropertyField(move, new GUIContent(move.displayName.Substring(6), move.tooltip), true);
                    }
                }
            }

        }
#else
        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            controllerWarning = new IMGUIContainer(() =>
            {
                EditorGUILayout.HelpBox("The Character Controller Y center must be half of the height. Set your Y center to " + characterController.height / 2 + "!", MessageType.Warning);
            });

            if (characterController != null)
            {
                controllerWarning.style.display = characterController.center.y != characterController.height / 2 ? DisplayStyle.Flex : DisplayStyle.None;
            }

            root.Add(controllerWarning);

            IVisualElementScheduledItem controllerCheck = root.schedule.Execute(() =>
            {
                if (characterController != null)
                {
                    controllerWarning.style.display = characterController.center.y != characterController.height / 2 ? DisplayStyle.Flex : DisplayStyle.None;
                }
            });
            controllerCheck.Every(100);

            IMGUIContainer toolbarContainer = new IMGUIContainer(() =>
            {
                int newTab = GUILayout.Toolbar(currentTab, tabs);
                if (newTab != currentTab)
                {
                    currentTab = newTab;
                    EditorPrefs.SetInt(SELECTED_TAB_PREFS, currentTab);
                    RebuildTab(currentTab);
                }
            });

            root.Add(toolbarContainer);
            root.Add(GoldPlayerUIHelper.GetSpace(3));

            inputElements = new VisualElement();

            CreateCameraGUI();
            CreateMovementGUI();
            CreateHeadBobGUI();
            CreateAudioGUI();

            root.Add(cameraElements);
            root.Add(movementElements);
            root.Add(headBobElements);
            root.Add(audioElements);
            root.Add(inputElements);

            RebuildTab(currentTab);

            return root;
        }

        private void RebuildTab(int tab)
        {
            cameraElements.style.display = tab == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            movementElements.style.display = tab == 1 ? DisplayStyle.Flex : DisplayStyle.None;
            headBobElements.style.display = tab == 2 ? DisplayStyle.Flex : DisplayStyle.None;
            audioElements.style.display = tab == 3 ? DisplayStyle.Flex : DisplayStyle.None;
            inputElements.style.display = tab == 4 ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void CreateCameraGUI()
        {
            inputElements.Add(GoldPlayerUIHelper.GetHeaderLabel("Camera"));

            cameraElements = new VisualElement();
            SerializedProperty it = camera.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(camera.name) && it.depth < 2)
                {
                    if (it.name.Equals("invertXAxis"))
                    {
                        cameraElements.Add(GoldPlayerUIHelper.GetSpace());
                    }

                    if (it.name.Equals("mouseSensitivity"))
                    {
                        cameraElements.Add(GoldPlayerUIHelper.GetSpace());
                    }

                    if (it.name.Equals("fieldOfViewKick"))
                    {
                        cameraElements.Add(GoldPlayerUIHelper.GetSpace());
                    }

                    if (it.name.Equals("cameraHead"))
                    {
                        cameraElements.Add(GoldPlayerUIHelper.GetSpace());
                    }

                    if (it.name.StartsWith("input_"))
                    {
                        inputElements.Add(new PropertyField(it, it.displayName.Substring(6)));
                        continue;
                    }

                    cameraElements.Add(new PropertyField(it));
                }
            }
        }

        private void CreateMovementGUI()
        {
            inputElements.Add(GoldPlayerUIHelper.GetHeaderLabel("Movement"));

            movementElements = new VisualElement();

            movement = serializedObject.FindProperty("movement");
            SerializedProperty it = movement.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(movement.name) && it.depth < 2)
                {
                    if (it.name.Equals("walkingSpeeds"))
                    {
                        movementElements.Add(GoldPlayerUIHelper.GetHeaderLabel("Walking"));
                    }

                    if (it.name.Equals("canRun"))
                    {
                        movementElements.Add(GoldPlayerUIHelper.GetHeaderLabel("Running"));
                    }

                    if (it.name.Equals("canJump"))
                    {
                        movementElements.Add(GoldPlayerUIHelper.GetHeaderLabel("Jumping"));
                    }

                    if (it.name.Equals("canCrouch"))
                    {
                        movementElements.Add(GoldPlayerUIHelper.GetHeaderLabel("Crouching"));
                    }

                    if (it.name.Equals("groundLayer"))
                    {
                        movementElements.Add(GoldPlayerUIHelper.GetHeaderLabel("Other"));
                    }

                    if (it.name.StartsWith("input_"))
                    {
                        inputElements.Add(new PropertyField(it, it.displayName.Substring(6)));
                        continue;
                    }

                    PropertyField field = new PropertyField(it);
                    movementElements.Add(field);
                    if (it.name.Equals("crouchHeight"))
                    {
                        //HACK: Maybe find a neat replacement for help boxes.
                        crouchHeightWarning = new IMGUIContainer(() =>
                        {
                            EditorGUILayout.HelpBox("The Crouch Height should not be less than 0.8 because it breaks the character controller!", MessageType.Warning);
                        });

                        movementElements.Add(crouchHeightWarning);

                        field.RegisterCallback<ChangeEvent<float>>((evt) => { ValidateCrouchHeight(evt.newValue); });

                        ValidateCrouchHeight(it.floatValue);
                    }

                    if (it.name.Equals("groundLayer"))
                    {
                        //HACK: Maybe find a neat replacement for help boxes.
                        groundLayerWarning = new IMGUIContainer(() =>
                        {
                            EditorGUILayout.HelpBox("The player layer should not be included as a Ground Layer!", MessageType.Warning);
                        });

                        field.RegisterCallback<ChangeEvent<int>>((evt) => { ValidateGroundLayer(evt.newValue); });

                        ValidateGroundLayer(it.intValue);
                    }
                }
            }
        }

        private void ValidateCrouchHeight(float height)
        {
            crouchHeightWarning.style.display = height < 0.8f ? DisplayStyle.Flex : (StyleEnum<DisplayStyle>)DisplayStyle.None;
        }

        private void ValidateGroundLayer(int layer)
        {
            groundLayerWarning.style.display = layer == (layer | (1 << goldPlayer.gameObject.layer)) ?
                (StyleEnum<DisplayStyle>)DisplayStyle.Flex : DisplayStyle.None;
        }

        private void CreateHeadBobGUI()
        {
            headBobElements = new VisualElement();

            SerializedProperty it = headBob.Copy();
            while (it.NextVisible(true))
            {
                if ((it.propertyPath.StartsWith(headBob.name) && !it.propertyPath.StartsWith(headBob.name + ".bobClass") && it.depth < 2) ||
                    it.propertyPath.StartsWith(headBob.name + ".bobClass") && it.depth >= 2)
                {
                    if (it.name.Equals("bobFrequency"))
                    {
                        headBobElements.Add(GoldPlayerUIHelper.GetSpace());
                    }

                    if (it.name.Equals("landMove"))
                    {
                        headBobElements.Add(GoldPlayerUIHelper.GetSpace());
                    }

                    if (it.name.Equals("enableStrafeTilting"))
                    {
                        headBobElements.Add(GoldPlayerUIHelper.GetSpace());
                    }

                    if (it.name.Equals("bobTarget"))
                    {
                        headBobElements.Add(GoldPlayerUIHelper.GetSpace());
                    }

                    headBobElements.Add(new PropertyField(it));
                }
            }

            it.Reset();
        }

        private void CreateAudioGUI()
        {
            audioElements = new VisualElement();

            SerializedProperty it = audio.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(audio.name) && it.depth < 2)
                {
                    if (it.name.Equals("basedOnHeadBob"))
                    {
                        audioElements.Add(GoldPlayerUIHelper.GetSpace());
                    }

                    if (it.name.Equals("walkFootsteps"))
                    {
                        audioElements.Add(GoldPlayerUIHelper.GetSpace());
                    }

                    if (it.name.Equals("footstepsSource"))
                    {
                        audioElements.Add(GoldPlayerUIHelper.GetSpace());
                    }

                    audioElements.Add(new PropertyField(it));
                }
            }
        }
#endif
    }
}
#endif
