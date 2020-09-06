using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Hertzole.GoldPlayer.Tests
{
    public class BaseGoldPlayerTest
    {
        protected GoldPlayerController player;
        protected GoldPlayerTestInput input;

        protected List<GameObject> sceneObjects = new List<GameObject>();

        [UnitySetUp]
        public IEnumerator SetupScene()
        {
            player = SetupPlayer();
            CreateTestScene();

            yield return new EnterPlayMode();
        }

        [UnityTearDown]
        public IEnumerator TearDownScene()
        {
            Object.DestroyImmediate(player.gameObject);
            for (int i = 0; i < sceneObjects.Count; i++)
            {
                Object.DestroyImmediate(sceneObjects[i]);
            }

            sceneObjects.Clear();

            yield return new ExitPlayMode();
        }

        private GoldPlayerController SetupPlayer()
        {
            GameObject playerGO = new GameObject("[TEST] Test Player", typeof(CharacterController));
            playerGO.transform.position = new Vector3(0, 0.08f, 0);
            playerGO.layer = 31;

            GameObject bobTarget = new GameObject("[TEST] Test Player Bob Target");
            bobTarget.transform.SetParent(playerGO.transform);
            bobTarget.transform.localPosition = new Vector3(0, 1.6f, 0);

            GameObject playerCameraHead = new GameObject("[TEST] Test Player Camera Head");
            playerCameraHead.transform.SetParent(bobTarget.transform);
            playerCameraHead.transform.localPosition = new Vector3(0, 0, 0);
            Camera camera = playerCameraHead.AddComponent<Camera>();

            input = playerGO.AddComponent<GoldPlayerTestInput>();

            GoldPlayerController playerController = playerGO.AddComponent<GoldPlayerController>();
            playerController.GetComponent<CharacterController>().center = new Vector3(0, 1, 0);

            playerController.Camera.CameraHead = playerCameraHead.transform;
            playerController.Camera.FieldOfViewKick.TargetCamera = camera;
            playerController.Camera.FieldOfViewKick.EnableFOVKick = true;

            playerController.HeadBob.BobTarget = bobTarget.transform;

            playerController.Movement.GroundLayer = 1;

            playerController.InitOnStart = false;
            playerController.GetReferences();
            playerController.Initialize();

            return playerController;
        }

        private void CreateTestScene()
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            plane.transform.eulerAngles = new Vector3(90, 0, 0);
            plane.transform.localScale = new Vector3(100, 100, 1);

            sceneObjects.Add(plane);
        }

        protected static void AreApproximatelyEqualVector3(Vector3 expected, Vector3 actual, float tolerance = 0.001f)
        {
            Assert.AreApproximatelyEqual(expected.x, actual.x, tolerance);
            Assert.AreApproximatelyEqual(expected.y, actual.y, tolerance);
            Assert.AreApproximatelyEqual(expected.z, actual.z, tolerance);
        }
    }
}
