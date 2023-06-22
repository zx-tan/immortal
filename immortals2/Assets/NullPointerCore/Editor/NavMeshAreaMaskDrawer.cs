using NullPointerCore;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor.AttributeExtension
{
	[CustomPropertyDrawer(typeof(NavMeshAreaMaskAttribute))]
	public class NavMeshAreaMaskDrawer : PropertyDrawer
	{
		// Draw the property inside the given rect
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var areaIndex = property.intValue;
			var areaNames = GameObjectUtility.GetNavMeshAreaNames();

			EditorGUI.BeginProperty(position, GUIContent.none, property);

			EditorGUI.BeginChangeCheck();
			areaIndex = EditorGUI.MaskField(position, label, areaIndex, areaNames);

			if (EditorGUI.EndChangeCheck())
				property.intValue = areaIndex;

			EditorGUI.EndProperty();
		}
	}
}