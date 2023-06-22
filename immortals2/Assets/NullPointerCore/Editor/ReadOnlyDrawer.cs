using NullPointerCore;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor.AttributeExtension
{
	[CustomPropertyDrawer (typeof (ReadOnlyAttribute))]
	public class ReadOnlyDrawer : PropertyDrawer
	{
		// Draw the property inside the given rect
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.PropertyField(position, property, true);
			EditorGUI.EndDisabledGroup();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label);
		}
	}
}