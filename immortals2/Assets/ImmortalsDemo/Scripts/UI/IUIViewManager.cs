namespace Immortals
{

	public interface IUIViewManager
	{
		void AddView(UIViewControllerBase ui);
		void RemoveView(UIViewControllerBase ui);
	}
}