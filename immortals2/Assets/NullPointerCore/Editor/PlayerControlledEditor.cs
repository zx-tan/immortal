using NullPointerCore;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(PlayerControlled))]
	class PlayerControlledEditor : Editor
	{
		private IEnumerable<PlayerControlled> Targets 
		{
			get
			{
				foreach(Object obj in targets)
					yield return obj as PlayerControlled;
			}
		}

		// Draw the property inside the given rect
		public override void OnInspectorGUI()
		{
			base.DrawDefaultInspector();

			SerializedProperty ownerProperty = this.serializedObject.FindProperty("initialOwner");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(ownerProperty);
			this.serializedObject.ApplyModifiedProperties();
			if( EditorGUI.EndChangeCheck() && Application.isPlaying )
			{
				foreach(PlayerControlled target in Targets)
					target.Owner = target.initialOwner;
			}
		}
	}
}
