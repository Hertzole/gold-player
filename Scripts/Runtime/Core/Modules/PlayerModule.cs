using UnityEngine;

namespace Hertzole.GoldPlayer
{
    public class PlayerModule
    {
        [SerializeField]
        private GoldPlayerController playerController = null;

        public GoldPlayerController PlayerController { get { return playerController; } set { playerController = value; } }
        public CharacterController CharacterController { get { return PlayerController.Controller; } }
        public Transform PlayerTransform { get { return PlayerController.transform; } }

#if UNITY_EDITOR || GOLD_PLAYER_DISABLE_OPTIMIZATIONS
        protected IGoldInput PlayerInput { get; private set; }

        /// <summary> True if the module has been initialized. </summary>
        public bool HasBeenInitialized { get; private set; }
#else
        [System.NonSerialized]
        protected IGoldInput PlayerInput;
        [System.NonSerialized]
        public bool HasBeenInitialized;
#endif

        #region Obsolete
#if UNITY_EDITOR
        [System.Obsolete("Use 'Initialize(IGoldInput input)' instead. This will be removed on build.", true)]
        public void Initialize(GoldPlayerController player, IGoldInput input) { }
#endif
        #endregion

        /// <summary>
        /// Initialize the module.
        /// </summary>
        /// <param name="player">The player controller itself.</param>
        /// <param name="input">Input, if available.</param>
        public void Initialize(IGoldInput input)
        {
            // If the module has already been initialized, stop here.
            if (HasBeenInitialized)
            {
                return;
            }

            if (input != null)
            {
                PlayerInput = input;
            }

            OnInitialize();

            HasBeenInitialized = true;
        }

#if UNITY_EDITOR
        internal void ForceInitialize(IGoldInput input)
        {
            if (input != null)
            {
                PlayerInput = input;
            }

            OnInitialize();

            HasBeenInitialized = true;
        }
#endif

        /// <summary>
        /// Called when the module is initialized.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Called in Update.
        /// </summary>
        public virtual void OnUpdate(float deltaTime) { }

        /// <summary>
        /// Called in FixedUpdate.
        /// </summary>
        public virtual void OnFixedUpdate(float fixedDeltaTime) { }

        /// <summary>
        /// Called in LateUpdate.
        /// </summary>
        public virtual void OnLateUpdate(float deltaTime) { }

        /// <summary>
        /// Equivalent to Input's GetButton/GetKey function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        protected bool GetButton(string buttonName)
        {
            return PlayerInput.GetButton(buttonName);
        }

        /// <summary>
        /// Equivalent to Input's GetButtonDown/GetKeyDown function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        protected bool GetButtonDown(string buttonName)
        {
            return PlayerInput.GetButtonDown(buttonName);
        }

        /// <summary>
        /// Equivalent to Input's GetButtonUp/GetKeyUp function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        protected bool GetButtonUp(string buttonName)
        {
            return PlayerInput.GetButtonUp(buttonName);
        }

        /// <summary>
        /// Equivalent to Input's GetAxis function.
        /// </summary>
        /// <param name="axisName">The axis name you want to get.</param>
        protected float GetAxis(string axisName)
        {
            return PlayerInput.GetAxis(axisName);
        }

        /// <summary>
        /// Equivalent to Input's GetAxisRaw function.
        /// </summary>
        /// <param name="axisName">The axis name you want to get.</param>
        protected float GetAxisRaw(string axisName)
        {
            return PlayerInput.GetAxisRaw(axisName);
        }

#if (!ENABLE_INPUT_SYSTEM || !GOLD_PLAYER_NEW_INPUT) && UNITY_EDITOR
        [System.Obsolete("GetVector2Input does nothing with the Input Manager. This will be removed on build.", true)]
#endif
#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT || UNITY_EDITOR
        protected Vector2 GetVector2Input(string action)
        {
#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
            return PlayerInput.GetVector2(action);
#else
            return Vector2.zero;
#endif
        }
#endif

        #region Obsolete
#if UNITY_EDITOR
        [System.Obsolete("Use 'GetButton' without defaultKey parameter instead. This will be removed on build.", true)]
        protected bool GetButton(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButton(buttonName); }

        [System.Obsolete("Use 'GetButtonDown' without defaultKey parameter instead. This will be removed on build.", true)]
        protected bool GetButtonDown(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButtonDown(buttonName); }

        [System.Obsolete("Use 'GetButtonUp' without defaultKey parameter instead. This will be removed on build.", true)]
        protected bool GetButtonUp(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButtonUp(buttonName); }

        [System.Obsolete("Use 'GetAxis' without defaultAxisName parameter instead. This will be removed on build.", true)]
        protected float GetAxis(string axisName, string defaultAxisName = "") { return GetAxis(axisName); }

        [System.Obsolete("Use 'GetAxisRaw' without defaultAxisName parameter instead. This will be removed on build.", true)]
        protected float GetAxisRaw(string axisName, string defaultAxisName = "") { return GetAxisRaw(axisName); }
#endif
        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// Called when something changes in the inspector.
        /// THIS IS EDITOR ONLY! SHOULD NOT BE USED OUTSIDE 'if UNITY_EDITOR' DEFINE!
        /// </summary>
        public virtual void OnValidate() { }
#endif
    }
}
