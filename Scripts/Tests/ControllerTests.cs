#if GOLD_PLAYER_TESTS
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Hertzole.GoldPlayer.Tests
{
    internal class ControllerTests : BaseGoldPlayerTest
    {
        [UnityTest]
        public IEnumerator TestNullInput()
        {
            Object.DestroyImmediate(input);
            player.Awake();

            string errorMessage = "[TEST] Test Player" +
#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
                " needs to have a input script derived from IGoldInput! Add the standard 'GoldPlayerInputSystem' to fix.";
#else
                " needs to have a input script derived from IGoldInput! Add the standard 'GoldPlayerInput' to fix.";
#endif
            LogAssert.Expect(LogType.Error, errorMessage);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestInitOnStart()
        {
            GameObject tempPlayer = new GameObject();
            tempPlayer.AddComponent<GoldPlayerTestInput>();
            GoldPlayerController pl = tempPlayer.AddComponent<GoldPlayerController>();

            pl.InitOnStart = false;

            Assert.IsFalse(pl.HasBeenFullyInitialized);

            yield return null;
            yield return null;

            Assert.IsFalse(pl.HasBeenFullyInitialized);

            Object.DestroyImmediate(tempPlayer);

            tempPlayer = new GameObject();
            tempPlayer.AddComponent<GoldPlayerTestInput>();
            pl = tempPlayer.AddComponent<GoldPlayerController>();
            pl.Camera.TargetCamera = player.Camera.TargetCamera;
            pl.Camera.FieldOfViewKick.EnableFOVKick = false;
            pl.HeadBob.EnableBob = false;
            pl.Camera.CameraHead = tempPlayer.transform;
            pl.InitOnStart = true;

            yield return null;
            yield return null;

            Assert.IsTrue(pl.HasBeenFullyInitialized);

            sceneObjects.Add(tempPlayer);
        }

        [UnityTest]
        public IEnumerator TestSetPosition()
        {
            yield return null;
            yield return null;
            AreApproximatelyEqualVector3(new Vector3(0, 0.08f, 0), player.transform.position);
            player.SetPosition(new Vector3(10, 0, 10));
            yield return null;
            yield return null;
            AreApproximatelyEqualVector3(new Vector3(10, 0.08f, 10), player.transform.position, 0.1f);
        }

        [UnityTest]
        public IEnumerator TestSetLocalPosition()
        {
            yield return null;
            yield return null;
            AreApproximatelyEqualVector3(new Vector3(0, 0.08f, 0), player.transform.position);
            player.SetLocalPosition(new Vector3(10, 0, 10));
            yield return null;
            yield return null;
            AreApproximatelyEqualVector3(new Vector3(10, 0.08f, 10), player.transform.position, 0.1f);
        }

        [UnityTest]
        public IEnumerator TestSetPositionAndRotation()
        {
            yield return null;
            yield return null;
            AreApproximatelyEqualVector3(new Vector3(0, 0.08f, 0), player.transform.position);
            player.SetPositionAndRotation(new Vector3(10, 0, 10), 180);
            yield return null;
            yield return null;
            AreApproximatelyEqualVector3(new Vector3(10, 0.08f, 10), player.transform.position, 0.1f);
            Assert.AreEqual(new Vector3(0, 180, 0), player.transform.eulerAngles);
            Assert.AreEqual(new Vector3(0, 0, 0), player.Camera.CameraHead.localEulerAngles);
        }

        [UnityTest]
        public IEnumerator TestSetPositionAndRotationCameraOnly()
        {
            player.Camera.RotateCameraOnly = true;

            yield return null;
            yield return null;
            AreApproximatelyEqualVector3(new Vector3(0, 0.08f, 0), player.transform.position);
            player.SetPositionAndRotation(new Vector3(10, 0, 10), 180);
            yield return null;
            yield return null;
            AreApproximatelyEqualVector3(new Vector3(10, 0.08f, 10), player.transform.position, 0.1f);
            Assert.AreEqual(new Vector3(0, 0, 0), player.transform.eulerAngles);
            Assert.AreEqual(new Vector3(0, 180, 0), player.Camera.CameraHead.localEulerAngles);
        }

        [UnityTest]
        public IEnumerator TestSetRotation()
        {
            Assert.AreEqual(player.transform.eulerAngles, Vector3.zero);
            Assert.AreEqual(player.Camera.CameraHead.localEulerAngles, Vector3.zero);
            yield return null;
            yield return null;
            player.SetRotation(90);
            yield return null;
            yield return null;
            Assert.AreEqual(player.transform.eulerAngles, new Vector3(0, 90, 0));
            Assert.AreEqual(player.Camera.CameraHead.localEulerAngles, Vector3.zero);
        }

        [UnityTest]
        public IEnumerator TestSetRotationCameraOnly()
        {
            player.Camera.RotateCameraOnly = true;

            Assert.AreEqual(player.transform.eulerAngles, Vector3.zero);
            Assert.AreEqual(player.Camera.CameraHead.localEulerAngles, Vector3.zero);
            yield return null;
            yield return null;
            player.SetRotation(90);
            yield return null;
            yield return null;
            Assert.AreEqual(player.transform.eulerAngles, Vector3.zero);
            Assert.AreEqual(player.Camera.CameraHead.localEulerAngles, new Vector3(0, 90, 0));
        }

        [UnityTest]
        public IEnumerator TestUnscaledTime()
        {
            player.Camera.FieldOfViewKick.UnscaledTime = false;
            player.Movement.UnscaledTime = true;
            player.Movement.Stamina.UnscaledTime = true;
            player.HeadBob.UnscaledTime = true;
            player.Audio.UnscaledTime = true;

            Assert.IsFalse(player.UnscaledTime);

            player.Camera.FieldOfViewKick.UnscaledTime = true;

            Assert.IsTrue(player.UnscaledTime);

            player.Camera.FieldOfViewKick.UnscaledTime = false;
            player.Movement.UnscaledTime = false;
            player.Movement.Stamina.UnscaledTime = false;
            player.HeadBob.UnscaledTime = false;
            player.Audio.UnscaledTime = false;
            Assert.IsFalse(player.UnscaledTime);

            yield return null;
        }
    }
}
#endif