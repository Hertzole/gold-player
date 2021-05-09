using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Hertzole.GoldPlayer.Tests
{
    internal class FOVKickTests : BaseGoldPlayerTest
    {
        [UnityTest]
        public IEnumerator NotEnabled()
        {
            yield return null;
            float originalFOV = player.Camera.TargetCamera.fieldOfView;
            player.Camera.FieldOfViewKick.EnableFOVKick = false;

            for (int i = 0; i < 60; i++)
            {
                player.Camera.FieldOfViewKick.ForceFOV(true);
                yield return null;
                Assert.AreApproximatelyEqual(originalFOV, player.Camera.TargetCamera.fieldOfView);
            }
        }

        [UnityTest]
        public IEnumerator IsEnabled()
        {
            yield return null;
            float targetFOV = player.Camera.FieldOfViewKick.TargetFieldOfView;
            player.Camera.FieldOfViewKick.EnableFOVKick = true;
            player.Camera.FieldOfViewKick.LerpTimeTo = 100f;

            for (int i = 0; i < 60; i++)
            {
                player.Camera.FieldOfViewKick.ForceFOV(true);
                yield return null;
            }
            Assert.AreApproximatelyEqual(targetFOV, player.Camera.TargetCamera.fieldOfView, 1f);
        }

#if GOLD_PLAYER_CINEMACHINE
        [UnityTest]
        public IEnumerator CinemachineVsCamera()
        {
            player.Camera.UseCinemachine = false;
            player.Camera.TargetVirtualCamera = null;
            player.Camera.FieldOfViewKick.ForceFOV(true);

            yield return null;

            GameObject virtualCamera = new GameObject("", typeof(Cinemachine.CinemachineVirtualCamera));

            player.Camera.UseCinemachine = true;
            player.Camera.TargetCamera = null;
            player.Camera.TargetVirtualCamera = virtualCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            player.Camera.FieldOfViewKick.ForceFOV(true);
        }
#endif

        [UnityTest]
        public IEnumerator CheckCameraNull()
        {
            player.Camera.TargetCamera = null;
#if GOLD_PLAYER_CINEMACHINE
            player.Camera.TargetVirtualCamera = null;
#endif
            player.Camera.FieldOfViewKick.EnableFOVKick = true;

            player.Camera.FieldOfViewKick.ForceInitialize(null);
            LogAssert.Expect(LogType.Error, "There's no camera set on field of view kick!");
            yield return null;
        }

        [UnityTest]
        public IEnumerator UninitializedTest()
        {
            FOVKickClass kick = new FOVKickClass();
            kick.INTERNAL__ForceHandleFOV();
            LogAssert.Expect(LogType.Error, "You need to call 'Initialize()' on your FOV kick before using it!");
            yield return null;
        }

        [UnityTest]
        public IEnumerator UpdateFOVNullCamera()
        {
            player.Camera.TargetCamera = null;
            
            FOVKickClass kick = new FOVKickClass
            {
                newFOV = 70,
                camera = player.Camera
            };
            kick.INTERNAL__UpdateNewFOV();
            Assert.AreEqual(kick.newFOV, 70);
            yield return null;
        }

        [UnityTest]
        public IEnumerator KickWhenNone()
        {
            float fov = player.Camera.TargetCamera.fieldOfView;
            player.Camera.FieldOfViewKick.KickWhen = RunAction.None;
            player.Camera.FieldOfViewKick.LerpTimeTo = 100;
            player.Camera.FieldOfViewKick.LerpTimeFrom = 100;
            input.moveDirection = new Vector2(0, 1);
            input.isRunning = true;

            for (int i = 0; i < 60; i++)
            {
                yield return null;
            }

            Assert.AreEqual(fov, player.Camera.TargetCamera.fieldOfView);
        }

        [UnityTest]
        public IEnumerator KickWhenRunning()
        {
            player.Camera.FieldOfViewKick.KickWhen = RunAction.IsRunning;
            player.Camera.FieldOfViewKick.LerpTimeTo = 100;
            player.Camera.FieldOfViewKick.LerpTimeFrom = 100;
            input.moveDirection = new Vector2(0, 1);
            input.isRunning = true;

            for (int i = 0; i < 60; i++)
            {
                yield return null;
            }

            Assert.AreApproximatelyEqual(player.Camera.TargetCamera.fieldOfView, player.Camera.FieldOfViewKick.TargetFieldOfView, 0.05f);

            input.isRunning = false;

            for (int i = 0; i < 60; i++)
            {
                yield return null;
            }

            Assert.AreApproximatelyEqual(player.Camera.TargetCamera.fieldOfView, player.Camera.FieldOfViewKick.originalFOV, 0.05f);
        }

        [UnityTest]
        public IEnumerator KickWhenPressingRun()
        {
            player.Camera.FieldOfViewKick.KickWhen = RunAction.PressingRun;
            player.Camera.FieldOfViewKick.LerpTimeTo = 100;
            player.Camera.FieldOfViewKick.LerpTimeFrom = 100;
            input.moveDirection = new Vector2(0, 0.1f);
            input.isRunning = true;

            for (int i = 0; i < 60; i++)
            {
                yield return null;
            }

            Assert.IsFalse(player.Movement.IsRunning);
            Assert.AreApproximatelyEqual(player.Camera.TargetCamera.fieldOfView, player.Camera.FieldOfViewKick.TargetFieldOfView, 0.05f);

            input.isRunning = false;

            for (int i = 0; i < 60; i++)
            {
                yield return null;
            }

            Assert.AreApproximatelyEqual(player.Camera.TargetCamera.fieldOfView, player.Camera.FieldOfViewKick.originalFOV, 0.05f);
        }

        [UnityTest]
        public IEnumerator OnValidateTest()
        {
            player.Camera.TargetCamera.fieldOfView = 90;
            player.Camera.FieldOfViewKick.OnValidate();
            Assert.AreEqual(player.Camera.FieldOfViewKick.TargetFieldOfView, 90 + player.Camera.FieldOfViewKick.KickAmount);
            yield return null;
        }
    }
}
