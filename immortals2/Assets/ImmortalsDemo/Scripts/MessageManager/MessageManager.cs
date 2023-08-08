using System.Collections;
using UnityEngine;

namespace Immortal
{
	public class MessageManager : GameSystem, IMessageManager
	{
		public interface IListener : MessageDispatcher<Message>.IListener { }

		private MessageDispatcher<Message> _messages = new MessageDispatcher<Message>();

		public void RegisterListener(IListener listener)
		{
			
		}

		public void UnregisterListener(IListener listener)
		{
			
		}

		public void Dispatch(GameMessage msg)
		{
		}
		
		public void DelayedDispatch(float delay, Message msg)
		{
		}
	}
}