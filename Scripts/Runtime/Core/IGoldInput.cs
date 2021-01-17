using UnityEngine;

namespace Hertzole.GoldPlayer
{
    public interface IGoldInput
    {
        /// <summary>
        /// Enables all actions.
        /// </summary>
        void EnableInput();

        /// <summary>
        /// Disables all actions.
        /// </summary>
        void DisableInput();

        /// <summary>
        /// Enable a specific action.
        /// </summary>
        /// <param name="action">The action to enable.</param>
        void EnableAction(string action);

        /// <summary>
        /// Disables a specific action.
        /// </summary>
        /// <param name="action">The action to disable.</param>
        void DisableAction(string action);

        /// <summary>
        /// Returns true if the button is being pressed.
        /// </summary>
        bool GetButton(string buttonName);

        /// <summary>
        /// Return true if the button was pressed this frame.
        /// </summary>
        bool GetButtonDown(string buttonName);

        /// <summary>
        /// Returns true if the button was released this frame.
        /// </summary>
        bool GetButtonUp(string buttoName);

        /// <summary>
        /// Returns the float value of an axis with some smoothing applied.
        /// </summary>
        float GetAxis(string axis);

        /// <summary>
        /// Returns the float value of an axis with no smoothing applied.
        /// </summary>
        float GetAxisRaw(string axis);

#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
        /// <summary>
        /// Returns a Vector2 axis.
        /// </summary>
        Vector2 GetVector2(string actionName);
#endif
    }
}
