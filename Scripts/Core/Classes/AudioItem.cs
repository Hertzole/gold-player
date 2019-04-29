using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("m_Enabled")]
        private bool enabled = true;
        [SerializeField]
        [Tooltip("Determines if the pitch should be randomized.")]
        [FormerlySerializedAs("m_RandomPitch")]
        private bool randomPitch = true;
        [SerializeField]
        [Tooltip("The pitch that the audio should play at.")]
        [FormerlySerializedAs("m_Pitch")]
        private float pitch = 1f;
        [SerializeField]
        [Tooltip("The minimum pitch the audio can play at.")]
        [FormerlySerializedAs("m_MinPitch")]
        private float minPitch = 0.9f;
        [SerializeField]
        [Tooltip("The maximum pitch the audio can play at.")]
        [FormerlySerializedAs("m_MaxPitch")]
        private float maxPitch = 1.1f;
        [SerializeField]
        [Tooltip("Determines if the volume should be changed when playing.")]
        [FormerlySerializedAs("m_ChangeVolume")]
        private bool changeVolume = false;
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("The volume that should be set when playing.")]
        [FormerlySerializedAs("m_Volume")]
        private float volume = 1f;
        [SerializeField]
        [Tooltip("All the audio clips.")]
        [FormerlySerializedAs("m_AudioClips")]
        private AudioClip[] audioClips = new AudioClip[0];

        /// <summary> Determines if this audio should be enabled. </summary>
        public bool Enabled { get { return enabled; } set { enabled = value; } }
        /// <summary> Determines if the pitch should be randomized. </summary>
        public bool RandomPitch { get { return randomPitch; } set { randomPitch = value; } }
        /// <summary> The pitch that the audio should play at. </summary>
        public float Pitch { get { return pitch; } set { pitch = value; } }
        /// <summary> The minimum pitch the audio can play at. </summary>
        public float MinPitch { get { return minPitch; } set { minPitch = value; } }
        /// <summary> The maximum pitch the audio can play at. </summary>
        public float MaxPitch { get { return maxPitch; } set { maxPitch = value; } }
        /// <summary> Determines if the volume should be changed when playing. </summary>
        public bool ChangeVolume { get { return changeVolume; } set { changeVolume = value; } }
        /// <summary> The volume that should be set when playing. </summary>
        public float Volume { get { return volume; } set { volume = value; } }
        /// <summary> All the audio clips. </summary>
        public AudioClip[] AudioClips { get { return audioClips; } set { audioClips = value; } }

        public AudioItem() { }

        public AudioItem(bool enabled)
        {
            // Set enabled to the provided enabled parameter.
            this.enabled = enabled;
        }

        public AudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch)
        {
            // Set enabled to the provided enabled parameter.
            this.enabled = enabled;
            // Set random pitch to the provided random pitch parameter.
            this.randomPitch = randomPitch;
            // Set pitch to the provided pitch parameter.
            this.pitch = pitch;
            // Set the minimum pitch to the provided minimum pitch parameter.
            this.minPitch = minPitch;
            // Set the maximum pitch to the provided maximum pitch parameter.
            this.maxPitch = maxPitch;
        }

        public AudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch, bool changeVolume, float volume)
        {
            // Set enabled to the provided enabled parameter.
            this.enabled = enabled;
            // Set random pitch to the provided random pitch parameter.
            this.randomPitch = randomPitch;
            // Set pitch to the provided pitch parameter.
            this.pitch = pitch;
            // Set the minimum pitch to the provided minimum pitch parameter.
            this.minPitch = minPitch;
            // Set the maximum pitch to the provided maximum pitch parameter.
            this.maxPitch = maxPitch;
            // Set change volume to the provided change volume parameter.
            this.changeVolume = changeVolume;
            // Set the volume to the provided volume parameter.
            this.volume = volume;
        }

        public AudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch, bool changeVolume, float volume, AudioClip audioClip)
        {
            // Set enabled to the provided enabled parameter.
            this.enabled = enabled;
            // Set random pitch to the provided random pitch parameter.
            this.randomPitch = randomPitch;
            // Set pitch to the provided pitch parameter.
            this.pitch = pitch;
            // Set the minimum pitch to the provided minimum pitch parameter.
            this.minPitch = minPitch;
            // Set the maximum pitch to the provided maximum pitch parameter.
            this.maxPitch = maxPitch;
            // Set change volume to the provided change volume parameter.
            this.changeVolume = changeVolume;
            // Set the volume to the provided volume parameter.
            this.volume = volume;
            // Set audio clips to an array with only one clip that was provided.
            audioClips = new AudioClip[1] { audioClip };
        }

        public AudioItem(bool enabled, bool randomPitch, float pitch, float minPitch, float maxPitch, bool changeVolume, float volume, AudioClip[] audioClips)
        {
            // Set enabled to the provided enabled parameter.
            this.enabled = enabled;
            // Set random pitch to the provided random pitch parameter.
            this.randomPitch = randomPitch;
            // Set pitch to the provided pitch parameter.
            this.pitch = pitch;
            // Set the minimum pitch to the provided minimum pitch parameter.
            this.minPitch = minPitch;
            // Set the maximum pitch to the provided maximum pitch parameter.
            this.maxPitch = maxPitch;
            // Set change volume to the provided change volume parameter.
            this.changeVolume = changeVolume;
            // Set the volume to the provided volume parameter.
            this.volume = volume;
            // Set audio clips to the array of clips provided.
            this.audioClips = audioClips;
        }

        /// <summary>
        /// Plays a random audio clip at on a audio source and uses the settings set on the item.
        /// </summary>
        /// <param name="audioSource">The source to play the sounds on.</param>
        public void Play(AudioSource audioSource)
        {
            // Only play if the audio item is enabled.
            if (enabled && audioSource != null)
            {
                // Only play if there are any audio clips.
                if (audioClips.Length > 0)
                {
                    // If random pitch is enabled, set the pitch to something random between min and max pitch.
                    // Else just set it to the pitch set when random pitch is disabled.
                    audioSource.pitch = randomPitch ? Random.Range(minPitch, maxPitch) : pitch;

                    // If change volume is enabled, set the volume.
                    if (changeVolume)
                        audioSource.volume = volume;

                    // If there are more than one audio clip, shuffle the clips.
                    // Else just play the one clip available.
                    // This also makes it so no clip will play right after itself. It's always a new clip.
                    if (audioClips.Length > 1)
                    {
                        // Get a random index between 1 and the length of the audio clips array.
                        int n = Random.Range(1, audioClips.Length);
                        // Set the clip on the audio source to the index.
                        audioSource.clip = audioClips[n];

                        // Move the clip at the random index to index 0.
                        audioClips[n] = audioClips[0];
                        // Set the audio clip at index 0 to the one in the audio source.
                        audioClips[0] = audioSource.clip;
                    }
                    else
                    {
                        // Set the clip on the audio source to the one audio clip available.
                        audioSource.clip = audioClips[0];
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
