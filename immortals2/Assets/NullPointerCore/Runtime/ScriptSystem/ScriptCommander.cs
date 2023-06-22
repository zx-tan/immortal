using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullPointerCore
{
	[System.Serializable, DisallowMultipleComponent]
	public class ScriptCommander : MonoBehaviour
	{
		public List<CmdSequence> sequences = new List<CmdSequence>();

		public int SequencesCount { get { return sequences.Count; } }

		public virtual void Play(string triggerName)
		{
			if (string.IsNullOrEmpty(triggerName))
				return;

			foreach(CmdSequence seq in sequences)
			{
				if(seq.id == triggerName)
				{
					StartCoroutine(seq.Play());
					break;
				}
			}
		}

		public virtual void Stop()
		{

		}

		public virtual void Update()
		{

		}

		public bool ContainsSequence(string newSequenceName)
		{
			return sequences.Exists(x => x.id == newSequenceName);
		}

		public void GetSequencesNames(List<string> sequenceNames)
		{
			if( sequenceNames.Count != sequences.Count )
			{
				sequenceNames.Clear();
				foreach (CmdSequence seq in sequences)
					sequenceNames.Add(seq.id);
			}
			else
			{
				for (int i=0; i<sequences.Count; i++)
					sequenceNames[i] = sequences[i].id;
			}
		}

		public void AddSequence(string newSequenceId)
		{
			sequences.Add(new CmdSequence(newSequenceId));
		}

		public void RenameSequence(int sequenceIndex, string renamedSequenceName)
		{
			if (sequenceIndex < 0 || sequenceIndex > sequences.Count)
				return;

			sequences[sequenceIndex].id = renamedSequenceName;
		}

		public void RemoveSequence(int sequenceIndex)
		{
			if (sequenceIndex < 0 || sequenceIndex > sequences.Count)
				return;
			sequences.RemoveAt(sequenceIndex);
		}
	}
}