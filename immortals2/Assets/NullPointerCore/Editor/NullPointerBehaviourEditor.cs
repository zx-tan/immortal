using NullPointerCore;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor
{
	[CustomEditor(typeof(NullPointerBehaviour), true)]
	public class NullPointerBehaviourEditor : Editor
	{
		static GUIContent helpContent = null;
		List<Type> requiredSystemTypes = new List<Type>();

		protected virtual void OnEnable()
		{
			RefreshRequiredSystems();
		}

		public override void OnInspectorGUI()
		{
			bool recheck = false;
			//GameSystem newGameSystem = null;
			if (IsInScene(target as Component))
			{
				foreach (Type systemType in requiredSystemTypes)
				{
					if (FixableWarning("Requires GameSystem: " + systemType.Name))
					{
						FixMissingGameSystem(systemType);
						//if (recheck = GameSystem.CreateDefault(systemType, out newGameSystem, target) )
						//	Selection.activeObject = newGameSystem.gameObject;
					}
				}
				if (recheck)
					RefreshRequiredSystems();

				EditorGUILayout.Space();
			}
			base.DrawDefaultInspector();
		}

		private bool IsInScene(Component comp)
		{
			return comp != null && comp.gameObject != null && comp.gameObject.scene.IsValid();

		}

		public static bool FixableWarning(string helpText)
		{
			if (helpContent == null)
				helpContent = EditorGUIUtility.IconContent("console.warnicon");
			helpContent.text = helpText;
			GUILayout.Label(helpContent, EditorStyles.helpBox, GUILayout.Height(22));
			Rect position = GUILayoutUtility.GetLastRect();
			position.x = position.x + position.width - 60;
			position.width = 60;
			return GUI.Button(position, "Fix");
		}

		private void RefreshRequiredSystems()
		{
			requiredSystemTypes.Clear();
			object[] attributes = target.GetType().GetCustomAttributes(typeof(RequireGameSystemAttribute), true);
			foreach (object attr in attributes)
			{
				RequireGameSystemAttribute requiredSystem = attr as RequireGameSystemAttribute;
				if( GameObject.FindObjectOfType(requiredSystem.gameSystemType) == null )
					requiredSystemTypes.Add(requiredSystem.gameSystemType);
			}
		}

		private bool FixMissingGameSystem(Type systemType)
		{
			List<Type> compTypes = EditorHelpers.CollectAvailableComponents(systemType);

			if (compTypes.Count > 1)
			{
				// create the menu and add items to it
				GenericMenu menu = new GenericMenu();
				// forward slashes nest menu items under submenus
				foreach (Type type in compTypes)
					menu.AddItem(new GUIContent(type.Name), false, OnAddRequestedGameSystem, type);
				// display the menu
				menu.ShowAsContext();
			}
			else if (compTypes.Count == 1)
			{
				AddRequestedGameSystem(compTypes[0]);
				return true;
			}
			return false;
		}

		private void OnAddRequestedGameSystem(object compTypeObj)
		{
			AddRequestedGameSystem(compTypeObj as Type);
		}

		private bool AddRequestedGameSystem(Type gameSystemType)
		{
			bool result = false;
			GameSystem newGameSystem = null;
			if (result = GameSystem.CreateDefault(gameSystemType, out newGameSystem, target))
				Selection.activeObject = newGameSystem.gameObject;
			return result;
		}
	}
}