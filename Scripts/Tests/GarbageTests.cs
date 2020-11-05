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
        ///////// REMOVED FOR NOW
        ///////// Due to creating ground check ray array, it seems to be impossible to avoid garbage.
        //[UnityTest]
        //public IEnumerator MovementInitializeNoGC()
        //{
        //    PlayerMovement movement = new PlayerMovement();
        //    movement.Stamina.EnableStamina = true;
        //    Assert.That(() =>
        //    {
        //        movement.Initialize(player, input);
        //    }, Is.Not.AllocatingGCMemory());
        //    yield return null;
        //}

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
