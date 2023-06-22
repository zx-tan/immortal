using NullPointerCore;
using UnityEngine;
using UnityEngine.Audio;

namespace Immortals
{
	public class CmdPlayLoopedSound : CmdAction
	{
		public enum Action
		{
			Play,
			Stop,
		}
		public AudioClip clip;
		public AudioMixerGroup mixerGroup;
		public Action action = Action.Play;

		private AudioManager audioManager;

		public override void Play()
		{
			if (clip == null)
				return;

			if (audioManager == null)
				audioManager = Game.GetSystem<AudioManager>();

			if (audioManager == null)
				return;

			if(action==Action.Play)
				audioManager.PlayLooped(clip, mixerGroup);
			else
				audioManager.StopLooped(clip);
		}
	}
}
