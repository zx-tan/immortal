using UnityEngine;
using UnityEngine.EventSystems;

namespace NullPointerCore
{
	/// <summary>
	/// Handles the input for the RTSCamera.
	/// <p>Reads the pan through the middle mouse button and through setting the cursor at the borders of the screen. 
	/// Requires to be attached next to a component capable of raycast the mouse pointer and deliver the BeginDrag, 
	/// Drag and EndDrag events (An Image for example). </p>
	/// </summary>
	public class RTSCameraUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler										
	{
		public PointerEventData.InputButton panButton;

		/// <summary>
		/// Reference to the RTSCamera in the scene.
		/// </summary>
		public GameCamera cameraController;
		/// <summary>
		/// Border factor applyed to the minimum value between the screen width and height and to be used as a  border to measure the camera panning when the cursor 
		/// reachs that border.
		/// </summary>
		public float viewportPanBorder = 0.14f;
		/// <summary>
		/// Enable or disable the scroll on the borders feature.
		/// </summary>
		public bool allowBorderPan = true;
		/// <summary>
		/// Enable or disable the scroll through drag.
		/// </summary>
		public bool allowDragPan = true;
		/// <summary>
		/// 
		/// </summary>
		public float zoomTouchFactor = 0.01f;

		private Vector2 prevCursorPosition;
		private bool panning = false;
		private bool touchZooming = false;

		/// <summary>
		/// Getter and setter that enable or disable the scroll on the borders feature.
		/// </summary>
		public bool AllowBorderPan { get { return allowBorderPan; }	set { allowBorderPan = value; } }
		/// <summary>
		/// Getter and setter that enables the middle mouse button scroll feature.
		/// </summary>
		public bool AllowDragPan { get { return allowDragPan; } set { allowDragPan = value;	} }

		void Update()
		{
			if (cameraController == null)
				return;

			if (Input.touchCount == 2)
			{
				touchZooming = true;
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);
				EndPan(touchZero.position);

				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

				float difference = currentMagnitude - prevMagnitude;
				Vector3 touchCenter = (touchOne.position + touchZero.position) / 2;

				cameraController.UpdateZoomScroll(difference * zoomTouchFactor, touchCenter);
			}
			else if (Input.touchCount < 2)
			{
				

				if (touchZooming && Input.touchCount == 1)
				{
					Touch touch = Input.GetTouch(0);
					if(touch.phase!=TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
						BeginPan(touch.position);
				}
				touchZooming = false;

				if (allowBorderPan && !panning && Input.mousePresent)
				{
					float definedBorder = Mathf.Min(Screen.width, Screen.height) * viewportPanBorder;
					Vector3 borderOverride = Vector3.zero;

					if (Input.mousePosition.x >= 0 && Input.mousePosition.y >= 0 &&
						Input.mousePosition.x <= Screen.width && Input.mousePosition.y <= Screen.height)
					{
						if (Input.mousePosition.x < definedBorder)
							borderOverride.x = Input.mousePosition.x - definedBorder;
						if (Input.mousePosition.y < definedBorder)
							borderOverride.y = Input.mousePosition.y - definedBorder;

						if (Input.mousePosition.x > Screen.width - definedBorder)
							borderOverride.x = Input.mousePosition.x - (Screen.width - definedBorder);
						if (Input.mousePosition.y > Screen.height - definedBorder)
							borderOverride.y = Input.mousePosition.y - (Screen.height - definedBorder);

						if (borderOverride != Vector3.zero)
							cameraController.TranslateInCursorDirection(borderOverride.magnitude / definedBorder);
					}
				}
			}
		}

		/// <summary>
		/// IBeginDragHandler implementation that allows to start the tracking of the middle mouse button scroll feature.
		/// </summary>
		/// <param name="eventData">PointerEventData provided by the EventSystem.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button == panButton && !touchZooming)
				BeginPan( eventData.position );
		}

		/// <summary>
		/// IDragHandler implementation that allows to keep tracking of the mouse middle button panning for scroll feature.
		/// </summary>
		/// <param name="eventData">PointerEventData provided by the EventSystem.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if(panning && eventData.button == panButton && !touchZooming)
			{
				if(eventData.position != prevCursorPosition && cameraController != null && allowDragPan)
					cameraController.TranslateByScreenPosDelta(eventData.position, prevCursorPosition);
				prevCursorPosition = eventData.position;
			}
		}

		/// <summary>
		/// IEndDragHandler implementation that allows to detect the end of the mouse middle button pan for the scroll feature.
		/// </summary>
		/// <param name="eventData"></param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (panning && eventData.button == panButton && !touchZooming)
			{
				if (eventData.pointerCurrentRaycast.gameObject == this.gameObject)
				{
					if (cameraController && allowDragPan)
						cameraController.TranslateByScreenPosDelta(eventData.position, prevCursorPosition);
				}
				EndPan( eventData.position );
			}
		}

		public void BeginPan(Vector2 touchPos)
		{
			if (!panning)
			{
				touchZooming = false;
				prevCursorPosition = touchPos;
				cameraController.PanningByDrag = true;
				panning = true;
			}
		}

		public void EndPan(Vector2 touchPos)
		{
			if (panning)
			{
				prevCursorPosition = touchPos;
				cameraController.PanningByDrag = false;
				panning = false;
			}
		}
	}
}