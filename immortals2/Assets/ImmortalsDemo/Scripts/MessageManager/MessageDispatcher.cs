using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Immortal
{
	public class MessageDispatcher<M> where M : Message
	{
		public interface IListener
		{
			void OnMessage(M msg);
		}
	}
}