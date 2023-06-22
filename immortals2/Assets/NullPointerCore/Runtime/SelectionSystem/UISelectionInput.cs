using UnityEngine;
using UnityEngine.EventSystems;

namespace NullPointerCore
{
	public interface IUIInputExtension {}

	/// <summary>
	/// Acts as a nexus between the input (clicks, selection box, etc) and the SelectionSystem.
	/// </summary>
	[RequireComponent(typeof(Graphics))]
	public class UISelectionInput : SelectionInputBase, IPointerEnterHandler, IPointerExitHandler
	{
		/// <summary>
		/// Indicates the Selection additive key.
		/// When holding this key enables to the user to add units to the current selection without losing the ones already selected.
		/// </summary>
		[Space]
		public KeyCode additiveKey = KeyCode.LeftControl;

		private bool isOverInputArea = false;

		/// <summary>
		/// Indicates if the cursor is currently over this Input area.
		/// </summary>
		public bool IsOverInputArea {  get { return isOverInputArea; } }

		/// <summary>
		/// Implementation of the Unity's builtin Update() method.
		/// </summary>
		protected override void Update()
		{
			base.Update();
			AdditiveSelection = Input.GetKey(additiveKey);
			if(isOverInputArea)
				ProcessHoveringRay(Camera.main.ScreenPointToRay(Input.mousePosition));
		}

		/// <summary>
		/// Implementation for the IPointerEnterHandler interface.
		/// </summary>
		/// <param name="eventData">Input event data related to the mouse pointer.</param>
		public void OnPointerEnter(PointerEventData eventData)
		{
			isOverInputArea = true;
		}

		/// <summary>
		/// Implementation for the IPointerExitHandler interface.
		/// </summary>
		/// <param name="eventData">Input event data related to the mouse pointer.</param>
		public void OnPointerExit(PointerEventData eventData)
		{
			isOverInputArea = false;
		}
	}
}
