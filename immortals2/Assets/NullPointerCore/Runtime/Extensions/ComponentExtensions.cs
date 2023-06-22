using System;
using System.Collections.Generic;
using UnityEngine.AI;

namespace UnityEngine
{
	/// <summary>
	/// Contains some usefull Component extension methods.
	/// </summary>
	public static class ComponentExtensions
	{
#if UNITY_EDITOR
		[UnityEditor.MenuItem("CONTEXT/Transform/Locate In NavMesh", false, 152)]
		static void CopyRotation()
		{
			NavMeshHit hit;
			if (NavMesh.SamplePosition(UnityEditor.Selection.activeTransform.position, out hit, 5.0f, 1))
				UnityEditor.Selection.activeTransform.position = hit.position;
		}
#endif

		/// <summary>
		/// Returns an Array of T components in the direct childrens of this GameObjects
		/// </summary>
		/// <typeparam name="T">The type of Component to retrieve.</typeparam>
		/// <param name="comp">The context component of this call.</param>
		/// <param name="includeInactive">Should Components on inactive GameObjects be included in the found set?</param>
		/// <returns><strong>T[]</strong> A component array of the matching type, if found.</returns>
		public static T[] GetComponentsInDirectChildren<T>(this Component comp, bool includeInactive) where T : Component
		{
			List<T> result = new List<T>();
			GetComponentsInDirectChildren(comp, includeInactive, result);
			return result.ToArray();
		}

		/// <summary>
		/// Obtains the list of T components in the direct childrens of the GameObject.
		/// A component is returned only if it is found on an active GameObject.<br>
		/// </summary>
		/// <typeparam name="T">The type of the component to be found.</typeparam>
		/// <param name="comp">The context component of this call.</param>
		/// <param name="result">Adds to this list each ocurrence of the components of type T in the direct children of this GameObject.</param>
		public static void GetComponentsInDirectChildren<T>(this Component comp, List<T> result) where T : Component
		{
			GetComponentsInDirectChildren(comp, false, result);
		}

		/// <summary>
		/// Returns all components of Type type in the direct childrens of the GameObject.
		/// </summary>
		/// <param name="comp">The context component of this call.</param>
		/// <param name="type">The type of Component to retrieve.</param>
		/// <param name="includeInactive">Should Components on inactive GameObjects be included in the found set?</param>
		/// <returns>All the components of Type type in the direct childrens of the GameObject.</returns>
		public static Component[] GetComponentsInDirectChildren(this Component comp, Type type, bool includeInactive=false)
		{
			List<Component> result = new List<Component>();
			for (int i = 0; i < comp.transform.childCount; i++)
			{
				Transform transf = comp.transform.GetChild(i);
				if (transf.gameObject.activeSelf || includeInactive)
				{
					foreach (Component c in transf.GetComponents(type))
					{
						if (!result.Contains(c))
							result.Add(c);
					}
				}
			}
			return result.ToArray();
		}

		/// <summary>
		/// Extension that collects the complete collection of components for the direct childrens of the GameObject of this Component.
		/// </summary>
		/// <typeparam name="T">The type of the component to be found.</typeparam>
		/// <param name="comp">The context component of this call.</param>
		/// <returns>An array of Components of type T that been found in the direct children of this GameObject.</returns>
		public static T[] GetComponentsInDirectChildren<T>(this Component comp) where T : Component
		{
			List<T> results = new List<T>();
			GetComponentsInDirectChildren(comp, false, results);
			return results.ToArray();
		}

		/// <summary>
		/// Extension that collects the complete collection of components for the direct childrens of the GameObject of this Component.
		/// </summary>
		/// <typeparam name="T">The type of the component to be found.</typeparam>
		/// <param name="comp">The context component of this call.</param>
		/// <param name="includeInactive">Indicates if the inactive GameObjects should be included in the search.</param>
		/// <param name="result">Adds to this list each ocurrence of the components of type T in the direct children of this GameObject.</param>
		public static void GetComponentsInDirectChildren<T>(this Component comp, bool includeInactive, List<T> result) where T : Component
		{
			for (int i = 0; i<comp.transform.childCount; i++)
			{
				Transform t = comp.transform.GetChild(i);
				if (t.gameObject.activeSelf || includeInactive)
				{
					foreach (T c in t.GetComponents<T>())
					{
						if (!result.Contains(c))
							result.Add(c);
					}
				}
			}
		}

		/// <summary>
		/// Extension that collect the first ocurrence of the requested component in the direct children of this Component.
		/// </summary>
		/// <typeparam name="T">Type of Component to search for.</typeparam>
		/// <param name="comp">The component context of this call.</param>
		/// <param name="includeInactive">Indicates if the inactive GameObjects should be included in the search.</param>
		/// <returns>The first ocurrence of the component of type T in the childrens of this GameObject.</returns>
		public static T GetComponentInDirectChildren<T>(this Component comp, bool includeInactive=false) where T : Component
		{
			for (int i = 0; i < comp.transform.childCount; i++)
			{
				Transform t = comp.transform.GetChild(i);
				if (t.gameObject.activeSelf || includeInactive)
				{
					T c = t.GetComponent<T>();
					if (c != null)
						return c;
				}
			}
			return null;
		}

		/// <summary>
		/// Calls DestroyImmediate if it's in Editor and isn't playing. Otherwise calls Destroy.
		/// </summary>
		/// <param name="comp">The related component in the context of this call.</param>
		/// <param name="go">The gameObject to destroy.</param>
		public static void SafeDestroy(this Component comp, Object go)
		{
#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlaying)
				GameObject.Destroy(go);
			else
				GameObject.DestroyImmediate(go);
#else
			GameObject.Destroy(go);
#endif
		}
	}
}