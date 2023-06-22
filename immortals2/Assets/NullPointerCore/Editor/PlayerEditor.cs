using NullPointerCore;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Player))]
	public class PlayerEditor : Editor
	{

		private IEnumerable<Player> Targets 
		{
			get
			{
				foreach(UnityEngine.Object obj in targets)
				{
					if( obj is Player )
						yield return obj as Player;
				}
			}
		}
	
		// Draw the property inside the given rect
		public override void OnInspectorGUI()
		{
			base.DrawDefaultInspector();

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if( GUILayout.Button("Add PlayerSystem") )
				ShowAddGameEntityComponentMenu();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}

		private void ShowAddGameEntityComponentMenu()
		{
			IEnumerable<Type> compTypes = EditorHelpers.CollectAvailableComponents<Player, PlayerSystem>(Targets);

			// create the menu and add items to it
			GenericMenu menu = new GenericMenu();
			// forward slashes nest menu items under submenus
			foreach(Type type in compTypes)
				menu.AddItem(new GUIContent(type.Name), false, OnComponentAdded, type);
			// display the menu
			menu.ShowAsContext();
		}

		private void OnComponentAdded(object compTypeObj)
		{
			Type componentType = compTypeObj as Type;

			foreach(Player ent in Targets)
			{
				if(ent.GetComponent(componentType)!=null)
					continue;
				ent.gameObject.AddComponent(componentType);
			}
		}
	}
}
