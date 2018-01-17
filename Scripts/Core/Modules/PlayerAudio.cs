using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    /// <summary>
    /// Used to apply audio to Gold Player.
    /// </summary>
    [System.Serializable]
    public class PlayerAudio : PlayerModule
    {
        [SerializeField]
        [Tooltip("Determines if any audio should be played.")]
        private bool m_EnableAudio = true;

        [Space]

        [SerializeField]
        [Tooltip("Determines if the audio should be based on head bob.")]
        private bool m_BasedOnHeadBob = true;
        [SerializeField]
        [Tooltip("Sets how frequent the footsteps are.")]
        private float m_StepTime = 1.5f;

        [Space]

        [SerializeField]
        [Tooltip("All the audio settings that plays when walking.")]
        private AudioItem m_WalkFootsteps = new AudioItem(true, true, 1f, 0.9f, 1.1f, true, 1f);
        [SerializeField]
        [Tooltip("All the audio settings that plays when running.")]
        private AudioItem m_RunFootsteps = new AudioItem(true, true, 1.4f, 1.4f, 1.6f, true, 1f);
        [SerializeField]
        [Tooltip("All the audio settings that plays when crouching.")]
        private AudioItem m_CrouchFootsteps = new AudioItem(true, true, 1f, 0.9f, 1.1f, true, 0.4f);
        [SerializeField]
        [Tooltip("All the audio settings that plays when jumping.")]
        private AudioItem m_Jumping = new AudioItem(true, true, 1f, 0.9f, 1.1f, true, 1f);
        [SerializeField]
        [Tooltip("All the audio settings that plays when landing.")]
        private AudioItem m_Landing = new AudioItem(true, true, 1f, 0.9f, 1.1f, true, 1f);

        [Space]

        [SerializeField]
        [Tooltip("The audio source where all the footsteps sounds will be played.")]
        private AudioSource m_FootstepsSource = null;
        [SerializeField]
        [Tooltip("The audio source where all the jump sounds will be played.")]
        private AudioSource m_JumpSource = null;
        [SerializeField]
        [Tooltip("The audio source where all the land sounds will be played.")]
        private AudioSource m_LandSource = null;

        // When the next step sound should occur.
        protected float m_NextStepTime = 0;
        // Where in the cycle the steps are.
        protected float m_StepCycle = 0;

        // Check if the player was previously grounded.
        protected bool m_PreviouslyGrounded = true;

        /// <summary> Determines if any audio should be played. </summary>
        public bool EnableAudio { get { return m_EnableAudio; } set { m_EnableAudio = value; } }
        /// <summary> Determines if the audio should be based on head bob. </summary>
        public bool BasedOnHeadBob { get { return m_BasedOnHeadBob; } set { m_BasedOnHeadBob = value; } }
        /// <summary> Sets how frequent the footsteps are. </summary>
        public float StepTime { get { return m_StepTime; } set { m_StepTime = value; } }

        /// <summary> All the audio settings that plays when walking. </summary>
        public AudioItem WalkFootsteps { get { return m_WalkFootsteps; } set { m_WalkFootsteps = value; } }
        /// <summary> All the audio settings that plays when running. </summary>
        public AudioItem RunFootsteps { get { return m_RunFootsteps; } set { m_RunFootsteps = value; } }
        /// <summary> All the audio settings that plays when crouching. </summary>
        public AudioItem CrouchFootsteps { get { return m_CrouchFootsteps; } set { m_CrouchFootsteps = value; } }
        /// <summary> All the audio settings that plays when jumping. </summary>
        public AudioItem Jumping { get { return m_Jumping; } set { m_Jumping = value; } }
        /// <summary> All the audio settings that plays when landing. </summary>
        public AudioItem Landing { get { return m_Landing; } set { m_Landing = value; } }

        /// <summary> The audio source where all the footsteps sounds will be played. </summary>
        public AudioSource FootstepsSource { get { return m_FootstepsSource; } set { m_FootstepsSource = value; } }
        /// <summary> The audio source where all the jump sounds will be played. </summary>
        public AudioSource JumpSource { get { return m_JumpSource; } set { m_JumpSource = value; } }
        /// <summary> The audio source where all the land sounds will be played. </summary>
        public AudioSource LandSource { get { return m_LandSource; } set { m_LandSource = value; } }

        protected override void OnInit()
        {
            // If based on head bob is true but head bob is not enabled, complain.
            if (m_BasedOnHeadBob && !PlayerController.HeadBob.EnableBob)
            {
                Debug.LogWarning("Audio on '" + PlayerController.gameObject.name + "' is set to be based on head bob, but head bob isn't actually enabled!", PlayerController);
            }
        }

        public override void OnUpdate()
        {
            DoStepCycle();
            AudioHandler();
        }

        /// <summary>
        /// Handles the step cycle logic.
        /// </summary>
        protected virtual void DoStepCycle()
        {
            // If the step cycle should be based on head bob, get it from there.
            // Else do it by itself.
            if (m_BasedOnHeadBob)
            {
                // If head bob is enabled, set the step cycle to the bob cycle.
                // Else do it by itself.
                if (PlayerController.HeadBob.EnableBob)
                {
                    // Head bob was enabled. Set the step cycle to the bob cycle.
                    m_StepCycle = PlayerController.HeadBob.BobCycle;
                }
                else
                {
                    // Head bob was disabled. Do the math here.
                    DoStepCycleMath();
                }
            }
            else
            {
                // It should not be based on head bob. Do the math here.
                DoStepCycleMath();
            }
        }

        /// <summary>
        /// Calculates the step cycle.
        /// </summary>
        protected virtual void DoStepCycleMath()
        {
            // Get the velocity from the character controller.
            float flatVelocity = new Vector3(CharacterController.velocity.x, 0, CharacterController.velocity.z).magnitude;
            // Calculate some stride thing. (Not 100% what everything here does)
            float strideLengthen = 1 + (flatVelocity * 0.3f);
            m_StepCycle += (flatVelocity / strideLengthen) * (Time.deltaTime / m_StepTime);
        }

        /// <summary>
        /// Handles all the audio features.
        /// </summary>
        protected virtual void AudioHandler()
        {
            // Only run if the audio feature is enabled.
            if (m_EnableAudio)
            {
                // If the player is grounded, play the footsteps and land sound.
                // Else play the jump sound.
                if (PlayerController.Movement.IsGrounded)
                {
                    // If the player wasn't previously grounded, play the land sound.
                    // Else play the footsteps sounds.
                    if (!m_PreviouslyGrounded)
                    {
                        // Play the land sound.
                        PlayLandSound();
                    }
                    else
                    {
                        // If the step cycle is above the next step time, play a footstep sound.
                        if (m_StepCycle > m_NextStepTime)
                        {
                            // Play a footstep sound.
                            PlayFootstepSound();
                        }
                    }
                    // Set previous grounded to true, as the player was previously grounded.
                    m_PreviouslyGrounded = true;
                }
                else
                {
                    // If the player was just grounded, play the jump sound.
                    if (m_PreviouslyGrounded)
                    {
                        // Play the jump sound.
                        PlayJumpSound();
                    }
                    // The player is no longer grounded, so set previously grounded to false.
                    m_PreviouslyGrounded = false;
                }
            }
        }

        /// <summary>
        /// Plays a random footstep sound.
        /// </summary>
        public virtual void PlayFootstepSound()
        {
            // Only run if the audio feature is enabled.
            if (m_EnableAudio)
            {
                // If the player is running, play the running footsteps.
                // Else if the player is crouching, play the crouching footsteps.
                // Else just play the walking footsteps.
                if (PlayerController.Movement.IsRunning)
                    m_RunFootsteps.Play(m_FootstepsSource);
                else if (PlayerController.Movement.IsCrouching)
                    m_CrouchFootsteps.Play(m_FootstepsSource);
                else
                    m_WalkFootsteps.Play(m_FootstepsSource);

                // Add some time to the next step time.
                m_NextStepTime = m_StepCycle + 0.5f;
            }
        }

        /// <summary>
        /// Plays a random jump sound.
        /// </summary>
        public virtual void PlayJumpSound()
        {
            // Only play if the audio feature is enabled and the jump sound is enabled.
            if (m_EnableAudio && m_Jumping.Enabled)
            {
                m_Jumping.Play(m_JumpSource);
            }
        }

        /// <summary>
        /// Plays a random land sound.
        /// </summary>
        public virtual void PlayLandSound()
        {
            // Only play if the audio feature is enabled and the landing sound is enabled.
            if (m_EnableAudio && m_Landing.Enabled)
            {
                m_Landing.Play(m_LandSource);
                // Add some time to the next step time.
                m_NextStepTime = m_StepCycle + 0.5f;
            }
        }

        /// <summary>
        /// Stops the current footstep sound for playing.
        /// </summary>
        public void StopFootstepSound()
        {
            m_FootstepsSource.Stop();
            m_FootstepsSource.clip = null;
        }

        /// <summary>
        /// Stops the current jump sound from playing.
        /// </summary>
        public void StopJumpSound()
        {
            m_JumpSource.Stop();
            m_JumpSource.clip = null;
        }

        /// <summary>
        /// Stops the current land sound from playing.
        /// </summary>
        public void StopLandSound()
        {
            m_LandSource.Stop();
            m_LandSource.clip = null;
        }

        /// <summary>
        /// Stops all sounds from playing.
        /// </summary>
        public void StopAllSounds()
        {
            StopFootstepSound();
            StopJumpSound();
            StopLandSound();
        }
    }
}
