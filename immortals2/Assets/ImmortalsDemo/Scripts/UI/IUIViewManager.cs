namespace Immortals
{

	public interface IUIViewManager
	{
		void AddView(UIViewControllerBase ui);
		void RemoveCurrentView();
		void Reset();
	}
}