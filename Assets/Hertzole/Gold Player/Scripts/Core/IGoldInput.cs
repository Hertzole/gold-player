using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    public interface IGoldInput
    {
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

#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
        /// <summary>
        /// Returns a Vector2 axis.
        /// </summary>
        Vector2 GetVector2(string actionName);
#endif
    }
}