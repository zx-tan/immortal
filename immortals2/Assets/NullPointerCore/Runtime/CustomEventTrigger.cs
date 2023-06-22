using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace NullPointerCore.Extras
{
	/// <summary>
	/// Receives events from the different components attached along with this component 
	/// and calls registered functions for each event. <br />
	/// The CustomEventTrigger can be used to specify functions you wish to be called for 
	/// each Action event triggered for other components. You can assign multiple functions 
	/// to a single event and whenever the CustomEventTrigger receives that event it will call 
	/// those functions in the order they were provided. <br />
	/// NOTE: Only parameterless Actions can be listened for the moment.<br />
	/// </summary>
	public class CustomEventTrigger : MonoBehaviour
	{
		/// <summary>
		/// Each entry to configure in the editor that represents an Action to listen and deliver.
		/// </summary>
		[Serializable]
		public class Entry
		{
			public Component obj;
			public string actionName;
			public Delegate invokeDelegate;
			public UnityEvent trigger;
		}

		/// <summary>
		/// The list of action entries to listen. Each entry will be configured from the editor.
		/// </summary>
		public List<Entry> entries = new List<Entry>();

		/// <summary>
		/// Registers to listen each configured action event.
		/// </summary>
		void Start ()
		{
			foreach(Entry trigger in entries)
				RegisterToEvent(trigger);
		}
	
		/// <summary>
		/// Unregisters each previously configured event.
		/// </summary>
		void OnDestroy ()
		{
			foreach(Entry trigger in entries)
				UnregisterToEvent(trigger);
		}

		public void AddEventTrigger(Component comp, string actionName)
		{
			Entry newTrigger = new Entry();
			newTrigger.actionName = actionName;
			newTrigger.obj = comp;
			entries.Add(newTrigger);
		}

		private void RegisterToEvent(Entry trigger)
		{
			if( trigger.obj == null )
				return;
			FieldInfo fieldInfo = trigger.obj.GetType().GetField(trigger.actionName);
			Delegate currentAction = fieldInfo.GetValue(trigger.obj) as Delegate;
			MethodInfo methodInfo = typeof(UnityEvent).GetMethod("Invoke");
			trigger.invokeDelegate = Delegate.CreateDelegate(fieldInfo.FieldType, trigger.trigger, methodInfo);
			if(currentAction==null)
				fieldInfo.SetValue(trigger.obj, trigger.invokeDelegate);
			else
				fieldInfo.SetValue(trigger.obj, Delegate.Combine(currentAction, trigger.invokeDelegate));
		}

		private void UnregisterToEvent(Entry trigger)
		{
			if( trigger.obj == null )
				return;
			FieldInfo fieldInfo = trigger.obj.GetType().GetField(trigger.actionName);
			Delegate currentAction = fieldInfo.GetValue(trigger.obj) as Delegate;
			
			if( currentAction == trigger.invokeDelegate )
				fieldInfo.SetValue(trigger.obj, null);
			else
				fieldInfo.SetValue(trigger.obj, Delegate.Remove(currentAction, trigger.invokeDelegate));
		}
	}
}