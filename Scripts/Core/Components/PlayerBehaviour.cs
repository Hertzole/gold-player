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

        /// <summary> Player input shortcut. It is not certain that this actually exists on the player! </summary>
        protected GoldInput PlayerInput { get { if (!playerInput) { playerInput = GetComponent<GoldInput>(); } return playerInput; } }
        /// <summary> Player controller shortcut. It is not certain that this actually exists on the player! </summary>
        public GoldPlayerController PlayerController { get { if (!playerController) { playerController = GetComponent<GoldPlayerController>(); } return playerController; } }

        /// <summary>
        /// Equivalent to Input's GetButton/GetKey function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        /// <param name="defaultKey">A default key in case the input script is null.</param>
        protected bool GetButton(string buttonName, KeyCode defaultKey = KeyCode.None)
        {
            // If player input isn't null, get the key using that. Else use the default key.
            return PlayerInput != null ? PlayerInput.GetButton(buttonName) : Input.GetKey(defaultKey);
        }

        /// <summary>
        /// Equivalent to Input's GetButtonDown/GetKeyDown function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        /// <param name="defaultKey">A default key in case the input script is null.</param>
        protected bool GetButtonDown(string buttonName, KeyCode defaultKey = KeyCode.None)
        {
            // If player input isn't null, get the key using that. Else use the default key.
            return PlayerInput != null ? PlayerInput.GetButtonDown(buttonName) : Input.GetKeyDown(defaultKey);
        }

        /// <summary>
        /// Equivalent to Input's GetButtonUp/GetKeyUp function.
        /// </summary>
        /// <param name="buttonName">The button name you want to get.</param>
        /// <param name="defaultKey">A default key in case the input script is null.</param>
        protected bool GetButtonUp(string buttonName, KeyCode defaultKey = KeyCode.None)
        {
            // If player input isn't null, get the key using that. Else use the default key.
            return PlayerInput != null ? PlayerInput.GetButtonUp(buttonName) : Input.GetKeyUp(defaultKey);
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
            {
                defaultAxisName = axisName;
            }

            // If player input isn't null, get the axis using that. Else use the default axis name.
            return PlayerInput != null ? PlayerInput.GetAxis(axisName) : Input.GetAxis(defaultAxisName);
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
            {
                defaultAxisName = axisName;
            }

            // If player input isn't null, get the axis using that. Else use the default axis name.
            return PlayerInput != null ? PlayerInput.GetAxisRaw(axisName) : Input.GetAxisRaw(defaultAxisName);
        }
    }
}
