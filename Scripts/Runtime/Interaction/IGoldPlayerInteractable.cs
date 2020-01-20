namespace Hertzole.GoldPlayer
{
    /// <summary>
    /// Used to easily hook into the GoldPlayerInteraction component. It will target this interface.
    /// </summary>
    public interface IGoldPlayerInteractable
    {
        bool UseCustomMessage { get; }

        string CustomMessage { get; }

        bool CanInteract { get; }

        bool IsHidden { get; }

        void Interact();
    }
}
