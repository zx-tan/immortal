using NullPointerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor
{
	[CustomEditor(typeof(UISelectionInput), true)]
	class UISelectionInputEditor : Editor
	{
		private GUIContent dropDownContent = new GUIContent("Add Extension");
		private GUIContent noMoreExtensions = new GUIContent("No More Extensions Available");

		private UISelectionInput Target { get { return target as UISelectionInput; } }

		public override void OnInspectorGUI()
		{
			base.DrawDefaultInspector();
			EditorGUILayout.Space();

			if (GUILayout.Button(dropDownContent))
				ShowExtensionsMenu();
		}

		private void ShowExtensionsMenu()
		{
			List<Type> compTypes = FilterAvailable(EditorHelpers.CollectAvailableComponents<IUIInputExtension>());

			if (compTypes.Count > 0)
			{
				// create the menu and add items to it
				GenericMenu menu = new GenericMenu();
				// forward slashes nest menu items under submenus
				foreach (Type type in compTypes)
					menu.AddItem(new GUIContent(type.Name), false, OnAddExtensionRequested, type);
				// display the menu
				menu.ShowAsContext();
			}
			else
			{

				GenericMenu menu = new GenericMenu();
				menu.AddDisabledItem(noMoreExtensions);
				menu.ShowAsContext();
			}
		}

		private List<Type> FilterAvailable(List<Type> list)
		{
			List<Type> result = new List<Type>();
			foreach (Type type in list)
			{
				if (Target.GetComponent(type) == null)
					result.Add(type);
			}
			return result;
		}

		private void OnAddExtensionRequested(object compTypeObj)
		{
			Type componentType = compTypeObj as Type;
			Target.gameObject.AddComponent(componentType);
		}
	}
}
