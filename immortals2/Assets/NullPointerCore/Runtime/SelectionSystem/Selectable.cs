using System;
using System.Collections.Generic;
using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// Handles the status of the unit regarding the selection system.<br>
	/// Those statuses can be: Selected, Highlighted, Hovered, Unselected, Unhighlighted, Unhovered.<br>
	/// For a description of what those status means, please refer to @ref selectable_states.<br>
	/// <br>
	/// @image html selectable.png
	/// </summary>
	[RequireGameSystem(typeof(SelectionSystem))]
	public class Selectable : NullPointerBehaviour
	{
		[SerializeField]
		private Collider m_selectableCollider;
		/// <summary>
		/// Gives a way to select specific units over others when selected in group
		/// </summary>
		[SerializeField]
		private int m_priority = 0;

		[SerializeField, ReadOnly]
		private bool m_IsSelected = false;
		[SerializeField, ReadOnly]
		private bool m_IsHighlighted = false;
		[SerializeField, ReadOnly]
		private bool m_IsHovered = false;

		/// <summary>
		/// Action triggered whenever the GameEntity becomes Selected according to the SelectionSystem.
		/// </summary>
		public Action Selected;
		/// <summary>
		/// Action triggered whenever the GameEntity becomes Unselected according to the SelectionSystem.
		/// </summary>
		public Action Unselected;
		/// <summary>
		/// Action triggered whenever the GameEntity becomes highlighted according to the SelectionSystem.
		/// </summary>
		public Action Highlighted;
		/// <summary>
		/// Action triggered whenever the GameEntity becomes unhighlighted according to the SelectionSystem.
		/// </summary>
		public Action Unhighlighted;
		/// <summary>
		/// Action triggered whenever the GameEntity becomes hovered according to the SelectionSystem.
		/// </summary>
		public Action Hovered;
		/// <summary>
		/// Action triggered whenever the GameEntity becomes unhovered according to the SelectionSystem.
		/// </summary>
		public Action Unhovered;

		private SelectionSystem m_selectionSystem;
		private int m_selectionOrder = 0; // keeps track of last selection index when hovering more than one unit.

		/// <summary>
		/// Returns the current selection order. (This value is increased after each selection and reseted after
		/// the GameEntity is no longer hovered).
		/// </summary>
		public int SelectionOrder { get { return m_selectionOrder; } }
		/// <summary>
		/// Gives a way to select specific units over others when selected in group
		/// </summary>
		public int Priority {  get { return m_priority; } set { m_priority = value; } }

		/// <summary>
		/// Returns the current selection status.
		/// </summary>
		public bool IsSelected { get { return m_IsSelected; } }
		/// <summary>
		/// Returns the current hovered status.
		/// </summary>
		public bool IsHovered { get { return m_IsHovered; } }
		/// <summary>
		/// Returns the current highlight status.
		/// </summary>
		public bool IsHighlighted { get { return m_IsHighlighted; } }
		/// <summary>
		/// The bounding collider used for detection of the selection.
		/// </summary>
		public virtual Collider BoundCollider { get { return m_selectableCollider; } set { m_selectableCollider = value; } }

		/// <summary>
		/// Cache used internally for reference to the SelectionSystem.
		/// </summary>
		protected SelectionSystem SelectionSystem 
		{
			get
			{
				if (m_selectionSystem==null)
					m_selectionSystem = Game.GetSystem<SelectionSystem>();
				return m_selectionSystem;
			}
		}

		#region Initializers

		/// <summary>
		/// Overradeable Unity's builtin method OnEnable().
		/// </summary>
		virtual protected void OnEnable()
		{
			RegisterSelectionCollider();
		}

		/// <summary>
		/// Overradeable Unity's builtin method OnDisable().
		/// </summary>
		virtual protected void OnDisable()
		{
			UnregisterSelectionCollider();
		}



		/// <summary>
		/// Register the Selectable in the SelectionSystem
		/// </summary>
		protected void RegisterSelectionCollider()
		{
			// Registers the collider and asociate it with this selectable.
			if (SelectionSystem != null && enabled && BoundCollider)
				SelectionSystem.Register(this);
		}

		/// <summary>
		/// Unregister the Selectable in the SelectionSystem.
		/// </summary>
		protected void UnregisterSelectionCollider()
		{
			// Unregisters the collider in the selection system.
			if (SelectionSystem != null && (!enabled || BoundCollider==null))
				SelectionSystem.Unregister(this);
		}

		#endregion Initializers

		/// <summary>
		/// Send the order to the selection system to select this Selectable.
		/// </summary>
		public virtual void Select()
		{
			SelectionSystem.Select(this);
		}

		/// <summary>
		/// Send the order to the selection system to deselect this Selectable.
		/// </summary>
		public virtual void Unselect()
		{
			SelectionSystem.Deselect(this);
		}

		#region SelectionSystem reports

		/// <summary>
		/// Called from SelectionSystem to inform that the GameEntity selection status was changed.
		/// </summary>
		/// <param name="enable">The new state of the selection status.</param>
		internal void ReportSelection(bool enable)
		{
			if (enable)
				OnSelected();
			else
				OnUnselected();

			if (enable)
				m_selectionOrder++;
			m_IsSelected = enable;

			OnSelectableChanged();

			if (m_IsSelected && Selected!=null)
				Selected.Invoke();
			if(!m_IsSelected && Unselected!=null)
				Unselected.Invoke();
		}

		/// <summary>
		/// Called from SelectionSystem to inform that the GameEntity hover status was changed.
		/// </summary>
		/// <param name="enable">The new state of the hover status.</param>
		internal void ReportCursorHovering(bool enable)
		{
			if(!enable) // If the hovering is off we clear the selectionOrder
				m_selectionOrder = 0; // to prevent the reach an infinite value.

			if (enable)
				OnHovered();
			else
				OnUnhovered();

			m_IsHovered = enable;
			OnSelectableChanged();

			if (m_IsHovered && Hovered!=null)
				Hovered.Invoke();
			if(!m_IsHovered && Unhovered!=null)
				Unhovered.Invoke();
		}

		/// <summary>
		/// Called from SelectionSystem to inform that the GameEntity selection highlight was changed.
		/// The selection highlight it's used to give user feedback about the next units to be selected
		/// after multiple units hovered.
		/// </summary>
		/// <param name="enable">The new state of the highlight status.</param>
		internal void ReportHighlight(bool enable)
		{
			if (enable)
				OnHighlighted();
			else
				OnUnhighlighted();

			m_IsHighlighted = enable;

			OnSelectableChanged();

			if (m_IsHighlighted && Highlighted!=null)
				Highlighted.Invoke();
			if(!m_IsHighlighted && Unhighlighted!=null)
				Unhighlighted.Invoke();
		}

		#endregion SelectionSystem reports

		#region Events

		/// <summary>
		/// Overrideable method called when this selectable is selected.
		/// </summary>
		protected virtual void OnSelectableChanged() { }

		/// <summary>
		/// Overrideable method called when this selectable is selected.
		/// </summary>
		protected virtual void OnSelected() { }
		/// <summary>
		/// Overrideable method called when this selectable is unselected.
		/// </summary>
		protected virtual void OnUnselected() { }
		/// <summary>
		/// Overrideable method called when this selectable is highlighted.
		/// </summary>
		protected virtual void OnHighlighted() { }
		/// <summary>
		/// Overrideable method called when this selectable is unhighlighted.
		/// </summary>
		protected virtual void OnUnhighlighted() { }
		/// <summary>
		/// Overrideable method called when this selectable is hovered.
		/// </summary>
		protected virtual void OnHovered() { }
		/// <summary>
		/// Overrideable method called when this selectable is unhovered.
		/// </summary>
		protected virtual void OnUnhovered() { }

		#endregion Events

		#region Static methods

		/// <summary>
		/// Returns the first buildable inside the hoverings Selectable collection.
		/// </summary>
		/// <param name="hoverings">The collection where to find the buildable.</param>
		/// <returns>The first buildable found or null if none is found.</returns>
		public static T FilterFirstComponent<T>(IEnumerable<Selectable> hoverings)
		{
			foreach (Selectable selectable in hoverings)
			{
				T result = selectable.GetComponent<T>();
				if (result != null)
					return result;
			}
			return default(T);
		}

		#endregion Static methods
	}
}