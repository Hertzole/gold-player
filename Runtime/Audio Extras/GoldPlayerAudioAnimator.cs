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
    /// Mainly used together with Gold Player Audio Animation Triggers to allow for an animation to trigger footstep sounds.
    /// </summary>
#if !OBSOLETE
    [AddComponentMenu("Gold Player/Gold Player Audio Animator", 50)]
#else
    [System.Obsolete("Gold Player Audio Extras has been disabled. GoldPlayerAudioAnimator will be removed on build.")]
    [AddComponentMenu("")]
#endif
    public class GoldPlayerAudioAnimator : PlayerAudioBehaviour
    {
#if OBSOLETE
        private void Awake()
        {
            Debug.LogError(gameObject.name + " has GoldPlayerAudioAnimator attached. It will be removed on build. Please remove this component if you don't intend to use it.", gameObject);
        }
#endif

        public override void PlayFootstepSound()
        {
            PlayerController.Audio.PlayFootstepSound();
        }

        public override void PlayJumpSound()
        {
            PlayerController.Audio.PlayJumpSound();
        }

        public override void PlayLandSound()
        {
            PlayerController.Audio.PlayLandSound();
        }
    }
}
#endif
