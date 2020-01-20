#if UNITY_2018_3_OR_NEWER
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
            WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
            for (int i = 0; i < 60; i++)
            {
                yield return frameEnd;
            }
            Assert.That(() =>
            {
                player.Update();
                player.LateUpdate();
                player.FixedUpdate();
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
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            GoldPlayerInputSystem input = playerGO.AddComponent<GoldPlayerInputSystem>();
            input.InputAsset = UnityEngine.InputSystem.InputActionAsset.FromJson(inputJson);
            input.EnableInput();
#else
            GoldPlayerInput input = playerGO.AddComponent<GoldPlayerInput>();
#endif
            playerController.GetComponent<CharacterController>().center = new Vector3(0, 1, 0);
            playerController.Camera.CameraHead = playerCameraHead.transform;
            playerController.Camera.FieldOfViewKick.EnableFOVKick = false;
            playerController.HeadBob.BobTarget = playerCameraHead.transform;

            playerController.Movement.MovingPlatforms.Enabled = true;
            playerController.Movement.MovingPlatforms.Initialize(playerController, input);

            //GameObject raycastWeaponGO = new GameObject("[Test] Raycast Weapon");
            //GoldPlayerWeapon raycastWeapon = raycastWeaponGO.AddComponent<GoldPlayerWeapon>();
            //raycastWeapon.ProjectileType = GoldPlayerWeapon.ProjectileTypeEnum.Raycast;
            //raycastWeapon.ShootOrigin = playerCameraHead.transform;

            //GoldPlayerWeapons weapons = playerGO.AddComponent<GoldPlayerWeapons>();
            //weapons.AvailableWeapons = new GoldPlayerWeapon[] { raycastWeapon };
            //weapons.AddWeapon(raycastWeapon);

            return playerController;
        }

        //private readonly string inputJK
        private readonly string inputJson = @"{
    ""name"": ""Example Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""d5016638-3003-48c1-aed0-3e9fa4352611"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""f85cfb3b-2bc3-4e87-ad97-cb579ffc3b3b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Button"",
                    ""id"": ""8b6bb957-5f78-40b6-adf3-c0ca449e44cc"",
                    ""expectedControlType"": """",
                    ""processors"": ""ScaleVector2(x=0.1,y=0.1)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""63f0c474-d1ac-4daa-9210-e2767da09a93"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""ef8186f6-a255-4c67-9ca2-9451e562a281"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""d2a5329e-8e7e-4382-9eaf-eb384832dfd3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""6428c99f-55be-403f-bd6b-1070cdbc44db"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9bc8ec56-ae7e-42b2-a024-dbdec660110b"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""92d9e2ff-418e-477b-a2a0-2920247b6931"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""4f2b02eb-5ba1-44bf-84a0-4b63bb2260c2"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""51201f6d-d7d0-4fc8-9ad8-be4a3e9f18a0"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""efb07998-c290-442f-b2e2-9d6dfcf9a3a8"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""58696137-6fbd-48c8-b2d3-b97d45e656b3"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""36db6508-e8eb-4e19-b156-2d2665983c48"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9cedc362-1bee-48b5-935c-79ea9e619298"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""79441df5-e92c-4bd4-ab4f-cf55ae2ac567"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3ce60a52-151e-4295-aa13-75406fa09020"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1ae0baf1-de9b-4de8-9195-f43c27aeed09"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}";
    }
}
#endif
