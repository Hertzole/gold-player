using UnityEngine;

namespace Hertzole.GoldPlayer
{
    /// <summary>
    /// Used for external player components that need to be a MonoBehaviour.
    /// Features some shortcuts in regard to the player.
    /// </summary>
    [AddComponentMenu("")]
    public abstract class PlayerBehaviour : MonoBehaviour
    {
        private IGoldInput playerInput;
        private GoldPlayerController playerController;

        /// <summary> Player input shortcut. </summary>
        protected IGoldInput PlayerInput { get { if (playerInput == null) { playerInput = GetComponent<IGoldInput>(); } return playerInput; } }
        /// <summary> Player controller shortcut. It is not certain that this actually exists on the player! </summary>
        public GoldPlayerController PlayerController { get { if (!playerController) { playerController = GetComponent<GoldPlayerController>(); } return playerController; } }

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
        [System.Obsolete("Use 'GetButton' without defaultKey parameter instead. This will be removed on build.", true)]
        [UnityEngine.TestTools.ExcludeFromCoverage]
        protected bool GetButton(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButton(buttonName); }

        [System.Obsolete("Use 'GetButtonDown' without defaultKey parameter instead. This will be removed on build.", true)]
        [UnityEngine.TestTools.ExcludeFromCoverage]
        protected bool GetButtonDown(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButtonDown(buttonName); }

        [System.Obsolete("Use 'GetButtonUp' without defaultKey parameter instead. This will be removed on build.", true)]
        [UnityEngine.TestTools.ExcludeFromCoverage]
        protected bool GetButtonUp(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButtonUp(buttonName); }

        [System.Obsolete("Use 'GetAxis' without defaultAxisName parameter instead. This will be removed on build.", true)]
        [UnityEngine.TestTools.ExcludeFromCoverage]
        protected float GetAxis(string axisName, string defaultAxisName = "") { return GetAxis(axisName); }

        [System.Obsolete("Use 'GetAxisRaw' without defaultAxisName parameter instead. This will be removed on build.", true)]
        [UnityEngine.TestTools.ExcludeFromCoverage]
        protected float GetAxisRaw(string axisName, string defaultAxisName = "") { return GetAxisRaw(axisName); }
#endif
        #endregion
    }
}
