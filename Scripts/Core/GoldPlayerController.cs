using Hertzole.GoldPlayer.Core;
using UnityEngine;

namespace Hertzole.GoldPlayer
{
    [RequireComponent(typeof(CharacterController))]
    public class GoldPlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerMovement m_Movement;
        public PlayerMovement Movement { get { return m_Movement; } }

        private bool m_InitOnStart = false;
        protected bool m_HasBeenInitialized = false;

        private GoldInput m_PlayerInput;
        private CharacterController m_Controller;

        /// <summary> Has all the scripts be initialized? </summary>
        public bool HasBeenInitialized { get { return m_HasBeenInitialized; } }
        /// <summary> If false, 'Initialize()' will not be called on Start and will only be called once another script calls it. </summary>
        public bool InitOnStart { get { return m_InitOnStart; } set { m_InitOnStart = value; } }

        /// <summary> The input system for the player. </summary>
        public GoldInput PlayerInput { get { return m_PlayerInput; } }
        /// <summary> The character controller on the player. </summary>
        public CharacterController Controller { get { return m_Controller; } }

        private void Start()
        {
            if (InitOnStart)
                Initialize();
        }

        private void Update()
        {
            Movement.OnUpdate();
        }

        private void FixedUpdate()
        {
            Movement.OnFixedUpdate();
        }

        private void LateUpdate()
        {
            Movement.OnLateUpdate();
        }

        /// <summary>
        /// Initializes all the modules and sets the player up for use.
        /// </summary>
        public virtual void Initialize()
        {
            GetReferences();

            InitializeModules();
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
            Movement.Init(this, PlayerInput);
        }
    }
}
