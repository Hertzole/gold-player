using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer.Core
{
    /// <summary>
    /// Used in Gold Player to make Input a little bit easier to handle.
    /// </summary>
    [Serializable]
    public struct InputItem
    {
        [SerializeField]
        [Tooltip("The name code will reference the item with.")]
        [FormerlySerializedAs("m_ButtonName")]
        private string buttonName;
        [SerializeField]
        [Tooltip("The name in the Input Manager.")]
        [FormerlySerializedAs("m_InputName")]
        private string inputName;
        [SerializeField]
        [Tooltip("The key code for the item.")]
        [FormerlySerializedAs("m_Key")]
        private KeyCode key;

        /// <summary> The name code will reference the item with. </summary>
        public string ButtonName { get { return buttonName; } set { buttonName = value; } }
        /// <summary> The name in the Input Manager. </summary>
        public string InputName { get { return inputName; } set { inputName = value; } }
        /// <summary> The key code for the item. </summary>
        public KeyCode Key { get { return key; } set { key = value; } }

        public InputItem(string buttonName, string inputName, KeyCode key)
        {
            this.buttonName = buttonName;
            this.inputName = inputName;
            this.key = key;
        }
    }
}
