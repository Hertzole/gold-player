using Hertzole.GoldPlayer.Core;
using UnityEngine;

namespace Hertzole.GoldPlayer
{
    public class PlayerModule
    {
        private string rootActionMap;

        private GoldPlayerController playerController;
        protected GoldPlayerController PlayerController { get { return playerController; } }

        protected CharacterController CharacterController { get { return playerController.Controller; } }

        private Transform playerTransform;
        protected Transform PlayerTransform { get { if (playerTransform == null) { playerTransform = playerController.transform; } return playerTransform; } }

        private GoldInput playerInput;
        protected GoldInput PlayerInput { get { return playerInput; } }

        /// <summary> True if the module has been initialized. </summary>
        public bool HasBeenInitialized { get; private set; }

        /// <summary>
        /// Initialize the module.
        /// </summary>
        /// <param name="player">The player controller itself.</param>
        /// <param name="input">Input, if available.</param>
        public void Initialize(GoldPlayerController player, GoldInput input)
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

            if (player != null)
            {
                rootActionMap = player.ActionMap;
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

        [System.Obsolete("Use 'GetButton' without defaultKey parameter instead.")]
        protected bool GetButton(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButton(buttonName); }

        /// <summary>
        /// Equivalent to Input's GetButton/GetKey function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        protected bool GetButton(string buttonName)
        {
            return PlayerInput.GetButton(string.IsNullOrWhiteSpace(rootActionMap) ? buttonName : rootActionMap + "/" + buttonName);
        }

        [System.Obsolete("Use 'GetButtonDown' without defaultKey parameter instead.")]
        protected bool GetButtonDown(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButtonDown(buttonName); }

        /// <summary>
        /// Equivalent to Input's GetButtonDown/GetKeyDown function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        protected bool GetButtonDown(string buttonName)
        {
            return PlayerInput.GetButtonDown(string.IsNullOrWhiteSpace(rootActionMap) ? buttonName : rootActionMap + "/" + buttonName);
        }

        [System.Obsolete("Use 'GetButtonUp' without defaultKey parameter instead.")]
        protected bool GetButtonUp(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButtonUp(buttonName); }

        /// <summary>
        /// Equivalent to Input's GetButtonUp/GetKeyUp function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        protected bool GetButtonUp(string buttonName)
        {
            return PlayerInput.GetButtonUp(string.IsNullOrWhiteSpace(rootActionMap) ? buttonName : rootActionMap + "/" + buttonName);
        }

        [System.Obsolete("Use 'GetAxis' without defaultAxisName parameter instead.")]
        protected float GetAxis(string axisName, string defaultAxisName = "") { return GetAxis(axisName); }

        /// <summary>
        /// Equivalent to Input's GetAxis function.
        /// </summary>
        /// <param name="axisName">The axis name you want to get.</param>
        protected float GetAxis(string axisName)
        {
            return PlayerInput.GetAxis(string.IsNullOrWhiteSpace(rootActionMap) ? axisName : rootActionMap + "/" + axisName);
        }

        [System.Obsolete("Use 'GetAxisRaw' without defaultAxisName parameter instead.")]
        protected float GetAxisRaw(string axisName, string defaultAxisName = "") { return GetAxisRaw(axisName); }

        /// <summary>
        /// Equivalent to Input's GetAxisRaw function.
        /// </summary>
        /// <param name="axisName">The axis name you want to get.</param>
        protected float GetAxisRaw(string axisName)
        {
            return PlayerInput.GetAxisRaw(string.IsNullOrWhiteSpace(rootActionMap) ? axisName : rootActionMap + "/" + axisName);
        }

#if !ENABLE_INPUT_SYSTEM
        [System.Obsolete("GetVector2Input does nothing with the Input Manager.")]
#endif
        protected Vector2 GetVector2Input(string action)
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            return PlayerInput.GetVector2(string.IsNullOrWhiteSpace(rootActionMap) ? action : rootActionMap + "/" + action);
#else
            return Vector2.zero;
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Called when something changes in the inspector.
        /// THIS IS EDITOR ONLY! SHOULD NOT BE USED OUTSIDE 'if UNITY_EDITOR' DEFINE!
        /// </summary>
        public virtual void OnValidate() { }
#endif
    }
}
