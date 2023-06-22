
using System.Collections.Generic;
using UnityEngine;

namespace NullPointerCore
{
	public class DebugSystem : GameSystem
	{
		public float timeToLive = 4.0f;
		[Range(4,48)]
		public int fontSize = 12;
		public bool showDebug = true;

		class DebugEntry
		{
			public string key;
			public string value;
			public float lastTime;
			public DebugEntry(string key) { this.key = key; }
		}


		private float keyMinWidth = 120;
		private float valueMinWidth = 200;
		private List<DebugEntry> entries = new List<DebugEntry>();
		private List<int> keysToRemove = new List<int>();

		private GUIStyle keyStyle;
		private GUIStyle valueStyle;
		private GUIStyle lineStyle;
		private GUIContent labelContent = new GUIContent();

		protected GUIStyle KeyStyle 
		{
			get
			{
				if (keyStyle == null)
				{
					keyStyle = new GUIStyle(GUI.skin.label);
					keyStyle.alignment = TextAnchor.MiddleRight;
					keyStyle.fontSize = fontSize;
					keyStyle.margin.top = keyStyle.margin.bottom = 2;
					keyStyle.padding.top = keyStyle.padding.bottom = 0;
				}
				return keyStyle;
			}
		}
		protected GUIStyle ValueStyle 
		{
			get
			{
				if (valueStyle == null)
				{
					valueStyle = new GUIStyle(GUI.skin.label);
					valueStyle.alignment = TextAnchor.MiddleLeft;
					valueStyle.fontSize = fontSize;
					valueStyle.margin.top = valueStyle.margin.bottom = 2;
					valueStyle.padding.top = valueStyle.padding.bottom = 0;
				}
				return valueStyle;
			}
		}
		protected GUIStyle LineStyle {
			get
			{
				if (lineStyle == null)
				{
					lineStyle = new GUIStyle(GUI.skin.box);
					lineStyle.alignment = TextAnchor.MiddleCenter;
					lineStyle.fontSize = fontSize;
					lineStyle.margin.top = lineStyle.margin.bottom = 0;
					lineStyle.padding.top = lineStyle.padding.bottom = 0;
				}
				return lineStyle;
			}
		}

		private void AddEntry(string key, string value)
		{
			DebugEntry info = null;
			if (!TryGetValue(key, out info))
				entries.Add(info = new DebugEntry(key));

			info.value = value;
			info.lastTime = Time.realtimeSinceStartup;
		}

		private bool TryGetValue(string key, out DebugEntry entry)
		{
			entry = null;
			for(int i=0; i<entries.Count; i++)
			{
				if (entries[i].key == key)
				{
					entry = entries[i];
					return true;
				}
			}
			return false;
		}

		protected virtual void Update()
		{
			float currTime = Time.realtimeSinceStartup;

			if (entries.Count > 0)
			{
				for (int i=0; i<entries.Count; i++)
				{
					DebugEntry entry = entries[i];
					if (currTime - entry.lastTime > timeToLive*1000.0f)
						keysToRemove.Add(i);
				}
				foreach (int index in keysToRemove)
					entries.RemoveAt(index);
			}
		}

		public void RefreshColumnsSizes(string key, string value)
		{
			labelContent.text = key;
			float keyWidth = KeyStyle.CalcSize(labelContent).x;
			labelContent.text = value;
			float valueWidth = ValueStyle.CalcSize(labelContent).x;
			if (keyMinWidth < keyWidth)
				keyMinWidth = keyWidth;
			if (valueMinWidth < valueWidth)
				valueMinWidth = valueWidth;
		}

#if UNITY_EDITOR
		private Rect buttonRect = new Rect();

		public void OnGUI()
		{
			buttonRect.x = Screen.width-26;
			buttonRect.y = 4;
			buttonRect.width = 22;
			buttonRect.height = 22;
			KeyStyle.fontSize = fontSize;
			ValueStyle.fontSize = fontSize;
			LineStyle.fontSize = fontSize;
			showDebug = GUI.Toggle(buttonRect, showDebug, "?", GUI.skin.button);
			if (showDebug && entries.Count > 0)
			{
				float height = entries.Count * (KeyStyle.lineHeight + KeyStyle.margin.top + KeyStyle.margin.bottom);
				float width = keyMinWidth + valueMinWidth + 4;
				Rect boxRect = new Rect(Screen.width - 4 - width, 30, width, height);
				GUI.Box(boxRect, "");
				GUI.color = Color.green;
				GUILayout.BeginArea(boxRect.MakeBorders(2, 2, 0, 0));
				for (int i = 0; i < entries.Count; i++)
				{
					RefreshColumnsSizes(entries[i].key, entries[i].value);
					GUILayout.BeginHorizontal(LineStyle);
					GUILayout.Label(entries[i].key, KeyStyle, GUILayout.Width(keyMinWidth));
					GUILayout.Space(4);
					GUILayout.Label(entries[i].value, ValueStyle, GUILayout.Width(valueMinWidth));
					GUILayout.EndHorizontal();
				}
				GUILayout.EndArea();
			}
		}
#endif
		public static void DrawText(string key, string value, Object context = null)
		{
			DebugSystem debug = Game.GetSystem<DebugSystem>();
			bool selectedInEditor = false;
#if UNITY_EDITOR
			selectedInEditor = context != null && UnityEditor.Selection.Contains(context);
#endif
			if (debug && (context == null || selectedInEditor))
				debug.AddEntry(key, value);
		}

		public static void DrawText(string key, Vector3 value, Object context = null)
		{
			DrawText(key, value.ToString(), context);
		}

		public static void DrawText(string key, float value, Object context = null)
		{
			DrawText(key, value.ToString(), context);
		}

		public static void DrawText(string key, int value, Object context = null)
		{
			DrawText(key, value.ToString(), context);
		}

		public static void DrawText(string key, bool value, Object context = null)
		{
			DrawText(key, value ? "true" : "false", context);
		}
	}
}
