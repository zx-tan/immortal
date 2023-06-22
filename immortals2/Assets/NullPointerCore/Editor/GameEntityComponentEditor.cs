using NullPointerCore;
using UnityEditor;

namespace NullPointerEditor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(GameEntityComponent), true)]
	public class GameEntityComponentEditor : Editor
	{
		private GameEntityComponent Target { get { return target as GameEntityComponent; } }

		// Draw the property inside the given rect
		public override void OnInspectorGUI()
		{
			NullPointerGUIUtility.DrawRequiredGameSystems(targets, Target.gameObject);
			base.DrawDefaultInspector();
		}
	}
}
