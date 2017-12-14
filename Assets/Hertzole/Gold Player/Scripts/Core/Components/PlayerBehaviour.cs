using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    /// <summary>
    /// Used for external player components that need to be a MonoBehaviour.
    /// Features some shortcuts in regard to the player.
    /// </summary>
    public class PlayerBehaviour : MonoBehaviour
    {
        private GoldInput m_PlayerInput;
        protected GoldInput PlayerInput { get { return m_PlayerInput; } }

        // Use this for initialization
        private void Awake()
        {
            m_PlayerInput = GetComponent<GoldInput>();

            OnAwake();
        }

        /// <summary>
        /// Called in Awake.
        /// </summary>
        protected virtual void OnAwake() { }

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
    }
}
