using NullPointerCore;
using UnityEngine.Events;

namespace NullPointerCore
{
	public class CmdTriggerEvent : CmdAction
	{
		public UnityEvent entry;

		public override void Play()
		{
			entry.Invoke();
		}
	}
}
