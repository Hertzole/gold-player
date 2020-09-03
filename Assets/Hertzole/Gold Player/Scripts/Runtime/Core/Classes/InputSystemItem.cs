#if !ENABLE_INPUT_SYSTEM || !GOLD_PLAYER_NEW_INPUT
#define OBSOLETE
#endif

#if OBSOLETE && !UNITY_EDITOR // If it's obsolete and not in the editor, remove it.
#define STRIP
#endif

#if !STRIP

using System;
using UnityEngine.InputSystem;

namespace Hertzole.GoldPlayer
{
    [Serializable]
#if OBSOLETE
    [Obsolete("You're not using the new Input System so this struct will be useless.")]
#endif
    public struct InputSystemItem : IEquatable<InputSystemItem>
    {
#pragma warning disable CA2235 // Mark all non-serializable fields
        public string actionName;
#if !OBSOLETE
        public InputActionReference action;

        public InputSystemItem(string actionName, InputActionReference action)
        {
            this.actionName = actionName;
            this.action = action;
        }
#endif

        public override bool Equals(object obj)
        {
            return obj != null && obj is InputSystemItem item && Equals(item);
        }

        public bool Equals(InputSystemItem other)
        {
#if !OBSOLETE
            return other.actionName == actionName && other.action == action;
#else
            return false;
#endif
        }

        public override int GetHashCode()
        {
            return (action.action.name + "." + actionName).GetHashCode();
        }

        public static bool operator ==(InputSystemItem left, InputSystemItem right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(InputSystemItem left, InputSystemItem right)
        {
            return !(left == right);
        }
#pragma warning restore CA2235 // Mark all non-serializable fields
    }
}
#endif
