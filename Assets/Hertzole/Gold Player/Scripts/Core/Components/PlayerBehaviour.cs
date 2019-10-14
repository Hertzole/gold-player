using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    /// <summary>
    /// Used for external player components that need to be a MonoBehaviour.
    /// Features some shortcuts in regard to the player.
    /// </summary>
    [AddComponentMenu("")]
    public abstract class PlayerBehaviour : MonoBehaviour
    {
        private GoldInput playerInput;
        private GoldPlayerController playerController;

        /// <summary> Player input shortcut. </summary>
        protected GoldInput PlayerInput { get { if (!playerInput) { playerInput = GetComponent<GoldInput>(); } return playerInput; } }
        /// <summary> Player controller shortcut. It is not certain that this actually exists on the player! </summary>
        public GoldPlayerController PlayerController { get { if (!playerController) { playerController = GetComponent<GoldPlayerController>(); } return playerController; } }

        [System.Obsolete("Use 'GetButton' without defaultKey parameter instead.")]
        protected bool GetButton(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButton(buttonName); }

        /// <summary>
        /// Equivalent to Input's GetButton/GetKey function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        /// <param name="defaultKey">A default key in case the input script is null.</param>
        protected bool GetButton(string buttonName)
        {
            return PlayerInput.GetButton(buttonName);
        }

        [System.Obsolete("Use 'GetButtonDown' without defaultKey parameter instead.")]
        protected bool GetButtonDown(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButtonDown(buttonName); }

        /// <summary>
        /// Equivalent to Input's GetButtonDown/GetKeyDown function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        /// <param name="defaultKey">A default key in case the input script is null.</param>
        protected bool GetButtonDown(string buttonName)
        {
            return PlayerInput.GetButtonDown(buttonName);
        }

        [System.Obsolete("Use 'GetButtonUp' without defaultKey parameter instead.")]
        protected bool GetButtonUp(string buttonName, KeyCode defaultKey = KeyCode.None) { return GetButtonUp(buttonName); }

        /// <summary>
        /// Equivalent to Input's GetButtonUp/GetKeyUp function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        /// <param name="defaultKey">A default key in case the input script is null.</param>
        protected bool GetButtonUp(string buttonName)
        {
            return PlayerInput.GetButtonUp(buttonName);
        }

        /// <summary>
        /// Equivalent to Input's GetAxis function.
        /// </summary>
        /// <param name="axisName">The axis name you want to get.</param>
        /// <param name="defaultAxisName">A default axis name in case the input script is null.</param>
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
        [System.Obsolete("GetAxis does nothing with the new input system.")]
#endif
        protected float GetAxis(string axisName, string defaultAxisName = "")
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            return 0;
#else
            // If the default axis name is blank, use the one provided in axisName.
            if (string.IsNullOrEmpty(defaultAxisName))
            {
                defaultAxisName = axisName;
            }

            // If player input isn't null, get the axis using that. Else use the default axis name.
            return PlayerInput != null ? PlayerInput.GetAxis(axisName) : Input.GetAxis(defaultAxisName);
#endif
        }

        /// <summary>
        /// Equivalent to Input's GetAxisRaw function.
        /// </summary>
        /// <param name="axisName">The axis name you want to get.</param>
        /// <param name="defaultAxisName">A default axis name in case the input script is null.</param>
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
        [System.Obsolete("GetAxisRaw does nothing with the new input system.")]
#endif
        protected float GetAxisRaw(string axisName, string defaultAxisName = "")
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            return 0;
#else
            // If the default axis name is blank, use the one provided in axisName.
            if (string.IsNullOrEmpty(defaultAxisName))
            {
                defaultAxisName = axisName;
            }

            // If player input isn't null, get the axis using that. Else use the default axis name.
            return PlayerInput != null ? PlayerInput.GetAxisRaw(axisName) : Input.GetAxisRaw(defaultAxisName);
#endif
        }

#if !ENABLE_INPUT_SYSTEM
        [System.Obsolete("GetVector2Input does nothing with the Input Manager.")]
#endif
        protected Vector2 GetVector2Input(string action)
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            return PlayerInput.GetVector2(action);
#else
            return Vector2.zero;
#endif
        }
    }
}
