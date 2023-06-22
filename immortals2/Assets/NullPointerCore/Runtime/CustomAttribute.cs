using UnityEngine;

namespace GameBase.AttributeExtension
{
	/// <summary>
	/// Use this PropertyAttribute to achieve indentation, readonly or conditional feature at the same time.
	/// </summary>
	public class CustomAttribute : PropertyAttribute
	{
		/// <summary>
		/// Name of the field that controls if this one can be visible or not. (must be a bool)
		/// </summary>
		public string conditional="";
		/// <summary>
		/// Indentation level for this field.
		/// </summary>
		public int indent=0;
		/// <summary>
		/// Indicates if this field is read only or can be edited by the inspector.
		/// </summary>
		public bool readOnly = false;

		public CustomAttribute()
		{
		}
	}
}
