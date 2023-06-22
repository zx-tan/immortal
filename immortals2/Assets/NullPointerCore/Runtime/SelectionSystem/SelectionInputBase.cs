using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// Base abstract module class for input detection for the selection system.
	/// </summary>
	public abstract class SelectionInputBase : GameService
	{
		/// <summary>
		/// The mask to be used by the raycast to filter which colliders are valid to hover, highlight and select.
		/// </summary>
		public LayerMask raycastMask = -1;
		/// <summary>
		/// Lenght of the raycast for click detection of units.
		/// </summary>
		public float raycastLength = 100.0f;
		/// <summary>
		/// Indicates if it's currently in the selection box mode (If there is a selection box active in the game).
		/// </summary>
		[Header("Debug")]
		[SerializeField, ReadOnly]
		private bool isSelectionBoxMode = false;
		/// <summary>
		///  Indicates if the Additive selection is active or not.
		/// </summary>
		[SerializeField, ReadOnly]
		private bool additiveSelection = false;

		private bool selectionBoxConfirmed = false;
		private Vector2 boxPosMin = Vector2.zero;
		private Vector2 boxPosMax = Vector2.zero;

		/// <summary>
		/// Reference cache to the selection System.
		/// </summary>
		public SelectionSystem SelectionSystem { get; private set; }
		/// <summary>
		/// Indicates if the Additive selection is active or not.
		/// </summary>
		public bool AdditiveSelection { get { return additiveSelection; } set { additiveSelection = value; } }
		/// <summary>
		/// Indicates if it's currently in the selection box mode (If there is a selection box active in the game).
		/// </summary>
		public bool IsSelectionBoxMode { get { return isSelectionBoxMode; } }

		/// <summary>
		/// Implementation of the unity's builtin Start() method.
		/// Initializes component properties.
		/// </summary>
		virtual protected void Start()
		{
			SelectionSystem = Game.GetSystem<SelectionSystem>();
		}

		/// <summary>
		/// Handles the box and single click selection modes.
		/// </summary>
		virtual protected void Update()
		{
			if (isSelectionBoxMode)
			{
				if (selectionBoxConfirmed)
				{
					SelectionSystem.ConfirmAllHighlightsAsSelected(AdditiveSelection);
					SelectionSystem.RemoveAllHovering();
					selectionBoxConfirmed = false;
					isSelectionBoxMode = false;
				}
				else
					ProcessHoveringBox(Camera.main.GetViewportBounds(boxPosMin, boxPosMax));
			}
		}

		/// <summary>
		/// When disabled actomatically cancels the selection box mode.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			if (isSelectionBoxMode)
				CancelSelectionBox();
		}

		/// <summary>
		/// Start a new selection box mode, also remove all the currently selected and hovered 
		/// GameEntities from the SelectionSystem.
		/// </summary>
		public void StartSelectionBox()
		{
			isSelectionBoxMode = true;
			if (!SelectionSystem.multipleSelection)
				SelectionSystem.DeselectAll();
			SelectionSystem.RemoveAllHovering();
		}

		/// <summary>
		/// Setups the current selection box mode with the two screen coords defining the bounds of that selection.
		/// </summary>
		/// <param name="min">Left bottom screen coordinates for the selection bounding box.</param>
		/// <param name="max">Right top screen coordinates for the selection bounding box.</param>
		public void SetupSelectionBox(Vector2 min, Vector2 max)
		{
			boxPosMin = min;
			boxPosMax = max;
		}

		/// <summary>
		/// Ends the selection box mode and confirms all highlighted GameEntities as Selected.
		/// </summary>
		public void EndSelectionBox()
		{
			selectionBoxConfirmed = true;
		}

		/// <summary>
		/// Cancels the current selection box mode and removes all current hovering flags from the SelectionSystem.
		/// </summary>
		public void CancelSelectionBox()
		{
			selectionBoxConfirmed = false;
			isSelectionBoxMode = false;
			SelectionSystem.RemoveAllHovering();
		}

		/// <summary>
		/// Process all GameEntities colliding with the defined viewport Bounds
		/// </summary>
		/// <param name="viewportBounds">Bounds in viewport coordinates where to find the GameEntities to hover.</param>
		internal void ProcessHoveringBox(Bounds viewportBounds)
		{
			// Collect all selectables that are inside the defined viewport bounds.
			IEnumerable<Selectable> toHover = SelectionSystem.CollectSelectablesInBounds(Camera.main, viewportBounds);
			// Marks each selectable as hovered.
			SelectionSystem.SetupHoveredSelectables(toHover);
		}

		/// <summary>
		/// Process all GameEntities colliding with the ray (normally the cursor ray from the camera)
		/// and marks all that GameEntites as hovered.
		/// </summary>
		/// <param name="ray">Ray against which the checks will be done.</param>
		internal void ProcessHoveringRay(Ray ray)
		{
			if (isSelectionBoxMode)
				return;

			// Check all the entites that are under the cursor
			RaycastHit[] hits = Physics.RaycastAll(ray, 100.0f, raycastMask);

			// For each collider its Selectable will be marked as hovered & highlighted if it's valid.
			SelectionSystem.SetupHoveredSelectables(hits.Select(x => x.collider));
		}

		/// <summary>
		/// Direct selection of the GameEntity through Left click.
		/// This method handles the additive selection when ctrl key is pressed.
		/// Also unselects current selected eGameEntities when no ctrl key is selected.
		/// </summary>
		public void ProcessSelectionEvent()
		{
			if (isSelectionBoxMode)
				return;
			SelectionSystem.SelectHovered(AdditiveSelection);
		}
	}
}