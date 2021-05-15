#if GOLD_PLAYER_TESTS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Hertzole.GoldPlayer.Tests
{
    internal class BaseGoldPlayerTest
    {
        protected GoldPlayerController player;
        protected GoldPlayerTestInput input;

        protected List<GameObject> sceneObjects = new List<GameObject>();

        [UnitySetUp]
        public IEnumerator SetupScene()
        {
            Time.timeScale = 1;

            player = SetupPlayer();
            CreateTestScene();

#if UNITY_EDITOR
            yield return new EnterPlayMode();
#else
            yield return null;
#endif
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

#if UNITY_EDITOR
            yield return new ExitPlayMode();
#else
            yield return null;
#endif
        }

        protected virtual GoldPlayerController SetupPlayer()
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
            playerController.Camera.TargetCamera = camera;
            playerController.Camera.FieldOfViewKick.EnableFOVKick = true;

            playerController.HeadBob.BobTarget = bobTarget.transform;

            playerController.Movement.GroundLayer = 1;

            playerController.Movement.MoveInput = GoldPlayerTestInput.MOVE;
            playerController.Movement.JumpInput = GoldPlayerTestInput.JUMP;
            playerController.Movement.RunInput = GoldPlayerTestInput.RUN;
            playerController.Movement.CrouchInput = GoldPlayerTestInput.CROUCH;

            playerController.Camera.LookInput = GoldPlayerTestInput.LOOK;

            playerController.InitOnStart = false;
            playerController.GetReferences();
            playerController.Initialize();

            return playerController;
        }

        protected void ArePropertyAndFieldSame(object property, object field)
        {
            Assert.AreEqual(property, field);
        }

        protected virtual void CreateTestScene()
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            plane.transform.eulerAngles = new Vector3(90, 0, 0);
            plane.transform.localScale = new Vector3(100, 100, 1);

            sceneObjects.Add(plane);
        }

        protected static void AreApproximatelyEqualVector2(Vector2 expected, Vector2 actual, float tolerance = 0.001f)
        {
            Assert.AreApproximatelyEqual(expected.x, actual.x, tolerance);
            Assert.AreApproximatelyEqual(expected.y, actual.y, tolerance);
        }

        protected static void AreApproximatelyEqualVector3(Vector3 expected, Vector3 actual, float tolerance = 0.001f)
        {
            Assert.AreApproximatelyEqual(expected.x, actual.x, tolerance);
            Assert.AreApproximatelyEqual(expected.y, actual.y, tolerance);
            Assert.AreApproximatelyEqual(expected.z, actual.z, tolerance);
        }

        protected static void AreApproximatelyEqualQuaternion(Quaternion expected, Quaternion actual, bool includeW, float tolerance = 0.001f)
        {
            Assert.AreApproximatelyEqual(expected.x, actual.x, tolerance);
            Assert.AreApproximatelyEqual(expected.y, actual.y, tolerance);
            Assert.AreApproximatelyEqual(expected.z, actual.z, tolerance);
            if (includeW)
            {
                Assert.AreApproximatelyEqual(expected.w, actual.w, tolerance);
            }
        }

        protected IEnumerator RunTimeScaleTest(IEnumerator normalScaleTest, IEnumerator frozenScaleTest)
        {
            player.UnscaledTime = false;
            Time.timeScale = 1;

            while (true)
            {
                object current;
                try
                {
                    if (normalScaleTest.MoveNext() == false)
                    {
                        break;
                    }

                    current = normalScaleTest.Current;
                }
                catch (AssertionException ex)
                {
                    Debug.LogAssertion(ex);
                    yield break;
                }

                yield return current;
            }

            player.SetPositionAndRotation(Vector3.zero, 0);
            player.UnscaledTime = true;
            Time.timeScale = 0;
            
            while (true)
            {
                object current;
                try
                {
                    if (frozenScaleTest.MoveNext() == false)
                    {
                        break;
                    }

                    current = frozenScaleTest.Current;
                }
                catch (AssertionException ex)
                {
                    Debug.LogAssertion(ex);
                    yield break;
                }

                yield return current;
            }
        }
    }
}
#endif