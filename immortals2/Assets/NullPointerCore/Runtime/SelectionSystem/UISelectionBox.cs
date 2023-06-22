using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NullPointerCore
{
	/// <summary>
	/// Handles the input for the selection system related to the selection box feature.
	/// </summary>
	[RequireComponent(typeof(UISelectionInput))]
	public class UISelectionBox : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IUIInputExtension
	{
		/// <summary>
		/// Indicates which mouse button is going to be used for the Selection box.
		/// </summary>
		[Tooltip("Indicates which mouse button is going to be used for the Selection box.")]
		public PointerEventData.InputButton inputButton = PointerEventData.InputButton.Left;

		/// <summary>
		/// Reference to the SelectionInput in the scene.
		/// </summary>
		private UISelectionInput selectionInput;
		/// <summary>
		/// Reference to the selection box RectTransform.
		/// </summary>
		public RectTransform selectionBox;

		/// <summary>
		/// Unity event that triggers when the selection box ends.
		/// </summary>
		public Action selectionBoxStarted;
		/// <summary>
		/// Unity event that triggers when the selection box starts.
		/// </summary>
		public Action selectionBoxEnded;

		/// <summary>
		/// Implements the Unity's builtin Start() method.
		/// </summary>
		public void Start()
		{
			DoValidate();
		}

		/// <summary>
		/// Implements the Unity's builtin OnValidate() method.
		/// </summary>
		public void OnValidate()
		{
			DoValidate();
		}

		/// <summary>
		/// Implements the Unity's builtin Reset() method.
		/// </summary>

		public void Reset()
		{
			DoValidate();
#if UNITY_EDITOR
			GenerateSelectionBoxHint();
#endif
		}

		/// <summary>
		/// Handles the initialization of unitialized members.
		/// </summary>
		public void DoValidate()
		{
			if (selectionInput == null)
				selectionInput = GetComponent<UISelectionInput>();
		}

		/// <summary>
		/// IBeginDragHandler implementation.
		/// </summary>
		/// <param name="eventData">PointerEventData provided by the EventSystem.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button == inputButton)
			{
				if (selectionInput)
				{
					if(selectionBox)
						selectionBox.gameObject.SetActive(true);
					selectionInput.StartSelectionBox();
					if(selectionBoxStarted!=null)
						selectionBoxStarted.Invoke();
				}
			}
		}

		/// <summary>
		/// IDragHandler implementation.
		/// </summary>
		/// <param name="eventData">PointerEventData provided by the EventSystem.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (eventData.button == inputButton)
			{
				Vector2 min = Vector2.Min(eventData.position, eventData.pressPosition);
				Vector2 max = Vector2.Max(eventData.position, eventData.pressPosition);

				if (selectionBox)
				{
					selectionBox.position = min;
					selectionBox.sizeDelta = max - min;
				}
				if(selectionInput)
					selectionInput.SetupSelectionBox(min, max);
			}
		}

		/// <summary>
		/// IEndDragHandler implementation.
		/// </summary>
		/// <param name="eventData">PointerEventData provided by the EventSystem.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button == inputButton)
			{
				if(selectionBox)
					selectionBox.gameObject.SetActive(false);
				if (selectionInput)
				{
					if (eventData.pointerCurrentRaycast.gameObject == this.gameObject)
						selectionInput.EndSelectionBox();
					else
						selectionInput.CancelSelectionBox();
					if(selectionBoxEnded!=null)
						selectionBoxEnded.Invoke();
				}
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Indicates if can be created in editor mode the selection box ui required to complete the feature.
		/// </summary>
		/// <returns>true if there is no selection box ui conffigured and a new one can be created,</returns>
		[ContextMenu("Generate Selection Box Hint", true)]
		public bool ShouldGenerateSelectionBoxHint()
		{
			return selectionBox == null;
		}

		/// <summary>
		/// Creates the Selection box UI needed for the feature.
		/// </summary>
		[ContextMenu("Generate Selection Box Hint")]
		public void GenerateSelectionBoxHint()
		{
			GameObject go = new GameObject("SelectionBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
			go.transform.SetParent(transform);
			RectTransform rcTransf = go.GetComponent<RectTransform>();
			rcTransf.pivot = Vector3.zero;
			Image img = go.GetComponent<Image>();
			img.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
			img.raycastTarget = false;
			img.type = Image.Type.Tiled;
			img.fillCenter = false;
			go.SetActive(false);
			selectionBox = rcTransf;
		}
#endif
	}
}