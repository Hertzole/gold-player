#if UNITY_2018_3_OR_NEWER
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace Hertzole.GoldPlayer.Tests
{
    internal class GarbageTests : BaseGoldPlayerTest
    {
        /// <summary>
        /// Used to test if PlayerCamera generates garbage on initialize.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CameraInitializeNoGC()
        {
            PlayerCamera camera = new PlayerCamera
            {
                CameraHead = player.Camera.CameraHead,
            };
            camera.FieldOfViewKick.EnableFOVKick = true;
            camera.FieldOfViewKick.TargetCamera = player.Camera.FieldOfViewKick.TargetCamera;
            Assert.That(() =>
            {
                camera.Initialize(player, input);
            }, Is.Not.AllocatingGCMemory());
            yield return null;
        }

        /// <summary>
        /// Used to test if PlayerMovement generates garbage on initialize.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator MovementInitializeNoGC()
        {
            PlayerMovement movement = new PlayerMovement();
            movement.Stamina.EnableStamina = true;
            Assert.That(() =>
            {
                movement.Initialize(player, input);
            }, Is.Not.AllocatingGCMemory());
            yield return null;
        }

        /// <summary>
        /// Used to test if PlayerBob generates garbage on initialize.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator BobInitializeNoGC()
        {
            PlayerBob bob = new PlayerBob
            {
                BobTarget = player.HeadBob.BobTarget
            };
            Assert.That(() =>
            {
                bob.Initialize(player, input);
            }, Is.Not.AllocatingGCMemory());
            yield return null;
        }

        /// <summary>
        /// Used to test if PlayerAudio generates garbage on initialize.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator AudioInitializeNoGC()
        {
            PlayerAudio audio = new PlayerAudio();
            Assert.That(() =>
            {
                audio.Initialize(player, input);
            }, Is.Not.AllocatingGCMemory());
            yield return null;
        }
    }
}
#endif
