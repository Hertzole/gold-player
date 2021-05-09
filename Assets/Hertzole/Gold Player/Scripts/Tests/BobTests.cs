#if GOLD_PLAYER_TESTS
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Hertzole.GoldPlayer.Tests
{
    internal class BobTests : BaseGoldPlayerTest
    {
        private PlayerBob Bob { get { return player.HeadBob; } }

        [UnityTest]
        public IEnumerator SetPropertiesTest()
        {
            ArePropertyAndFieldSame(Bob.EnableBob, Bob.BobClass.enableBob);
            ArePropertyAndFieldSame(Bob.UnscaledTime, Bob.BobClass.unscaledTime);
            ArePropertyAndFieldSame(Bob.BobFrequency, Bob.BobClass.bobFrequency);
            ArePropertyAndFieldSame(Bob.BobHeight, Bob.BobClass.bobHeight);
            ArePropertyAndFieldSame(Bob.SwayAngle, Bob.BobClass.swayAngle);
            ArePropertyAndFieldSame(Bob.SideMovement, Bob.BobClass.sideMovement);
            ArePropertyAndFieldSame(Bob.HeightMultiplier, Bob.BobClass.heightMultiplier);
            ArePropertyAndFieldSame(Bob.StrideMultiplier, Bob.BobClass.strideMultiplier);
            ArePropertyAndFieldSame(Bob.LandMove, Bob.BobClass.landMove);
            ArePropertyAndFieldSame(Bob.LandTilt, Bob.BobClass.landTilt);
            ArePropertyAndFieldSame(Bob.EnableStrafeTilting, Bob.BobClass.enableStrafeTilting);
            ArePropertyAndFieldSame(Bob.StrafeTilt, Bob.BobClass.strafeTilt);
            ArePropertyAndFieldSame(Bob.BobTarget, Bob.BobClass.bobTarget);

            yield return null;
        }

        [UnityTest]
        public IEnumerator NoTargetTest()
        {
            Vector3 tempPos = new Vector3(999, 999, 999);
            Bob.BobClass.OriginalHeadLocalPosition = tempPos;
            Bob.BobTarget = null;
            Assert.IsNull(Bob.BobTarget);
            AreApproximatelyEqualVector3(tempPos, Bob.BobClass.OriginalHeadLocalPosition);
            Bob.BobClass.Initialize();
            LogAssert.Expect(LogType.Error, "No Bob Target set!");
            AreApproximatelyEqualVector3(Vector3.zero, Bob.BobClass.OriginalHeadLocalPosition);

            Bob.ForceInitialize(null);
            LogAssert.Expect(LogType.Error, "No Bob Target set on '" + player.gameObject.name + "'!");

            yield return null;
        }

        [UnityTest]
        public IEnumerator HeadLocationTest()
        {
            Vector3 tempPos = new Vector3(999, 999, 999);
            Bob.BobClass.OriginalHeadLocalPosition = tempPos;
            Bob.BobTarget = null;
            Assert.IsNull(Bob.BobTarget);
            AreApproximatelyEqualVector3(tempPos, Bob.BobClass.OriginalHeadLocalPosition);
            Bob.BobTarget = player.transform.GetChild(0);
            Bob.BobClass.Initialize();
            AreApproximatelyEqualVector3(new Vector3(0, 1.6f, 0), Bob.BobClass.OriginalHeadLocalPosition, 0.1f);

            yield return null;
        }

        [UnityTest]
        public IEnumerator DeltaTimeTest()
        {
            player.UnscaledTime = true;
            player.HeadBob.UnscaledTime = false;

            Time.timeScale = 0;

            input.moveDirection = new Vector2(0, 1);

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(0, Bob.BobCycle);
                yield return null;
            }

            player.SetPosition(Vector3.zero);
            player.UnscaledTime = true;

            for (int i = 0; i < 100; i++)
            {
                yield return null;
                Assert.IsTrue(Bob.BobCycle > 0);
            }
        }

        [UnityTest]
        public IEnumerator StrafeTest()
        {
            Bob.EnableStrafeTilting = true;
            input.moveDirection = new Vector2(-1f, 0);

            for (int i = 0; i < 100; i++)
            {
                yield return null;
            }

            Assert.IsTrue(Bob.BobTarget.localEulerAngles.z > 0);

            input.moveDirection = new Vector2(0f, 0f);

            for (int i = 0; i < 100; i++)
            {
                yield return null;
            }
            AreApproximatelyEqualQuaternion(Quaternion.identity, Quaternion.Euler(Bob.BobTarget.localEulerAngles), false, 0.05f);

            Bob.EnableStrafeTilting = false;
            input.moveDirection = new Vector2(-1f, 0);

            for (int i = 0; i < 100; i++)
            {
                yield return null;
            }

            AreApproximatelyEqualQuaternion(Quaternion.identity, Quaternion.Euler(Bob.BobTarget.localEulerAngles), false, 0.05f);
        }

        [UnityTest]
        public IEnumerator NaNTest()
        {
            input.moveDirection = new Vector2(0, 1);
            for (int i = 0; i < 30; i++)
            {
                yield return null;
            }

            Assert.IsTrue(player.HeadBob.BobClass.bobCycle > 0);
            float backup = player.HeadBob.BobClass.bobCycleBackup;
            Time.timeScale = 0;
            for (int i = 0; i < 30; i++)
            {
                yield return null;
            }
            Time.timeScale = 1;
            yield return null;
            Assert.AreEqual(player.HeadBob.BobClass.bobCycle, player.HeadBob.BobClass.bobCycleBackup);
        }
    }
}
#endif