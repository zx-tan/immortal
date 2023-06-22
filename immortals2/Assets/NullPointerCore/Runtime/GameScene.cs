using System;
using System.Collections.Generic;
using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// Root Component that acts as central point between the GameEntities, their components and the GameSystems.
	/// </summary>
	[Obsolete]
	public class GameScene : MonoBehaviour
	{
		public static string systemsContainerName = "GameSystems";
		public static string entitiesContainerName = "GameEntities";
		public static string playersContainerName = "GamePlayers";

		[Header("GameScene")]
		//[SerializeField][ReadOnly]
		//private List<GameSceneSystem> systems = new List<GameSceneSystem>();
		//[SerializeField][ReadOnly]
		//private List<Player> players = new List<Player>();
		[SerializeField][ReadOnly]
		private Transform systemsContainer;
		[SerializeField][ReadOnly]
		private Transform entitesContainer;
		[SerializeField][ReadOnly]
		private Transform playersContainer;

		[SerializeField, HideInInspector]
		private int version = 1;
		[SerializeField, HideInInspector]
		private int major = 2;
		[SerializeField, HideInInspector]
		private int minor = 4;

		// This is just temporal until i manage to do a proper singleton. First I need to take some design decisions.
		static private GameScene instance = null;
		static private GameScene Get() { if(instance==null) instance = GameObject.FindObjectOfType<GameScene>(); return instance; }
		static public Transform EntitiesParent { get { return Get()!=null ? Get().EntitiesContainer : null; } }
		static public Transform SystemsParent { get { return Get()!=null ? Get().SystemsContainer : null; } }
		static public Transform PlayersParent { get { return Get()!=null ? Get().PlayersContainer : null; } }

		public Transform SystemsContainer {	get	{ return GetValidContainer( ref systemsContainer, systemsContainerName); }	}
		public Transform EntitiesContainer {	get	{ return GetValidContainer( ref entitesContainer, entitiesContainerName); }	}
		public Transform PlayersContainer {	get	{ return GetValidContainer( ref playersContainer, playersContainerName); }	}

		#region Maintenance Methods

		/// <summary>
		/// This function is called when the script is loaded or a value is changed in the inspector.
		/// Cleans the list of SceneSystems, Removing null components.
		/// (Called in the editor only).
		/// </summary>
		private void OnValidate()
		{
			UpgradeVersionCheck();
			GetValidContainer( ref systemsContainer, systemsContainerName);
			GetValidContainer( ref entitesContainer, entitiesContainerName);
			GetValidContainer( ref playersContainer, playersContainerName);
			//RefreshGameSystemsLists();
		}

		private void Reset()
		{
			GetValidContainer( ref systemsContainer, systemsContainerName);
			GetValidContainer( ref entitesContainer, entitiesContainerName);
			GetValidContainer( ref playersContainer, playersContainerName);
			//RefreshGameSystemsLists();
		}

		//private void RefreshGameSystemsLists()
		//{
		//	List<GameSceneSystem> newList = new List<GameSceneSystem>();
		//	// Remove empty slots
		//	foreach (GameSceneSystem system in systems)
		//	{
		//		if (system != null && system.gameObject != null)
		//			newList.Add(system);
		//	}
		//	systems = newList;
		//}

		private Transform GetValidContainer(ref Transform result, string name)
		{
			if(result==null || result.gameObject == null)
			{
				result = transform.Find(name);
				if(result==null)
				{
					GameObject go = new GameObject(name);
					go.transform.SetParent(transform);
					go.transform.localPosition = Vector3.zero;
					result = go.transform;
				}
			}
			return result;
		}
		#endregion Maintenance Methods

		#region Static Helpers
		/// <summary>
		/// Find the GameScene component jumping from parent to parent until its found and starting at the given transform
		/// </summary>
		/// <param name="startingAt">Transform where to start the search.</param>
		/// <returns>Returns the current GameScene of null if not found.</returns>
		public static GameScene FindInHierarchy(Transform startingAt)
		{
			Transform searchOn = startingAt;
			while( searchOn != null )
			{
				GameScene result = searchOn.GetComponent<GameScene>();
				if( result != null )
					return result;
				searchOn = searchOn.parent;
			}
			return null;
		}

		public static bool ValidateExistsInHierarchy(Transform startingAt)
		{
			if( FindInHierarchy(startingAt) == null )
			{
				Debug.LogError("Requires a GameScene.", startingAt.gameObject);
				return false;
			}
			return true;
		}

		public static bool ValidateExists(UnityEngine.Object context =null)
		{
			GameScene result = GameObject.FindObjectOfType<GameScene>();
			if(result==null)
			{
				Debug.LogError("Requires a GameScene.", context);
				return false;
			}
			return true;
		}

		#endregion Static Helpers

		public int Version { get { return CalcVersion(version, major, minor); } }
		private int CalcVersion(int v,int maj, int min) { return v * 10000 + maj * 100 + min; }
		private void SetVersion(int v,int maj, int min) { version = v; major = maj; minor = min; }
		private bool RequiresUpgrade(int v,int maj, int min) { return Version < CalcVersion(v,maj,min); }

		private void UpgradeVersionCheck()
		{
			if( RequiresUpgrade(1,2,4) )
			{
				// Upgrading to version 1.2.4
				SetVersion(1,2,4);
			}
		}


	}
}