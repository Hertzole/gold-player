#if GOLD_PLAYER_DISABLE_INTERACTION
#define OBSOLETE
#endif

#if OBSOLETE && !UNITY_EDITOR
#define STRIP
#endif

#if !STRIP
namespace Hertzole.GoldPlayer
{
    /// <summary>
    /// Used to easily hook into the GoldPlayerInteraction component. It will target this interface.
    /// </summary>
#if OBSOLETE
    [System.Obsolete("Gold Player Interaction has been disabled. IGoldPlayerInteractable will be removed on build.")]    
#endif
    public interface IGoldPlayerInteractable
    {
        bool UseCustomMessage { get; }

        string CustomMessage { get; }

        bool CanInteract { get; }

        bool IsHidden { get; }

        void Interact();
    }
}
#endif
