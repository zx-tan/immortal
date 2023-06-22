using NullPointerCore;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor.AttributeExtension
{
	[CustomPropertyDrawer (typeof (HelpBoxAttribute))]
	public class HelpBoxDrawer : PropertyDrawer
	{
		// Draw the property inside the given rect
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			if(string.IsNullOrEmpty(property.stringValue))
				return;
			//EditorGUI.HelpBox(position, property.stringValue, MessageType.Warning);
			float titleHeight = EditorStyles.miniBoldLabel.lineHeight*2;
			Rect titleRect = new Rect(position.x, position.y, position.width, titleHeight);
			Rect contentRect = new Rect(position.x, position.y+titleHeight, position.width, position.height-titleHeight);

			EditorGUI.HelpBox(titleRect, "Validation Results", MessageType.Warning);
			EditorGUI.SelectableLabel(contentRect, property.stringValue, EditorStyles.helpBox);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if(string.IsNullOrEmpty(property.stringValue))
				return 0;
			int linesCount = 0;
			int startIndex = -1;
			while( (startIndex = property.stringValue.IndexOf('\n', startIndex+1)) != -1)
				linesCount++;
			float titleHeight = EditorStyles.miniBoldLabel.lineHeight*2;
			float borders = EditorStyles.helpBox.border.top + EditorStyles.helpBox.border.bottom;
			return titleHeight + borders + EditorStyles.helpBox.lineHeight*linesCount;
			//return EditorGUI.GetPropertyHeight(property) * linesCount+1;
			//return EditorStyles.helpBox.Calc
			//return EditorStyles.helpBox.lineHeight * linesCount;
			//return base.GetPropertyHeight(property, label) * linesCount;
		}
	}
}
