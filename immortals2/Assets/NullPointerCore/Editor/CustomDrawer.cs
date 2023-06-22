using GameBase.AttributeExtension;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor.AttributeExtension
{
	[CustomPropertyDrawer (typeof (CustomAttribute))]
	public class CustomDrawer : PropertyDrawer
	{
		// Draw the property inside the given rect
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			bool visible = true;
			bool readOnly = true;
			CustomAttribute customAttr = (CustomAttribute)attribute;
			if(!string.IsNullOrEmpty(customAttr.conditional))
				visible = FindPropertyStatus(property, customAttr.conditional);
			readOnly = customAttr.readOnly;

			if(visible)
			{
				int savedIndentation = EditorGUI.indentLevel;

				if(readOnly)
					EditorGUI.BeginDisabledGroup(true);

				EditorGUI.indentLevel = savedIndentation + customAttr.indent;
				EditorGUI.PropertyField(position, property, true);
				EditorGUI.indentLevel = savedIndentation;

				if(readOnly)
					EditorGUI.EndDisabledGroup();
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			bool visible = true;
			CustomAttribute condHideAttribute = (CustomAttribute)attribute;
			if(!string.IsNullOrEmpty(condHideAttribute.conditional))
				visible = FindPropertyStatus(property, condHideAttribute.conditional);
			if(visible)
				return EditorGUI.GetPropertyHeight(property, label);
			return 0.0f;
		}

		public bool FindPropertyStatus(SerializedProperty property, string fieldName)
		{
			SerializedProperty evalProperty = property.serializedObject.FindProperty(fieldName);
			if(evalProperty!=null)
				return evalProperty.boolValue;
			return true;
		}
	}
}
