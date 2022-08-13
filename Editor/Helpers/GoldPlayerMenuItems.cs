using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    public static class GoldPlayerMenuItems
    {
        private const string STEP1_GUID = "44e59b73420f0e64fb30e755c49b3823";
        private const string STEP2_GUID = "62e6bf700540ea9478b91b33afa85e7e";
        private const string STEP3_GUID = "bbe4ed33245f5804db42ff8c43d75412";
        private const string STEP4_GUID = "1e5761292ed025241bdb02ff49c552dd";
        private const string JUMP_GUID = "11cc82c4daa9f304dbbfab62c9207608";
        private const string LAND_GUID = "ceb0f9d730c213441b4eaccb59f6fcde";

        [MenuItem("GameObject/3D Object/Gold Player Controller")]
        public static void CreateGoldPlayer(MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent != null ? parent.transform : null, "Gold Player Controller");

            // Create the root object.
            GameObject root = new GameObject(uniqueName) { tag = "Player", layer = LayerMask.NameToLayer("TransparentFX") };
            // Place the root in the scene.
            PlaceInScene(root, parent);

            // Create the graphics holder.
            GameObject graphicsHolder = CreateChild("Graphics", root);
            // Create the actual graphic.
            GameObject graphic = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            // Set up the graphic.
            graphic.name = "Capsule";
            graphic.layer = root.layer;
            graphic.transform.SetParent(graphicsHolder.transform, false);
            graphic.transform.localScale = new Vector3(0.8f, 1, 0.8f);
            graphic.transform.localPosition = Vector3.up;
            // Remove any colliders, if they are present.
            Collider graphicsCollider = graphic.GetComponent<Collider>();
            if (graphicsCollider != null)
            {
                Object.DestroyImmediate(graphicsCollider);
            }

            // Create the camera head.
            GameObject cameraHead = CreateChild("Camera Head", root);
            cameraHead.transform.localPosition = new Vector3(0f, 1.6f, 0f);

            // Create the bob target and set the position.
            GameObject bobTarget = CreateChild("Bob Target", cameraHead);

            // Create the player camera.
            GameObject playerCameraGo = CreateChild("Player Camera", bobTarget);
#if GOLD_PLAYER_CINEMACHINE
            Cinemachine.CinemachineVirtualCamera playerCamera = playerCameraGo.AddComponent<Cinemachine.CinemachineVirtualCamera>();
            playerCamera.m_Lens.FieldOfView = 80;
            playerCamera.m_Lens.NearClipPlane = 0.01f;
            playerCamera.m_Lens.FarClipPlane = 1000f;
#else
            Camera playerCamera = playerCameraGo.AddComponent<Camera>();
            playerCamera.clearFlags = CameraClearFlags.Skybox;
            playerCamera.fieldOfView = 80;
            playerCamera.nearClipPlane = 0.01f;
            playerCamera.farClipPlane = 1000f;
            playerCamera.tag = "MainCamera";
#endif

            playerCameraGo.AddComponent<FlareLayer>();
            playerCameraGo.AddComponent<AudioListener>();

            // Create the audio object.
            GameObject audioGo = CreateChild("Audio", root);

            AudioSource stepsSource = audioGo.AddComponent<AudioSource>();
            AudioSource jumpSource = audioGo.AddComponent<AudioSource>();
            AudioSource landSource = audioGo.AddComponent<AudioSource>();

            // Add the character controller and set it up.
            CharacterController characterController = root.AddComponent<CharacterController>();
            characterController.radius = 0.4f;
            characterController.height = 2;
            characterController.center = new Vector3(0, 1, 0);

            // Add the actual Gold Player Controller and set it up.
            GoldPlayerController goldController = root.AddComponent<GoldPlayerController>();

            goldController.Camera.CameraHead = cameraHead.transform;
#if GOLD_PLAYER_CINEMACHINE
            goldController.Camera.UseCinemachine = true;
            goldController.Camera.TargetVirtualCamera = playerCamera;
#else
            goldController.Camera.TargetCamera = playerCamera;
#endif

            goldController.Movement.GroundLayer = 1;

            goldController.HeadBob.BobTarget = bobTarget.transform;

            goldController.Audio.FootstepsSource = stepsSource;
            goldController.Audio.JumpSource = jumpSource;
            goldController.Audio.LandSource = landSource;

            AudioClip step1 = GetAsset<AudioClip>(STEP1_GUID);
            AudioClip step2 = GetAsset<AudioClip>(STEP2_GUID);
            AudioClip step3 = GetAsset<AudioClip>(STEP3_GUID);
            AudioClip step4 = GetAsset<AudioClip>(STEP4_GUID);
            AudioClip jump = GetAsset<AudioClip>(JUMP_GUID);
            AudioClip land = GetAsset<AudioClip>(LAND_GUID);

            AudioItem walkSounds = CreateAudioItem(true, true, 1f, 0.9f, 1.1f, true, 1f, step1, step2, step3, step4);
            AudioItem runSounds = CreateAudioItem(true, true, 1.4f, 1.4f, 1.6f, true, 1f, step1, step2, step3, step4);
            AudioItem crouchSounds = CreateAudioItem(true, true, 1f, 0.9f, 1.1f, true, 0.4f, step1, step2, step3, step4);
            AudioItem jumpSound = CreateAudioItem(true, true, 1f, 0.9f, 1.1f, true, 1f, jump);
            AudioItem landSound = CreateAudioItem(true, true, 1f, 0.9f, 1.1f, true, 1f, land);

            goldController.Audio.WalkFootsteps = walkSounds;
            goldController.Audio.RunFootsteps = runSounds;
            goldController.Audio.CrouchFootsteps = crouchSounds;
            goldController.Audio.Jumping = jumpSound;
            goldController.Audio.Landing = landSound;

#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
            root.AddComponent<GoldPlayerInputSystem>();
#else
            root.AddComponent<GoldPlayerInput>();
#endif

            // Register the undo.
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
        }

        private static GameObject CreateChild(string name, GameObject parent)
        {
            GameObject go = new GameObject(name) { layer = parent.layer };
            go.transform.SetParent(parent.transform, false);
            return go;
        }

        private static T GetAsset<T>(string guid) where T : Object
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private static AudioItem CreateAudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch, bool changeVol, float vol, params AudioClip[] clips)
        {
            AudioItem item = new AudioItem(enabled, randomPitch, pitch, minPitch, maxPitch, changeVol, vol);
            int clipSize = 0;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null)
                {
                    clipSize++;
                }
            }

            item.AudioClips = new AudioClip[clipSize];
            int index = 0;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] != null)
                {
                    item.AudioClips[index] = clips[i];
                    index++;
                }
            }

            return item;
        }

        /*
         * Borrowed from
         * https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Commands/GOCreationCommands.cs#L15
         * */
        private static void PlaceInScene(GameObject go, GameObject parent)
        {
            if (parent != null)
            {
                Transform transform = go.transform;
                Undo.SetTransformParent(transform, parent.transform, "Reparenting");
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }
            else
            {
                PlaceGameObjectInFrontOfSceneView(go);
#if UNITY_2018_3_OR_LATER
                StageUtility.PlaceGameObjectInCurrentStage(go);
#endif
            }

            Selection.activeGameObject = go;
        }

        /*
         * Borrowed from
         * https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/SceneView/SceneView.cs#L641
         * */
        private static void PlaceGameObjectInFrontOfSceneView(GameObject go)
        {
            SceneView view = SceneView.currentDrawingSceneView;
            if (!view)
            {
                view = SceneView.sceneViews[0] as SceneView;
            }

            if (view)
            {
                view.MoveToView(go.transform);
            }
        }
    }
}
