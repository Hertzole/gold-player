using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    //DOCUMENT: Constants
    public static class Constants
    {
        /// <summary> Used for moving left to right. </summary>
        public const string HORIZONTAL_AXIS = "Horizontal";
        /// <summary> Used for moving up and down. </summary>
        public const string VERTICAL_AXIS = "Vertical";

        public const string MOUSE_X = "Mouse X";
        public const string MOUSE_Y = "Mouse Y";

        public const string JUMP_BUTTON_NAME = "Space";
        public const KeyCode JUMP_DEFAULT_KEY = KeyCode.Space;

        public const string RUN_BUTTON_NAME = "Run";
        public const KeyCode RUN_DEFAULT_KEY = KeyCode.LeftShift;

        public const string CROUCH_BUTTON_NAME = "Crouch";
        public const KeyCode CROUCH_DEFAULT_KEY = KeyCode.C;
    }
}
