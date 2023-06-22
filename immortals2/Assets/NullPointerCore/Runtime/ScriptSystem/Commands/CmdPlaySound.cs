using Immortals;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace NullPointerCore
{

	public class CmdPlaySound : CmdAction
	{
		[FormerlySerializedAs("attackSnd")]
		public AudioSource source;
		public AudioMixerGroup mixerGroup;
		public List<AudioClip> randomClips = new List<AudioClip>();

		private AudioManager audioManager;

		public override void Play()
		{
			if (source == null)
				return;
			if (mixerGroup != null)
				source.outputAudioMixerGroup = mixerGroup;

			if (audioManager == null)
				audioManager = Game.GetSystem<AudioManager>();

			if (randomClips.Count > 0)
			{
				int selected = (int)(Random.value * randomClips.Count);
				if (randomClips[selected] != null)
				{
					if (audioManager == null || !audioManager.IsLocked(randomClips[selected]))
					{
						if (audioManager != null)
							audioManager.TimedLock(randomClips[selected]);
						source.PlayOneShot(randomClips[selected]);
					}
				}
				else if (source.clip != null)
				{
					if (audioManager == null || !audioManager.IsLocked(source.clip))
					{
						if (audioManager != null)
							audioManager.TimedLock(source.clip);
						source.Play();
					}
				}
			}
			else
			{
				if (source.clip != null && (audioManager == null || !audioManager.IsLocked(source.clip)))
				{
					if (audioManager != null)
						audioManager.TimedLock(source.clip);
					source.Play();
				}
			}
		}
	}
}