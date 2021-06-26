using UnityEngine;

namespace Hertzole.GoldPlayer
{
    public class PlayerModule
    {
        public GoldPlayerController PlayerController { get; set; }
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

            PlayerController = player;

            if (input != null)
            {
                PlayerInput = input;
            }

            OnInitialize();

            HasBeenInitialized = true;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Only uses for tests to forcefully initialize modules.
        /// </summary>
        /// <param name="input"></param>
        [UnityEngine.TestTools.ExcludeFromCoverage]
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
        public virtual void OnUpdate(float deltaTime, float unscaledDeltaTime) { }

        /// <summary>
        /// Called in FixedUpdate.
        /// </summary>
        public virtual void OnFixedUpdate(float fixedDeltaTime, float fixedUnscaledDeltaTime) { }

        /// <summary>
        /// Called in LateUpdate.
        /// </summary>
        public virtual void OnLateUpdate(float deltaTime, float unscaledDeltaTime) { }

        /// <summary>
        /// Equivalent to Input's GetButton/GetKey function.
        /// It's much more recommended to cache a hash using <see cref="GoldPlayerController.InputNameToHash"/>
        /// and call <see cref="GetButton(int)"/> instead.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        protected bool GetButton(string buttonName)
        {
            return GetButton(GoldPlayerController.InputNameToHash(buttonName));
        }

        /// <summary>
        /// Equivalent to Input's GetButton/GetKey function.
        /// </summary>
        /// <param name="buttonHash">The hash of the action you want to get.</param>
        protected bool GetButton(int buttonHash)
        {
            return PlayerInput.GetButton(buttonHash);
        }

        /// <summary>
        /// Equivalent to Input's GetButtonDown/GetKeyDown function.
        /// It's much more recommended to cache a hash using <see cref="GoldPlayerController.InputNameToHash"/>
        /// and call <see cref="GetButtonDown(int)"/> instead.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        protected bool GetButtonDown(string buttonName)
        {
            return GetButtonDown(GoldPlayerController.InputNameToHash(buttonName));
        }

        /// <summary>
        /// Equivalent to Input's GetButtonDown/GetKeyDown function.
        /// </summary>
        /// <param name="buttonHash">The hash of the action you want to get.</param>
        protected bool GetButtonDown(int buttonHash)
        {
            return PlayerInput.GetButtonDown(buttonHash);
        }

        /// <summary>
        /// Equivalent to Input's GetButtonUp/GetKeyUp function.
        /// It's much more recommended to cache a hash using <see cref="GoldPlayerController.InputNameToHash"/>
        /// and call <see cref="GetButtonUp(int)"/> instead.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        protected bool GetButtonUp(string buttonName)
        {
            return GetButtonUp(GoldPlayerController.InputNameToHash(buttonName));
        }

        /// <summary>
        /// Equivalent to Input's GetButtonUp/GetKeyUp function.
        /// </summary>
        /// <param name="buttonHash">The hash of the action you want to get.</param>
        protected bool GetButtonUp(int buttonHash)
        {
            return PlayerInput.GetButtonUp(buttonHash);
        }

        /// <summary>
        /// Equivalent to Input's GetAxis function.
        /// It's much more recommended to cache a hash using <see cref="GoldPlayerController.InputNameToHash"/>
        /// and call <see cref="GetAxis(int)"/> instead.
        /// </summary>
        /// <param name="axisName">The axis name you want to get.</param>
        protected float GetAxis(string axisName)
        {
            return GetAxis(GoldPlayerController.InputNameToHash(axisName));
        }

        /// <summary>
        /// Equivalent to Input's GetAxis function.
        /// </summary>
        /// <param name="axisHash">The hash of the axis you want to get.</param>
        protected float GetAxis(int axisHash)
        {
            return PlayerInput.GetAxis(axisHash);
        }

        /// <summary>
        /// Equivalent to Input's GetAxisRaw function.
        /// It's much more recommended to cache a hash using <see cref="GoldPlayerController.InputNameToHash"/>
        /// and call <see cref="GetAxisRaw(int)"/> instead.
        /// </summary>
        /// <param name="axisName">The axis name you want to get.</param>
        protected float GetAxisRaw(string axisName)
        {
            return GetAxisRaw(GoldPlayerController.InputNameToHash(axisName));
        }

        /// <summary>
        /// Equivalent to Input's GetAxisRaw function.
        /// </summary>
        /// <param name="axisHash">The hash of the axis you want to get.</param>
        protected float GetAxisRaw(int axisHash)
        {
            return PlayerInput.GetAxisRaw(axisHash);
        }

        /// <summary>
        /// Gets a Vector2 input value.
        /// It's much more recommended to cache a hash using <see cref="GoldPlayerController.InputNameToHash"/>
        /// and call <see cref="GetAxisRaw(int)"/> instead.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected Vector2 GetVector2Input(string action)
        {
            return GetVector2Input(GoldPlayerController.InputNameToHash(action));
        }

        /// <summary>
        /// Gets a Vector2 input value.
        /// </summary>
        /// <param name="actionHash"></param>
        /// <returns></returns>
        protected Vector2 GetVector2Input(int actionHash)
        {
            return PlayerInput.GetVector2(actionHash);
        }

        #region Obsolete
#if UNITY_EDITOR
        [System.Obsolete("Use 'GetButton' without defaultKey parameter instead. This will be removed on build.", true), UnityEngine.TestTools.ExcludeFromCoverage]
        protected bool GetButton(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButton(buttonName); }

        [System.Obsolete("Use 'GetButtonDown' without defaultKey parameter instead. This will be removed on build.", true), UnityEngine.TestTools.ExcludeFromCoverage]
        protected bool GetButtonDown(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButtonDown(buttonName); }

        [System.Obsolete("Use 'GetButtonUp' without defaultKey parameter instead. This will be removed on build.", true), UnityEngine.TestTools.ExcludeFromCoverage]
        protected bool GetButtonUp(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButtonUp(buttonName); }

        [System.Obsolete("Use 'GetAxis' without defaultAxisName parameter instead. This will be removed on build.", true), UnityEngine.TestTools.ExcludeFromCoverage]
        protected float GetAxis(string axisName, string defaultAxisName = "") { return GetAxis(axisName); }

        [System.Obsolete("Use 'GetAxisRaw' without defaultAxisName parameter instead. This will be removed on build.", true), UnityEngine.TestTools.ExcludeFromCoverage]
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
