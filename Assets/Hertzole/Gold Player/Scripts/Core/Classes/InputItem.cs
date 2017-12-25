using System;
using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    /// <summary>
    /// Used in Gold Player to make Input a little bit easier to handle.
    /// </summary>
    [Serializable]
    public class InputItem
    {
        [SerializeField]
        [Tooltip("The name code will reference the item with.")]
        private string m_ButtonName;
        [SerializeField]
        [Tooltip("The name in the Input Manager.")]
        private string m_InputName;
        [SerializeField]
        [Tooltip("The key code for the item.")]
        private KeyCode m_Key;

        /// <summary> The name code will reference the item with. </summary>
        public string ButtonName { get { return m_ButtonName; } set { m_ButtonName = value; } }
        /// <summary> The name in the Input Manager. </summary>
        public string InputName { get { return m_InputName; } set { m_InputName = value; } }
        /// <summary> The key code for the item. </summary>
        public KeyCode Key { get { return m_Key; } set { m_Key = value; } }

    }
}
