using System.Collections.Generic;
using UnityEngine;

namespace NullPointerCore
{

	/// <summary>
	/// Component class that does all the magic to store, index and return the important components from child GameObjects.
	/// </summary>
	public class ComponentProxy : MonoBehaviour, ISerializationCallbackReceiver
	{
		/// <summary>
		/// Contains all the data for each property
		/// </summary>
		[System.Serializable]
		public class ProxyData
		{
			public string name;		// name of the property
			public GameObject obj;	// Reference to the GameObject that owns the saved Component
			public Component comp;	// Component reference 
		}

		/// <summary>
		/// The Actual list of stored ProxyData that we have.
		/// </summary>
		[SerializeField]
		private List<ProxyData> items = new List<ProxyData>();
		/// <summary>
		/// Dictionary required to quick access to the data by name.
		/// </summary>
		[System.NonSerialized]
		private Dictionary<string, int> indexByName = new Dictionary<string, int>();

		/// <summary>
		/// Returns the quantity of registered elements.
		/// </summary>
		public int ElementsCount { get { return items.Count; } }

		// *****************************************************************
		#region Add, Set and Remove
		/// <summary>
		/// Adds a new item at the end of the list with empty data in it.
		/// </summary>
		/// <returns>The index of the last property added.</returns>
		public int AddEmptyElement()
		{
			DoAddProperty("", null, null);
			return items.Count - 1;
		}

		/// <summary>
		/// Changes the Component reference for the requested property. if the name of the property
		/// doesn't exist then we add it.
		/// </summary>
		/// <param name="propName">Name of the property to change</param>
		/// <param name="comp">Component reference to setup</param>
		public void SetPropertyData(string propName, Component comp)
		{
			int itemIndex = 0;
			GameObject go = comp != null ? comp.gameObject : null;
			// The property name exists?
			if (indexByName.TryGetValue(propName, out itemIndex))
			{
				// setup the new data
				items[itemIndex].comp = comp;
				items[itemIndex].obj = go;
			}
			else // if not we need to add it.
				DoAddProperty(propName, go, comp);
		}

		/// <summary>
		/// Changes the GameObject Reference for the requested property. also sets null in the component reference data. 
		/// If the name of the property doesn't exist then we add it.
		/// </summary>
		/// <param name="propName">Name of the property to change</param>
		/// <param name="go">GameObject reference to setup</param>
		public void SetPropertyData(string propName, GameObject go)
		{
			int itemIndex = 0;
			// The property name exists?
			if (indexByName.TryGetValue(propName, out itemIndex))
			{
				// setup the new data
				items[itemIndex].comp = null;
				items[itemIndex].obj = go;
			}
			else // if not we need to add it.
				DoAddProperty(propName, go, null);
		}

		/// <summary>
		/// Changes the name of the property at the requested index.
		/// </summary>
		/// <param name="index">Index of the property to change</param>
		/// <param name="newName">The new name of the property</param>
		public void ChangePropertyName(int index, string newName)
		{
			// Index out of range assert check.
			if (index < 0 || index >= items.Count)
				throw new System.IndexOutOfRangeException();
			// Nothing to do if the name is the same.
			if (newName == items[index].name)
				return;
			// duplicated name assertion.
			if (!string.IsNullOrEmpty(newName) && indexByName.ContainsKey(newName))
			{
				Debug.LogError("The property name '" + newName + "' is already in use.", this);
				return;
			}
			// Only valid names are indexed. null or empty string are not considered as valid.
			if(!string.IsNullOrEmpty(items[index].name))
				indexByName.Remove(items[index].name); // Remove the old name from the dictionary.
			// Setup the data with the new name.
			items[index].name = newName;
			// Same for the new name. must be a valid name.
			if(!string.IsNullOrEmpty(newName))
				indexByName.Add(newName, index); // Adds the new name to the dictionary pointing to the same property.
		}

		/// <summary>
		/// Remove a property by index position.
		/// </summary>
		/// <param name="index">The index position of the property to remove.</param>
		public void RemoveAt(int index)
		{
			// Index out of range assert check. 
			if (index < 0 || index >= items.Count)
				throw new System.IndexOutOfRangeException();
			// Only valid names are indexed. null or empty string are not considered as valid.
			if(!string.IsNullOrEmpty(items[index].name))
				indexByName.Remove(items[index].name);
			// Remove the property from the list.
			items.RemoveAt(index);
			// Here we need to revalidate all the indexes in the dictionary because they have an offset
			// because of the removed property in the list.
			RevalidateIndexes(index);
		}

		/// <summary>
		/// Remove a property by it name.
		/// </summary>
		/// <param name="propName">Name of the property to remove.</param>
		public void Remove(string propName)
		{
			int index = 0;
			// If the name its registered we need to find that index to remove the element.
			if (!indexByName.TryGetValue(propName, out index))
				return;

			// Removing the property by index position.
			items.RemoveAt(index);
			// Also removing the property name from the dictionary for a clean remove.
			indexByName.Remove(propName);
			// Here we need to revalidate all the indexes in the dictionary because they have an offset
			// because of the removed property in the list.
			RevalidateIndexes(index);
		}
		#endregion Add, Set and Remove

		// *****************************************************************
		#region Getters
	
		/// <summary>
		/// Returns the property data located at <paramref name="index"/>.
		/// </summary>
		/// <param name="index">Index of the property to return its data.</param>
		/// <param name="dataOut">Data structure that will be filled with the property requested.</param>
		public void ElementAt(int index, out ProxyData dataOut )
		{
			dataOut = items[index];
		}

		/// <summary>
		/// Returns true if there is a property registered with the name <paramref name="propName"/>.
		/// </summary>
		/// <param name="propName"></param>
		/// <returns></returns>
		public bool HasProperty(string propName)
		{
			return indexByName.ContainsKey(propName);
		}

		/// <summary>
		/// Return an already casted component registered by the <paramref name="propName"/>
		/// </summary>
		/// <typeparam name="T">Object type to return. T must inherit from Component.</typeparam>
		/// <param name="propName">Name of requested property.</param>
		/// <returns>The Cast of the component requested by <paramref name="propName"/> or null if not found.</returns>
		public T GetPropertyValue<T>(string propName) where T : Component
		{
			// Only valid names are indexed. null or empty string are not considered as valid.
			if(string.IsNullOrEmpty(propName))
			{
				Debug.LogError("Invalid Parameter: propName is empty or null.");
				return null;
			}
			int index = 0;
			if( indexByName.TryGetValue(propName, out index) )
				return items[index].comp as T; // Returns already converted as T.
			return null; // Returns null it the property is not found by its name.
		}

		/// <summary>
		/// Similar to GetPropertyValue() but returns a IEnumerable of type <typeparamref name="T"/> of all
		/// properties that match the start of its name with <paramref name="startsWith"/> and are casteable
		/// to <typeparamref name="T"/> type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="startsWith"></param>
		/// <returns>A IEnumerable interface of type <typeparamref name="T"/> with all the matched results.</returns>
		public IEnumerable<T> GetAllPropertyValues<T>(string startsWith) where T : Component
		{
			System.Type typeOf = typeof(T);
			// Iterates through all the properties
			foreach(ProxyData data in items)
			{
				// We need to match the start of the property name and the type of component
				if(data.name.StartsWith(startsWith) && typeOf.IsInstanceOfType(data.comp))
					yield return data.comp as T;
			}
			yield break; // No more elements, we just break the iteration
		}

		/// <summary>
		/// Returns a component registered by the <paramref name="propName"/>
		/// </summary>
		/// <param name="propName">Name of requested property.</param>
		/// <returns>The Component requested by <paramref name="propName"/> or null if not found.</returns>
		public Component GetPropertyValue(string propName)
		{
			// Only valid names are indexed. null or empty string are not considered as valid.
			if(string.IsNullOrEmpty(propName))
			{
				Debug.LogError("Invalid Parameter: propName is empty or null.");
				return null;
			}
			int index = 0;
			if( indexByName.TryGetValue(propName, out index) )
				return items[index].comp; // Returns already converted as T.
			return null; // Returns null it the property is not found by its name.
		}

		/// <summary>
		/// Returns the GameObject that belongs to the property <paramref name="propName"/>.
		/// </summary>
		/// <param name="propName">Name of the property to find.</param>
		/// <returns>The reference to the GameObject found, or null on fail.</returns>
		public GameObject GetPropertyGameObject(string propName)
		{
			// Only valid names are indexed. null or empty string are not considered as valid.
			if (string.IsNullOrEmpty(propName))
			{
				Debug.LogError("Invalid Parameter: propName is empty or null.");
				return null;
			}
			int index = 0;
			if (indexByName.TryGetValue(propName, out index))
				return items[index].obj;
			return null; // Returns null if propName is not found.
		}

		/// <summary>
		/// Return an already casted component at the <paramref name="index"/> position.
		/// </summary>
		/// <typeparam name="T">Object type to return. T must inherit from Component.</typeparam>
		/// <param name="index">Index position of requested property.</param>
		/// <returns>The Cast of the component requested by <paramref name="index"/> or null if not found.</returns>
		public T GetPropertyValueAt<T>(int index) where T : Component
		{
			if (index >= 0 && index < items.Count)
				return items[index].comp as T;
			return null;
		}

		/// <summary>
		/// Returns the GameObject that belongs to the property <paramref name="index"/>.
		/// </summary>
		/// <param name="index">index of the property to find.</param>
		/// <returns>The reference to the GameObject found, or null on fail.</returns>
		public GameObject GetPropertyGameObjectAt(int index)
		{
			if (index >= 0 && index < items.Count)
				return items[index].obj;
			return null;
		}
		#endregion Getters

		// *****************************************************************
		#region Private helper methods
	
		/// <summary>
		/// Adds a new property to the end of the list of properties.
		/// </summary>
		/// <param name="propName">Name of the new property.</param>
		/// <param name="go">GameObject reference for the new property. Generally will be <paramref name="comp"/>.gameObject.</param>
		/// <param name="comp">Component reference for the new property.</param>
		private void DoAddProperty(string propName, GameObject go, Component comp)
		{
			// We asume that all the checks were already made from outside of this method.
			// e.g.: duplicated names.
			ProxyData toAdd = new ProxyData();
			toAdd.name = propName;
			toAdd.comp = comp;
			toAdd.obj = go;
			// Also we need to index the new name if it's a valid name.
			if(!string.IsNullOrEmpty(propName))
				indexByName.Add(propName, items.Count);
			// Adds the new property to the list.
			items.Add(toAdd);
		}

		/// <summary>
		/// Revalidates all the indexes in the dictionary.
		/// </summary>
		/// <param name="startFrom">Just do the job for the indexes from <paramref name="startFrom"/> to the end.</param>
		private void RevalidateIndexes(int startFrom)
		{
			for (int i=startFrom; i<items.Count; i++)
			{
				if(!string.IsNullOrEmpty(items[i].name))
					indexByName[items[i].name] = i;
			}
		}
		#endregion

		// *****************************************************************
		#region Data Serialization

		/// <summary>
		/// Implementation of the method in ISerializationCallbackReceiver. 
		/// We don't need to do anything here but we can't remove this method.
		/// </summary>
		public void OnBeforeSerialize()
		{
		}

		/// <summary>
		/// Implementation of the method in ISerializationCallbackReceiver that allows us to rebuild the
		/// property names dictionary after scripts reloading.
		/// </summary>
		public void OnAfterDeserialize()
		{
			indexByName = new Dictionary<string, int>();
			for (int i = 0; i < items.Count; i++)
			{
				if(!string.IsNullOrEmpty(items[i].name))
					indexByName.Add(items[i].name, i);
			}
		}
		#endregion Data Serialization
	}
}