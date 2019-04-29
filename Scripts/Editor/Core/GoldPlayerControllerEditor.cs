#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#if UNITY_2019_2_OR_NEWER
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace Hertzole.GoldPlayer.Editor
{
    [CustomEditor(typeof(GoldPlayerController))]
    internal class GoldPlayerControllerEditor : UnityEditor.Editor
    {
        private int currentTab = 0;

        private readonly string[] tabs = new string[] { "Camera", "Movement", "Head Bob", "Audio" };
        private const string SELECTED_TAB_PREFS = "HERTZ_GOLD_PLAYER_SELECTED_TAB";

        private GoldPlayerController goldPlayer;
        private CharacterController characterController;

        private SerializedProperty camera;
        private SerializedProperty movement;
        private SerializedProperty headBob;
        private SerializedProperty audio;

#if UNITY_2019_2_OR_NEWER
        private VisualElement root;

        private VisualElement cameraElements;
        private VisualElement movementElements;
        private VisualElement headBobElements;
        private VisualElement audioElements;

        private VisualElement crouchHeightWarning;
        private VisualElement groundLayerWarning;

        private int crouchHeightIndex;
        private int groundLayerIndex;
#endif

        private void OnEnable()
        {
            currentTab = EditorPrefs.GetInt(SELECTED_TAB_PREFS, 0);

            if (currentTab < 0)
                currentTab = 0;
            if (currentTab > 3)
                currentTab = 3;

            camera = serializedObject.FindProperty("camera");
            movement = serializedObject.FindProperty("movement");
            headBob = serializedObject.FindProperty("headBob");
            audio = serializedObject.FindProperty("audio");

            goldPlayer = (GoldPlayerController)target;
            characterController = goldPlayer.GetComponent<CharacterController>();
        }

#if !UNITY_2019_2_OR_NEWER
        public override void OnInspectorGUI()
        {
            if (characterController.center.y != characterController.height / 2)
                EditorGUILayout.HelpBox("The Character Controller Y center must be half of the height. Set your Y center to " + characterController.height / 2 + "!", MessageType.Warning);

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
            serializedObject.ApplyModifiedProperties();
        }

        private void DoCameraGUI()
        {
            SerializedProperty it = camera.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(camera.name) && it.depth < 2)
                    EditorGUILayout.PropertyField(it, true);
            }
        }

        private void DoMovementGUI()
        {
            SerializedProperty it = movement.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(movement.name) && it.depth < 2)
                {
                    EditorGUILayout.PropertyField(it, true);
                    if (it.name.Equals("crouchHeight") && it.floatValue < 0.8f)
                        EditorGUILayout.HelpBox("The Crouch Height should not be less than 0.8 because it breaks the character controller!", MessageType.Warning);
                    if (it.name.Equals("groundLayer") && it.intValue == (it.intValue | (1 << goldPlayer.gameObject.layer)))
                        EditorGUILayout.HelpBox("The player layer should not be included as a Ground Layer!", MessageType.Warning);
                }
            }
        }

        private void DoHeadBobGUI()
        {
            SerializedProperty it = headBob.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(headBob.name) && !it.propertyPath.StartsWith(headBob.name + ".bobClass") && it.depth < 2)
                    EditorGUILayout.PropertyField(it, true);

                if (it.propertyPath.StartsWith(headBob.name + ".bobClass") && it.depth >= 2)
                    EditorGUILayout.PropertyField(it, true);
            }
        }

        private void DoAudioGUI()
        {
            SerializedProperty it = audio.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(audio.name) && it.depth < 2)
                    EditorGUILayout.PropertyField(it, true);
            }
        }
#else 
        public override VisualElement CreateInspectorGUI()
        {
            //TODO: Somehow show warning about incorrect character controller settings.

            root = new VisualElement();

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

            CreateCameraGUI();
            CreateMovementGUI();
            CreateHeadBobGUI();
            CreateAudioGUI();

            root.Add(cameraElements);
            root.Add(movementElements);
            root.Add(headBobElements);
            root.Add(audioElements);

            RebuildTab(currentTab);

            return root;
        }

        private void RebuildTab(int tab)
        {
            cameraElements.style.display = (tab == 0 ? DisplayStyle.Flex : DisplayStyle.None);
            movementElements.style.display = (tab == 1 ? DisplayStyle.Flex : DisplayStyle.None);
            headBobElements.style.display = (tab == 2 ? DisplayStyle.Flex : DisplayStyle.None);
            audioElements.style.display = (tab == 3 ? DisplayStyle.Flex : DisplayStyle.None);
        }

        private void CreateCameraGUI()
        {
            cameraElements = new VisualElement();
            SerializedProperty it = camera.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(camera.name) && it.depth < 2)
                {
                    if (it.name.Equals("invertXAxis"))
                        cameraElements.Add(GoldPlayerUIHelper.GetSpace());

                    if (it.name.Equals("mouseSensitivity"))
                        cameraElements.Add(GoldPlayerUIHelper.GetSpace());

                    if (it.name.Equals("fieldOfViewKick"))
                        cameraElements.Add(GoldPlayerUIHelper.GetSpace());

                    if (it.name.Equals("cameraHead"))
                        cameraElements.Add(GoldPlayerUIHelper.GetSpace());

                    cameraElements.Add(new PropertyField(it));
                }
            }
        }

        private void CreateMovementGUI()
        {
            movementElements = new VisualElement();

            movement = serializedObject.FindProperty("movement");
            SerializedProperty it = movement.Copy();
            int index = 0;
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(movement.name) && it.depth < 2)
                {
                    if (it.name.Equals("walkingSpeeds"))
                    {
                        movementElements.Add(GoldPlayerUIHelper.GetHeaderLabel("Walking"));
                        index++;
                    }

                    if (it.name.Equals("canRun"))
                    {
                        movementElements.Add(GoldPlayerUIHelper.GetHeaderLabel("Running"));
                        index++;
                    }

                    if (it.name.Equals("canJump"))
                    {
                        movementElements.Add(GoldPlayerUIHelper.GetHeaderLabel("Jumping"));
                        index++;
                    }

                    if (it.name.Equals("canCrouch"))
                    {
                        movementElements.Add(GoldPlayerUIHelper.GetHeaderLabel("Crouching"));
                        index++;
                    }

                    if (it.name.Equals("groundLayer"))
                    {
                        movementElements.Add(GoldPlayerUIHelper.GetHeaderLabel("Other"));
                        index++;
                    }

                    PropertyField field = new PropertyField(it);
                    movementElements.Add(field);
                    if (it.name.Equals("crouchHeight"))
                    {
                        crouchHeightIndex = index;
                        field.RegisterCallback<ChangeEvent<float>>((evt) => { ValidateCrouchHeight(movementElements, evt.newValue); });

                        ValidateCrouchHeight(movementElements, it.floatValue);
                    }

                    if (it.name.Equals("groundLayer"))
                    {
                        groundLayerIndex = index;
                        field.RegisterCallback<ChangeEvent<int>>((evt) => { ValidateGroundLayer(movementElements, evt.newValue); });

                        ValidateGroundLayer(movementElements, it.intValue);
                    }

                    index++;
                }
            }
        }

        private void ValidateCrouchHeight(VisualElement parent, float height)
        {
            if (height < 0.8f)
            {
                if (!parent.Contains(crouchHeightWarning))
                {
                    //HACK: Maybe find a neat replacement for help boxes.
                    crouchHeightWarning = new IMGUIContainer(() =>
                    {
                        EditorGUILayout.HelpBox("The Crouch Height should not be less than 0.8 because it breaks the character controller!", MessageType.Warning);
                    });
                    parent.Insert(crouchHeightIndex + 1, crouchHeightWarning);
                }
            }
            else
            {
                if (parent.Contains(crouchHeightWarning))
                    parent.Remove(crouchHeightWarning);
            }
        }

        private void ValidateGroundLayer(VisualElement parent, int layer)
        {
            if (layer == (layer | (1 << goldPlayer.gameObject.layer)))
            {
                if (!parent.Contains(groundLayerWarning))
                {
                    //HACK: Maybe find a neat replacement for help boxes.
                    groundLayerWarning = new IMGUIContainer(() =>
                    {
                        EditorGUILayout.HelpBox("The player layer should not be included as a Ground Layer!", MessageType.Warning);
                    });

                    parent.Insert(groundLayerIndex + (parent.Contains(crouchHeightWarning) ? 2 : 1), groundLayerWarning);
                }
            }
            else
            {
                if (parent.Contains(groundLayerWarning))
                    parent.Remove(groundLayerWarning);
            }
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
                        headBobElements.Add(GoldPlayerUIHelper.GetSpace());

                    if (it.name.Equals("landMove"))
                        headBobElements.Add(GoldPlayerUIHelper.GetSpace());

                    if (it.name.Equals("bobTarget"))
                        headBobElements.Add(GoldPlayerUIHelper.GetSpace());

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
                        audioElements.Add(GoldPlayerUIHelper.GetSpace());

                    if (it.name.Equals("walkFootsteps"))
                        audioElements.Add(GoldPlayerUIHelper.GetSpace());

                    if (it.name.Equals("footstepsSource"))
                        audioElements.Add(GoldPlayerUIHelper.GetSpace());

                    audioElements.Add(new PropertyField(it));
                }
            }
        }
#endif
    }
}
#endif
