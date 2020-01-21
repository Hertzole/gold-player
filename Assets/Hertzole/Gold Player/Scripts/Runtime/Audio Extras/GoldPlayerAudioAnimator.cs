using UnityEngine;

namespace Hertzole.GoldPlayer
{
    /// <summary>
    /// Mainly used together with Gold Player Audio Animation Triggers to allow for an animation to trigger footstep sounds.
    /// </summary>
    [AddComponentMenu("Gold Player/Gold Player Audio Animator", 50)]
    public class GoldPlayerAudioAnimator : PlayerAudioBehaviour
    {
        public override void PlayFoostepSound()
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
