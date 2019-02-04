namespace Hertzole.GoldPlayer.Core
{
    public enum RunAction
    {
        FasterThanRunSpeed = 0,
        FasterThanRunSpeedAndPressingRun = 1
    }

    public enum GroundCheckType
    {
        Sphere,
        Box
    }

    public enum AudioTypes
    {
        Standard = 0,
        Custom = 2
    }

    /// <summary>
    /// Type of toggling mode for running.
    /// </summary>
    public enum RunToggleMode
    {
        /// <summary>
        /// Toggle is off and run button must be held down to run.
        /// </summary>
        Off = 0,

        /// <summary>
        /// Running is permanently toggled. Player must press run
        /// button again to toggle off.
        /// </summary>
        Permanent = 1,

        /// <summary>
        /// Running is toggled on until the player presses the run button
        /// again OR there is no movement input from the player.
        /// </summary>
        UntilNoInput = 2,
    }

    /// <summary>
    /// Type of toggling mode for crouching.
    /// </summary>
    public enum CrouchToggleMode
    {
        /// <summary>
        /// Toggle is off and crouch button must be held down to crouch.
        /// </summary>
        Off = 0,

        /// <summary>
        /// Crouching is permanently toggled. Player must press crouch
        /// button again to toggle off.
        /// </summary>
        Permanent = 1,
    }
}
