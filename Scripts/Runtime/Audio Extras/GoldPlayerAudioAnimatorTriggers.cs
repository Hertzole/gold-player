#if GOLD_PLAYER_DISABLE_AUDIO_EXTRAS
#define OBSOLETE
#endif

#if OBSOLETE && !UNITY_EDITOR
#define STRIP
#endif

#if !STRIP
using UnityEngine;

namespace Hertzole.GoldPlayer
{
    /// <summary>
    /// Used together with any Gold Player Audio Behaviour to allow for an animation to trigger footstep sounds.
    /// </summary>
#if !OBSOLETE
    [AddComponentMenu("Gold Player/Gold Player Audio Animator Triggers", 50)]
#else
    [System.Obsolete("Gold Player Audio Extras has been disabled. GoldPlayerAudioAnimatorTriggers will be removed on build.")]
    [AddComponentMenu("")]
#endif
    public class GoldPlayerAudioAnimatorTriggers : MonoBehaviour
    {
        [SerializeField]
        private PlayerAudioBehaviour audioTarget = null;

        public PlayerAudioBehaviour AudioAnimator { get { return audioTarget; } }

#if OBSOLETE
        private void Awake()
        {
            Debug.LogError(gameObject.name + " has GoldPlayerAudioAnimatorTriggers attached. It will be removed on build. Please remove this component if you don't intend to use it.", gameObject);
        }
#endif

        public void PlayFootstepSound()
        {
            if (audioTarget != null)
            {
                audioTarget.PlayFootstepSound();
            }
        }

        public void PlayJumpSound()
        {
            if (audioTarget != null)
            {
                audioTarget.PlayJumpSound();
            }
        }

        public void PlayLandSound()
        {
            if (audioTarget != null)
            {
                audioTarget.PlayLandSound();
            }
        }
    }
}
#endif
