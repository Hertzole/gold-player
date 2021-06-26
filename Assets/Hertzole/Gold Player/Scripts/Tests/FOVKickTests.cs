#if GOLD_PLAYER_TESTS
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
            player.Camera.FieldOfViewKick.LerpTimeTo = 0f;

            for (int i = 0; i < 5; i++)
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
                PlayerController = player
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
            player.Camera.FieldOfViewKick.LerpTimeTo = 0;
            player.Camera.FieldOfViewKick.LerpTimeFrom = 0;
            input.moveDirection = new Vector2(0, 1);
            input.isRunning = true;

            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }

            Assert.AreEqual(fov, player.Camera.TargetCamera.fieldOfView);
        }

        [UnityTest]
        public IEnumerator KickWhenRunning()
        {
            player.Camera.FieldOfViewKick.KickWhen = RunAction.IsRunning;
            input.moveDirection = new Vector2(0, 1);

            yield return RunTimeScaleTest(Test(), Test());

            IEnumerator Test()
            {
                player.Camera.FieldOfViewKick.LerpTimeTo = 0;
                player.Camera.FieldOfViewKick.LerpTimeFrom = 0;
            
                yield return InnerTest();
            
                player.Camera.FieldOfViewKick.LerpTimeTo = 0.1f;
                player.Camera.FieldOfViewKick.LerpTimeFrom = 0.1f;

                yield return InnerTest();

                IEnumerator InnerTest()
                {
                    input.isRunning = true;

                    yield return new WaitForSecondsRealtime(0.5f);

                    Assert.AreApproximatelyEqual(player.Camera.TargetCamera.fieldOfView, player.Camera.FieldOfViewKick.TargetFieldOfView, 0.05f);

                    input.isRunning = false;

                    yield return new WaitForSecondsRealtime(0.5f);

                    Assert.AreApproximatelyEqual(player.Camera.TargetCamera.fieldOfView, player.Camera.FieldOfViewKick.originalFOV, 0.05f);
                }
            }
        }

        [UnityTest]
        public IEnumerator KickWhenPressingRun()
        {
            player.Movement.Acceleration = 0;
            player.Camera.FieldOfViewKick.KickWhen = RunAction.PressingRun;
            input.moveDirection = new Vector2(0, 0.1f);

            yield return RunTimeScaleTest(Test(), Test());

            IEnumerator Test()
            {
                player.Camera.FieldOfViewKick.LerpTimeTo = 0f;
                player.Camera.FieldOfViewKick.LerpTimeFrom = 0f;

                yield return InnerTest();
            
                player.Camera.FieldOfViewKick.LerpTimeTo = 0.1f;
                player.Camera.FieldOfViewKick.LerpTimeFrom = 0.1f;
        
                yield return InnerTest();

                IEnumerator InnerTest()
                {
                    input.isRunning = true;
                
                    yield return new WaitForSecondsRealtime(0.5f);

                    Assert.IsFalse(player.Movement.IsRunning, "Player was running");
                    Assert.AreApproximatelyEqual(player.Camera.TargetCamera.fieldOfView, player.Camera.FieldOfViewKick.TargetFieldOfView, 0.05f, "Field of view was not kicked");

                    input.isRunning = false;

                    yield return new WaitForSecondsRealtime(0.5f);

                    Assert.IsFalse(player.Movement.IsRunning);
                    Assert.AreApproximatelyEqual(player.Camera.TargetCamera.fieldOfView, player.Camera.FieldOfViewKick.originalFOV, 0.05f);
                }
            }
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
#endif