using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NullPointerCore
{
	/// <summary>
	/// Helper Monobehavior that handles input related to the Pointer click event.
	/// </summary>
	[RequireComponent(typeof(UISelectionInput))]
	public class UISelectionClick : MonoBehaviour, IPointerClickHandler, IUIInputExtension
	{
		/// <summary>
		/// Reference to the SelectionInput in the scene.
		/// </summary>
		protected UISelectionInput selectionInput;

		/// <summary>
		/// Indicates which mouse button should be used to confirm a selection.
		/// </summary>
		[Space]
		public PointerEventData.InputButton mouseConfirmButton = PointerEventData.InputButton.Left;

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

		/// <summary>
		/// Implementation for the IPointerClickHandler interface.
		/// Process the click event and send it to the selection input module.
		/// </summary>
		/// <param name="eventData">Input event data related to the click.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			if (selectionInput.IsOverInputArea)
				ProcessClickEvent(eventData);
		}

		/// <summary>
		/// Handle the click event and filters the left click to select a GameEntity.
		/// After that the event is derived to the ProcessLeftClickEvent() method.
		/// </summary>
		/// <param name="eventData">Input event data related to the click.</param>
		public virtual void ProcessClickEvent(BaseEventData eventData)
		{
			PointerEventData pointerData = eventData as PointerEventData;
			if (pointerData != null && pointerData.button == mouseConfirmButton)
				selectionInput.ProcessSelectionEvent();
		}
	}
}