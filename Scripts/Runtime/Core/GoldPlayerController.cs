using UnityEngine;
using UnityEngine.Serialization;

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

        private bool initOnStart = true;
        protected bool hasBeenInitialized = false;

        private IGoldInput playerInput;
        private CharacterController controller;

        /// <summary> True if all the modules have been initialized. </summary>
        public bool HasBeenFullyInitialized
        {
            get { return cam.HasBeenInitialized && movement.HasBeenInitialized && headBob.HasBeenInitialized && sounds.HasBeenInitialized; }
        }
        /// <summary> If false, 'Initialize()' will not be called on Start and will only be called once another script calls it. </summary>
        public bool InitOnStart { get { return initOnStart; } set { initOnStart = value; } }
        /// <summary> If true, Gold Player will use unscaled delta time. </summary>
        public bool UnscaledTime
        {
            get { return cam.FieldOfViewKick.UnscaledTime && movement.UnscaledTime && headBob.UnscaledTime && sounds.UnscaledTime; }
            set { cam.FieldOfViewKick.UnscaledTime = value; movement.UnscaledTime = value; headBob.UnscaledTime = value; sounds.UnscaledTime = value; }
        }

        /// <summary> Everything related to the player camera (mouse movement). </summary>
        public PlayerCamera Camera { get { return cam; } set { cam = value; } }
        /// <summary> Everything related to movement. </summary>
        public PlayerMovement Movement { get { return movement; } set { movement = value; } }
        /// <summary> Everything related to the head bob. </summary>
        public PlayerBob HeadBob { get { return headBob; } set { headBob = value; } }
        /// <summary> Everything related to audio (footsteps, landing and jumping). </summary>
        public PlayerAudio Audio { get { return sounds; } set { sounds = value; } }

        #region Obsolete
#if UNITY_EDITOR
        /// <summary> The main action map for the Input Actions. </summary>
        [System.Obsolete("No longer used. This will be removed on build.", true)]
        public string ActionMap
        {
            get
            {
#if !ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
                Debug.LogWarning("GoldPlayerController.ActionMap is useless when not using the new Input System.");
#endif
                return string.Empty;
            }
            set
            {
#if !ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
                Debug.LogWarning("GoldPlayerController.ActionMap is useless when not using the new Input System.");
#endif
            }
        }
        /// <summary> Has all the scripts be initialized? </summary>
        [System.Obsolete("Use HasBeenFullyInitialized instead. This will be removed on build.", true)]
        public bool HasBeenInitialized { get { return HasBeenFullyInitialized; } }
#endif // UNITY_EDITOR
        #endregion

        /// <summary> The input system for the player. </summary>
        public IGoldInput PlayerInput { get { return playerInput; } }
        /// <summary> The character controller on the player. </summary>
        public CharacterController Controller { get { return controller; } }

        private void Awake()
        {
            // Get all the references as soon as possible.
            GetReferences();

            // Complain if there's no input present.
            // Only do this in development builds and the Unity editor. We save a small amount of performance by removing it.
#if DEBUG || UNITY_EDITOR
            if (playerInput == null)
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

            if (movement.HasBeenInitialized)
            {
                movement.OnUpdate(deltaTime);
            }

            if (cam.HasBeenInitialized)
            {
                cam.OnUpdate(deltaTime);
            }

            if (headBob.HasBeenInitialized)
            {
                headBob.OnUpdate(deltaTime);
            }

            if (sounds.HasBeenInitialized)
            {
                sounds.OnUpdate(deltaTime);
            }
        }

        public void FixedUpdate()
        {
            float fixedDeltaTime = Time.fixedDeltaTime;

            if (movement.HasBeenInitialized)
            {
                movement.OnFixedUpdate(fixedDeltaTime);
            }

            if (cam.HasBeenInitialized)
            {
                cam.OnFixedUpdate(fixedDeltaTime);
            }

            if (headBob.HasBeenInitialized)
            {
                headBob.OnFixedUpdate(fixedDeltaTime);
            }

            if (sounds.HasBeenInitialized)
            {
                sounds.OnFixedUpdate(fixedDeltaTime);
            }
        }

        public void LateUpdate()
        {
            float deltaTime = Time.deltaTime;

            if (movement.HasBeenInitialized)
            {
                movement.OnLateUpdate(deltaTime);
            }

            if (cam.HasBeenInitialized)
            {
                cam.OnLateUpdate(deltaTime);
            }

            if (headBob.HasBeenInitialized)
            {
                headBob.OnLateUpdate(deltaTime);
            }

            if (sounds.HasBeenInitialized)
            {
                sounds.OnLateUpdate(deltaTime);
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
            cam.OnValidate();
            headBob.OnValidate();
            sounds.OnValidate();
        }

        private void OnDrawGizmosSelected()
        {
            Color oColor = Gizmos.color;

            float radius = GetComponent<CharacterController>().radius;

            if (movement.GroundCheck == GroundCheckType.Raycast)
            {
                Vector3[] rays = new Vector3[movement.RayAmount + 1];
                movement.CreateGroundCheckRayCircle(ref rays, transform.position, radius);

                for (int i = 0; i < rays.Length; i++)
                {
                    if (Application.isPlaying)
                    {
                        bool hit = Physics.Raycast(rays[i], Vector3.down, movement.RayLength, movement.GroundLayer, QueryTriggerInteraction.Ignore);
                        Gizmos.color = hit ? new Color(0f, 1f, 0f, 1f) : new Color(1f, 0f, 0f, 1f);
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                    }

                    Gizmos.DrawLine(rays[i], new Vector3(rays[i].x, rays[i].y - movement.RayLength, rays[i].z));
                }
            }
            else if (movement.GroundCheck == GroundCheckType.Sphere)
            {
                Vector3 pos = new Vector3(transform.position.x, transform.position.y + radius - 0.1f, transform.position.z);
                if (Application.isPlaying)
                {
                    Gizmos.color = movement.IsGrounded ? new Color(0f, 1f, 0f, 0.25f) : new Color(1f, 0f, 0f, 0.25f);
                }
                else
                {
                    Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
                }
                Gizmos.DrawSphere(pos, radius);
                if (Application.isPlaying)
                {
                    Gizmos.color = movement.IsGrounded ? new Color(0f, 1f, 0f, 1f) : new Color(1f, 0f, 0f, 1f);
                }
                else
                {
                    Gizmos.color = new Color(0f, 1f, 0f, 1f);
                }
                Gizmos.DrawWireSphere(pos, radius);
            }

            Gizmos.color = oColor;
        }
#endif
    }
}
