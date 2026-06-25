using UnityEngine;

namespace Hertzole.GoldPlayer
{
    internal static class EntityIdExtensions
    {
        public static EntityId GetEntityId(this RaycastHit hit)
        {
#if UNITY_6000_4_OR_NEWER
            return hit.colliderEntityId;
#else
            return new EntityId(hit.colliderInstanceID);
#endif
        }
    }
}