using UnityEngine;

namespace Hertzole.GoldPlayer
{
    public class PlayerModule
    {
        private GoldPlayerController playerController;
        protected GoldPlayerController PlayerController { get { return playerController; } }

        protected CharacterController CharacterController { get { return playerController.Controller; } }

        private Transform playerTransform;
        protected Transform PlayerTransform { get { if (playerTransform == null) { playerTransform = playerController.transform; } return playerTransform; } }

        private IGoldInput playerInput;
        protected IGoldInput PlayerInput { get { return playerInput; } }

        /// <summary> True if the module has been initialized. </summary>
        public bool HasBeenInitialized { get; private set; }

        /// <summary>
        /// Initialize the module.
        /// </summary>
        /// <param name="player">The player controller itself.</param>
        /// <param name="input">Input, if available.</param>
        public void Initialize(GoldPlayerController player, IGoldInput input)
        {
            // If the module has already been initialized, stop here.
            if (HasBeenInitialized)
            {
                return;
            }

            playerController = player;
            if (input != null)
            {
                playerInput = input;
            }

            OnInitialize();

            HasBeenInitialized = true;
        }

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

#if UNITY_EDITOR
        [System.Obsolete("Use 'GetButton' without defaultKey parameter instead. This will be removed on build.", true)]
        protected bool GetButton(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButton(buttonName); }
#endif

        /// <summary>
        /// Equivalent to Input's GetButton/GetKey function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        protected bool GetButton(string buttonName)
        {
            return PlayerInput.GetButton(buttonName);
        }

#if UNITY_EDITOR
        [System.Obsolete("Use 'GetButtonDown' without defaultKey parameter instead. This will be removed on build.", true)]
        protected bool GetButtonDown(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButtonDown(buttonName); }
#endif

        /// <summary>
        /// Equivalent to Input's GetButtonDown/GetKeyDown function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        protected bool GetButtonDown(string buttonName)
        {
            return PlayerInput.GetButtonDown(buttonName);
        }

#if UNITY_EDITOR
        [System.Obsolete("Use 'GetButtonUp' without defaultKey parameter instead. This will be removed on build.", true)]
        protected bool GetButtonUp(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButtonUp(buttonName); }
#endif

        /// <summary>
        /// Equivalent to Input's GetButtonUp/GetKeyUp function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        protected bool GetButtonUp(string buttonName)
        {
            return PlayerInput.GetButtonUp(buttonName);
        }

#if UNITY_EDITOR
        [System.Obsolete("Use 'GetAxis' without defaultAxisName parameter instead. This will be removed on build.", true)]
        protected float GetAxis(string axisName, string defaultAxisName = "") { return GetAxis(axisName); }
#endif

        /// <summary>
        /// Equivalent to Input's GetAxis function.
        /// </summary>
        /// <param name="axisName">The axis name you want to get.</param>
        protected float GetAxis(string axisName)
        {
            return PlayerInput.GetAxis(axisName);
        }

#if UNITY_EDITOR
        [System.Obsolete("Use 'GetAxisRaw' without defaultAxisName parameter instead. This will be removed on build.", true)]
        protected float GetAxisRaw(string axisName, string defaultAxisName = "") { return GetAxisRaw(axisName); }
#endif

        /// <summary>
        /// Equivalent to Input's GetAxisRaw function.
        /// </summary>
        /// <param name="axisName">The axis name you want to get.</param>
        protected float GetAxisRaw(string axisName)
        {
            return PlayerInput.GetAxisRaw(axisName);
        }

#if !ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT && UNITY_EDITOR
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

#if UNITY_EDITOR
        /// <summary>
        /// Called when something changes in the inspector.
        /// THIS IS EDITOR ONLY! SHOULD NOT BE USED OUTSIDE 'if UNITY_EDITOR' DEFINE!
        /// </summary>
        public virtual void OnValidate() { }
#endif
    }
}
