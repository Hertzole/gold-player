using Hertzole.GoldPlayer.Core;
using UnityEngine;

namespace Hertzole.GoldPlayer
{
    public class PlayerModule
    {
        private GoldPlayerController m_PlayerController;
        protected GoldPlayerController PlayerController { get { return m_PlayerController; } }

        protected CharacterController CharacterController { get { return m_PlayerController.Controller; } }

        private Transform m_PlayerTransform;
        protected Transform PlayerTransform { get { if (m_PlayerTransform == null) m_PlayerTransform = m_PlayerController.transform; return m_PlayerTransform; } }

        private GoldInput m_PlayerInput;
        protected GoldInput PlayerInput { get { return m_PlayerInput; } }

        /// <summary> True if the module has been initialized. </summary>
        public bool HasBeenInitialized { get; private set; }

        /// <summary>
        /// Initialize the module.
        /// </summary>
        /// <param name="player">The player controller itself.</param>
        /// <param name="input">Input, if available.</param>
        public void Initialize(GoldPlayerController player, GoldInput input = null)
        {
            // If the module has already been initialized, stop here.
            if (HasBeenInitialized)
                return;

            m_PlayerController = player;
            if (input != null)
                m_PlayerInput = input;

            OnInitialize();

            HasBeenInitialized = true;
        }

        [System.Obsolete("Use 'Initialize' instead! This will be removed in a future version.")]
        public void Init(GoldPlayerController player, GoldInput input = null)
        {
            Initialize(player, input);
        }

        /// <summary>
        /// Called when the module is initialized.
        /// </summary>
        protected virtual void OnInitialize() { }

        [System.Obsolete("Use 'OnInitialize' instead! This will be removed in a future version.")]
        protected virtual void OnInit() { }

        /// <summary>
        /// Called in Update.
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// Called in FixedUpdate.
        /// </summary>
        public virtual void OnFixedUpdate() { }

        /// <summary>
        /// Called in LateUpdate.
        /// </summary>
        public virtual void OnLateUpdate() { }

        /// <summary>
        /// Equivalent to Input's GetButton/GetKey function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        /// <param name="defaultKey">A default key in case the input script is null.</param>
        protected bool GetButton(string buttonName, KeyCode defaultKey = KeyCode.None)
        {
            // If player input isn't null, get the key using that. Else use the default key.
            if (PlayerInput != null)
                return PlayerInput.GetButton(buttonName);
            else
                return Input.GetKey(defaultKey);
        }

        /// <summary>
        /// Equivalent to Input's GetButtonDown/GetKeyDown function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        /// <param name="defaultKey">A default key in case the input script is null.</param>
        protected bool GetButtonDown(string buttonName, KeyCode defaultKey = KeyCode.None)
        {
            // If player input isn't null, get the key using that. Else use the default key.
            if (PlayerInput != null)
                return PlayerInput.GetButtonDown(buttonName);
            else
                return Input.GetKeyDown(defaultKey);
        }

        /// <summary>
        /// Equivalent to Input's GetButtonUp/GetKeyUp function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        /// <param name="defaultKey">A default key in case the input script is null.</param>
        protected bool GetButtonUp(string buttonName, KeyCode defaultKey = KeyCode.None)
        {
            // If player input isn't null, get the key using that. Else use the default key.
            if (PlayerInput != null)
                return PlayerInput.GetButtonUp(buttonName);
            else
                return Input.GetKeyUp(defaultKey);
        }

        /// <summary>
        /// Equivalent to Input's GetAxis function.
        /// </summary>
        /// <param name="axisName">The axis name you want to get.</param>
        /// <param name="defaultAxisName">A default axis name in case the input script is null.</param>
        protected float GetAxis(string axisName, string defaultAxisName = "")
        {
            // If the default axis name is blank, use the one provided in axisName.
            if (string.IsNullOrEmpty(defaultAxisName))
                defaultAxisName = axisName;

            // If player input isn't null, get the axis using that. Else use the default axis name.
            if (PlayerInput != null)
                return PlayerInput.GetAxis(axisName);
            else
                return Input.GetAxis(defaultAxisName);
        }

        /// <summary>
        /// Equivalent to Input's GetAxisRaw function.
        /// </summary>
        /// <param name="axisName">The axis name you want to get.</param>
        /// <param name="defaultAxisName">A default axis name in case the input script is null.</param>
        protected float GetAxisRaw(string axisName, string defaultAxisName = "")
        {
            // If the default axis name is blank, use the one provided in axisName.
            if (string.IsNullOrEmpty(defaultAxisName))
                defaultAxisName = axisName;

            // If player input isn't null, get the axis using that. Else use the default axis name.
            if (PlayerInput != null)
                return PlayerInput.GetAxisRaw(axisName);
            else
                return Input.GetAxisRaw(defaultAxisName);
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
