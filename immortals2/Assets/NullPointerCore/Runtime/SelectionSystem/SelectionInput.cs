using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NullPointerCore
{
	/// <summary>
	/// Acts as a nexus between the input (clicks, selection box, etc) and the SelectionSystem.
	/// </summary>
	public class SelectionInput : SelectionInputBase
	{
		/// <summary>
		/// Indicates which key should be used to mark that the next units requested to be selected should be added to the already selected units instead of removing the previously selected.
		/// </summary>
		public KeyCode additiveKey = KeyCode.LeftControl;

		/// <summary>
		/// Indicates if it's allowed to select units through a selection box.
		/// </summary>
		[Header("Customize Behavior")]
		public bool selectionBox = true;

		/// <summary>
		/// Indicates if the system must use a simple implementation of the input reading (using the standard Input class).
		/// Or there is a custom implementation that will handle the correct calls to ProcessLeftClickEvent, StartSelectionBox, etc.
		/// </summary>
		[Header("Customized Input")]
		public bool customClickInput = false;

		/// <summary>
		/// Indicates if the system must use a simple implementation of the input reading (using the standard Input class).
		/// Or there is a custom implementation that will handle the correct calls to ProcessHoveringRay.
		/// </summary>
		public bool handleHoverInput = false;

		private Vector3 startDragPos = Vector3.zero;

		override protected void Update()
		{
			UpdateDefaultInput();
			base.Update();

			if (!IsSelectionBoxMode)
			{
				if (handleHoverInput)
					ProcessHoveringRay(Camera.main.ScreenPointToRay(Input.mousePosition));
				else
					SelectionSystem.RemoveAllHovering();
			}
		}

		private void UpdateDefaultInput()
		{
			float dragDist = 0.0f;
			AdditiveSelection = Input.GetKey(additiveKey);

			if (Input.GetMouseButtonDown(0))
				startDragPos = Input.mousePosition;
			if(Input.GetMouseButton(0))
			{
				dragDist = Vector3.Distance(Input.mousePosition, startDragPos);
				if(dragDist > 4.0f && !IsSelectionBoxMode && selectionBox)
					StartSelectionBox();
				SetupSelectionBox(startDragPos, Input.mousePosition);
			}
			
			if(Input.GetMouseButtonUp(0))
			{
				if(IsSelectionBoxMode)
					EndSelectionBox();
				else
					ProcessSelectionEvent();
			}
		}
	}
}