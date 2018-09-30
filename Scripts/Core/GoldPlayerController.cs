#if HERTZLIB_UPDATE_MANAGER
using Hertzole.HertzLib;
#endif
using Hertzole.GoldPlayer.Core;
using UnityEngine;

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
        private PlayerCamera m_Camera = new PlayerCamera();
        [SerializeField]
        private PlayerMovement m_Movement = new PlayerMovement();
        [SerializeField]
        private PlayerBob m_HeadBob = new PlayerBob();
        [SerializeField]
        private PlayerAudio m_Audio = new PlayerAudio();

        private bool m_InitOnStart = true;
        protected bool m_HasBeenInitialized = false;

        private GoldInput m_PlayerInput;
        private CharacterController m_Controller;

        /// <summary> Has all the scripts be initialized? </summary>
        public bool HasBeenInitialized { get { return m_HasBeenInitialized; } }
        /// <summary> If false, 'Initialize()' will not be called on Start and will only be called once another script calls it. </summary>
        public bool InitOnStart { get { return m_InitOnStart; } set { m_InitOnStart = value; } }

        /// <summary> Everything related to the player camera (mouse movement). </summary>
        public PlayerCamera Camera { get { return m_Camera; } set { m_Camera = value; } }
        /// <summary> Everything related to movement. </summary>
        public PlayerMovement Movement { get { return m_Movement; } set { m_Movement = value; } }
        /// <summary> Everything related to the head bob. </summary>
        public PlayerBob HeadBob { get { return m_HeadBob; } set { m_HeadBob = value; } }
        /// <summary> Everything related to audio (footsteps, landing and jumping). </summary>
        public PlayerAudio Audio { get { return m_Audio; } set { m_Audio = value; } }

        /// <summary> The input system for the player. </summary>
        public GoldInput PlayerInput { get { return m_PlayerInput; } }
        /// <summary> The character controller on the player. </summary>
        public CharacterController Controller { get { return m_Controller; } }

        private void Start()
        {
            if (InitOnStart)
                Initialize();
        }

#if HERTZLIB_UPDATE_MANAGER
        protected virtual void OnEnable()
        {
            UpdateManager.AddUpdate(this);
            UpdateManager.AddFixedUpdate(this);
            UpdateManager.AddLateUpdate(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.RemoveUpdate(this);
            UpdateManager.RemoveFixedUpdate(this);
            UpdateManager.RemoveLateUpdate(this);
        }
#endif

#if HERTZLIB_UPDATE_MANAGER
        public void OnUpdate()
#else
        private void Update()
#endif
        {
            if (HasBeenInitialized)
            {
                Movement.OnUpdate();
                Camera.OnUpdate();
                HeadBob.OnUpdate();
                Audio.OnUpdate();
            }
        }

#if HERTZLIB_UPDATE_MANAGER
        public void OnFixedUpdate()
#else
        private void FixedUpdate()
#endif
        {
            if (HasBeenInitialized)
            {
                Movement.OnFixedUpdate();
                Camera.OnFixedUpdate();
                HeadBob.OnFixedUpdate();
                Audio.OnFixedUpdate();
            }
        }

#if HERTZLIB_UPDATE_MANAGER
        public void OnLateUpdate()
#else
        private void LateUpdate()
#endif
        {
            if (HasBeenInitialized)
            {
                Movement.OnLateUpdate();
                Camera.OnLateUpdate();
                HeadBob.OnLateUpdate();
                Audio.OnLateUpdate();
            }
        }

        /// <summary>
        /// Initializes all the modules and sets the player up for use.
        /// </summary>
        public virtual void Initialize()
        {
            GetReferences();

            InitializeModules();

            m_HasBeenInitialized = true;
        }

        /// <summary>
        /// Gets all the references the player needs.
        /// </summary>
        protected virtual void GetReferences()
        {
            m_PlayerInput = GetComponent<GoldInput>();
            m_Controller = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Initializes all the modules.
        /// </summary>
        protected virtual void InitializeModules()
        {
            Movement.Initialize(this, PlayerInput);
            Camera.Initialize(this, PlayerInput);
            HeadBob.Initialize(this, PlayerInput);
            Audio.Initialize(this, PlayerInput);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Movement.OnValidate();
            Camera.OnValidate();
            HeadBob.OnValidate();
            Audio.OnValidate();
        }
#endif
    }
}
