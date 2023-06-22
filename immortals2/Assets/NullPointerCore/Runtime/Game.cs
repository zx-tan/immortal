using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// The singleton class that controls the systems and services for the game.
	/// </summary>
	public class Game
	{
		private static Game ms_SingletonInstance = null;

		[SerializeField]
		[ReadOnly]
		private List<GameSystem> m_Systems = new List<GameSystem>();
		[SerializeField]
		[ReadOnly]
		private List<GameService> m_Services = new List<GameService>();
		[SerializeField]
		[ReadOnly]
		private List<Player> players = new List<Player>();

		private Transform systemsContainerCache = null;

		internal static Game Get()
		{
			if (ms_SingletonInstance == null)
				ms_SingletonInstance = new Game();
			return ms_SingletonInstance;
		}

		#region GameSystems

		public static TSystem GetSystem<TSystem>() where TSystem : GameSystem
		{
			return Get().GetValidSystem<TSystem>();
		}

		public static GameSystem GetSystem(Type type)
		{
			return Get().GetValidSystem(type);
		}

		internal void Register(GameSystem x)
		{
			if( x!=null && !m_Systems.Contains(x) )
				m_Systems.Add(x);
		}

		internal void Unregister(GameSystem x)
		{
			if (x != null && m_Systems.Contains(x))
				m_Systems.Remove(x);
		}

		internal TSystem GetValidSystem<TSystem>() where TSystem : GameSystem
		{
			TSystem result = m_Systems.FirstOrDefault(x => x is TSystem) as TSystem;
			if (result == null)
				result = GameObject.FindObjectOfType<TSystem>();
			return result;
		}

		internal GameSystem GetValidSystem(Type type)
		{
			GameSystem result = m_Systems.FirstOrDefault(x => x!=null && x.GetType().IsInstanceOfType(type));
			if (result == null)
				result = GameObject.FindObjectOfType(type) as GameSystem;
			return result as GameSystem;
		}

		public static GameSystem CreateDefaultSystem(Type systemType, UnityEngine.Object context = null)
		{
			return Get().internal_CreateDefaultSystem(systemType, context);
		}

		internal GameSystem internal_CreateDefaultSystem(Type systemType, UnityEngine.Object context = null)
		{
			//if (IsSystemRegistered(systemType))
			//{
			//	Debug.LogError("There is a GameSceneSystem of type: " + systemType.Name, context);
			//	return null;
			//}
			if (!systemType.IsSubclassOf(typeof(GameSystem)))
			{
				Debug.LogError("systemType must inherit from GameSceneSystem. Requested type: " + systemType.Name, context);
				return null;
			}
			GameObject sceneBoundsGO = new GameObject(systemType.Name);
			sceneBoundsGO.transform.SetParent(SystemsContainer);
			sceneBoundsGO.transform.localPosition = Vector3.zero;
			GameSystem result = sceneBoundsGO.AddComponent(systemType) as GameSystem;
			Register(result);
			return result;
		}

		public Transform SystemsContainer 
		{
			get
			{
				if (systemsContainerCache == null)
				{
					GameObject go = GameObject.Find("GameSystems");
					if (go != null)
						systemsContainerCache = go.transform;
				}
				if (systemsContainerCache == null)
				{
					GameObject go = new GameObject("GameSystems");
					if (go != null)
						systemsContainerCache = go.transform;
				}
				return systemsContainerCache;
			}
		}

		#endregion GameSystems

		#region Players

		/// <summary>
		/// Determines if the given Player is already registered.
		/// </summary>
		/// <param name="player">The Player to be checked.</param>
		/// <returns>True if it's registered or false in otherwise.</returns>
		public bool IsRegistered(Player player)
		{
			return players.Contains(player);
		}

		/// <summary>
		/// Registers the Player. 
		/// </summary>
		/// <param name="player">the player to be registered.</param>
		public void Register(Player player)
		{
			if(!players.Contains(player))
				players.Add(player);
		}

		/// <summary>
		/// Unregisters the Player.
		/// </summary>
		/// <param name="player">the player to be unregistered.</param>
		public void Unregister(Player player)
		{
			if(players.Contains(player))
				players.Remove(player);
		}

		/// <summary>
		/// Enumerates the registered players
		/// </summary>
		public IEnumerable<Player> Players { get { return players; } }
		/// <summary>
		/// Returns the count of already registered players.
		/// </summary>
		public int PlayersCount { get { return players.Count; } }

		public Transform PlayersContainer 
		{
			get
			{
				if (systemsContainerCache == null)
				{
					GameObject go = GameObject.Find("GamePlayers");
					if (go != null)
						systemsContainerCache = go.transform;
				}
				if (systemsContainerCache == null)
				{
					GameObject go = new GameObject("GamePlayers");
					if (go != null)
						systemsContainerCache = go.transform;
				}
				return systemsContainerCache;
			}
		}

		#endregion Players Registration

		#region Services

		public static void Register(GameService service)
		{
			Debug.Assert(service != null, "Invalid param. The service to register can't be null.");
			Get().m_Services.Add(service);
		}

		public static void Unregister(GameService service)
		{
			Debug.Assert(service != null, "Invalid param. The service to unregister can't be null.");
			Get().m_Services.Remove(service);
		}

		public static ServiceType GetService<ServiceType>() where ServiceType : GameService
		{
			return (ServiceType) Get().m_Services.FirstOrDefault(x => x is ServiceType);
		}

		public static GameService GetService(Type serviceType)
		{
			return Get().m_Services.FirstOrDefault(x => serviceType.IsAssignableFrom(x.GetType()));
		}

		#endregion Services
	}
}