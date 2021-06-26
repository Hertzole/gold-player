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
        /// <summary> Determines if the interactable should use a custom message. </summary>
        bool UseCustomMessage { get; }

        /// <summary> The custom message to display. </summary>
        string CustomMessage { get; }

        /// <summary> Determines if the object can be interacted with. </summary>
        bool CanInteract { get; }

        /// <summary> Determines if a interactable prompt should show up. </summary>
        bool IsHidden { get; }

        /// <summary> Invokes the interact event. </summary>
        void Interact();
    }
}
#endif
