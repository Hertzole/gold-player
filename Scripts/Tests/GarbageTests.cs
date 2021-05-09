#if GOLD_PLAYER_TESTS && UNITY_2018_3_OR_NEWER
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace Hertzole.GoldPlayer.Tests
{
    internal class GarbageTests : BaseGoldPlayerTest
    {
        /// <summary>
        /// Used to test if sphere ground check generates garbage.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator GroundCheckSphereNoGC()
        {
            player.Movement.GroundCheck = GroundCheckType.Sphere;
            yield return null;

            for (int i = 0; i < 60; i++)
            {
                Assert.That(() =>
                {
                    player.Movement.CheckGrounded();
                }, Is.Not.AllocatingGCMemory());
                yield return null;
            }
        }

        /// <summary>
        /// Used to test if raycast ground check generates garbage.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator GroundCheckRaycastNoGC()
        {
            player.Movement.GroundCheck = GroundCheckType.Raycast;
            yield return null;

            for (int i = 0; i < 60; i++)
            {
                Assert.That(() =>
                {
                    player.Movement.CheckGrounded();
                }, Is.Not.AllocatingGCMemory());
                yield return null;
            }
        }
    }
}
#endif
