using GameBase;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// Represents a player in the game (no matter if its ai, human or remote controlled)
	/// Gives a sence of ownership to the GameEntities inside the game.
	/// Just need to attach a PlayerControlled component in the GameEntity that want to be controlled
	/// and the proper player assignment.
	/// </summary>
	public sealed class Player : MonoBehaviour
	{
		private List<PlayerControlled> ownUnits = new List<PlayerControlled>();

		/// <summary>
		/// Action triggered every time a PlayerControlled is registered.
		/// </summary>
		public Action<PlayerControlled> OnRegistered;
		/// <summary>
		/// Action triggered every time a PlayerControlled is unregistered.
		/// </summary>
		public Action<PlayerControlled> OnUnregistered;
		
		public void Start()
		{
			Game.Get().Register(this);
		}

		public void OnDestroy()
		{
			Game.Get().Unregister(this);
		}

		/// <summary>
		/// The Collection of GameEntity that belongs to this player
		/// </summary>
		public IEnumerable<PlayerControlled> OwnUnits { get { return ownUnits; } }

		/// <summary>
		/// Loop through this function to obtain all the components of type T (if it has) that 
		/// are attached along with all the game entities belonging to this player.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IEnumerable<T> GetOwnUnits<T>()
		{
			foreach(PlayerControlled pc in ownUnits)
			{
				T result = pc.GetComponent<T>();
				if(result != null)
					yield return result;
			}
			yield break;
		}

		/// <summary>
		/// Loop through this function to obtain all the current registered GameEntity that belongs to this player.
		/// </summary>
		/// <returns>An IEnumerable of game entities that belongs to this Player.</returns>
		public IEnumerable<GameEntity> GetOwnUnits()
		{
			foreach(PlayerControlled pc in ownUnits)
				yield return pc.ThisEntity;
			yield break;
		}

		/// <summary>
		/// Collects all the components of type <typeparamref name="T"/> from each GameEntity only if that GameEntity
		/// belongs to this Player.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entities">Collection of GameEntity where to find the components.</param>
		/// <returns>A collection of components of type <typeparamref name="T"/>.</returns>
		public IEnumerable<T> CollectOwned<T>(IEnumerable<GameEntity> entities)
		{
			foreach (GameEntity entity in entities)
			{
				if(entity==null)
					continue;
				PlayerControlled pc = entity.GetComponent<PlayerControlled>();
				if(pc==null)
					continue;
				if(pc.Owner != this)
					continue;
				T comp = entity.GetComponent<T>();
				if(comp != null)
					yield return comp;
			}
			yield break;
		}

		/// <summary>
		/// Returns the specified Player System or null if not found
		/// </summary>
		/// <typeparam name="T">Type of PlayerSystem to return.</typeparam>
		/// <returns>The Casted PlayerSystem</returns>
		public T GetSystem<T>()	where T : PlayerSystem { return GetComponent<T>(); }

		/// <summary>
		/// Registers the PlayerControlled GameEntity. 
		/// </summary>
		/// <param name="controlled"></param>
		public void Register(PlayerControlled controlled)
		{
			if(controlled==null)
			{
				Debug.LogError("Unable to register this PlayerControlled. Is null!!");
				return;
			}
			if(ownUnits.Contains(controlled))
			{
				Debug.LogWarning("Unable to register this PlayerControlled. Already registered. "+controlled.gameObject.name);
				return;
			}
			ownUnits.Add(controlled);
			if(OnRegistered!=null)
				OnRegistered.Invoke(controlled);
		}

		/// <summary>
		/// Registers the PlayerControlled GameEntity.
		/// </summary>
		/// <param name="controlled"></param>
		public void Unregister(PlayerControlled controlled)
		{
			if(controlled==null)
			{
				Debug.LogError("Unable to unregister this PlayerControlled. Is null!!");
				return;
			}
			if( ownUnits.Remove(controlled))
			{
				if(OnUnregistered!=null)
					OnUnregistered.Invoke(controlled);
			}			
			else
				Debug.LogWarning("Unable to unregister this PlayerControlled. Was not previously registered. "+controlled.gameObject.name);
		}
	}
}
