using System;
using System.Collections.Generic;
using Immortals;
using UnityEngine.Events;

namespace NullPointerCore
{
	public class EventManager
	{

		private Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();

		private static EventManager eventManager;

		private static EventManager instance {
			get
			{
				if (eventManager == null)
					eventManager = new EventManager();
				return eventManager;
			}
		}

		public static void StartListening(string eventName, UnityAction listener)
		{
			UnityEvent thisEvent = null;
			if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
			{
				thisEvent.AddListener(listener);
			}
			else
			{
				thisEvent = new UnityEvent();
				thisEvent.AddListener(listener);
				instance.eventDictionary.Add(eventName, thisEvent);
			}
		}

		public static void StopListening(string eventName, UnityAction listener)
		{
			if (eventManager == null) return;
			UnityEvent thisEvent = null;
			if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
			{
				thisEvent.RemoveListener(listener);
			}
		}

		public static void TriggerEvent(string eventName)
		{
			UnityEvent thisEvent = null;
			if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
			{
				thisEvent.Invoke();
			}
		}

		public static event Action<GameEvent> OnEventHappened;

		public static void EventHappened(GameEvent ge)
		{
			if (OnEventHappened != null)
				OnEventHappened(ge);
		}
	}
}