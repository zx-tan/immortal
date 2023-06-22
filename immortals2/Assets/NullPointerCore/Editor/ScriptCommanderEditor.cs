using System;
using System.Collections.Generic;
using NullPointerCore;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NullPointerEditor
{
	//[CanEditMultipleObjects]
	[CustomEditor(typeof(ScriptCommander), true)]
	public class ScriptCommanderEditor : Editor
	{
		static GUIContent newSequenceLabel = new GUIContent("Add Sequence");
		static GUIContent editSequenceLabel = new GUIContent("Edit Sequence");
		static GUIContent renameSquenceLabel = new GUIContent("Trigger Name");
		static GUIContent undefinedActionTypeLabel = new GUIContent("<Undefined>");
		static GUIContent noSequenceLabel = new GUIContent("<No Sequence Available>");
		static GUIStyle titleStyle = null;
		static GUIStyle undefinedStyle = null;

		private SerializedProperty scriptProperty;
		private string newSequenceName = "";
		private string sequenceNameWarnText = "";
		private string sequenceRenameWarnText = "";
		private string renamedSequenceName;
		private List<string> sequenceNames = new List<string>();
		private ReorderableList rl;
		private SerializedProperty sequenceActionsProperty = null;
		private List<string> actionTypesNames = null;
		private Editor cmdActionEditor = null;
		private CmdAction editorCurrentAction = null;
		private float sequenceDelayTime = 0.0f;
		static private int secuenceToEdit = 0;

		private ScriptCommander Target { get { return target as ScriptCommander; } }
		static public GUIStyle TitleStyle 
		{
			get
			{
				if (titleStyle == null)
				{
					titleStyle = new GUIStyle(EditorStyles.miniLabel);
					titleStyle.alignment = TextAnchor.MiddleCenter;
				}
				return titleStyle;
			}
		}
		static public GUIStyle UndefinedStyle {
			get
			{
				if (undefinedStyle == null)
				{
					undefinedStyle = new GUIStyle(GUI.skin.box);
					undefinedStyle.alignment = TextAnchor.MiddleCenter;
					undefinedStyle.fontSize = 10;
				}
				return undefinedStyle;
			}
		}

		private List<string> ActionTypeNames 
		{
			get
			{
				if(actionTypesNames==null)
					RebuildActionTypeNames();
				return actionTypesNames;
			}
		}

		private void RebuildActionTypeNames()
		{
			actionTypesNames = new List<string>();
			actionTypesNames.Add("<Undefined>");
			List<Type> cmdActions = EditorHelpers.CollectAvailableComponents<CmdAction>();
			foreach (Type t in cmdActions)
				actionTypesNames.Add(t.Name);
		}

		public void OnEnable()
		{
			scriptProperty = serializedObject.FindProperty("m_Script");
			RebuildActionTypeNames();
			UpdateHideFlags();
		}

		public void OnDisable()
		{
			ClearCommandEditor();
		}

		public void SetCommandEditor(CmdAction newAction)
		{
			if (editorCurrentAction == newAction)
				return;
			ClearCommandEditor();
			if (newAction != null)
			{
				cmdActionEditor = Editor.CreateEditor(newAction);
				editorCurrentAction = newAction;
			}
		}

		public void ClearCommandEditor()
		{
			if (cmdActionEditor)
				GameObject.DestroyImmediate(cmdActionEditor);
			cmdActionEditor = null;
			editorCurrentAction = null;
		}

		public void UpdateHideFlags()
		{
			foreach( CmdSequence seq in Target.sequences )
			{
				foreach( CmdSequence.CmdActionEntry entry in seq.actions )
				{
					if (entry.command != null)
						entry.command.hideFlags = HideFlags.HideInInspector;
				}
			}
		}

		// Draw the property inside the given rect
		public override void OnInspectorGUI()
		{
			GUI.enabled = false;
			EditorGUILayout.PropertyField(scriptProperty);
			GUI.enabled = true;

			NullPointerGUIUtility.DrawRequiredGameSystems(targets, Target.gameObject);
			EditorGUILayout.Space();

			// ====[ Add Sequence ] ============================
			EditorGUILayout.BeginHorizontal();
			newSequenceName = EditorGUILayout.TextField(newSequenceLabel, newSequenceName, EditorStyles.textField);
			EditorGUI.BeginDisabledGroup(!ValidateNewSequenceName(newSequenceName));
			if (GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.Width(45)))
			{
				Target.AddSequence(newSequenceName);
				newSequenceName = "";
				secuenceToEdit = Target.SequencesCount-1;
				renamedSequenceName = newSequenceName;
				RebuildReorderableActionList(null);
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
			if (!string.IsNullOrEmpty(sequenceNameWarnText))
				EditorGUILayout.HelpBox(sequenceNameWarnText, MessageType.Error);
			// ====[ Add Sequence ] ============================

			EditorGUILayout.Space();

			// ====[ Edit Sequence ] ============================
			if (Target.SequencesCount == 0)
			{
				EditorGUILayout.LabelField(editSequenceLabel, noSequenceLabel, EditorStyles.popup);
				RebuildReorderableActionList(null);
			}
			else
			{
				Target.GetSequencesNames(sequenceNames);
				if (secuenceToEdit < 0 || secuenceToEdit >= sequenceNames.Count)
					secuenceToEdit = 0;
				EditorGUI.BeginChangeCheck();
				secuenceToEdit = EditorGUILayout.Popup(editSequenceLabel, secuenceToEdit, sequenceNames.ToArray());
				if (EditorGUI.EndChangeCheck() || sequenceActionsProperty == null)
				{
					RebuildReorderableActionList( serializedObject.FindProperty("sequences.Array.data[" + secuenceToEdit + "].actions") );
					renamedSequenceName = sequenceNames[secuenceToEdit];
				}
			}
			EditorGUILayout.HelpBox("Select the sequence to edit.", MessageType.Info);
			// ====[ Edit Sequence ] ============================
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GUI.skin.box);
			CmdAction currentAction = null;
			if (Target.SequencesCount > 0)
			{
				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
				EditorGUI.BeginDisabledGroup(!Application.isPlaying);
				if (GUILayout.Button("Play", EditorStyles.toolbarButton))
					Target.Play(sequenceNames[secuenceToEdit]);
				EditorGUI.EndDisabledGroup();
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				
				EditorGUILayout.BeginHorizontal();
				renamedSequenceName = EditorGUILayout.TextField(renameSquenceLabel, renamedSequenceName, EditorStyles.textField);
				EditorGUI.BeginDisabledGroup(!ValidateRenameSequenceName(sequenceNames[secuenceToEdit], renamedSequenceName));
				if (GUILayout.Button("Rename", EditorStyles.miniButtonRight, GUILayout.Width(60)))
					Target.RenameSequence(secuenceToEdit, renamedSequenceName);
				EditorGUI.EndDisabledGroup();
				if (GUILayout.Button("Remove", EditorStyles.miniButtonRight, GUILayout.Width(60)))
				{
					if (EditorUtility.DisplayDialog("Remove Sequence", "Removing sequence. All data will be lost.", "Remove", "Cancel"))
					{
						Target.RemoveSequence(secuenceToEdit);
						RebuildReorderableActionList(null);
					}
				}
				EditorGUILayout.EndHorizontal();
				if (!string.IsNullOrEmpty(sequenceRenameWarnText))
					EditorGUILayout.HelpBox(sequenceRenameWarnText, MessageType.Error);

				EditorGUILayout.Space();
				serializedObject.Update();

				if (rl != null)
				{
					sequenceDelayTime = 0.0f;
					rl.DoLayoutList();
					currentAction = rl.index >= 0 ? Target.sequences[secuenceToEdit].actions[rl.index].command : null;
				}
				else
					currentAction = null;

				serializedObject.ApplyModifiedProperties();
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			if( currentAction != null )
			{
				SetCommandEditor(currentAction);
				cmdActionEditor.OnInspectorGUI();
			}
		}

		private void RebuildReorderableActionList(SerializedProperty actionListProp)
		{
			if (actionListProp != null)
			{
				rl = new ReorderableList(serializedObject, actionListProp, true, true, true, true);
				rl.drawHeaderCallback = ActionsDrawHeader;
				rl.drawElementCallback = ActionsDrawElement;
				rl.onAddCallback = ActionsAddNewAction;
			}
			else
				rl = null;
			sequenceActionsProperty = actionListProp;
		}

		private bool ValidateNewSequenceName(string newSequence)
		{
			if (string.IsNullOrEmpty(newSequence))
			{
				sequenceNameWarnText = "";
				return false;
			}
			else if (Target.ContainsSequence(newSequence))
			{
				sequenceNameWarnText = "That Sequence name already exist.";
				return false;
			}
			sequenceNameWarnText = "";
			return true;
		}

		private bool ValidateRenameSequenceName(string oldName, string newName)
		{
			if (string.IsNullOrEmpty(newName))
			{
				sequenceRenameWarnText = "The Sequence name can't be empty.";
				return false;
			}
			else if(oldName == newName)
			{
				sequenceRenameWarnText = "";
				return false;
			}
			else if (Target.ContainsSequence(newName))
			{
				sequenceRenameWarnText = "That Sequence name already exist.";
				return false;
			}
			return true;
		}

		public void ActionsDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty item = sequenceActionsProperty.GetArrayElementAtIndex(index);
			CmdSequence.CmdActionEntry entry = Target.sequences[secuenceToEdit].actions[index];

			SerializedProperty delayProp = item.FindPropertyRelative("delay");
			SerializedProperty descProp = item.FindPropertyRelative("desc");

			rect = rect.MakeBorders(0, 0, 2, 3);
			int selectedIndex = entry.command != null ? GetActionTypeNameIndex(entry.command.GetType().Name) : 0;
			EditorGUI.PropertyField(rect.HorizontalCut(0, 50), delayProp, GUIContent.none);
			sequenceDelayTime += delayProp.floatValue;
			EditorGUI.LabelField(rect.HorizontalCut(50, 50).MakeBorders(4, 4, 0, 0), sequenceDelayTime.ToString(), UndefinedStyle);
			if (ActionTypeNames.Count > 1)
			{
				int newSelectedIndex = EditorGUI.Popup(rect.HorizontalCut(100, 150).MakeBorders(4, 4, 0, 0), selectedIndex, ActionTypeNames.ToArray());
				if( newSelectedIndex != selectedIndex )
				{
					RemoveCommand(entry);
					if (newSelectedIndex > 0)
						AddCommand(entry, newSelectedIndex);
						
				}
			}
			else
				EditorGUI.LabelField(rect.HorizontalCut(100, 150).MakeBorders(4, 4, 0, 0), undefinedActionTypeLabel, UndefinedStyle);
			EditorGUI.PropertyField(rect.HorizontalCut(250), descProp, GUIContent.none);
		}

		private void ActionsAddNewAction(ReorderableList list)
		{
			Target.sequences[secuenceToEdit].actions.Add(new CmdSequence.CmdActionEntry());
			EditorUtility.SetDirty(target);
		}

		private void AddCommand(CmdSequence.CmdActionEntry entry, int newSelectedIndex)
		{
			List<Type> cmdActions = EditorHelpers.CollectAvailableComponents<CmdAction>();
			entry.command = Target.gameObject.AddComponent(cmdActions[newSelectedIndex - 1]) as CmdAction;
			entry.command.hideFlags = HideFlags.HideInInspector;
		}

		private void RemoveCommand(CmdSequence.CmdActionEntry entry)
		{
			if (entry.command != null)
			{
				if (Application.isPlaying)
					GameObject.Destroy(entry.command);
				else
					GameObject.DestroyImmediate(entry.command);
				GUIUtility.ExitGUI();
			}
			entry.command = null;
		}

		private int GetActionTypeNameIndex(string name)
		{
			if (ActionTypeNames.Count <= 1)
				return 0;
			return ActionTypeNames.IndexOf(name);
		}

		public void ActionsDrawHeader(Rect rect)
		{
			GUI.Box(rect.MakeBorders(-4, -4, 0, 0), GUIContent.none, EditorStyles.toolbar);
			rect = rect.MakeBorders(10, 0, 0, 0);
			EditorGUI.LabelField(rect.HorizontalCut(0, 50), "Delay", TitleStyle);
			EditorGUI.LabelField(rect.HorizontalCut(50, 50), "Time", TitleStyle);
			EditorGUI.LabelField(rect.HorizontalCut(100, 150), "Type", TitleStyle);
			EditorGUI.LabelField(rect.HorizontalCut(250), "Description", TitleStyle);
		}
	}
}