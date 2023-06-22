using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullPointerCore
{
	[System.Serializable]
	public class CmdSequence : System.Object
	{
		[System.Serializable]
		public class CmdActionEntry
		{
			public float delay = 0.0f;
			public string desc = "Command Description";
			public CmdAction command;
		}
		public string id;

		public CmdSequence() { }
		public CmdSequence(string newId) { id = newId; }
		public List<CmdActionEntry> actions = new List<CmdActionEntry>();


		public virtual IEnumerator Play()
		{
			foreach (CmdActionEntry action in actions)
			{
				if (action.delay > 0)
					yield return new WaitForSeconds(action.delay);
				if (action.command != null && action.command.gameObject != null)
					action.command.Play();
			}
			yield return null;
		}
	}
}