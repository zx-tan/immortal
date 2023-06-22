using NullPointerCore;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor
{
	[CustomEditor(typeof(ComponentProxy))]
	public class ComponentProxyEditor : Editor
	{

		// Draw the property inside the given rect
		public override void OnInspectorGUI()
		{
			bool objChanged = false;
			ComponentProxy myTarget = (ComponentProxy)target;
			int itemsCount = myTarget.ElementsCount;
			int toRemove = -1;
			ComponentProxy.ProxyData item;

			EditorGUI.BeginChangeCheck();

			// Draws the line with the quantity of properties (Also the button to add properties).
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label("Elements:", EditorStyles.boldLabel);
				GUILayout.Label(itemsCount.ToString(), EditorStyles.helpBox);
				// We add new empty properties with this button.
				if (GUILayout.Button("+"))
				{
					Undo.RecordObject(myTarget, "ComponentProxy add element");
					myTarget.AddEmptyElement();
					objChanged = true;
				}
			}
			EditorGUILayout.EndHorizontal();

			// Here we draw the list of properties
			for ( int i=0; i < myTarget.ElementsCount; i++)
			{
				myTarget.ElementAt(i, out item);

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.Space();

					// Text field to hold the property name. This only will be valid when the text is confirmed
					// (must press intro to confirm)
					// Seems to be a bug here in DelayedTextField, the text is never confirmed if the user changes the
					// focus completly (through a click) and the text would be erased.
					// With some luck this will be fixed in next versions of unity
					// Here are the reports that i manage to find about the subject:
					// https://issuetracker.unity3d.com/issues/settings-under-terrain-settings-resolution-get-reverted-if-lose-focus
					// https://issuetracker.unity3d.com/issues/script-define-symbols-value-gets-reverted-to-its-previous-value-when-losing-focus
					string newName = EditorGUILayout.DelayedTextField(item.name);
					if (newName != item.name)
					{
						Undo.RecordObject(myTarget, "Name Changed");
						objChanged = true;
						myTarget.ChangePropertyName(i, newName);
					}

					// Next: The object field for the GameObject that has the component we want to add
					GameObject newObj = EditorGUILayout.ObjectField(item.obj, typeof(GameObject), true) as GameObject;
					objChanged |= (newObj != item.obj);
					if(objChanged)
						Undo.RecordObject(myTarget, "Object Reference");
					item.obj = newObj;
					if (newObj == null)
						item.comp = null;

					List<Component> components = new List<Component>();
					List<string> enumItems = new List<string>();
					int selectedComp = 0;
					enumItems.Add("Undefined Component");

					if( item.obj != null )
					{
						item.obj.GetComponents(typeof(Component), components);
						foreach (Component comp in components)
						{
							if(comp!=null && comp.gameObject!=null)
								enumItems.Add(comp.GetType().Name.ToString());
						}
						if (item.comp != null)
							selectedComp = components.IndexOf(item.comp)+1;
					}

					int selectedFinal = EditorGUILayout.Popup(selectedComp, enumItems.ToArray());
					if (selectedFinal != selectedComp)
					{
						Undo.RecordObject(myTarget, "Component Selection");
						objChanged |= true;
						if (selectedFinal == 0)
							item.comp = null;
						else
							item.comp = components[selectedFinal - 1];
					}
					if (GUILayout.Button("-", EditorStyles.miniButton))
					{
						objChanged = true;
						toRemove = i;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Space();
			if(objChanged)
				EditorUtility.SetDirty(target);

			if ( toRemove >= 0)
			{
				Undo.RecordObject(myTarget, "Element Removed");
				myTarget.RemoveAt(toRemove);
				toRemove = -1;
			}
		}
	}
}