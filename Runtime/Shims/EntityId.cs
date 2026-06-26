#if !UNITY_6000_3_OR_NEWER
using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    /// <summary>
    ///     Represents Unity's new EntityId for versions below Unity 6.4.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    internal struct EntityId : IEquatable<EntityId>
    {
        private readonly int value;

        public EntityId(int value)
        {
            this.value = value;
        }

        /// <inheritdoc />
        public bool Equals(EntityId other)
        {
            return value == other.value;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is EntityId other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return value;
        }

        public static bool operator ==(EntityId left, EntityId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityId left, EntityId right)
        {
            return !left.Equals(right);
        }
    }
}
#endif