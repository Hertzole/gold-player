using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    /// <summary>
    /// Used for easily playing audio clip(s) with parameters.
    /// </summary>
    [System.Serializable]
    public class AudioItem
    {
        [SerializeField]
        [Tooltip("Determines if this audio should be enabled.")]
        private bool m_Enabled = true;
        [SerializeField]
        [Tooltip("Determines if the pitch should be randomized.")]
        private bool m_RandomPitch = true;
        [SerializeField]
        [Tooltip("The pitch that the audio should play at.")]
        private float m_Pitch = 1f;
        [SerializeField]
        [Tooltip("The minimum pitch the audio can play at.")]
        private float m_MinPitch = 0.9f;
        [SerializeField]
        [Tooltip("The maximium pitch the audio can play at.")]
        private float m_MaxPitch = 1.1f;
        [SerializeField]
        [Tooltip("Determines if the volume should be changed when playing.")]
        private bool m_ChangeVolume = false;
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The volume that should be set when playing.")]
        private float m_Volume = 1f;
        [SerializeField]
        [Tooltip("All the audio clips.")]
        private AudioClip[] m_AudioClips;

        /// <summary> Determines if this audio should be enabled. </summary>
        public bool Enabled { get { return m_Enabled; } set { m_Enabled = value; } }
        /// <summary> Determines if the pitch should be randomized. </summary>
        public bool RandomPitch { get { return m_RandomPitch; } set { m_RandomPitch = value; } }
        /// <summary> The pitch that the audio should play at. </summary>
        public float Pitch { get { return m_Pitch; } set { m_Pitch = value; } }
        /// <summary> The minimum pitch the audio can play at. </summary>
        public float MinPitch { get { return m_MinPitch; } set { m_MinPitch = value; } }
        /// <summary> The maximium pitch the audio can play at. </summary>
        public float MaxPitch { get { return m_MaxPitch; } set { m_MaxPitch = value; } }
        /// <summary> Determines if the volume should be changed when playing. </summary>
        public bool ChangeVolume { get { return m_ChangeVolume; } set { m_ChangeVolume = value; } }
        /// <summary> The volume that should be set when playing. </summary>
        public float Volume { get { return m_Volume; } set { m_Volume = value; } }
        /// <summary> All the audio clips. </summary>
        public AudioClip[] AudioClips { get { return m_AudioClips; } set { m_AudioClips = value; } }

        public AudioItem() { }

        public AudioItem(bool enabled)
        {
            // Set enabled to the provided enabled parameter.
            m_Enabled = enabled;
        }

        public AudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch)
        {
            // Set enabled to the provided enabled parameter.
            m_Enabled = enabled;
            // Set random pitch to the provided random pitch paramater.
            m_RandomPitch = randomPitch;
            // Set pitch to the provided pitch paramater.
            m_Pitch = pitch;
            // Set the minimum pitch to the provided minimum pitch paramater.
            m_MinPitch = minPitch;
            // Set the maximum pitch to the provided maximum pitch paramater.
            m_MaxPitch = maxPitch;
        }

        public AudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch, bool changeVolume, float volume)
        {
            // Set enabled to the provided enabled parameter.
            m_Enabled = enabled;
            // Set random pitch to the provided random pitch paramater.
            m_RandomPitch = randomPitch;
            // Set pitch to the provided pitch paramater.
            m_Pitch = pitch;
            // Set the minimum pitch to the provided minimum pitch paramater.
            m_MinPitch = minPitch;
            // Set the maximum pitch to the provided maximum pitch paramater.
            m_MaxPitch = maxPitch;
            // Set change volume to the provided change volume parameter.
            m_ChangeVolume = changeVolume;
            // Set the volume to the provided volume parameter.
            m_Volume = volume;
        }

        public AudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch, bool changeVolume, float volume, AudioClip audioClip)
        {
            // Set enabled to the provided enabled parameter.
            m_Enabled = enabled;
            // Set random pitch to the provided random pitch paramater.
            m_RandomPitch = randomPitch;
            // Set pitch to the provided pitch paramater.
            m_Pitch = pitch;
            // Set the minimum pitch to the provided minimum pitch paramater.
            m_MinPitch = minPitch;
            // Set the maximum pitch to the provided maximum pitch paramater.
            m_MaxPitch = maxPitch;
            // Set change volume to the provided change volume parameter.
            m_ChangeVolume = changeVolume;
            // Set the volume to the provided volume parameter.
            m_Volume = volume;
            // Set audio clips to an array with only one clip that was provided.
            m_AudioClips = new AudioClip[1] { audioClip };
        }

        public AudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch, bool changeVolume, float volume, AudioClip[] audioClips)
        {
            // Set enabled to the provided enabled parameter.
            m_Enabled = enabled;
            // Set random pitch to the provided random pitch paramater.
            m_RandomPitch = randomPitch;
            // Set pitch to the provided pitch paramater.
            m_Pitch = pitch;
            // Set the minimum pitch to the provided minimum pitch paramater.
            m_MinPitch = minPitch;
            // Set the maximum pitch to the provided maximum pitch paramater.
            m_MaxPitch = maxPitch;
            // Set change volume to the provided change volume parameter.
            m_ChangeVolume = changeVolume;
            // Set the volume to the provided volume parameter.
            m_Volume = volume;
            // Set audio clips to the array of clips provided.
            m_AudioClips = audioClips;
        }

        /// <summary>
        /// Plays a random audio clip at on a audio source and uses the settings set on the item.
        /// </summary>
        /// <param name="audioSource">The source to play the sounds on.</param>
        public void Play(AudioSource audioSource)
        {
            // Only play if the audio item is enabled-
            if (m_Enabled)
            {
                // Only play if there are any audio clips.
                if (m_AudioClips.Length > 0)
                {
                    // If random pitch is enabled, set the pitch to something random between min and max pitch.
                    // Else just set it to the pitch set when random pitch is disabled.
                    if (m_RandomPitch)
                        audioSource.pitch = Random.Range(m_MinPitch, m_MaxPitch);
                    else
                        audioSource.pitch = m_Pitch;

                    // If change volume is enabled, set the volume.
                    if (m_ChangeVolume)
                        audioSource.volume = m_Volume;

                    // If there are more than one audio clip, shuffle the clips.
                    // Else just play the one clip available.
                    // This also makes it so no clip will play right after itself. It's always a new clip.
                    if (m_AudioClips.Length > 1)
                    {
                        // Get a random index between 1 and the length of the audio clips array.
                        int n = Random.Range(1, m_AudioClips.Length);
                        // Set the clip on the audio source to the index.
                        audioSource.clip = m_AudioClips[n];

                        // Move the clip at the random index to index 0.
                        m_AudioClips[n] = m_AudioClips[0];
                        // Set the audio clip at index 0 to the one in the audio source.
                        m_AudioClips[0] = audioSource.clip;
                    }
                    else
                    {
                        // Set the clip on the audio source to the one audio clip available.
                        audioSource.clip = m_AudioClips[0];
                    }

                    // Play the audio source.
                    audioSource.Play();
                }
                else
                {
                    // There were no audio clips, so tell the user about it.
                    Debug.LogWarning("Tried to play audio on '" + audioSource.name + "' but no audio clips have been set!");
                }
            }
        }
    }
}
