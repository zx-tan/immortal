using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// Represents a Component reference that can be assigned directly through editor or must be taken
	/// from a component proxy in runtime.
	/// </summary>
	[System.Serializable]
	public class ProxyRef
	{
		/// <summary>
		/// Types of references for the component cache.
		/// </summary>
		public enum RefType
		{
			/// <summary>
			/// The reference cache must be assigned directly
			/// </summary>
			Direct,
			/// <summary>
			/// The reference cache must be taken from a ComponentProxy property.
			/// </summary>
			UseProxy,
		}
		/// <summary>
		/// The name of the property in the ComponentProxy to use. This value is not used if the refType is Direct.
		/// </summary>
		public string refname;
		/// <summary>
		/// The cache reference for the component to use.
		/// </summary>
		public Component cache;
		/// <summary>
		/// Indicates if the component cache must be assigned directly or through a ComponentProxy property.
		/// </summary>
		public RefType refType = RefType.UseProxy;
		/// <summary>
		/// The Component type allowed to be assigned in the editor.
		/// </summary>
		public System.Type allowedType;

		/// <summary>
		/// The casted component stored in the cache.
		/// </summary>
		/// <typeparam name="T">The Component type to convert in the result.</typeparam>
		/// <returns>The casted component reference in the cache or null if not found or the conversion is invalid.</returns>
		public T Get<T>() where T : Component { return cache as T; }

		/// <summary>
		/// Set the cached reference and sets the the allowedType as RefType.Direct.
		/// </summary>
		/// <param name="val">The new component reference value to cache.</param>
		public void SetDirect(Component val) { cache = val; refType=RefType.Direct; }

		/// <summary>
		/// Parameterless constructor. Sets the allowedType field as typeof(Component).
		/// </summary>
		public ProxyRef()
		{
			allowedType = typeof(Component);
		}

		/// <summary>
		/// Constructor that sets the component allowed type and the default property name for the ComponentProxy setting.
		/// </summary>
		/// <param name="type">The valid type for reference assignment.</param>
		/// <param name="defaultName">The default name for the ComponentProxy property.</param>
		public ProxyRef(System.Type type, string defaultName="")
		{
			allowedType = type;
			refname = defaultName;
		}

		#region Cache Assignment

		/// <summary>
		/// Assign a cached reference stored in the ComponentProxy according with the internal property name.
		/// </summary>
		/// <param name="proxy">The ComponentProxy where to take the reference.</param>
		public void Assign(ComponentProxy proxy)
		{ 
			if(proxy==null)
				return;
			if( string.IsNullOrEmpty(refname) )
				return;
			cache = proxy.GetPropertyValue(refname);
			refType = RefType.UseProxy;
		}

		/// <summary>
		/// Assigns the component reference stored in the GameEntity VisualProxy according with the stored property name.
		/// </summary>
		/// <param name="entity">The GameEntity that contains a VisualProxy where to take the reference.</param>
		public void Assign(GameEntity entity)
		{
			if(entity==null)
				return;
			Assign(entity.VisualProxy);
		}
				
		/// <summary>
		/// Assigns the cached component only if internally was configured to use a proxy component
		/// </summary>
		/// <param name="entity">The GameEntity that contains a VisualProxy where to take the reference.</param>
		public void SafeAssign(GameEntity entity)
		{
			if(entity==null || refType != RefType.UseProxy)
				return;
			Assign(entity.VisualProxy);
		}
		
		#endregion Cache Assignment

		#region Cache Clearment
		/// <summary>
		/// Forces to clear the cache reference of the component.
		/// </summary>
		public void Clear()
		{
			cache = null;
		}

		/// <summary>
		/// Removes the component reference only if internally was configured to use a proxy component.
		/// </summary>
		public void SafeClear()
		{
			if(refType == RefType.UseProxy)
				cache = null;
		}
		#endregion Cache Clearment

		#region Static Methods

		/// <summary>
		/// Indicates whether the specified ProxyRef is null or has an invalid cached reference.
		/// </summary>
		/// <param name="val">The ProxyRef to test.</param>
		/// <returns>true if the val parameter is null or contains an invalid cached reference; otherwise false.</returns>
		public static bool IsInvalid(ProxyRef val)
		{
			return val == null || val.cache == null || !val.allowedType.IsInstanceOfType(val.cache);
		}

		public static bool IsValid(ProxyRef val)
		{
			return !IsInvalid(val);
		}

		#endregion Static Methods
	}
}