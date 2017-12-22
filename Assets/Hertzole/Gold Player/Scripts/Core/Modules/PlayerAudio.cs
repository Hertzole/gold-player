using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    //DOCUMENT: PlayerAudio
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

        protected float m_NextStepTime = 0;
        protected float m_StepCycle = 0;

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
            if (m_BasedOnHeadBob && !PlayerController.HeadBob.EnableBob)
            {
                Debug.LogWarning("Audio on '" + PlayerController.gameObject.name + "' is set to be based on head bob, but head bob isn't actually enabled!", PlayerController);
            }
        }

        public override void OnUpdate()
        {
            DoStepCycle();
            FootstepsHandler();
        }

        protected virtual void DoStepCycle()
        {
            if (m_BasedOnHeadBob)
            {
                if (PlayerController.HeadBob.EnableBob)
                {
                    m_StepCycle = PlayerController.HeadBob.BobCycle;
                }
                else
                {
                    DoStepCycleMath();
                }
            }
            else
            {
                DoStepCycleMath();
            }
        }

        protected virtual void DoStepCycleMath()
        {
            float flatVelocity = new Vector3(CharacterController.velocity.x, 0, CharacterController.velocity.z).magnitude;
            float strideLengthen = 1 + (flatVelocity * 0.3f);
            m_StepCycle += (flatVelocity / strideLengthen) * (Time.deltaTime / m_StepTime);
        }

        protected virtual void FootstepsHandler()
        {
            if (m_EnableAudio)
            {
                if (PlayerController.Movement.IsGrounded)
                {
                    if (!m_PreviouslyGrounded)
                    {
                        PlayLandSound();
                        m_NextStepTime = m_StepCycle + 0.5f;
                    }
                    else
                    {
                        if (m_StepCycle > m_NextStepTime)
                        {
                            m_NextStepTime = m_StepCycle + 0.5f;
                            PlayFootstepSound();
                        }
                    }
                    m_PreviouslyGrounded = true;
                }
                else
                {
                    if (m_PreviouslyGrounded)
                    {
                        PlayJumpSound();
                    }
                    m_PreviouslyGrounded = false;
                }
            }
        }

        public virtual void PlayFootstepSound()
        {
            if (m_EnableAudio)
            {
                if (PlayerController.Movement.IsRunning)
                    m_RunFootsteps.Play(m_FootstepsSource);
                else if (PlayerController.Movement.IsCrouching)
                    m_CrouchFootsteps.Play(m_FootstepsSource);
                else
                    m_WalkFootsteps.Play(m_FootstepsSource);
            }
        }

        public virtual void PlayJumpSound()
        {
            m_Jumping.Play(m_JumpSource);
        }

        public virtual void PlayLandSound()
        {
            m_Landing.Play(m_LandSource);
        }

        public void StopFootstepSound()
        {
            m_FootstepsSource.Stop();
            m_FootstepsSource.clip = null;
        }

        public void StopJumpSound()
        {
            m_JumpSource.Stop();
            m_JumpSource.clip = null;
        }

        public void StopLandSound()
        {
            m_LandSource.Stop();
            m_LandSource.clip = null;
        }

        public void StopAllSounds()
        {
            StopFootstepSound();
            StopJumpSound();
            StopLandSound();
        }
    }
}
