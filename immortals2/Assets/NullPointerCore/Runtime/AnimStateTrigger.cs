using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NullPointerCore
{
	public interface IAnimStateBindings
	{
		void OnAnimStateEnter(int stateHash);
		void OnAnimStateExit(int stateHash);
	}
	public class AnimStateTrigger : StateMachineBehaviour
	{
		public bool searchListenerInParent = false;

		override public void OnStateEnter(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
		{
			List<IAnimStateBindings> bindings = new List<IAnimStateBindings>(anim.GetComponents<IAnimStateBindings>());
			if (searchListenerInParent)
				bindings.AddRange(anim.GetComponentsInParent<IAnimStateBindings>());
			foreach (IAnimStateBindings binding in bindings)
			{
				if (binding != null)
					binding.OnAnimStateEnter(stateInfo.shortNameHash);
			}
		}

		override public void OnStateExit(Animator anim, AnimatorStateInfo stateInfo, int layerIndex)
		{
			List<IAnimStateBindings> bindings = new List<IAnimStateBindings>(anim.GetComponents<IAnimStateBindings>());
			if (searchListenerInParent)
				bindings.AddRange(anim.GetComponentsInParent<IAnimStateBindings>());
			foreach (IAnimStateBindings binding in bindings)
			{
				if (binding != null)
					binding.OnAnimStateExit(stateInfo.shortNameHash);
			}
		}
	}
}