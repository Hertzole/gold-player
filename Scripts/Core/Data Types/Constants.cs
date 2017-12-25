using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    /// <summary>
    /// A collection of constant values related to Gold Player.
    /// </summary>
    public static class GoldPlayerConstants
    {
        /// <summary> Used for moving left to right. </summary>
        public const string HORIZONTAL_AXIS = "Horizontal";
        /// <summary> Used for moving up and down. </summary>
        public const string VERTICAL_AXIS = "Vertical";

        /// <summary> Used for mouse left and right. </summary>
        public const string MOUSE_X = "Mouse X";
        /// <summary> Used for mouse up and down. </summary>
        public const string MOUSE_Y = "Mouse Y";

        /// <summary> Used for the jump input. </summary>
        public const string JUMP_BUTTON_NAME = "Jump";
        /// <summary> Used for the default jump key. </summary>
        public const KeyCode JUMP_DEFAULT_KEY = KeyCode.Space;

        /// <summary> Used for the run input. </summary>
        public const string RUN_BUTTON_NAME = "Run";
        /// <summary> Used for the default run key. </summary>
        public const KeyCode RUN_DEFAULT_KEY = KeyCode.LeftShift;

        /// <summary> Used for the crouch input. </summary>
        public const string CROUCH_BUTTON_NAME = "Crouch";
        /// <summary> Used for the default crouch key. </summary>
        public const KeyCode CROUCH_DEFAULT_KEY = KeyCode.C;
    }
}
