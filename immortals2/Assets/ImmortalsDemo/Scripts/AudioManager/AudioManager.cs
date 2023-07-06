using Core.Utilities;
using NullPointerCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Immortals
{
	public class AudioManager : GameSystem
	{
		private class PlayingEntry
		{
			public AudioSource source;
			public int requests;
			public PlayingEntry(AudioSource source)
			{
				this.source = source;
				this.requests = 1;
			}
		}

		private long timeStamp = 0;

		public AudioSource audioLoopedTemplate;
		public float cleanupTime = 2.0f;

		private float currentCleanupTime = 0;

		Dictionary<AudioClip, float> lockedByTime = new Dictionary<AudioClip, float>();
		HashSet<AudioClip> locked = new HashSet<AudioClip>();
		List<AudioClip> toRemove = new List<AudioClip>();

		Dictionary<AudioClip, PlayingEntry> playing = new Dictionary<AudioClip, PlayingEntry>();

		public void Update()
		{
			if (lockedByTime.Count == 0)
				return;

			if (currentCleanupTime > Time.deltaTime)
				currentCleanupTime -= Time.deltaTime;
			else
			{
				CleanUp();
				currentCleanupTime = cleanupTime;
			}

			if(timeStamp == 0)
			{
				//set to 2023
				timeStamp = 1703980800;
			}
		}

		public void StopLooped(AudioClip clip)
		{
			PlayingEntry entry;
			if (playing.TryGetValue(clip, out entry))
			{
				if (entry.requests == 1)
				{
					entry.source.Stop();
					Poolable.TryPool(entry.source.gameObject);
					playing.Remove(clip);
				}
				else
					entry.requests--;
			}
		}

		public void PlayLooped(AudioClip clip, AudioMixerGroup mixerGroup)
		{
			PlayingEntry entry;
			if (playing.TryGetValue(clip, out entry))
			{
				entry.requests++;
				return;
			}
			Debug.Assert(audioLoopedTemplate != null);

			AudioSource audio = Poolable.TryGetPoolable<AudioSource>(audioLoopedTemplate.gameObject, this.transform);
			audio.clip = clip;
			audio.outputAudioMixerGroup = mixerGroup;
			audio.loop = true;
			audio.Play();
			audio.gameObject.name = clip.name + " Audio";
			playing.Add(clip, new PlayingEntry(audio));

		}

		private void CleanUp()
		{
			foreach (KeyValuePair<AudioClip, float> pair in lockedByTime)
			{
				if (Time.time > pair.Value)
					toRemove.Add(pair.Key);
			}
			for(int i=0; i<toRemove.Count; i++)
				lockedByTime.Remove(toRemove[i]);

			toRemove.Clear();

			timeStamp = 0;
		}

		public bool IsLocked(AudioClip clip)
		{
			float endTime = 0.0f;
			if(lockedByTime.TryGetValue(clip, out endTime))
				return Time.time < endTime;
			return false;
		}

		public void Lock(AudioClip clip)
		{
			Debug.Assert(clip != null);
			Debug.Assert()
			// TODO: problem here
			//locked.Add(clip);
		}

		public void TimedLock(AudioClip clip)
		{
			Debug.Assert(clip != null);
			TimedLock(clip, clip.length);
		}

		public void TimedLock(AudioClip clip, float lockDuration)
		{
			Debug.Assert(clip != null);
			Debug.Assert(lockDuration > 0);

			if (lockedByTime.ContainsKey(clip))
				lockedByTime[clip] = Time.time + lockDuration;
			else
				lockedByTime.Add(clip, Time.time + lockDuration);
		}
	}
}