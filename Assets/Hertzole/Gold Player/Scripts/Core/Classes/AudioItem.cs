using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    //DOCUMENT: AudioItem
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
            m_Enabled = enabled;
        }

        public AudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch)
        {
            m_Enabled = enabled;
            m_RandomPitch = randomPitch;
            m_Pitch = pitch;
            m_MinPitch = minPitch;
            m_MaxPitch = maxPitch;
        }

        public AudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch, bool changeVolume, float volume)
        {
            m_Enabled = enabled;
            m_RandomPitch = randomPitch;
            m_Pitch = pitch;
            m_MinPitch = minPitch;
            m_MaxPitch = maxPitch;
            m_ChangeVolume = changeVolume;
            m_Volume = volume;
        }

        public AudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch, bool changeVolume, float volume, AudioClip audioClip)
        {
            m_Enabled = enabled;
            m_RandomPitch = randomPitch;
            m_Pitch = pitch;
            m_MinPitch = minPitch;
            m_MaxPitch = maxPitch;
            m_ChangeVolume = changeVolume;
            m_Volume = volume;
            m_AudioClips = new AudioClip[1] { audioClip };
        }

        public AudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch, bool changeVolume, float volume, AudioClip[] audioClips)
        {
            m_Enabled = enabled;
            m_RandomPitch = randomPitch;
            m_Pitch = pitch;
            m_MinPitch = minPitch;
            m_MaxPitch = maxPitch;
            m_ChangeVolume = changeVolume;
            m_Volume = volume;
            m_AudioClips = audioClips;
        }

        /// <summary>
        /// Plays a random audio clip at on a audio source and uses the settings set on the item.
        /// </summary>
        /// <param name="audioSource">The source to play the sounds on.</param>
        public void Play(AudioSource audioSource)
        {
            if (m_Enabled)
            {
                if (m_AudioClips.Length > 0)
                {
                    if (m_RandomPitch)
                        audioSource.pitch = Random.Range(m_MinPitch, m_MaxPitch);
                    else
                        audioSource.pitch = m_Pitch;

                    if (m_ChangeVolume)
                        audioSource.volume = m_Volume;

                    if (m_AudioClips.Length > 1)
                    {
                        int n = Random.Range(1, m_AudioClips.Length);
                        audioSource.clip = m_AudioClips[n];

                        m_AudioClips[n] = m_AudioClips[0];
                        m_AudioClips[0] = audioSource.clip;
                    }
                    else
                    {
                        audioSource.clip = m_AudioClips[0];
                    }

                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning("Tried to play audio on '" + audioSource.name + "' but no audio clips have been set!");
                }
            }
        }
    }
}
