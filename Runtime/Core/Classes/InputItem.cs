#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT && !ENABLE_LEGACY_INPUT_MANAGER // Mark this as obsolete if the new input system is enabled.
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
        public enum InputType { Button = 0, Axis = 1, Vector2 = 2 }

        [SerializeField]
        [Tooltip("The name code will reference the item with.")]
        [FormerlySerializedAs("m_ButtonName")]
        private string buttonName;
        [SerializeField]
        [Tooltip("The type of input this will be.")]
        private InputType type;
        [SerializeField]
        [Tooltip("The name in the Input Manager.")]
        [FormerlySerializedAs("m_InputName")]
        private string inputName;
        [SerializeField]
        [Tooltip("The second name in the Input Manager. This is only used for Vector2 values.")]
        private string inputNameSecondary;
        [SerializeField]
        [Tooltip("The key code for the item.")]
        [FormerlySerializedAs("m_Key")]
        private KeyCode key;

        /// <summary> The name code will reference the item with. </summary>
        public string ButtonName { get { return buttonName; } set { buttonName = value; } }
        /// <summary> The type of input this will be. </summary>
        public InputType Type { get { return type; } set { type = value; } }
        /// <summary> The name in the Input Manager. </summary>
        public string InputName { get { return inputName; } set { inputName = value; } }
        /// <summary> The second name in the Input Manager. This is only used for Vector2 values. </summary>
        public string InputNameSecondary { get { return inputNameSecondary; } set { inputNameSecondary = value; } }
        /// <summary> The key code for the item. </summary>
        public KeyCode Key { get { return key; } set { key = value; } }

        public InputItem(string buttonName, string inputName, KeyCode key)
        {
            this.buttonName = buttonName;
            this.inputName = inputName;
            this.key = key;

            type = InputType.Button;
            inputNameSecondary = null;
        }

        public InputItem(string buttonName, string xAxis, string yAxis)
        {
            this.buttonName = buttonName;
            inputName = xAxis;
            inputNameSecondary = yAxis;
            type = InputType.Vector2;
            key = KeyCode.None;
        }

        public InputItem(string buttonName, InputType type, string inputName, string inputNameSecondary, KeyCode key)
        {
            this.buttonName = buttonName;
            this.type = type;
            this.inputName = inputName;
            this.inputNameSecondary = inputNameSecondary;
            this.key = key;
        }

        public override bool Equals(object obj)
        {
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
            return obj is InputItem item && Equals(item);
#else
            return obj is InputItem && Equals((InputItem)obj);
#endif
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
