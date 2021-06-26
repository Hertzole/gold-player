using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Hertzole.GoldPlayer.Tests")]
#endif
namespace Hertzole.GoldPlayer
{
    [RequireComponent(typeof(CharacterController))]
    [DisallowMultipleComponent]
    [AddComponentMenu("Gold Player/Gold Player Controller", 0)]
    [HelpURL("https://github.com/Hertzole/gold-player")]
    [SelectionBase]
    public class GoldPlayerController : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("m_Camera")]
        [FormerlySerializedAs("camera")]
        private PlayerCamera cam = new PlayerCamera();
        [SerializeField]
        [FormerlySerializedAs("m_Movement")]
        private PlayerMovement movement = new PlayerMovement();
        [SerializeField]
        [FormerlySerializedAs("m_HeadBob")]
        private PlayerBob headBob = new PlayerBob();
        [SerializeField]
        [FormerlySerializedAs("m_Audio")]
        [FormerlySerializedAs("audio")]
        private PlayerAudio sounds = new PlayerAudio();

        [SerializeField]
        [HideInInspector]
        private CharacterController controller = null;

        /// <summary> True if all the modules have been initialized. </summary>
        public bool HasBeenFullyInitialized
        {
            get { return cam.HasBeenInitialized && movement.HasBeenInitialized && headBob.HasBeenInitialized && sounds.HasBeenInitialized; }
        }
        /// <summary> If true, Gold Player will use unscaled delta time. </summary>
        public bool UnscaledTime
        {
            get
            {
                return cam.FieldOfViewKick.UnscaledTime && 
                       movement.UnscaledTime && 
                       movement.Stamina.UnscaledTime &&
                       headBob.UnscaledTime &&
                       sounds.UnscaledTime;
            }
            set 
            {
                cam.FieldOfViewKick.UnscaledTime = value; 
                movement.UnscaledTime = value;
                movement.Stamina.UnscaledTime = value;
                headBob.UnscaledTime = value; 
                sounds.UnscaledTime = value; }
        }

        /// <summary> Everything related to the player camera (mouse movement). </summary>
        public PlayerCamera Camera { get { return cam; } set { cam = value; } }
        /// <summary> Everything related to movement. </summary>
        public PlayerMovement Movement { get { return movement; } set { movement = value; } }
        /// <summary> Everything related to the head bob. </summary>
        public PlayerBob HeadBob { get { return headBob; } set { headBob = value; } }
        /// <summary> Everything related to audio (footsteps, landing and jumping). </summary>
        public PlayerAudio Audio { get { return sounds; } set { sounds = value; } }

        public Vector3 Velocity { get { return movement.Velocity; } }

#if UNITY_EDITOR || GOLD_PLAYER_DISABLE_OPTIMIZATIONS
        private bool initOnStart = true;
        /// <summary> If false, 'Initialize()' will not be called on Start and will only be called once another script calls it. </summary>
        public bool InitOnStart { get { return initOnStart; } set { initOnStart = value; } }
#else
        [System.NonSerialized]
        public bool InitOnStart = true;
#endif

        /// <summary> The character controller on the player. </summary>
        public CharacterController Controller { get { return controller; } }
#if UNITY_EDITOR || GOLD_PLAYER_DISABLE_OPTIMIZATIONS
        /// <summary> The input system for the player. </summary>
        public IGoldInput PlayerInput { get; set; }
#else
        [System.NonSerialized]
        public IGoldInput PlayerInput;
#endif

        internal void Awake()
        {
            // Get all the references as soon as possible.
            GetReferences();

            // Complain if there's no input present.
            // Only do this in development builds and the Unity editor. We save a small amount of performance by removing it.
#if DEBUG || UNITY_EDITOR
            if (PlayerInput == null)
            {
#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
                Debug.LogError(gameObject.name + " needs to have a input script derived from IGoldInput! Add the standard 'GoldPlayerInputSystem' to fix.");
#else
                Debug.LogError(gameObject.name + " needs to have a input script derived from IGoldInput! Add the standard 'GoldPlayerInput' to fix.");
#endif
                return;
            }
#endif
        }

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
            float unscaledDeltaTime = Time.unscaledDeltaTime;

            if (movement.HasBeenInitialized)
            {
                movement.OnUpdate(deltaTime, unscaledDeltaTime);
            }

            if (cam.HasBeenInitialized)
            {
                cam.OnUpdate(deltaTime, unscaledDeltaTime);
            }

            if (headBob.HasBeenInitialized)
            {
                headBob.OnUpdate(deltaTime, unscaledDeltaTime);
            }

            if (sounds.HasBeenInitialized)
            {
                sounds.OnUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        public void FixedUpdate()
        {
            float fixedDeltaTime = Time.fixedDeltaTime;
            float fixedUnscaledDeltaTime = Time.fixedUnscaledDeltaTime;

            if (movement.HasBeenInitialized)
            {
                movement.OnFixedUpdate(fixedDeltaTime, fixedUnscaledDeltaTime);
            }

            if (cam.HasBeenInitialized)
            {
                cam.OnFixedUpdate(fixedDeltaTime, fixedUnscaledDeltaTime);
            }

            if (headBob.HasBeenInitialized)
            {
                headBob.OnFixedUpdate(fixedDeltaTime, fixedUnscaledDeltaTime);
            }

            if (sounds.HasBeenInitialized)
            {
                sounds.OnFixedUpdate(fixedDeltaTime, fixedUnscaledDeltaTime);
            }
        }

        public void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.deltaTime;

            if (movement.HasBeenInitialized)
            {
                movement.OnLateUpdate(deltaTime, unscaledDeltaTime);
            }

            if (cam.HasBeenInitialized)
            {
                cam.OnLateUpdate(deltaTime, unscaledDeltaTime);
            }

            if (headBob.HasBeenInitialized)
            {
                headBob.OnLateUpdate(deltaTime, unscaledDeltaTime);
            }

            if (sounds.HasBeenInitialized)
            {
                sounds.OnLateUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        /// <summary>
        /// Initializes all the modules and sets the player up for use.
        /// </summary>
        public virtual void Initialize()
        {
            InitializeModules();
        }

        /// <summary>
        /// Gets all the references the player needs.
        /// </summary>
        public virtual void GetReferences()
        {
            PlayerInput = gameObject.GetComponent<IGoldInput>();
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
            cam.Initialize(this, PlayerInput);
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
            sounds.Initialize(this, PlayerInput);
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

        public void SetRotation(float yRotation)
        {
            if (cam.RotateCameraOnly)
            {
                cam.BodyAngle = yRotation;
            }
            else
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            }
        }

#if UNITY_EDITOR
        [System.Obsolete("Use SetPositionAndRotation with yRotation instead of rotation. This will be removed on build.", true)]
        [UnityEngine.TestTools.ExcludeFromCoverage]
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            SetPositionAndRotation(position, 0);
        }
#endif

        public void SetPositionAndRotation(Vector3 position, float yRotation)
        {
            if (!cam.RotateCameraOnly)
            {
                controller.enabled = false;
                transform.SetPositionAndRotation(position, Quaternion.Euler(transform.eulerAngles.x, yRotation, transform.eulerAngles.z));
                controller.enabled = true;
            }
            else
            {
                SetPosition(position);
                SetRotation(yRotation);
            }
        }

        /// <summary>
        /// Generates a stable hash code from a string.
        /// </summary>
        /// <param name="actionName">The action name.</param>
        /// <returns>The action name hash code.</returns>
        public static int InputNameToHash(string actionName)
        {
            unchecked
            {
                int hash = 7;
                for (int i = 0; i < actionName.Length; i++)
                {
                    hash = hash * 8 + actionName[i];
                }

                return hash;
            }
        }

#if UNITY_EDITOR
        [UnityEngine.TestTools.ExcludeFromCoverage]
        private void Reset()
        {
            GetComponents();
        }

        [UnityEngine.TestTools.ExcludeFromCoverage]
        private void OnValidate()
        {
            GetComponents();

            movement.OnValidate();
            cam.OnValidate();
            headBob.OnValidate();
            sounds.OnValidate();
        }

        [UnityEngine.TestTools.ExcludeFromCoverage]
        private void GetComponents()
        {
            if (cam.PlayerController == null)
            {
                cam.PlayerController = this;
            }

            if (movement.PlayerController == null)
            {
                movement.PlayerController = this;
            }

            if (headBob.PlayerController == null)
            {
                headBob.PlayerController = this;
            }

            if (Audio.PlayerController == null)
            {
                Audio.PlayerController = this;
            }

            if (controller == null)
            {
                controller = GetComponent<CharacterController>();
            }
        }
#endif
    }
}
