using System;
using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// Indicates the player owner for a given GameEntity.
	/// </summary>
	[DisallowMultipleComponent]
	public sealed class PlayerControlled : GameEntityComponent
	{
		/// <summary>
		/// Controller to set when the component is initialized (in Start() method).
		/// </summary>
		[HideInInspector] 
		public Player initialOwner;

		/// <summary>
		/// Delegate to be called whenever the player owner changes for this GameEntity.
		/// </summary>
		public Action OwnerChanged;
		/// <summary>
		/// The internal reference of the current player owner.
		/// </summary>
		private Player currentOwner = null;
		/// <summary>
		/// Setup for the player owner of this GameEntity
		/// </summary>
		public Player Owner 
		{
			get { return currentOwner; }
			set { ChangeController(value); }
		}

		/// <summary>
		/// Registers the GameEntity in the RTSPlayer controller.
		/// Setup all the required components.
		/// </summary>
		void Start()
		{
			if (initialOwner!=null)
				ChangeController(initialOwner);
		}

		/// <summary>
		/// Unregisters the GameEntity from the RTSPlayer controller.
		/// </summary>
		void OnDestroy()
		{
			ChangeController(null, true);
		}

		/// <summary>
		/// Internal Change Controller method.
		/// </summary>
		/// <param name="newController"></param>
		/// <param name="silent"></param>
		private void ChangeController(Player newController, bool silent=false)
		{
			// If its the same reference just do nothing!
			if(currentOwner==newController)
				return;
			// We need to unregister from the previous owner?
			if(currentOwner!=null)
				currentOwner.Unregister(this);
			// Setting the new owner and registering on it.
			currentOwner = newController;
			initialOwner = newController;
			if(currentOwner!=null)
				currentOwner.Register(this);
			// Triggering the controller changed event! (unless the silent param is true)
			if (OwnerChanged!=null && !silent)
				OwnerChanged.Invoke();
		}
	}
}
