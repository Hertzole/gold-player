using Hertzole.GoldPlayer.Core;
using UnityEngine;

namespace Hertzole.GoldPlayer.Weapons
{
    public partial class GoldPlayerWeapon
    {
        [SerializeField]
        private AudioItem m_EquipSound;
        public AudioItem EquipSound { get { return m_EquipSound; } set { m_EquipSound = value; } }
        [SerializeField]
        private AudioItem m_PrimaryAttackSound;
        public AudioItem PrimaryAttackSound { get { return m_PrimaryAttackSound; } set { m_PrimaryAttackSound = value; } }
        [SerializeField]
        private AudioItem m_DryShootSound;
        public AudioItem DryShootSound { get { return m_DryShootSound; } set { m_DryShootSound = value; } }
        [SerializeField]
        private AudioItem m_ReloadSound;
        public AudioItem ReloadSound { get { return m_ReloadSound; } set { m_ReloadSound = value; } }
        [SerializeField]
        private AudioSource m_EquipAudioSource;
        public AudioSource EquipAudioSource { get { return m_EquipAudioSource; } set { m_EquipAudioSource = value; } }
        [SerializeField]
        private AudioSource m_PrimaryAttackAudioSource;
        public AudioSource PrimaryAttackAudioSource { get { return m_PrimaryAttackAudioSource; } set { m_PrimaryAttackAudioSource = value; } }
        [SerializeField]
        private AudioSource m_DryShootAudioSource;
        public AudioSource DryShootAudioSource { get { return m_DryShootAudioSource; } set { m_DryShootAudioSource = value; } }
        [SerializeField]
        private AudioSource m_ReloadAudioSource;
        public AudioSource ReloadAudioSource { get { return m_ReloadAudioSource; } set { m_ReloadAudioSource = value; } }

        protected void PlayEquipSound()
        {
            if (m_EquipAudioSource)
                m_EquipSound.Play(m_EquipAudioSource);
        }

        protected void PlayPrimaryAttackSound()
        {
            if (m_PrimaryAttackAudioSource)
                m_PrimaryAttackSound.Play(m_PrimaryAttackAudioSource);
        }

        protected void PlayDryAttackSound()
        {
            if (m_DryShootAudioSource)
                m_DryShootSound.Play(m_DryShootAudioSource);
        }

        protected void PlayReloadSound()
        {
            if (m_ReloadAudioSource)
                m_ReloadSound.Play(m_ReloadAudioSource);
        }
    }
}
