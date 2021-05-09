#if GOLD_PLAYER_TESTS && UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Hertzole.GoldPlayer.Tests
{
	public class AudioItemTests
	{
		private const string STEP1_GUID = "44e59b73420f0e64fb30e755c49b3823";
		private const string STEP2_GUID = "62e6bf700540ea9478b91b33afa85e7e";
		private const string STEP3_GUID = "bbe4ed33245f5804db42ff8c43d75412";
		private const string STEP4_GUID = "1e5761292ed025241bdb02ff49c552dd";
		private const string JUMP_GUID = "11cc82c4daa9f304dbbfab62c9207608";
		private const string LAND_GUID = "ceb0f9d730c213441b4eaccb59f6fcde";
		
		private static T GetAsset<T>(string guid) where T : Object
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<T>(path);
		}

		private AudioSource source;

		private AudioClip step1;
		private AudioClip step2;
		private AudioClip step3;
		private AudioClip step4;
		private AudioClip jump;
		private AudioClip land;

		private AudioItem item;

		[UnitySetUp]
		public IEnumerator SetUp()
		{
			source = new GameObject("Audio").AddComponent<AudioSource>();

			step1 = GetAsset<AudioClip>(STEP1_GUID);
			step2 = GetAsset<AudioClip>(STEP2_GUID);
			step3 = GetAsset<AudioClip>(STEP3_GUID);
			step4 = GetAsset<AudioClip>(STEP4_GUID);
			jump = GetAsset<AudioClip>(JUMP_GUID);
			land = GetAsset<AudioClip>(LAND_GUID);

			item = new AudioItem(true, false, 1f, 0.9f, 1.1f, true, 1f, new AudioClip[] { step1, step2, step3, step4, jump, land });
			
			yield return null;
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			Object.DestroyImmediate(source.gameObject);

			yield return null;
		}

		[UnityTest]
		public IEnumerator TestPlay()
		{
			Assert.IsNull(source.clip);
			item.Play(source);
			yield return null;
			Assert.IsNotNull(source.clip);
		}
		
		[UnityTest]
		public IEnumerator TestPlayOneSound()
		{
			item = new AudioItem(true, true, 1f, 0.9f, 1.1f, true, 1f, step1);
			
			Assert.IsNull(source.clip);
			item.Play(source);
			yield return null;
			Assert.AreEqual(source.clip, step1);
		}
		
		[UnityTest]
		public IEnumerator TestPlayNoClips()
		{
			item.AudioClips = null;
			item.Play(source);
			yield return null;
			LogAssert.Expect(LogType.Warning, "Tried to play audio on 'Audio' but no audio clips have been set!");
		}
		
		[UnityTest]
		public IEnumerator TestEnabled()
		{
			item.Enabled = false;
			Assert.IsNull(source.clip);
			item.Play(source);
			yield return null;
			Assert.IsNull(source.clip);
			item.Enabled = true;
			item.Play(source);
			yield return null;
			Assert.IsNotNull(source.clip);
		}
		
		[UnityTest]
		public IEnumerator TestRandomPitch()
		{
			item.RandomPitch = false;
			item.MinPitch = 0.5f;
			item.MaxPitch = 1.5f;
			Assert.AreEqual(source.pitch, 1f);
			item.Play(source);
			yield return null;
			Assert.AreEqual(source.pitch, 1f);
			item.RandomPitch = true;
			item.Play(source);
			yield return null;
			Assert.AreNotEqual(source.pitch, 1f);
			Assert.IsTrue(source.pitch >= item.MinPitch && source.pitch <= item.MaxPitch);
		}
		
		[UnityTest]
		public IEnumerator TestPitch()
		{
			item.Pitch = 2f;
			Assert.AreEqual(source.pitch, 1f);
			item.Play(source);
			yield return null;
			Assert.AreEqual(source.pitch, 2f);
		}
		
		[UnityTest]
		public IEnumerator TestVolume()
		{
			item.Volume = 0.5f;
			item.ChangeVolume = false;
			Assert.AreEqual(source.volume, 1f);
			item.Play(source);
			yield return null;
			Assert.AreEqual(source.volume, 1f);

			item.ChangeVolume = true;
			item.Play(source);
			yield return null;
			Assert.AreEqual(source.volume, 0.5f);
		}
		
		[UnityTest]
		public IEnumerator TestEquals()
		{
			AudioItem a = new AudioItem(true);
			AudioItem b = new AudioItem(true);

			Assert.IsTrue(item.Equals(item));
			Assert.IsTrue(a.Equals(b));
			Assert.IsFalse(a.Equals(new object()));

			b.Enabled = false;

			Assert.IsFalse(a.Equals(b));
			
			yield return null;
		}
		
		[UnityTest]
		public IEnumerator TestAudioEquals()
		{
			AudioItem a = new AudioItem(true) { AudioClips = null };
			AudioItem b = new AudioItem(true) { AudioClips = null };

			Assert.IsTrue(a.Equals(b));
			
			a = new AudioItem(true) { AudioClips = null };
			b = new AudioItem(true);
			
			Assert.IsFalse(a.Equals(b));

			a = new AudioItem(true);
			b = new AudioItem(true) { AudioClips = null };
			
			Assert.IsFalse(a.Equals(b));

			a = new AudioItem(true) { AudioClips = new AudioClip[] { step1 } };
			b = new AudioItem(true) { AudioClips = new AudioClip[] { step1, step2 } };
			
			Assert.IsFalse(a.Equals(b));

			a = new AudioItem(true) { AudioClips = new AudioClip[] { step1 } };
			b = new AudioItem(true) { AudioClips = new AudioClip[] { step2 } };
			
			Assert.IsFalse(a.Equals(b));
			
			yield return null;
		}
		
		[UnityTest]
		public IEnumerator TestEqualsOperator()
		{
			AudioItem a = new AudioItem(true);
			AudioItem b = new AudioItem(true);

			Assert.IsTrue(a == b);
			Assert.IsFalse(a != b);

			b.Enabled = false;

			Assert.IsFalse(a == b);
			
			yield return null;
		}
		
		[UnityTest]
		public IEnumerator TestHashCode()
		{
			AudioItem a = new AudioItem(true);
			AudioItem b = new AudioItem(true);

			Assert.AreEqual(a.GetHashCode(), b.GetHashCode());

			a.AudioClips = null;

			Assert.AreNotEqual(a.GetHashCode(), item.GetHashCode());

			
			yield return null;
		}
	}
}
#endif