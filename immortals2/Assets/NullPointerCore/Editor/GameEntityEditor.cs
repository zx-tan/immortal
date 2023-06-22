using NullPointerCore;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(GameEntity))]
	public class GameEntityEditor : Editor
	{

		private IEnumerable<GameEntity> Targets 
		{
			get
			{
				foreach(UnityEngine.Object obj in targets)
				{
					if( obj is GameEntity )
						yield return obj as GameEntity;
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
			if( GUILayout.Button("Add GameEntityComponent") )
				ShowAddGameEntityComponentMenu();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}

		private void ShowAddGameEntityComponentMenu()
		{
			IEnumerable<Type> compTypes = EditorHelpers.CollectAvailableComponents<GameEntity, GameEntityComponent>(Targets);

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

			foreach(GameEntity ent in Targets)
			{
				if(ent.GetComponent(componentType)!=null)
					continue;
				ent.gameObject.AddComponent(componentType);
			}
		}
	}
}