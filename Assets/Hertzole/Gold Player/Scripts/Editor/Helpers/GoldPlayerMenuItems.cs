using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#if GOLD_PLAYER_CINEMACHINE
using Cinemachine;
#endif
#if UNITY_2018_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

namespace Hertzole.GoldPlayer.Editor
{
	public static class GoldPlayerMenuItems
	{
		private const string STEP1_GUID = "44e59b73420f0e64fb30e755c49b3823";
		private const string STEP2_GUID = "62e6bf700540ea9478b91b33afa85e7e";
		private const string STEP3_GUID = "bbe4ed33245f5804db42ff8c43d75412";
		private const string STEP4_GUID = "1e5761292ed025241bdb02ff49c552dd";
		private const string JUMP_GUID = "11cc82c4daa9f304dbbfab62c9207608";
		private const string LAND_GUID = "ceb0f9d730c213441b4eaccb59f6fcde";

		[MenuItem("GameObject/3D Object/Gold Player Controller")]
		public static void CreateGoldPlayer(MenuCommand menuCommand)
		{
			GameObject parent = menuCommand.context as GameObject;
			string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent != null ? parent.transform : null, "Gold Player Controller");

			Undo.IncrementCurrentGroup();

			GameObject root = new GameObject(uniqueName) { tag = "Player", layer = LayerMask.NameToLayer("TransparentFX") };
			Undo.RegisterCreatedObjectUndo(root, "Create test");

			PlaceInScene(root, parent);

			GameObject graphicsHolder = CreateChild("Graphics", root);

			GameObject graphic = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			Undo.RegisterCreatedObjectUndo(graphic, "Create graphic");

			// Set up the graphic.
			Undo.RecordObject(graphic, "Set graphic properties");
			graphic.name = "Capsule";
			graphic.layer = root.layer;

			Undo.SetTransformParent(graphic.transform, graphicsHolder.transform, false, "Set graphics parent");

			SetObjectTransform(graphic.transform, Vector3.up, null, new Vector3(0.8f, 1f, 0.8f));

			// Remove any colliders, if they are present.
			Collider graphicsCollider = graphic.GetComponent<Collider>();
			if (graphicsCollider != null)
			{
				Undo.DestroyObjectImmediate(graphicsCollider);
			}

			// Create the camera head.
			GameObject cameraHead = CreateChild("Camera Head", root);
			SetObjectTransform(cameraHead.transform, new Vector3(0f, 1.6f, 0f), null, null);

			// Create the bob target and set the position.
			GameObject bobTarget = CreateChild("Bob Target", cameraHead);

			// Create the player camera.
#if GOLD_PLAYER_CINEMACHINE_3
			GameObject playerCameraGo = CreateChild("Player Camera", bobTarget, typeof(CinemachineCamera));
			CinemachineCamera playerCamera = playerCameraGo.GetComponent<CinemachineCamera>();
			playerCamera.Lens.FieldOfView = 80;
			playerCamera.Lens.NearClipPlane = 0.01f;
			playerCamera.Lens.FarClipPlane = 1000f;
			
			CreateCinemachineBrainIfNeeded();
#elif GOLD_PLAYER_CINEMACHINE
			GameObject playerCameraGo = CreateChild("Player Camera", bobTarget, typeof(CinemachineVirtualCamera));
			CinemachineVirtualCamera playerCamera = playerCameraGo.GetComponent<CinemachineVirtualCamera>();

			// We need to add and remove a component to make sure the component pipeline is updated.
			// Otherwise we won't be able to undo.
			CinemachineComposer composer = Undo.AddComponent<CinemachineComposer>(playerCamera.GetComponentOwner().gameObject);
			Undo.DestroyObjectImmediate(composer);
			playerCamera.InvalidateComponentPipeline();
			Undo.RecordObject(playerCamera, "Set camera properties");
			playerCamera.m_Lens.FieldOfView = 80;
			playerCamera.m_Lens.NearClipPlane = 0.01f;
			playerCamera.m_Lens.FarClipPlane = 1000f;

			CreateCinemachineBrainIfNeeded();
#else
            GameObject playerCameraGo = CreateChild("Player Camera", bobTarget, typeof(Camera));
            Camera playerCamera = playerCameraGo.GetComponent<Camera>();
            
            Undo.RecordObject(playerCamera, "Set camera properties");
            playerCamera.clearFlags = CameraClearFlags.Skybox;
            playerCamera.fieldOfView = 80;
            playerCamera.nearClipPlane = 0.01f;
            playerCamera.farClipPlane = 1000f;
            playerCamera.tag = "MainCamera";
            Undo.AddComponent<FlareLayer>(playerCameraGo);
            Undo.AddComponent<AudioListener>(playerCameraGo);
#endif

			// Create the audio object.
			GameObject audioGo = CreateChild("Audio", root);

			AudioSource stepsSource = Undo.AddComponent<AudioSource>(audioGo);
			AudioSource jumpSource = Undo.AddComponent<AudioSource>(audioGo);
			AudioSource landSource = Undo.AddComponent<AudioSource>(audioGo);

			// Add the character controller and set it up.
			CharacterController characterController = Undo.AddComponent<CharacterController>(root);
			characterController.radius = 0.4f;
			characterController.height = 2;
			characterController.center = new Vector3(0, 1, 0);

			// Add the actual Gold Player Controller and set it up.
			GoldPlayerController goldController = Undo.AddComponent<GoldPlayerController>(root);

			Undo.RecordObject(goldController, "Set Gold Player Controller properties");
			goldController.Camera.CameraHead = cameraHead.transform;
#if GOLD_PLAYER_CINEMACHINE
			goldController.Camera.UseCinemachine = true;
			goldController.Camera.TargetVirtualCamera = playerCamera;
#else
            goldController.Camera.TargetCamera = playerCamera;
#endif

			goldController.Movement.GroundLayer = 1;

			goldController.HeadBob.BobTarget = bobTarget.transform;

			goldController.Audio.FootstepsSource = stepsSource;
			goldController.Audio.JumpSource = jumpSource;
			goldController.Audio.LandSource = landSource;

			AudioClip step1 = GetAsset<AudioClip>(STEP1_GUID);
			AudioClip step2 = GetAsset<AudioClip>(STEP2_GUID);
			AudioClip step3 = GetAsset<AudioClip>(STEP3_GUID);
			AudioClip step4 = GetAsset<AudioClip>(STEP4_GUID);
			AudioClip jump = GetAsset<AudioClip>(JUMP_GUID);
			AudioClip land = GetAsset<AudioClip>(LAND_GUID);

			AudioItem walkSounds = CreateAudioItem(true, true, 1f, 0.9f, 1.1f, true, 1f, step1, step2, step3, step4);
			AudioItem runSounds = CreateAudioItem(true, true, 1.4f, 1.4f, 1.6f, true, 1f, step1, step2, step3, step4);
			AudioItem crouchSounds = CreateAudioItem(true, true, 1f, 0.9f, 1.1f, true, 0.4f, step1, step2, step3, step4);
			AudioItem jumpSound = CreateAudioItem(true, true, 1f, 0.9f, 1.1f, true, 1f, jump);
			AudioItem landSound = CreateAudioItem(true, true, 1f, 0.9f, 1.1f, true, 1f, land);

			goldController.Audio.WalkFootsteps = walkSounds;
			goldController.Audio.RunFootsteps = runSounds;
			goldController.Audio.CrouchFootsteps = crouchSounds;
			goldController.Audio.Jumping = jumpSound;
			goldController.Audio.Landing = landSound;

#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
			Undo.AddComponent<GoldPlayerInputSystem>(root);
#else
            Undo.AddComponent<GoldPlayerInput>(root);
#endif

			Undo.SetCurrentGroupName("Created Gold Player Controller");
		}

#if GOLD_PLAYER_CINEMACHINE
		private static void CreateCinemachineBrainIfNeeded()
		{
			#if UNITY_2023_1_OR_NEWER
			CinemachineBrain[] brains = Object.FindObjectsByType<CinemachineBrain>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
			CinemachineBrain[] brains = Object.FindObjectsOfType<CinemachineBrain>();
#endif
			CinemachineBrain brain = brains != null && brains.Length > 0 ? brains[0] : null;
			if (brain == null)
			{
				Camera cam = Camera.main;
				if (cam == null)
				{
#if UNITY_2023_1_OR_NEWER
					Camera[] cams = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
					Camera[] cams = Object.FindObjectsOfType<Camera>();
#endif

					if (cams != null && cams.Length > 0)
					{
						cam = cams[0];
					}
				}

				if (cam != null)
				{
					Undo.AddComponent<CinemachineBrain>(cam.gameObject);
				}
			}
		}
#endif

		private static void SetObjectTransform(Transform transform, Vector3? position, Vector3? rotation, Vector3? scale)
		{
			Undo.RegisterFullObjectHierarchyUndo(transform, "Set object transform");
			if (position != null)
			{
				transform.localPosition = position.Value;
			}

			if (rotation != null)
			{
				transform.localEulerAngles = rotation.Value;
			}

			if (scale != null)
			{
				transform.localScale = scale.Value;
			}
		}

		private static GameObject CreateChild(string name, GameObject parent, params Type[] components)
		{
			GameObject go = ObjectFactory.CreateGameObject(name, components);
			Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
			Undo.RecordObject(go, "Modify " + go.name);
			go.layer = parent.layer;
			Undo.SetTransformParent(go.transform, parent.transform, false, "Create " + go.name);
			return go;
		}

		private static T GetAsset<T>(string guid) where T : Object
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<T>(path);
		}

		private static AudioItem CreateAudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch, bool changeVol, float vol, params AudioClip[] clips)
		{
			AudioItem item = new AudioItem(enabled, randomPitch, pitch, minPitch, maxPitch, changeVol, vol);
			int clipSize = 0;
			for (int i = 0; i < clips.Length; i++)
			{
				if (clips[i] != null)
				{
					clipSize++;
				}
			}

			item.AudioClips = new AudioClip[clipSize];
			int index = 0;
			for (int i = 0; i < clips.Length; i++)
			{
				if (clips[i] != null)
				{
					item.AudioClips[index] = clips[i];
					index++;
				}
			}

			return item;
		}

		/*
		 * Borrowed from
		 * https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Commands/GOCreationCommands.cs#L15
		 * */
		private static void PlaceInScene(GameObject go, GameObject parent)
		{
			if (parent != null)
			{
				Transform transform = go.transform;
				Undo.SetTransformParent(transform, parent.transform, "Reparenting");
				Undo.RegisterFullObjectHierarchyUndo(transform, "Resetting position");
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
			}
			else
			{
				PlaceGameObjectInFrontOfSceneView(go);
#if UNITY_2018_3_OR_NEWER
				StageUtility.PlaceGameObjectInCurrentStage(go);
#endif
			}

			Selection.activeGameObject = go;
		}

		/*
		 * Borrowed from
		 * https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/SceneView/SceneView.cs#L641
		 * */
		private static void PlaceGameObjectInFrontOfSceneView(GameObject go)
		{
			SceneView view = SceneView.currentDrawingSceneView;
			if (!view)
			{
				view = SceneView.sceneViews[0] as SceneView;
			}

			if (view)
			{
				view.MoveToView(go.transform);
			}
		}
	}
}