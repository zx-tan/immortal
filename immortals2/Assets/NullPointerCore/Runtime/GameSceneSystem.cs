using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// Base class for all the game systems.
	/// </summary>
	[System.Obsolete]
	public class GameSceneSystem : MonoBehaviour
	{
		/*
		/// <summary>
		/// Reference to the current GameScene.
		/// </summary>
		[SerializeField, HideInInspector]
		private GameScene _gameScene = null;

		public GameScene gameScene 
		{
			get
			{
				if (_gameScene == null)
					_gameScene = GameScene.FindInHierarchy(transform);
				if (_gameScene && !_gameScene.IsRegistered(this))
					_gameScene.Register(this);
				return _gameScene;
			}
		}
		/// <summary>
		/// Finds the GameScene in the current hierarchy going from parent to parent.
		/// Also registers the scene system in that GameScene.
		/// </summary>
		virtual protected void OnValidate()
		{
			ValidateSceneSystem();
		}

		virtual protected void Reset()
		{
			ValidateSceneSystem();
		}

		protected virtual void ValidateSceneSystem()
		{
			if( this.gameObject.scene.IsValid())
			{
				if (_gameScene == null)
					_gameScene = GameScene.FindInHierarchy(transform);
				if (_gameScene && !_gameScene.IsRegistered(this))
					_gameScene.Register(this);
			}
		}

		public static bool ValidateExists<T>(UnityEngine.Object context) where T : GameSceneSystem
		{
			GameScene gameScene = GameObject.FindObjectOfType<GameScene>();
			T result = gameScene.Get<T>();
			if(result == null)
			{
				Debug.LogWarning("Requires the GameSceneSystem: "+ typeof(T).Name+". Creating a new one with default params.", context);
				return false;
			}
			return true;
		}

		public static void CreateDefault<T>(UnityEngine.Object context) where T : GameSceneSystem
		{
			GameScene gameScene = GameObject.FindObjectOfType<GameScene>();
			gameScene.CreateDefaultSystem(typeof(T), context);
		}

		public static T Find<T>(UnityEngine.Object context) where T : GameSceneSystem
		{
			GameScene gameScene = GameObject.FindObjectOfType<GameScene>();
			if(gameScene==null)
				return default(T);
			return gameScene.Get<T>();
		}

		public static T GetValid<T>(UnityEngine.Object context) where T : GameSceneSystem
		{
			if( !GameSceneSystem.ValidateExists<T>(context) )
				GameSceneSystem.CreateDefault<T>(context);
			return GameSceneSystem.Find<T>(context);
		}

		public virtual void GenerateDefaultValues()
		{
		}*/
	}
}