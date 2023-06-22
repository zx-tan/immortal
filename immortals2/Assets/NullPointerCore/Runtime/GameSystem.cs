using System;
using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// Base class for all the game systems.
	/// </summary>
	public class GameSystem : NullPointerBehaviour
	{
		/// <summary>
		/// Implementation of the Unity's built in Awake() method. 
		/// </summary>
		virtual protected void Awake()
		{
			Game.Get().Register(this);
		}

		public void OnDestroy()
		{
			Game.Get().Unregister(this);
		}

		public virtual void OnEnable() { }
		public virtual void OnDisable() { }

		/// <summary>
		/// Creates the default instance for the requested GameSystem
		/// </summary>
		/// <typeparam name="T">The type of requested GameSystem</typeparam>
		/// <param name="result">returns the reference to the newly created GameSystem.</param>
		/// <param name="context">The Object context for the creation of the system. usually the game object that requested it.</param>
		public static void CreateDefault<T>(out GameSystem result, UnityEngine.Object context) where T : GameSystem
		{
			CreateDefault(typeof(T), out result, context);
		}

		/// <summary>
		/// Creates the default instance for the requested GameSystem
		/// </summary>
		/// <param name="type">The type of GameSystem to create.</param>
		/// <param name="result">returns the reference to the newly created GameSystem.</param>
		/// <param name="context">The Object context for the creation of the system. usually the game object that requested it.</param>
		/// <returns>true in case of success, false in otherwise.</returns>
		public static bool CreateDefault(Type type, out GameSystem result, UnityEngine.Object context)
		{
			return (result = Game.CreateDefaultSystem(type, context)) != null;
		}

		/// <summary>
		/// overrideable method to initialize the GameSystem with its default values.
		/// </summary>
		public virtual void GenerateDefaultValues()
		{
		}
	}
}