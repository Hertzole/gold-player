using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    public static class GoldPlayerMenuItems
    {
        [MenuItem("GameObject/3D Object/Gold Player Controller")]
        public static void CreateGoldPlayer(MenuCommand menuCommand)
        {
            // Create the root object.
            GameObject root = new GameObject("Gold Player Controller") { tag = "Player", layer = LayerMask.NameToLayer("TransparentFX") };
            // Place the root in the scene.
            PlaceInScene(root, menuCommand.context as GameObject);

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
                Object.DestroyImmediate(graphicsCollider);

            // Create the bob target and set the position.
            GameObject bobTarget = CreateChild("Bob Target", root);
            bobTarget.transform.localPosition = new Vector3(0f, 1.6f, 0f);

            // Create the camera head.
            GameObject cameraHead = CreateChild("Camera Head", bobTarget);

            // Create the player camera.
            GameObject playerCameraGo = CreateChild("Player Camera", cameraHead);
            Camera playerCamera = playerCameraGo.AddComponent<Camera>();
            playerCamera.clearFlags = CameraClearFlags.Skybox;
            playerCamera.fieldOfView = 80;
            playerCamera.nearClipPlane = 0.01f;
            playerCamera.farClipPlane = 1000f;

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
            goldController.Camera.FOVKick.TargetCamera = playerCamera;

            goldController.Movement.GroundLayer = 1;

            goldController.HeadBob.BobTarget = bobTarget.transform;

            goldController.Audio.FootstepsSource = stepsSource;
            goldController.Audio.JumpSource = jumpSource;
            goldController.Audio.LandSource = landSource;

            // Register the undo.
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
        }

        private static GameObject CreateChild(string name, GameObject parent)
        {
            GameObject go = new GameObject(name) { layer = parent.layer };
            go.transform.SetParent(parent.transform, false);
            return go;
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
                StageUtility.PlaceGameObjectInCurrentStage(go);
            }

            GameObjectUtility.EnsureUniqueNameForSibling(go);

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
                view = SceneView.sceneViews[0] as SceneView;
            if (view)
                view.MoveToView(go.transform);
        }
    }
}
