using System.Collections.Generic;
using UnityEngine;

namespace NullPointerCore
{

	public interface IProxyModuleChanged
	{
		void OnVisualModuleSetted();
		void OnVisualModuleRemoved();
	}

	/// <summary>
	/// Core Component for each interactable Entity in the Game. Acts as a Controller for the visual Avatar
	/// through its ComponentProxy.
	/// </summary>
	[SelectionBase]
	public class GameEntity : MonoBehaviour
	{
		/// <summary>
		/// Reference to the ComponentProxy located at the root of the controlled Avatar.
		/// </summary>
		public ComponentProxy visualModule;

		private ComponentProxy currentModule = null;

		/// <summary>
		/// Returns the ComponentProxy (View) for this GameEntity (Controller)
		/// </summary>
		public ComponentProxy VisualProxy { get { return visualModule; } }


		//public PropertySet Properties { get { return properties; } }

		private bool ReadyToInitialize { get { return currentModule != null; } }

		/// <summary>
		/// Initialize the GameScene reference and if there is a VisualModule then notifies 
		/// to each GameEntityComponent that it can initialize their own systems.
		/// </summary>
		virtual protected void Start ()
		{
			if(visualModule!=null)
				ChangeVisualModule(visualModule);
		}

		virtual protected void OnDestroy()
		{
			if(visualModule!=null)
				ChangeVisualModule(null);
		}

		/// <summary>
		/// Change the visual module of this GameEntity. If already has a reference to a module than notifies to
		/// each GameEntityComponent that the VisualModule has been removed. Then assign the new module and 
		/// notifies to each GameEntityComponent that a visual module has been set.
		/// </summary>
		/// <param name="newVisualModule"></param>
		public void ChangeVisualModule( ComponentProxy newVisualModule )
		{
			if(currentModule == newVisualModule)
				return;
			if( currentModule != null )
			{
				GameEntityComponent [] components = GetComponents<GameEntityComponent>();
				foreach(GameEntityComponent component in components)
					component.OnVisualModuleRemoved();
				IProxyModuleChanged[] listeners = GetComponents<IProxyModuleChanged>();
				foreach (IProxyModuleChanged listener in listeners)
					listener.OnVisualModuleRemoved();
			}
			visualModule = newVisualModule;
			currentModule = newVisualModule;

			if( currentModule != null )
			{
				visualModule.transform.parent = this.transform;
				visualModule.transform.localPosition = Vector3.zero;
				visualModule.transform.localRotation = Quaternion.identity;

				GameEntityComponent [] components = GetComponents<GameEntityComponent>();
				foreach(GameEntityComponent component in components)
					component.OnVisualModuleSetted();
				IProxyModuleChanged[] listeners = GetComponents<IProxyModuleChanged>();
				foreach (IProxyModuleChanged component in listeners)
					component.OnVisualModuleSetted();
			}
		}
	
		/// <summary>
		/// Collects all the components of type <typeparamref name="T"/> from each GameEntity.
		/// Note that will return only components in the same GameObject of GameEntity and if they are found.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entities">Collection of GameEntity where to find the components.</param>
		/// <returns>A collection of components of type <typeparamref name="T"/>.</returns>
		public static IEnumerable<T> Collect<T>(IEnumerable<GameEntity> entities)
		{
			foreach (GameEntity entity in entities)
			{
				if(entity==null)
					continue;
				T comp = entity.GetComponent<T>();
				if(comp != null)
					yield return comp;
			}
			yield break;
		}
	}
}
