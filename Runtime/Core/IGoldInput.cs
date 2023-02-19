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
        /// <param name="actionIndex">The action to enable.</param>
        void EnableAction(int actionIndex);

        /// <summary>
        /// Disables a specific action.
        /// </summary>
        /// <param name="actionIndex">The action to disable.</param>
        void DisableAction(int actionIndex);

        /// <summary>
        /// Returns true if the button is being pressed.
        /// </summary>
        bool GetButton(int buttonHash);

        /// <summary>
        /// Return true if the button was pressed this frame.
        /// </summary>
        bool GetButtonDown(int buttonHash);

        /// <summary>
        /// Returns true if the button was released this frame.
        /// </summary>
        bool GetButtonUp(int buttonHash);

        /// <summary>
        /// Returns the float value of an axis with some smoothing applied.
        /// </summary>
        float GetAxis(int axisHash);

        /// <summary>
        /// Returns the float value of an axis with no smoothing applied.
        /// </summary>
        float GetAxisRaw(int axisHash);
        
        /// <summary>
        /// Returns a Vector2 axis.
        /// </summary>
        Vector2 GetVector2(int actionHash);
    }
}
