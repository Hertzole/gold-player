#if UNITY_EDITOR
#if UNITY_2019_1_OR_NEWER
// Disabled because IMGUI simply offers a better experience as of right now.
//#define USE_UI_ELEMENTS
#endif
#if USE_UI_ELEMENTS
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Hertzole.GoldPlayer.Editor.GoldPlayerUIHelper;

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

        // Camera properties
        private SerializedProperty canLookAround;
        private SerializedProperty shouldLockCursor;
        private SerializedProperty rotateCameraOnly;
        private SerializedProperty invertXAxis;
        private SerializedProperty invertYAxis;
        private SerializedProperty lookSensitivity;
        private SerializedProperty lookDamping;
        private SerializedProperty minimumX;
        private SerializedProperty maximumX;
        private SerializedProperty cameraHead;
#if GOLD_PLAYER_CINEMACHINE
        private SerializedProperty useCinemachine;
        private SerializedProperty targetVirtualCamera;
#endif
        private SerializedProperty targetCamera;
        
        private SerializedProperty enableZooming;
        private SerializedProperty targetZoom;
        private SerializedProperty zoomInTime;
        private SerializedProperty zoomInCurve;
        private SerializedProperty zoomOutTime;
        private SerializedProperty zoomOutCurve;
        private SerializedProperty zoomInput;

        private SerializedProperty fieldOfViewKick;
        private SerializedProperty fovEnable;
        private SerializedProperty fovUnscaledTime;
        private SerializedProperty fovKickWhen;
        private SerializedProperty fovKickAmount;
        private SerializedProperty fovTimeTo;
        private SerializedProperty fovToCurve;
        private SerializedProperty fovTimeFrom;
        private SerializedProperty fovFromCurve;

        // Movement properties
        private SerializedProperty canMoveAround;
        private SerializedProperty moveUnscaledTime;

        private SerializedProperty walkingSpeed;

        private SerializedProperty canRun;
        private SerializedProperty runToggleMode;
        private SerializedProperty runSpeeds;
        private SerializedProperty stamina;
        private SerializedProperty enableStamina;
        private SerializedProperty staminaUnscaledTime;
        private SerializedProperty drainStaminaWhen;
        private SerializedProperty maxStamina;
        private SerializedProperty drainRate;
        private SerializedProperty stillThreshold;
        private SerializedProperty regenRateStill;
        private SerializedProperty regenRateMoving;
        private SerializedProperty regenWait;

        private SerializedProperty canCrouch;
        private SerializedProperty crouchToggleMode;
        private SerializedProperty crouchSpeeds;
        private SerializedProperty crouchJumping;
        private SerializedProperty crouchHeight;
        private SerializedProperty crouchTime;
        private SerializedProperty crouchCurve;
        private SerializedProperty standUpTime;
        private SerializedProperty standUpCurve;

        private SerializedProperty canJump;
        private SerializedProperty jumpingRequiresStamina;
        private SerializedProperty jumpStaminaRequire;
        private SerializedProperty jumpStaminaCost;
        private SerializedProperty jumpHeight;
        private SerializedProperty airJump;
        private SerializedProperty airJumpTime;
        private SerializedProperty airJumpsAmount;
        private SerializedProperty allowAirJumpDirectionChange;

        private SerializedProperty movingPlatforms;
        private SerializedProperty movingPlatformsEnabled;
        private SerializedProperty movingPlatformsPosition;
        private SerializedProperty movingPlatformsRotation;
        private SerializedProperty movingPlatformsMaxAngle;

        private SerializedProperty groundLayer;
        private SerializedProperty acceleration;
        private SerializedProperty gravity;
        private SerializedProperty airControl;
        private SerializedProperty enableGroundStick;
        private SerializedProperty groundStick;
        private SerializedProperty groundCheck;
        private SerializedProperty rayAmount;
        private SerializedProperty rayHeight;
        private SerializedProperty rayLength;

        // Head bob properties
        private SerializedProperty enableBob;
        private SerializedProperty bobUnscaledTime;
        private SerializedProperty bobFrequency;
        private SerializedProperty bobHeight;
        private SerializedProperty swayAngle;
        private SerializedProperty sideMovement;
        private SerializedProperty heightMultiplier;
        private SerializedProperty strideMultiplier;
        private SerializedProperty landMove;
        private SerializedProperty landTilt;
        private SerializedProperty enableStrafeTilting;
        private SerializedProperty strafeTilt;
        private SerializedProperty bobTarget;

        // Audio properties
        private SerializedProperty enableAudio;
        private SerializedProperty audioUnscaledTime;
        private SerializedProperty audioType;
        private SerializedProperty basedOnHeadBob;
        private SerializedProperty stepTime;
        private SerializedProperty walkFootsteps;
        private SerializedProperty runFootsteps;
        private SerializedProperty crouchFootsteps;
        private SerializedProperty jumpingSteps;
        private SerializedProperty landingSteps;
        private SerializedProperty footstepsSource;
        private SerializedProperty jumpSource;
        private SerializedProperty landSource;

#if USE_UI_ELEMENTS
        private VisualElement root;

        private VisualElement cameraElements;
        private VisualElement movementElements;
        private VisualElement headBobElements;
        private VisualElement audioElements;
        private VisualElement inputElements;

        private VisualElement controllerWarning;
        private VisualElement controllerWarningFix;
        private VisualElement crouchHeightWarning;
        private VisualElement groundLayerWarning;
#endif

        private static GUIContent[] inputContents;

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

            camera = serializedObject.FindProperty("cam");
            movement = serializedObject.FindProperty("movement");
            headBob = serializedObject.FindProperty("headBob");
            audio = serializedObject.FindProperty("sounds");

            canLookAround = camera.FindPropertyRelative("canLookAround");
            shouldLockCursor = camera.FindPropertyRelative("shouldLockCursor");
            rotateCameraOnly = camera.FindPropertyRelative("rotateCameraOnly");
            invertXAxis = camera.FindPropertyRelative("invertXAxis");
            invertYAxis = camera.FindPropertyRelative("invertYAxis");
            lookSensitivity = camera.FindPropertyRelative("lookSensitivity");
            lookDamping = camera.FindPropertyRelative("lookDamping");
            minimumX = camera.FindPropertyRelative("minimumX");
            maximumX = camera.FindPropertyRelative("maximumX");
            cameraHead = camera.FindPropertyRelative("cameraHead");
#if GOLD_PLAYER_CINEMACHINE
            useCinemachine = camera.FindPropertyRelative("useCinemachine");
            targetVirtualCamera = camera.FindPropertyRelative("targetVirtualCamera");
#endif
            targetCamera = camera.FindPropertyRelative("targetCamera");
            
            enableZooming = camera.FindPropertyRelative("enableZooming");
            targetZoom = camera.FindPropertyRelative("targetZoom");
            zoomInTime = camera.FindPropertyRelative("zoomInTime");
            zoomOutCurve = camera.FindPropertyRelative("zoomOutCurve");
            zoomOutTime = camera.FindPropertyRelative("zoomOutTime");
            zoomInCurve = camera.FindPropertyRelative("zoomInCurve");
            zoomInput = camera.FindPropertyRelative("zoomInput");
            
            fieldOfViewKick = camera.FindPropertyRelative("fieldOfViewKick");
            fovEnable = fieldOfViewKick.FindPropertyRelative("enableFOVKick");
            fovUnscaledTime = fieldOfViewKick.FindPropertyRelative("unscaledTime");
            fovKickWhen = fieldOfViewKick.FindPropertyRelative("kickWhen");
            fovKickAmount = fieldOfViewKick.FindPropertyRelative("kickAmount");
            fovTimeTo = fieldOfViewKick.FindPropertyRelative("lerpTimeTo");
            fovToCurve = fieldOfViewKick.FindPropertyRelative("lerpToCurve");
            fovTimeFrom = fieldOfViewKick.FindPropertyRelative("lerpTimeFrom");
            fovFromCurve = fieldOfViewKick.FindPropertyRelative("lerpFromCurve");

            canMoveAround = movement.FindPropertyRelative("canMoveAround");
            moveUnscaledTime = movement.FindPropertyRelative("unscaledTime");

            walkingSpeed = movement.FindPropertyRelative("walkingSpeeds");

            canRun = movement.FindPropertyRelative("canRun");
            runToggleMode = movement.FindPropertyRelative("runToggleMode");
            runSpeeds = movement.FindPropertyRelative("runSpeeds");
            stamina = movement.FindPropertyRelative("stamina");
            enableStamina = stamina.FindPropertyRelative("enableStamina");
            staminaUnscaledTime = stamina.FindPropertyRelative("unscaledTime");
            drainStaminaWhen = stamina.FindPropertyRelative("drainStaminaWhen");
            maxStamina = stamina.FindPropertyRelative("maxStamina");
            drainRate = stamina.FindPropertyRelative("drainRate");
            stillThreshold = stamina.FindPropertyRelative("stillThreshold");
            regenRateStill = stamina.FindPropertyRelative("regenRateStill");
            regenRateMoving = stamina.FindPropertyRelative("regenRateMoving");
            regenWait = stamina.FindPropertyRelative("regenWait");

            canCrouch = movement.FindPropertyRelative("canCrouch");
            crouchToggleMode = movement.FindPropertyRelative("crouchToggleMode");
            crouchSpeeds = movement.FindPropertyRelative("crouchSpeeds");
            crouchJumping = movement.FindPropertyRelative("crouchJumping");
            crouchHeight = movement.FindPropertyRelative("crouchHeight");
            crouchTime = movement.FindPropertyRelative("crouchTime");
            crouchCurve = movement.FindPropertyRelative("crouchCurve");
            standUpTime = movement.FindPropertyRelative("standUpTime");
            standUpCurve = movement.FindPropertyRelative("standUpCurve");

            canJump = movement.FindPropertyRelative("canJump");
            jumpingRequiresStamina = movement.FindPropertyRelative("jumpingRequiresStamina");
            jumpStaminaRequire = movement.FindPropertyRelative("jumpStaminaRequire");
            jumpStaminaCost = movement.FindPropertyRelative("jumpStaminaCost");
            jumpHeight = movement.FindPropertyRelative("jumpHeight");
            airJump = movement.FindPropertyRelative("airJump");
            airJumpTime = movement.FindPropertyRelative("airJumpTime");
            airJumpsAmount = movement.FindPropertyRelative("airJumpsAmount");
            allowAirJumpDirectionChange = movement.FindPropertyRelative("allowAirJumpDirectionChange");

            movingPlatforms = movement.FindPropertyRelative("movingPlatforms");
            movingPlatformsEnabled = movingPlatforms.FindPropertyRelative("enabled");
            movingPlatformsPosition = movingPlatforms.FindPropertyRelative("movePosition");
            movingPlatformsRotation = movingPlatforms.FindPropertyRelative("moveRotation");
            movingPlatformsMaxAngle = movingPlatforms.FindPropertyRelative("maxAngle");

            groundLayer = movement.FindPropertyRelative("groundLayer");
            acceleration = movement.FindPropertyRelative("acceleration");
            gravity = movement.FindPropertyRelative("gravity");
            airControl = movement.FindPropertyRelative("airControl");
            enableGroundStick = movement.FindPropertyRelative("enableGroundStick");
            groundStick = movement.FindPropertyRelative("groundStick");
            groundCheck = movement.FindPropertyRelative("groundCheck");
            rayAmount = movement.FindPropertyRelative("rayAmount");
            rayHeight = movement.FindPropertyRelative("rayHeight");
            rayLength = movement.FindPropertyRelative("rayLength");

            enableBob = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("enableBob");
            bobUnscaledTime = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("unscaledTime");
            bobFrequency = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("bobFrequency");
            bobHeight = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("bobHeight");
            swayAngle = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("swayAngle");
            sideMovement = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("sideMovement");
            heightMultiplier = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("heightMultiplier");
            strideMultiplier = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("strideMultiplier");
            landMove = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("landMove");
            landTilt = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("landTilt");
            enableStrafeTilting = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("enableStrafeTilting");
            strafeTilt = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("strafeTilt");
            bobTarget = headBob.FindPropertyRelative("bobClass").FindPropertyRelative("bobTarget");

            enableAudio = audio.FindPropertyRelative("enableAudio");
            audioUnscaledTime = audio.FindPropertyRelative("unscaledTime");
            audioType = audio.FindPropertyRelative("audioType");
            basedOnHeadBob = audio.FindPropertyRelative("basedOnHeadBob");
            stepTime = audio.FindPropertyRelative("stepTime");
            walkFootsteps = audio.FindPropertyRelative("walkFootsteps");
            runFootsteps = audio.FindPropertyRelative("runFootsteps");
            crouchFootsteps = audio.FindPropertyRelative("crouchFootsteps");
            jumpingSteps = audio.FindPropertyRelative("jumping");
            landingSteps = audio.FindPropertyRelative("landing");
            footstepsSource = audio.FindPropertyRelative("footstepsSource");
            jumpSource = audio.FindPropertyRelative("jumpSource");
            landSource = audio.FindPropertyRelative("landSource");

            goldPlayer = (GoldPlayerController)target;
            characterController = goldPlayer.GetComponent<CharacterController>();

            if (inputContents == null)
            {
                List<GUIContent> list = new List<GUIContent>();
                SerializedProperty cam = camera.Copy();
                while (cam.NextVisible(true))
                {
                    if (cam.propertyPath.StartsWith(camera.name) && cam.depth < 2)
                    {
                        if (cam.name.StartsWith("input_"))
                        {
                            list.Add(new GUIContent(cam.displayName.Substring(6), cam.tooltip));
                        }
                    }
                }

                SerializedProperty move = movement.Copy();
                while (move.NextVisible(true))
                {
                    if (move.propertyPath.StartsWith(movement.name) && move.depth < 2)
                    {
                        if (move.name.StartsWith("input_"))
                        {
                            list.Add(new GUIContent(move.displayName.Substring(6), move.tooltip));
                        }
                    }
                }
                
                inputContents = list.ToArray();
            }
        }

#if !USE_UI_ELEMENTS
        public override void OnInspectorGUI()
        {
            if (characterController != null)
            {
                Vector3 scale = characterController.transform.localScale;
                if (Math.Abs(scale.x - 1f) > 0.00001f || Math.Abs(scale.y - 1f) > 0.00001f || Math.Abs(scale.z - 1f) > 0.00001f)
                {
                    EditorGUILayout.HelpBox("Your transform scale needs to be 1, 1, 1 in order to work properly!", MessageType.Error);
                    if (GUILayout.Button("Fix"))
                    {
                        Undo.RecordObject(characterController.transform, "Fixed player scale");
                        characterController.transform.localScale = Vector3.one;
                    }
                }
                
                if (Math.Abs(characterController.center.y - characterController.height / 2f) > 0.00001f)
                {
                    EditorGUILayout.HelpBox("The Character Controller Y center must be half of the height. Set your Y center to " + characterController.height / 2 + "!", MessageType.Warning);
                    if (GUILayout.Button("Fix"))
                    {
                        Undo.RecordObject(characterController, "Fixed player center");
                        characterController.center = new Vector3(characterController.center.x, characterController.height / 2, characterController.center.z);
                    }
                }
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
            EditorGUILayout.PropertyField(canLookAround);
            EditorGUILayout.PropertyField(shouldLockCursor);
            EditorGUILayout.PropertyField(rotateCameraOnly);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(invertXAxis);
            EditorGUILayout.PropertyField(invertYAxis);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(lookSensitivity);
            EditorGUILayout.PropertyField(lookDamping);
            EditorGUILayout.PropertyField(minimumX);
            EditorGUILayout.PropertyField(maximumX);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(cameraHead);
#if GOLD_PLAYER_CINEMACHINE
            EditorGUILayout.PropertyField(useCinemachine);
            if (useCinemachine.boolValue)
            {
                EditorGUILayout.PropertyField(targetVirtualCamera);
            }
            else
#endif
                EditorGUILayout.PropertyField(targetCamera);

            EditorGUILayout.Space();

            DrawFancyFoldout(enableZooming, "Zooming", true, () =>
            {
                EditorGUILayout.PropertyField(enableZooming);
                DrawElementsConditional(enableZooming, () =>
                {
                    EditorGUILayout.PropertyField(targetZoom);
                    EditorGUILayout.PropertyField(zoomInTime);
                    EditorGUILayout.PropertyField(zoomInCurve);
                    EditorGUILayout.PropertyField(zoomOutTime);
                    EditorGUILayout.PropertyField(zoomOutCurve);
                    EditorGUILayout.PropertyField(zoomInput);
                });
            });
            
            EditorGUILayout.Space();

            DrawFancyFoldout(fieldOfViewKick, "Field of View Kick", true, () =>
            {
                EditorGUILayout.PropertyField(fovEnable);
                DrawElementsConditional(fovEnable, () =>
                {
                    if (enableZooming.boolValue)
                    {
                        EditorGUILayout.HelpBox("Zooming and field of view kick may cause some visual issues when both are active!", MessageType.Warning);
                    }
                    
                    EditorGUILayout.PropertyField(fovUnscaledTime);
                    EditorGUILayout.PropertyField(fovKickWhen);
                    EditorGUILayout.PropertyField(fovKickAmount);
                    EditorGUILayout.PropertyField(fovTimeTo);
                    EditorGUILayout.PropertyField(fovToCurve);
                    EditorGUILayout.PropertyField(fovTimeFrom);
                    EditorGUILayout.PropertyField(fovFromCurve);
                });
            });
        }

        private void DoMovementGUI()
        {
            EditorGUILayout.PropertyField(canMoveAround);
            EditorGUILayout.PropertyField(moveUnscaledTime);

            EditorGUILayout.Space();

            DrawFancyFoldout(walkingSpeed, "Walking", false, () => EditorGUILayout.PropertyField(walkingSpeed));

            EditorGUILayout.Space();

            DrawFancyFoldout(canRun, "Running", false, () =>
            {
                EditorGUILayout.PropertyField(canRun);

                DrawElementsConditional(canRun, () =>
                {
                    EditorGUILayout.PropertyField(runToggleMode);
                    EditorGUILayout.PropertyField(runSpeeds);
                    EditorGUILayout.PropertyField(stamina, false);
                    if (stamina.isExpanded)
                    {
                        EditorGUI.indentLevel++;

                        EditorGUILayout.PropertyField(enableStamina);
                        DrawElementsConditional(enableStamina, () =>
                        {
                            EditorGUILayout.PropertyField(staminaUnscaledTime);
                            EditorGUILayout.PropertyField(drainStaminaWhen);
                            EditorGUILayout.PropertyField(maxStamina);
                            EditorGUILayout.PropertyField(drainRate);
                            EditorGUILayout.PropertyField(stillThreshold);
                            EditorGUILayout.PropertyField(regenRateStill);
                            EditorGUILayout.PropertyField(regenRateMoving);
                            EditorGUILayout.PropertyField(regenWait);
                        });
                        EditorGUI.indentLevel--;
                    }
                });
            });

            EditorGUILayout.Space();

            DrawFancyFoldout(canCrouch, "Crouching", false, () =>
            {
                EditorGUILayout.PropertyField(canCrouch);
                DrawElementsConditional(canCrouch, () =>
                {
                    EditorGUILayout.PropertyField(crouchToggleMode);
                    EditorGUILayout.PropertyField(crouchSpeeds);
                    EditorGUILayout.PropertyField(crouchJumping);
                    if (crouchHeight.floatValue < 0.8f)
                    {
                        EditorGUILayout.HelpBox("The Crouch Height should not be less than 0.8 because it breaks the character controller!", MessageType.Warning);
                    }
                    EditorGUILayout.PropertyField(crouchHeight);
                    EditorGUILayout.PropertyField(crouchTime);
                    EditorGUILayout.PropertyField(crouchCurve);
                    EditorGUILayout.PropertyField(standUpTime);
                    EditorGUILayout.PropertyField(standUpCurve);
                });
            });

            EditorGUILayout.Space();

            DrawFancyFoldout(canJump, "Jumping", false, () =>
            {
                EditorGUILayout.PropertyField(canJump);
                DrawElementsConditional(canJump, () =>
                {
                    DrawElementsConditional(enableStamina, () =>
                    {
                        EditorGUILayout.PropertyField(jumpingRequiresStamina);
                        DrawElementsConditional(jumpingRequiresStamina, () =>
                        {
                            EditorGUILayout.PropertyField(jumpStaminaRequire);
                            EditorGUILayout.PropertyField(jumpStaminaCost);
                        });
                    });

                    EditorGUILayout.PropertyField(jumpHeight);
                    EditorGUILayout.PropertyField(airJump);

                    DrawElementsConditional(airJump, () =>
                    {
                        EditorGUILayout.PropertyField(airJumpTime);
                        EditorGUILayout.PropertyField(airJumpsAmount);
                        EditorGUILayout.PropertyField(allowAirJumpDirectionChange);
                    });
                });
            });

            EditorGUILayout.Space();

            DrawFancyFoldout(movingPlatforms, "Moving Platforms", false, () =>
            {
                EditorGUILayout.PropertyField(movingPlatformsEnabled);
                DrawElementsConditional(movingPlatformsEnabled, () =>
                {
                    EditorGUILayout.PropertyField(movingPlatformsPosition);
                    EditorGUILayout.PropertyField(movingPlatformsRotation);
                    EditorGUILayout.PropertyField(movingPlatformsMaxAngle);
                });
            });

            EditorGUILayout.Space();

            DrawFancyFoldout(acceleration, "Miscellaneous", false, () =>
            {
                if (groundLayer.intValue == (groundLayer.intValue | 1 << goldPlayer.gameObject.layer))
                {
                    EditorGUILayout.HelpBox("The player layer should not be included as a Ground Layer!", MessageType.Warning);
                }
                EditorGUILayout.PropertyField(groundLayer);
                EditorGUILayout.PropertyField(acceleration);
                EditorGUILayout.PropertyField(gravity);
                EditorGUILayout.PropertyField(airControl);
                EditorGUILayout.PropertyField(enableGroundStick);
                DrawElementsConditional(enableGroundStick, () => EditorGUILayout.PropertyField(groundStick));
                EditorGUILayout.PropertyField(groundCheck);
                if (groundCheck.intValue == 1)
                {
                    EditorGUILayout.PropertyField(rayAmount);
                    EditorGUILayout.PropertyField(rayHeight);
                    EditorGUILayout.PropertyField(rayLength);
                }
            });
        }

        private void DoHeadBobGUI()
        {
            EditorGUILayout.PropertyField(enableBob);
            DrawElementsConditional(enableBob, () =>
            {
                EditorGUILayout.PropertyField(bobUnscaledTime);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(bobFrequency);
                EditorGUILayout.PropertyField(bobHeight);
                EditorGUILayout.PropertyField(swayAngle);
                EditorGUILayout.PropertyField(sideMovement);
                EditorGUILayout.PropertyField(heightMultiplier);
                EditorGUILayout.PropertyField(strideMultiplier);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(landMove);
                EditorGUILayout.PropertyField(landTilt);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(enableStrafeTilting);
                DrawElementsConditional(enableStrafeTilting, () => EditorGUILayout.PropertyField(strafeTilt));

                EditorGUILayout.Space();

                if (cameraHead.objectReferenceValue == bobTarget.objectReferenceValue)
                {
                    EditorGUILayout.HelpBox("You should not target the same object as your camera head. " +
                            "It's recommended that you create a parent to this object that you can target your bobbing on.", MessageType.Warning);
                }

                EditorGUILayout.PropertyField(bobTarget);
            });
        }

        private void DoAudioGUI()
        {
            EditorGUILayout.PropertyField(enableAudio);
            DrawElementsConditional(enableAudio, () =>
            {
                EditorGUILayout.PropertyField(audioUnscaledTime);
                EditorGUILayout.PropertyField(audioType);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(basedOnHeadBob);
                DrawElementsConditional(!basedOnHeadBob.boolValue, () => EditorGUILayout.PropertyField(stepTime));

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(walkFootsteps, true);
                EditorGUILayout.PropertyField(runFootsteps);
                EditorGUILayout.PropertyField(crouchFootsteps);
                EditorGUILayout.PropertyField(jumpingSteps);
                EditorGUILayout.PropertyField(landingSteps);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(footstepsSource);
                EditorGUILayout.PropertyField(jumpSource);
                EditorGUILayout.PropertyField(landSource);
            });
        }

        private void DoInputGUI()
        {
            EditorGUILayout.LabelField("Camera", EditorStyles.boldLabel);

            int index = 0;
            
            SerializedProperty cam = camera.Copy();
            while (cam.NextVisible(true))
            {
                if (cam.propertyPath.StartsWith(camera.name) && cam.depth < 2)
                {
                    if (cam.name.StartsWith("input_"))
                    {
                        EditorGUILayout.PropertyField(cam, inputContents[index], true);
                        index++;
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
                        EditorGUILayout.PropertyField(move, inputContents[index], true);
                        index++;
                    }
                }
            }
        }

        private void OnSceneGUI()
        {
            GoldPlayerController controller = target as GoldPlayerController;

            if (controller == null)
            {
                return;
            }

            Color oColor = Handles.color;

            Handles.color = Color.blue;

            Vector3 origin = controller.transform.position + controller.Controller.center;

            Handles.DrawLine(origin, origin + controller.Camera.BodyForward);

            if (GoldPlayerProjectSettings.Instance.ShowGroundCheckGizmos)
            {
                float radius = controller.GetComponent<CharacterController>().radius;

                if (controller.Movement.GroundCheck == GroundCheckType.Raycast)
                {
                    Vector3[] rays = new Vector3[controller.Movement.RayAmount + 1];
                    controller.Movement.CreateGroundCheckRayCircle(ref rays, controller.transform.position, radius);

                    for (int i = 0; i < rays.Length; i++)
                    {
                        if (Application.isPlaying)
                        {
                            bool hit = Physics.Raycast(rays[i], Vector3.down, controller.Movement.RayLength, controller.Movement.GroundLayer, QueryTriggerInteraction.Ignore);
                            Handles.color = hit ? new Color(0f, 1f, 0f, 1f) : new Color(1f, 0f, 0f, 1f);
                        }
                        else
                        {
                            Handles.color = Color.white;
                        }

                        Handles.DrawLine(rays[i], new Vector3(rays[i].x, rays[i].y - controller.Movement.RayLength, rays[i].z));
                    }
                }
                else if (controller.Movement.GroundCheck == GroundCheckType.Sphere)
                {
                    Vector3 pos = new Vector3(controller.transform.position.x, controller.transform.position.y + radius - 0.1f, controller.transform.position.z);
                    if (Application.isPlaying)
                    {
                        Handles.color = controller.Movement.IsGrounded ? new Color(0f, 1f, 0f, 0.25f) : new Color(1f, 0f, 0f, 0.25f);
                    }
                    else
                    {
                        Handles.color = new Color(0f, 1f, 0f, 0.25f);
                    }
                    Handles.SphereHandleCap(0, pos, Quaternion.identity, radius * 2, EventType.Repaint);
                    if (Application.isPlaying)
                    {
                        Handles.color = controller.Movement.IsGrounded ? new Color(0f, 1f, 0f, 1f) : new Color(1f, 0f, 0f, 1f);
                    }
                    else
                    {
                        Handles.color = new Color(0f, 1f, 0f, 1f);
                    }
                    //Handles.DrawWireSphere(pos, radius);
                    Handles.RadiusHandle(Quaternion.identity, pos, radius, false);
                }

            }
            Handles.color = oColor;
        }

#else
        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            controllerWarning = new IMGUIContainer(() =>
            {
                EditorGUILayout.HelpBox("The Character Controller Y center must be half of the height. Set your Y center to " + characterController.height / 2 + "!", MessageType.Warning);
            });
            controllerWarningFix = new Button(() =>
            {
                if (characterController != null)
                {
                    Undo.RecordObject(characterController, "Fixed player center");
                    characterController.center = new Vector3(characterController.center.x, characterController.height / 2, characterController.center.z);
                }
            })
            {
                text = "Fix"
            };

            if (characterController != null)
            {
                bool show = characterController.center.y != characterController.height / 2;
                controllerWarning.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
                controllerWarningFix.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            }
            else
            {
                controllerWarning.style.display = DisplayStyle.None;
                controllerWarningFix.style.display = DisplayStyle.None;
            }

            root.Add(controllerWarning);
            root.Add(controllerWarningFix);

            IVisualElementScheduledItem controllerCheck = root.schedule.Execute(() =>
            {
                if (characterController != null)
                {
                    bool show = characterController.center.y != characterController.height / 2;
                    controllerWarning.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
                    controllerWarningFix.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
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
            root.Add(GetSpace(3));

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
            inputElements.Add(GetHeaderLabel("Camera"));

            cameraElements = new VisualElement();
            SerializedProperty it = camera.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(camera.name) && it.depth < 2)
                {
                    if (it.name.Equals("invertXAxis"))
                    {
                        cameraElements.Add(GetSpace());
                    }

                    if (it.name.Equals("mouseSensitivity"))
                    {
                        cameraElements.Add(GetSpace());
                    }

                    if (it.name.Equals("fieldOfViewKick"))
                    {
                        cameraElements.Add(GetSpace());
                    }

                    if (it.name.Equals("cameraHead"))
                    {
                        cameraElements.Add(GetSpace());
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
            inputElements.Add(GetHeaderLabel("Movement"));

            movementElements = new VisualElement();

            movement = serializedObject.FindProperty("movement");
            SerializedProperty it = movement.Copy();
            while (it.NextVisible(true))
            {
                if (it.propertyPath.StartsWith(movement.name) && it.depth < 2)
                {
                    if (it.name.Equals("walkingSpeeds"))
                    {
                        movementElements.Add(GetHeaderLabel("Walking"));
                    }

                    if (it.name.Equals("canRun"))
                    {
                        movementElements.Add(GetHeaderLabel("Running"));
                    }

                    if (it.name.Equals("canJump"))
                    {
                        movementElements.Add(GetHeaderLabel("Jumping"));
                    }

                    if (it.name.Equals("canCrouch"))
                    {
                        movementElements.Add(GetHeaderLabel("Crouching"));
                    }

                    if (it.name.Equals("groundLayer"))
                    {
                        movementElements.Add(GetHeaderLabel("Other"));
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
                        crouchHeightWarning = GetHelpBox("The Crouch Height should not be less than 0.8 because it breaks the character controller!", MessageType.Warning);

                        movementElements.Add(crouchHeightWarning);

                        field.RegisterCallback<ChangeEvent<float>>((evt) => { ValidateCrouchHeight(evt.newValue); });

                        ValidateCrouchHeight(it.floatValue);
                    }

                    if (it.name.Equals("groundLayer"))
                    {
                        groundLayerWarning = GetHelpBox("The player layer should not be included as a Ground Layer!", MessageType.Warning);

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
                        headBobElements.Add(GetSpace());
                    }

                    if (it.name.Equals("landMove"))
                    {
                        headBobElements.Add(GetSpace());
                    }

                    if (it.name.Equals("enableStrafeTilting"))
                    {
                        headBobElements.Add(GetSpace());
                    }

                    if (it.name.Equals("bobTarget"))
                    {
                        headBobElements.Add(GetSpace());
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
                        audioElements.Add(GetSpace());
                    }

                    if (it.name.Equals("walkFootsteps"))
                    {
                        audioElements.Add(GetSpace());
                    }

                    if (it.name.Equals("footstepsSource"))
                    {
                        audioElements.Add(GetSpace());
                    }

                    audioElements.Add(new PropertyField(it));
                }
            }
        }
#endif
    }
}
#endif
