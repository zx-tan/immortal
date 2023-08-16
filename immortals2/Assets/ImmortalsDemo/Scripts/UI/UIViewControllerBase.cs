using System.Collections.Generic;
using UnityEngine;

namespace Immortals
{

	public abstract class UIViewControllerBase : Monobehaviour
	{
		public void DisplayView(bool display)
		{
			this.gameObject.SetActive(display)
		}
	}
}