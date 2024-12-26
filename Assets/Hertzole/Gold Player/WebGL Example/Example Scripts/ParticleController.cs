using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Hertzole.GoldPlayer.Example
{
	[Preserve]
	public sealed class ParticleController : MonoBehaviour
	{
		private ParticleSystem particles;

		private void Awake()
		{
			particles = GetComponent<ParticleSystem>();
		}

		[Preserve]
		public void StartParticles()
		{
			particles.Play();
		}
		
		[Preserve]
		public void StopParticles()
		{
			particles.Stop();
		}

		[Preserve]
		public void PauseParticles()
		{
			particles.Pause();
		}
	}
}

