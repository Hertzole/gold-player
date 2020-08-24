#if !GOLD_PLAYER_DISABLE_AUDIO_EXTRAS
using UnityEngine;

namespace Hertzole.GoldPlayer
{
    /// <summary>
    /// Used together with any Gold Player Audio Behaviour to allow for an animation to trigger footstep sounds.
    /// </summary>
    [AddComponentMenu("Gold Player/Gold Player Audio Animator Triggers", 50)]
    public class GoldPlayerAudioAnimatorTriggers : MonoBehaviour
    {
        [SerializeField]
        private PlayerAudioBehaviour audioTarget = null;

        public PlayerAudioBehaviour AudioAnimator { get { return audioTarget; } }

        public void PlayFootstepSound()
        {
            if (audioTarget != null)
            {
                audioTarget.PlayFoostepSound();
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
