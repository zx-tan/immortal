using Immortals;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullPointerCore
{
	public class GameEventTrigger : MonoBehaviour
	{
		[System.Serializable]
		public class Entry
		{
			public string eventName;
			public UnityEvent onTrigger;

			public void StartListening()
			{
				if (string.IsNullOrEmpty(eventName))
					EventManager.StartListening(eventName, onTrigger.Invoke);
			}
			public void StopListening()
			{
				if (string.IsNullOrEmpty(eventName))
					EventManager.StopListening(eventName, onTrigger.Invoke);
			}
		}
		public List<Entry> entries = new List<Entry>();

		public void OnEnable()
		{
			foreach (Entry entry in entries)
				entry.StartListening();
		}

		public void OnDisable()
		{
			foreach (Entry entry in entries)
				entry.StopListening();
		}
	}
}