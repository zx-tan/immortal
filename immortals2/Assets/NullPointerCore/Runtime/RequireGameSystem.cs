using System;
using System.Collections.Generic;

namespace NullPointerCore
{
	/// <summary>
	/// Attribute used on NullPointerBehaviours to indicate that the component requires a specific GameSystem to work properly.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public class RequireGameSystemAttribute : Attribute
	{
		/// <summary>
		/// The required GameSystem type.
		/// </summary>
		public Type gameSystemType;
		/// <summary>
		/// Class attribute for 
		/// </summary>
		/// <param name="systemType">The type of the required GameSystem.</param>
		public RequireGameSystemAttribute(Type systemType) : base()
		{
			gameSystemType = systemType;
		}

		/// <summary>
		/// Collects all the RequireGameSystemAttribute in the class.
		/// </summary>
		/// <param name="objects">Array of object where to take the collection of attributes.</param>
		/// <returns>The collection of GameSystem classes types required by the Component.</returns>
		public static IEnumerable<Type> Collect(object [] objects)
		{
			HashSet<Type> requiredTypes = new HashSet<Type>();
			foreach( object compTarget in objects)
			{
				foreach( object objAttr in compTarget.GetType().GetCustomAttributes(true) )
				{
					if(objAttr is RequireGameSystemAttribute)
						requiredTypes.Add( (objAttr as RequireGameSystemAttribute).gameSystemType );
				}
			}
			foreach(Type requiredType in requiredTypes)
				yield return requiredType;
		}
	}
}
