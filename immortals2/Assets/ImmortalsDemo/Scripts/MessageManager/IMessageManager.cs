using System.Collections;
using UnityEngine;

namespace Immortal
{
	public interface IMessageManager
	{
		public interface IListener { }

		void RegisterListener(IListener listener);
		void UnregisterListener(IListener listener);
		void Dispatch(GameMessage msg);
		void DelayedDispatch(float delay, Message msg);
	}
}