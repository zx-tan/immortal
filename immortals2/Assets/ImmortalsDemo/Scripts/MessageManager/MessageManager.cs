using System.Collections;
using UnityEngine;

namespace Immortal
{
	public class MessageManager : GameSystem, IMessageManager
	{
		public interface IListener : MessageDispatcher<Message>.IListener { }

		private MessageDispatcher<Message> _messageDispatchers = new MessageDispatcher<Message>();

		public void RegisterListener(IListener listener)
		{
			_messageDispatchers.RegisterListener(typeof(T), listener);
		}

		public void UnregisterListener(IListener listener)
		{
			_messageDispatchers.UnregisterListener(typeof(T), listener);
		}

		public void Dispatch(Message msg)
		{
			_messageDispatchers.Dispatch(msg);
		}
		
		public void DelayedDispatch(float delay, Message msg)
		{
		}
	}
}