using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    /// <summary>
    /// Used to make custom player audio behaviours.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public abstract class PlayerAudioBehaviour : MonoBehaviour
    {
        /// <summary> The player controller. </summary>
        protected GoldPlayerController PlayerController { get; private set; }
        /// <summary> The player input. </summary>
        protected GoldInput PlayerInput { get; private set; }

        /// <summary> The audio source where all the footsteps sounds will be played. </summary>
        protected AudioSource FootstepsSource { get; private set; }
        /// <summary> The audio source where all the jump sounds will be played. </summary>
        protected AudioSource JumpSource { get; private set; }
        /// <summary> The audio source where all the land sounds will be played. </summary>
        protected AudioSource LandSource { get; private set; }

        /// <summary>
        /// Initializes the audio behaviour.
        /// </summary>
        /// <param name="footstepsSource"></param>
        /// <param name="jumpSource"></param>
        /// <param name="landSource"></param>
        public void Initialize(GoldPlayerController playerController, GoldInput input, AudioSource footstepsSource, AudioSource jumpSource, AudioSource landSource)
        {
            PlayerController = playerController;
            PlayerInput = input;

            FootstepsSource = footstepsSource;
            JumpSource = jumpSource;
            LandSource = landSource;
        }

        /// <summary>
        /// Called when the behaviour has been initialized.
        /// </summary>
        protected virtual void OnInitialized() { }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// Called at every fixed framerate frame.
        /// </summary>
        public virtual void OnFixedUpdate() { }

        /// <summary>
        /// Called every frame, after OnUpdate.
        /// </summary>
        public virtual void OnLateUpdate() { }

        /// <summary>
        /// Called when a footstep should be played.
        /// </summary>
        public abstract void PlayFoostepSound();

        /// <summary>
        /// Called when a jump sound should be played.
        /// </summary>
        public abstract void PlayJumpSound();

        /// <summary>
        /// Called when a land sound should be played.
        /// </summary>
        public abstract void PlayLandSound();
    }
}
