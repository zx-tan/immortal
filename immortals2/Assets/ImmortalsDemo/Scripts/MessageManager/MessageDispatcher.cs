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

		private Dictionary<Type, List<IListener>> _listeners = new Dictionary<Type, List<IListener>>();

		public void RegisterListener<T>(IListener listener) where T : M
		{
			RegisterListener(typeof(T), listener);
		}

		public void UnregisterListener<T>(IListener listener)
		{
			UnregisterListener(typeof(T), listener);
		}

		private void RegisterListener(Type msgType, IListener listener)
		{
			if (!_listeners.ContainsKey(msgType))
			{
				_listeners.Add(msgType, new List<IListener>());
			}

			if (!_listeners[msgType].Contains(listener))
			{
				_listeners[msgType].Add(listener);
			}
		}

		private void UnregisterListener(Type msgType, IListener listener)
		{
			if (_listeners.TryGetValue(msgType, out var node))
			{
				if (node.Contains(listener))
				{
					node.Remove(listener);
				}
				
				if (node.Count == 0)
				{
					_listeners.Remove(msgType);
				}
			}
		}
	}
}