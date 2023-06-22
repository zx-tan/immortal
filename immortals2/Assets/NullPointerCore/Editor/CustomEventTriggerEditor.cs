using NullPointerCore.Extras;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor
{
	[CustomEditor(typeof(CustomEventTrigger))]
	public class CustomEventTriggerEditor : Editor
	{
		class ActionData
		{
			public string menuName;
			public Component component;
			public string fieldName;
		}

		List<ActionData> availableActions = new List<ActionData>();

		private SerializedProperty triggersProp;

		public void OnEnable ()
		{
			triggersProp = serializedObject.FindProperty("entries");
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if( GUILayout.Button("Add Event", GUILayout.Width(200)) )
			{
				CollectAvailableAction(target as Component);

				// create the menu and add items to it
				GenericMenu menu = new GenericMenu();
				// forward slashes nest menu items under submenus
				foreach(ActionData data in availableActions)
					menu.AddItem(new GUIContent(data.menuName), false, OnEventAdded, data);
				// display the menu
				menu.ShowAsContext();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			serializedObject.Update ();
			int toDelete=-1;
			for( int i=0; i<triggersProp.arraySize; i++)
			{
				SerializedProperty triggerProp = triggersProp.GetArrayElementAtIndex(i);
				SerializedProperty eventProp = serializedObject.FindProperty(triggerProp.propertyPath+".trigger");
				SerializedProperty fieldProp = serializedObject.FindProperty(triggerProp.propertyPath+".actionName");
				SerializedProperty compProp = serializedObject.FindProperty(triggerProp.propertyPath+".obj");
				string name = compProp.objectReferenceValue.GetType().Name + "." + fieldProp.stringValue;
				EditorGUILayout.PropertyField (eventProp, new GUIContent (name));
				Rect rc = GUILayoutUtility.GetLastRect();
				rc.xMin = rc.xMax - 20;
				rc.yMin += 1; 
				rc.height = 14;
				rc.width = 14;
				if( GUI.Button(rc, "-", GUI.skin.GetStyle("OL Minus")) )
					toDelete = i;
			}
			EditorGUILayout.Separator();

			if(toDelete >= 0)
				triggersProp.DeleteArrayElementAtIndex(toDelete);

			serializedObject.ApplyModifiedProperties ();
		}

		private void OnEventAdded(object objData)
		{
			ActionData actionData = objData as ActionData;
			CustomEventTrigger cet = target as CustomEventTrigger;
			cet.AddEventTrigger(actionData.component, actionData.fieldName);
		}

		private void CollectAvailableAction(Component obj)
		{
			availableActions.Clear();
			Type actionType = typeof(System.Action);
			Component[] componets = obj.GetComponents<Component>();
			foreach(Component comp in componets)
			{
				FieldInfo [] fields = comp.GetType().GetFields();
				foreach(FieldInfo fieldInfo in fields)
				{
					if( fieldInfo.FieldType == actionType )
					{
						ActionData actionData = new ActionData();
						actionData.menuName = comp.GetType().Name + "/" + fieldInfo.Name;
						actionData.fieldName = fieldInfo.Name;
						actionData.component = comp;
						availableActions.Add(actionData);
					}
				}
			}
		}
	}
}