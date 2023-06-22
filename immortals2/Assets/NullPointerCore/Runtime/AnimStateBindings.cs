using UnityEngine;
using UnityEngine.Events;

namespace NullPointerCore
{
	[RequireComponent(typeof(Animator))]
	public class AnimStateBindings : MonoBehaviour, IAnimStateBindings
	{
		public enum CallState
		{
			OnEnter,
			OnExit,
		}

		public string stateName;
		public UnityEvent onEnter;
		public UnityEvent onExit;

		public void OnAnimStateEnter(int stateHash)
		{
			int hash = Animator.StringToHash(stateName);
			if (hash == stateHash)
				onEnter.Invoke();
		}

		public void OnAnimStateExit(int stateHash)
		{
			int hash = Animator.StringToHash(stateName);
			if (hash == stateHash)
				onExit.Invoke();
		}
	}
}