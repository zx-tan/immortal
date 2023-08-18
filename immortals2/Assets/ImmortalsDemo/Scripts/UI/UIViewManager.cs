using System.Collections.Generic;
using UnityEngine;

namespace Immortals
{

	public abstract class UIViewManager : Monobehaviour, IUIViewManager
	{
		Stack<UIViewControllerBase> views = new Stack<UIViewControllerBase>();

		public void AddView(UIViewControllerBase ui)
		{
			views.Push(ui);
		}

		public void RemoveCurrentView()
		{
			views.Pop();
		}

		public void Reset()
		{
			views.Clear();
		}

		private Update()
		{
			
		}
	}
}