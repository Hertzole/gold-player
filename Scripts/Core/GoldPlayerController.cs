#if HERTZLIB_UPDATE_MANAGER
using Hertzole.HertzLib;
#endif
using Hertzole.GoldPlayer.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
    [RequireComponent(typeof(CharacterController))]
    [DisallowMultipleComponent]
    [AddComponentMenu("Gold Player/Gold Player Controller", 01)]
#if HERTZLIB_UPDATE_MANAGER
    public class GoldPlayerController : MonoBehaviour, IUpdate, IFixedUpdate, ILateUpdate
#else
    public class GoldPlayerController : MonoBehaviour
#endif
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

        private bool initOnStart = true;
        protected bool hasBeenInitialized = false;

        private GoldInput playerInput;
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

        /// <summary> The input system for the player. </summary>
        public GoldInput PlayerInput { get { return playerInput; } }
        /// <summary> The character controller on the player. </summary>
        public CharacterController Controller { get { return controller; } }

        private void Start()
        {
            if (InitOnStart)
            {
                Initialize();
            }
        }

        protected virtual void OnEnable()
        {
#if HERTZLIB_UPDATE_MANAGER
            UpdateManager.AddUpdate(this);
            UpdateManager.AddFixedUpdate(this);
            UpdateManager.AddLateUpdate(this);
#endif
        }

        protected virtual void OnDisable()
        {
#if HERTZLIB_UPDATE_MANAGER
            UpdateManager.RemoveUpdate(this);
            UpdateManager.RemoveFixedUpdate(this);
            UpdateManager.RemoveLateUpdate(this);
#endif
        }

#if HERTZLIB_UPDATE_MANAGER
        public void OnUpdate()
#else
        public void Update()
#endif
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

#if HERTZLIB_UPDATE_MANAGER
        public void OnFixedUpdate()
#else
        public void FixedUpdate()
#endif
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

#if HERTZLIB_UPDATE_MANAGER
        public void OnLateUpdate()
#else
        public void LateUpdate()
#endif
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
            playerInput = gameObject.GetComponent<GoldInput>();
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
