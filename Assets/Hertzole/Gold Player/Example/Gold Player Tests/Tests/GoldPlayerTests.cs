#if UNITY_2018_3_OR_NEWER
using Hertzole.GoldPlayer.Core;
//using Hertzole.GoldPlayer.Weapons;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace Hertzole.GoldPlayer.Tests
{
    public class GoldPlayerTests
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator MovingPlatformGeneratesGarbage()
        {
            // Use the Assert class to test conditions
            GoldPlayerController player = SetupPlayer();
            yield return null;

            Assert.That(() =>
            {
                player.Movement.MovingPlatforms.OnUpdate(Time.deltaTime);
            }, Is.Not.AllocatingGCMemory());
        }

        //[UnityTest]
        //public IEnumerator WeaponsShootsGeneratesGarbage()
        //{
        //    // Use the Assert class to test conditions
        //    GoldPlayerController player = SetupPlayer();
        //    yield return null;

        //    Assert.That(() =>
        //    {
        //        player.GetComponent<GoldPlayerWeapons>().CurrentWeapon.PrimaryAttack();
        //    }, Is.Not.AllocatingGCMemory());
        //}

        [UnityTest]
        public IEnumerator PlayerFollowsMovingPlatform()
        {
            // Use the Assert class to test conditions
            GoldPlayerController player = SetupPlayer();
            GameObject platform = new GameObject("Platform");
            platform.AddComponent<BoxCollider>();
            platform.tag = "Respawn";
            player.transform.position = platform.transform.position + new Vector3(0, 0.5f, 0);
            yield return null;

            Assert.AreSame(player.transform.parent, platform.transform);
            yield return null;
            platform.transform.position = new Vector3(10, 0, 10);
            yield return null;
            Assert.AreEqual(new Vector3(player.transform.position.x, 0, player.transform.position.z), new Vector3(platform.transform.position.x, 0, platform.transform.position.z));
        }

        [UnityTest]
        public IEnumerator GoldPlayerInitializeGeneratesGarbage()
        {
            GoldPlayerController player = SetupPlayer();
            player.InitOnStart = false;
            Assert.That(() =>
            {
                player.Initialize();
            }, Is.Not.AllocatingGCMemory());
            yield return null;
        }

        [UnityTest]
        public IEnumerator GoldPlayerUpdateGeneratesGarbage()
        {
            GoldPlayerController player = SetupPlayer();
            yield return null;
            Assert.That(() =>
            {
                player.Update();
            }, Is.Not.AllocatingGCMemory());
        }

        [UnityTest]
        public IEnumerator FOVKickGeneratesGarbage()
        {
            Transform camera = new GameObject("[TEST] Camera").transform;
            Camera cam = camera.gameObject.AddComponent<Camera>();
            FOVKickClass fov = new FOVKickClass
            {
                TargetCamera = cam,
                EnableFOVKick = true
            };

            Assert.That(() =>
            {
                fov.Initialize(null, null);
            }, Is.Not.AllocatingGCMemory());

            yield return null;
        }

        [UnityTest]
        public IEnumerator InitializeMovingPlatformsGeneratesNoGarbage()
        {
            GoldPlayerController player = SetupPlayer();
            MovingPlatformsClass movingPlatforms = new MovingPlatformsClass();

            Assert.That(() =>
            {
                movingPlatforms.Initialize(player, null);
            }, Is.Not.AllocatingGCMemory());

            yield return null;
        }

        private GoldPlayerController SetupPlayer()
        {
            GameObject playerGO = new GameObject("[TEST] Test Player");
            GoldPlayerController playerController = playerGO.AddComponent<GoldPlayerController>();
            GameObject playerCameraHead = new GameObject("[TEST] Test Player Camera Head");
            Camera camera = playerCameraHead.AddComponent<Camera>();
            playerController.GetComponent<CharacterController>().center = new Vector3(0, 1, 0);
            playerController.Camera.CameraHead = playerCameraHead.transform;
            playerController.Camera.FieldOfViewKick.EnableFOVKick = false;
            playerController.HeadBob.BobTarget = playerCameraHead.transform;

            playerController.Movement.MovingPlatforms.Enabled = true;
            playerController.Movement.MovingPlatforms.PlatformTags = new string[] { "Respawn" };
            playerController.Movement.MovingPlatforms.Initialize(playerController);

            //GameObject raycastWeaponGO = new GameObject("[Test] Raycast Weapon");
            //GoldPlayerWeapon raycastWeapon = raycastWeaponGO.AddComponent<GoldPlayerWeapon>();
            //raycastWeapon.ProjectileType = GoldPlayerWeapon.ProjectileTypeEnum.Raycast;
            //raycastWeapon.ShootOrigin = playerCameraHead.transform;

            //GoldPlayerWeapons weapons = playerGO.AddComponent<GoldPlayerWeapons>();
            //weapons.AvailableWeapons = new GoldPlayerWeapon[] { raycastWeapon };
            //weapons.AddWeapon(raycastWeapon);

            return playerController;
        }
    }
}
#endif
