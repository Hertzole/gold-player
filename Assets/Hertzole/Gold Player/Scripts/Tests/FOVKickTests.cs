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
            float originalFOV = player.Camera.FieldOfViewKick.TargetCamera.fieldOfView;
            player.Camera.FieldOfViewKick.EnableFOVKick = false;

            for (int i = 0; i < 60; i++)
            {
                player.Camera.FieldOfViewKick.ForceFOV(true);
                yield return null;
                Assert.AreApproximatelyEqual(originalFOV, player.Camera.FieldOfViewKick.TargetCamera.fieldOfView);
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
            Assert.AreApproximatelyEqual(targetFOV, player.Camera.FieldOfViewKick.TargetCamera.fieldOfView, 1f);
        }

#if GOLD_PLAYER_CINEMACHINE
        [UnityTest]
        public IEnumerator CinemachineVsCamera()
        {
            player.Camera.FieldOfViewKick.UseCinemachine = false;
            player.Camera.FieldOfViewKick.TargetVirtualCamera = null;
            player.Camera.FieldOfViewKick.ForceFOV(true);

            yield return null;

            GameObject virtualCamera = new GameObject("", typeof(Cinemachine.CinemachineVirtualCamera));

            player.Camera.FieldOfViewKick.UseCinemachine = true;
            player.Camera.FieldOfViewKick.TargetCamera = null;
            player.Camera.FieldOfViewKick.TargetVirtualCamera = virtualCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            player.Camera.FieldOfViewKick.ForceFOV(true);
        }
#endif
    }
}
