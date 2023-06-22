using NullPointerCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Immortals
{
	public class UIInputArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, 
												IBeginDragHandler, IEndDragHandler, 
												IPointerClickHandler, IUIInputExtension
	{
		public bool IsCursorOverArea { get; private set; }
		public bool IsDragging { get; private set; }
		public bool IsPointerClick { get; private set; }

		private UISelectionInput selectionInput;

		/// <summary>
		/// Implementation of the Unity's built in Start() method. 
		/// </summary>
		public void Start()
		{
			DoValidate();
		}

		/// <summary>
		/// Implementation of the Unity's built in OnValidate() method. 
		/// Handles the Component data Validation each time is requested.
		/// </summary>
		public void OnValidate()
		{
			DoValidate();
		}

		/// <summary>
		/// Implementation of the Unity's built in Reset() method. 
		/// Validate component data every time this component is reseted.
		/// </summary>
		public void Reset()
		{
			DoValidate();
		}

		/// <summary>
		/// Validates this Component initializing unitialized data.
		/// </summary>
		public void DoValidate()
		{
			if (selectionInput == null)
				selectionInput = GetComponent<UISelectionInput>();
		}

		void LateUpdate()
		{
			IsPointerClick = false;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			IsCursorOverArea = true;
			//if (selectionInput)
			//	selectionInput.handleHoverInput = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			IsCursorOverArea = false;
			//if (selectionInput)
			//	selectionInput.handleHoverInput = false;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if( eventData.button == PointerEventData.InputButton.Left )
			{
				IsDragging = true;
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
				IsDragging = false;
		}

		public void OnPointerClick(PointerEventData eventData)
		{

			if (eventData.button == PointerEventData.InputButton.Left && IsCursorOverArea && !eventData.dragging)
			{
				if (selectionInput)
				{
					selectionInput.ProcessHoveringRay(Camera.main.ScreenPointToRay(eventData.position));
					//selectionInput.ProcessSelectionEvent();
				}
				IsPointerClick = true;
			}
		}
	}
}