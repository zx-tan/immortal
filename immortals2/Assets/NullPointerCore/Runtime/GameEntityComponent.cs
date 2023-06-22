using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// Base class for all the Components that need to comunicate with the GameEntity and it's Visual Module.
	/// </summary>
	[RequireComponent (typeof(GameEntity))]
	public class GameEntityComponent : MonoBehaviour
	{
		private GameEntity controlledEntity;

		/// <summary>
		/// The GameEntity attached to this component.
		/// </summary>
		public GameEntity ThisEntity { get { return GetSafeGameEntityReference(); } }
		/// <summary>
		/// Shortcut for the Message sink located at the GameEntity attached to this component.
		/// </summary>
		//public MessageSink Messages { get { return ThisEntity.Messages; } }
		/// <summary>
		/// Shortcut for the ComponentProxy located at the GameEntity attached to this component.
		/// </summary>
		public ComponentProxy VisualProxy { get { return ThisEntity.VisualProxy; } }

		[SerializeField][HelpBox]
		private string helpText;

		// Use this for initialization
		void Awake ()
		{
			LogsClear();
			GetSafeGameEntityReference();
			#pragma warning disable 618
			SetupEntityComponent();
			#pragma warning restore 618
			PushLogs();
		}

		/// <summary>
		/// virtual class to be implemented. This will be called on the Awake to do a proper initialization of the component 
		/// ensuring that the GameEntity thisEntity it's already defined at this point.
		/// Version 1.1: This will no longer be used in the future. Use instead OnVisualModuleSetted and OnVisualModuleRemoved.
		/// </summary>
		[System.Obsolete("Use instead OnVisualModuleSetted and OnVisualModuleRemoved.", false)]
		virtual protected void SetupEntityComponent()
		{
		}

		/// <summary>
		/// Called by unity when the script is reloaded. Here we setup the component and check that all is setted correctly.
		/// </summary>
		[ContextMenu("ReValidate")]
		virtual protected void OnValidate()
		{
			LogsClear();
			GetSafeGameEntityReference();
			#pragma warning disable 618
			SetupEntityComponent();
			#pragma warning restore 618
		}

		/// <summary>
		/// Called when the Visual Module is setted. Here we need to initialize all the component related functionality.
		/// </summary>
		virtual public void OnVisualModuleSetted()
		{
		}

		/// <summary>
		/// Called when the Visual Module is removed. Here we need to uninitialize all the component related functionality.
		/// </summary>
		virtual public void OnVisualModuleRemoved()
		{
		}

		/// <summary>
		/// Shortcut for access the property located at the GameEntity visual module.
		/// </summary>
		/// <typeparam name="T">Type of the request Component.</typeparam>
		/// <param name="name">Name of the requested property.</param>
		/// <returns>The component returned by the visual module.</returns>
		protected T GetVisualProxyProperty<T>(string name) where T : Component
		{
			if(ThisEntity.VisualProxy == null)
				return null;

			T result = ThisEntity.VisualProxy.GetPropertyValue<T>(name);
			if( result == default(T))
				Log("* Requires a "+typeof(T).Name+" called '" + name + "' in the VisualModule");
			return result;
		}

		/// <summary>
		/// Clears all log messages
		/// </summary>
		protected void LogsClear()
		{
			helpText = "";
		}

		/// <summary>
		/// Stores the message as a log that can be used later to show a warning in the editor or console.
		/// </summary>
		/// <param name="message"></param>
		protected void Log(string message)
		{
			helpText += message + "\n";
		}

		/// <summary>
		/// Push all the stored logs to the console.
		/// </summary>
		protected void PushLogs()
		{
			if(!string.IsNullOrEmpty(helpText))
				Debug.Log(helpText, this);
		}

		private GameEntity GetSafeGameEntityReference()
		{
			if(controlledEntity==null)
				controlledEntity = GetComponent<GameEntity>();
			return controlledEntity;
		}
	}
}