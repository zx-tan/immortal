using System.Collections;
using UnityEngine;

namespace Immortal
{
	public class MessageManager : GameSystem, IMessageManager
	{
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