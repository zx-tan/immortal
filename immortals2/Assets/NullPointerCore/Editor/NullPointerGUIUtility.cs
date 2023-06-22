using NullPointerCore;
using System;
using UnityEditor;
using UnityEngine;

namespace NullPointerEditor
{
	public class NullPointerGUIUtility
	{
		static GUIContent warnContent = null;
		static private GUIContent WarnContent { get { if(warnContent==null) warnContent = EditorGUIUtility.IconContent("console.warnicon"); return warnContent; } }
		static private GUIContent GetWarnContent(string text) { WarnContent.text = text; return WarnContent; }

		public static bool DrawWarnBox(string warnText, string btnText=null)
		{
			EditorGUILayout.LabelField(GUIContent.none, GetWarnContent(warnText), EditorStyles.helpBox, GUILayout.Height(EditorGUIUtility.singleLineHeight+6));
			if(!string.IsNullOrEmpty(btnText))
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				lastRect.yMin += 3;
				lastRect.yMax -= 3;
				lastRect.xMax -= 3;
				lastRect.xMin = lastRect.xMax-50;
				return GUI.Button(lastRect, btnText);
			}
			return false;
		}

		public static void DrawRequiredGameSystems(object[] objects, GameObject context)
		{
			if(EditorApplication.isPlaying || !context.scene.IsValid() )
				return;

			//GameScene gameScene = EditorHelpers.GetGameScene();

			foreach ( Type type in RequireGameSystemAttribute.Collect(objects))
			{
				GameSystem system = Game.GetSystem(type);
				if (system!=null)
					continue;

				if(DrawWarnBox("GameSystem Required: "+type.Name, "Fix"))
				{
					system = Game.CreateDefaultSystem(type, context);
					if(system!=null)
						EditorGUIUtility.PingObject(system);
				}
			}

		}
	}
}
