using NullPointerCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor
{
	[CustomEditor(typeof(SelectionSystem))]
	class SelectionSystemEditor : Editor
	{
		private SelectionSystem Target { get { return target as SelectionSystem; } }

		public override void OnInspectorGUI()
		{
			base.DrawDefaultInspector();
			EditorGUILayout.Space();

			SelectionInputBase input = GameObject.FindObjectOfType<SelectionInputBase>();
			if ( input == null )
			{
				if( NullPointerBehaviourEditor.FixableWarning("Requires SelectionInputBase.") )
				{
					FixMissingSelectionInputBase();
				}
			}


			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Space();
			EditorGUI.BeginDisabledGroup(true);
			GUILayout.BeginVertical("Stats", GUI.skin.box);
			GUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.IntField("Selectables Count: ", Target.SelectablesCount);
			EditorGUILayout.Space();
			EditorGUILayout.IntField("Hover Count: ", Target.HoveringCount);
			EditorGUILayout.IntField("Selected Count: ", Target.SelectedCount);
			GUILayout.EndVertical();
			EditorGUI.EndDisabledGroup();
		}

		public override bool RequiresConstantRepaint()
		{
			return Application.isPlaying;
		}

		private IEnumerable<Component> Targets {
			get
			{
				foreach (UnityEngine.Object obj in targets)
				{
					if (obj is Component)
						yield return obj as Component;
				}
			}
		}

		private void FixMissingSelectionInputBase()
		{
			List<Type> compTypes = EditorHelpers.CollectAvailableComponents<SelectionInputBase>();

			if (compTypes.Count > 1)
			{
				// create the menu and add items to it
				GenericMenu menu = new GenericMenu();
				// forward slashes nest menu items under submenus
				foreach (Type type in compTypes)
					menu.AddItem(new GUIContent(type.Name), false, OnAddSelectionInputBaseRequested, type);
				// display the menu
				menu.ShowAsContext();
			}
		}

		private void OnAddSelectionInputBaseRequested(object compTypeObj)
		{
			Type componentType = compTypeObj as Type;

			// A little hardcode first...
			if (componentType == typeof(UISelectionInput))
				CreateDefaultUISelectionInput();
			else
			{
				MethodInfo createMethod = componentType.GetMethod("CreateDefaultGameObject");
				if (createMethod != null)
				{
					createMethod.Invoke(null, null);
				}
				else
					Debug.LogWarning("[" + componentType.Name + "] Unable to create the default object. Requires an static method called CreateDefaultGameObject().");
			}
		}

		private void CreateDefaultUISelectionInput()
		{
			GameObject canvas = EditorUIHelpers.CreateNewUI();
			NullPointerMenu.CreateUISelectionInput(canvas);
		}
	}
}
