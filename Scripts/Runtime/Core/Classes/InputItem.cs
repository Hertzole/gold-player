#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT // Mark this as obsolete if the new input system is enabled.
#define OBSOLETE
#endif

#if OBSOLETE && !UNITY_EDITOR // If it's obsolete and not in the editor, remove it.
#define STRIP
#endif

#if !STRIP
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
    /// <summary>
    /// Used in Gold Player to make Input a little bit easier to handle.
    /// </summary>
#if OBSOLETE
    [Obsolete("You're using the new Input System so this will become useless. This will be removed on build.", true)]
#endif
    [Serializable]
    public struct InputItem : IEquatable<InputItem>
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

        public override bool Equals(object obj)
        {
            return obj is InputItem item && Equals(item);
        }

        public bool Equals(InputItem other)
        {
            return key == other.key && buttonName == other.buttonName && inputName == other.inputName;
        }

        public override int GetHashCode()
        {
            int hashCode = -1721682042;
            hashCode = hashCode * -1521134295 + key.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(buttonName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(inputName);
            return hashCode;
        }

        public static bool operator ==(InputItem left, InputItem right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InputItem left, InputItem right)
        {
            return !(left == right);
        }
    }
}
#endif
