using Hertzole.GoldPlayer.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
    [RequireComponent(typeof(CharacterController))]
    [DisallowMultipleComponent]
    [AddComponentMenu("Gold Player/Gold Player Controller", 01)]
    public class GoldPlayerController : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("m_Camera")]
        private new PlayerCamera camera = new PlayerCamera();
        [SerializeField]
        [FormerlySerializedAs("m_Movement")]
        private PlayerMovement movement = new PlayerMovement();
        [SerializeField]
        [FormerlySerializedAs("m_HeadBob")]
        private PlayerBob headBob = new PlayerBob();
        [SerializeField]
        [FormerlySerializedAs("m_Audio")]
        private new PlayerAudio audio = new PlayerAudio();

        [SerializeField]
#if !ENABLE_INPUT_SYSTEM || !UNITY_2019_3_OR_NEWER
        [HideInInspector]
#endif
        [Tooltip("The main action map for the Input Actions.")]
        private string actionMap = "Player";

        private bool initOnStart = true;
        protected bool hasBeenInitialized = false;

        private IGoldInput playerInput;
        private CharacterController controller;

        /// <summary> Has all the scripts be initialized? </summary>
        [System.Obsolete("Use HasBeenFullyInitialized instead.")]
        public bool HasBeenInitialized { get { return HasBeenFullyInitialized; } }

        /// <summary> True if all the modules have been initialized. </summary>
        public bool HasBeenFullyInitialized
        {
            get { return camera.HasBeenInitialized && movement.HasBeenInitialized && headBob.HasBeenInitialized && audio.HasBeenInitialized; }
        }
        /// <summary> If false, 'Initialize()' will not be called on Start and will only be called once another script calls it. </summary>
        public bool InitOnStart { get { return initOnStart; } set { initOnStart = value; } }

        /// <summary> Everything related to the player camera (mouse movement). </summary>
        public PlayerCamera Camera { get { return camera; } set { camera = value; } }
        /// <summary> Everything related to movement. </summary>
        public PlayerMovement Movement { get { return movement; } set { movement = value; } }
        /// <summary> Everything related to the head bob. </summary>
        public PlayerBob HeadBob { get { return headBob; } set { headBob = value; } }
        /// <summary> Everything related to audio (footsteps, landing and jumping). </summary>
        public PlayerAudio Audio { get { return audio; } set { audio = value; } }

        /// <summary> The main action map for the Input Actions. </summary>
        public string ActionMap
        {
            get
            {
#if !ENABLE_INPUT_SYSTEM || !UNITY_2019_3_OR_NEWER
                Debug.LogWarning("GoldPlayerController.ActionMap is useless when not using the new Input System.");
#endif
                return actionMap;
            }
            set
            {
#if !ENABLE_INPUT_SYSTEM || !UNITY_2019_3_OR_NEWER
                Debug.LogWarning("GoldPlayerController.ActionMap is useless when not using the new Input System.");
#endif
                actionMap = value;
            }
        }

        /// <summary> The input system for the player. </summary>
        public IGoldInput PlayerInput { get { return playerInput; } }
        /// <summary> The character controller on the player. </summary>
        public CharacterController Controller { get { return controller; } }

        private void Start()
        {
            if (InitOnStart)
            {
                Initialize();
            }
        }

        public void Update()
        {
            float deltaTime = Time.deltaTime;

            if (movement.HasBeenInitialized)
            {
                movement.OnUpdate(deltaTime);
            }

            if (camera.HasBeenInitialized)
            {
                camera.OnUpdate(deltaTime);
            }

            if (headBob.HasBeenInitialized)
            {
                headBob.OnUpdate(deltaTime);
            }

            if (audio.HasBeenInitialized)
            {
                audio.OnUpdate(deltaTime);
            }
        }

        public void FixedUpdate()
        {
            float fixedDeltaTime = Time.fixedDeltaTime;

            if (movement.HasBeenInitialized)
            {
                movement.OnFixedUpdate(fixedDeltaTime);
            }

            if (camera.HasBeenInitialized)
            {
                camera.OnFixedUpdate(fixedDeltaTime);
            }

            if (headBob.HasBeenInitialized)
            {
                headBob.OnFixedUpdate(fixedDeltaTime);
            }

            if (audio.HasBeenInitialized)
            {
                audio.OnFixedUpdate(fixedDeltaTime);
            }
        }

        public void LateUpdate()
        {
            float deltaTime = Time.deltaTime;

            if (movement.HasBeenInitialized)
            {
                movement.OnLateUpdate(deltaTime);
            }

            if (camera.HasBeenInitialized)
            {
                camera.OnLateUpdate(deltaTime);
            }

            if (headBob.HasBeenInitialized)
            {
                headBob.OnLateUpdate(deltaTime);
            }

            if (audio.HasBeenInitialized)
            {
                audio.OnLateUpdate(deltaTime);
            }
        }

        /// <summary>
        /// Initializes all the modules and sets the player up for use.
        /// </summary>
        public virtual void Initialize()
        {
            GetReferences();

            if (playerInput == null)
            {
                Debug.LogError(gameObject.name + " needs to have a input script derived from GoldInputSystem! Add the standard 'GoldPlayerInputSystem' to fix.");
                return;
            }

            InitializeModules();
        }

        /// <summary>
        /// Gets all the references the player needs.
        /// </summary>
        public virtual void GetReferences()
        {
            playerInput = gameObject.GetComponent<IGoldInput>();
            controller = gameObject.GetComponent<CharacterController>();
        }

        /// <summary>
        /// Initializes all the modules.
        /// </summary>
        protected virtual void InitializeModules()
        {
            InitializeMovement();
            InitializeCamera();
            InitializeHeadBob();
            InitializeAudio();
        }

        /// <summary>
        /// Initializes the movement module.
        /// </summary>
        public virtual void InitializeMovement()
        {
            movement.Initialize(this, PlayerInput);
        }

        /// <summary>
        /// Initializes the camera module.
        /// </summary>
        public virtual void InitializeCamera()
        {
            camera.Initialize(this, PlayerInput);
        }

        /// <summary>
        /// Initializes the head bob module.
        /// </summary>
        public virtual void InitializeHeadBob()
        {
            headBob.Initialize(this, PlayerInput);
        }

        /// <summary>
        /// Initializes the audio module.
        /// </summary>
        public virtual void InitializeAudio()
        {
            audio.Initialize(this, PlayerInput);
        }

        /// <summary>
        /// Sets the world position. Required because the character controller can stop 'transform.position' from working.
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            controller.enabled = false;
            transform.position = position;
            controller.enabled = true;
        }

        /// <summary>
        /// Sets the local position. Required because the character controller can stop 'transform.localPosition' from working.
        /// </summary>
        public void SetLocalPosition(Vector3 position)
        {
            controller.enabled = false;
            transform.localPosition = position;
            controller.enabled = true;
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(position, rotation);
            controller.enabled = true;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            movement.OnValidate();
            camera.OnValidate();
            headBob.OnValidate();
            audio.OnValidate();
        }
#endif
    }
}
