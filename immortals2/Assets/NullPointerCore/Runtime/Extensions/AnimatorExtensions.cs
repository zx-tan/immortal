using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{

	/// <summary>
	/// Contains some usefull Animator extension methods.
	/// </summary>
	public static class AnimatorExtensions
	{
		/// <summary>
		/// Indicates if the given parameter belongs to this Animator.
		/// </summary>
		/// <param name="anim">The Animator reference context of this call.</param>
		/// <param name="name">The name of the parameter to check.</param>
		/// <returns>true in case the parameters belongs to this Animator.</returns>
		public static bool HasParameter(this Animator anim, string name)
		{
			foreach( AnimatorControllerParameter param in anim.parameters )
			{
				if (param.name == name)
					return true;
			}
			return false;
		}

	}
}