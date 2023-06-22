using NullPointerCore;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor
{
	[CustomPropertyDrawer(typeof(ProxyRef), true)]
	public class ProxyRefDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			//SerializedProperty componentProp = property.FindPropertyRelative("cache");
			SerializedProperty nameProp = property.FindPropertyRelative("refname");
			SerializedProperty refTypeProp = property.FindPropertyRelative("refType");
			// I dont know why allways returning null for the "allowedType" field
			//SerializedProperty compTypeProp = property.FindPropertyRelative("allowedType");
			ProxyRef proxyRef = EditorHelpers.GetPropertySource<ProxyRef>(property);
		
			ProxyRef.RefType currType = (ProxyRef.RefType) (refTypeProp!=null?refTypeProp.enumValueIndex:0);
		

			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);
			Rect rectContent = EditorGUI.PrefixLabel(position, label);

			float typeWidth = 80.0f;
			Rect rectObject = new Rect(rectContent.x, rectContent.y, rectContent.width-typeWidth, rectContent.height );
			Rect rectType = new Rect(rectContent.x+rectContent.width-typeWidth, rectContent.y, typeWidth, rectContent.height );

			if(currType== ProxyRef.RefType.Direct)
			{
				//EditorGUI.ObjectField(rectObject, componentProp, proxyRef.allowedType, GUIContent.none);
				if(property.serializedObject.isEditingMultipleObjects)
					EditorGUI.HelpBox(rectObject, "Multi-Value editing not allowed", MessageType.Warning);
				else
				{
				
					EditorGUI.BeginChangeCheck();
					Component newVal = EditorGUI.ObjectField(rectObject, GUIContent.none, proxyRef.cache, proxyRef.allowedType, true ) as Component;
					if( EditorGUI.EndChangeCheck() )
					{
						Undo.RecordObject(property.serializedObject.targetObject, "Assign reference");
						proxyRef.cache = newVal;
					}
				}
			}
			else
				EditorGUI.PropertyField(rectObject, nameProp, GUIContent.none);
			EditorGUI.PropertyField(rectType, refTypeProp, GUIContent.none);
			EditorGUI.EndProperty();
		}
	}
}