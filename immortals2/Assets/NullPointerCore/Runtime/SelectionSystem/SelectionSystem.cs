using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// The system in control of all GameEntities that can be hovered, highlighted and selected.
	/// </summary>
	public class SelectionSystem : GameSystem
	{
		/// <summary>
		/// Indicates if it's allowed to select multiple units of just one at the time.
		/// </summary>
		public bool multipleSelection = true;
		/// <summary>
		/// Action triggered whenever a unit is selected
		/// </summary>
		public Action<Selectable>	OnSelected;
		/// <summary>
		/// Action triggered whenever a unit is deselected
		/// </summary>
		public Action<Selectable>	OnDeselected;
		/// <summary>
		/// Action triggered when all selecteds are unselected at once.
		/// </summary>
		public Action				selectionRemovedAll;

		private List<Selectable> hoveringEntities = new List<Selectable>();
		private List<Selectable> selectedEntities = new List<Selectable>();

		private Dictionary<Collider, Selectable> colliders = new Dictionary<Collider, Selectable>();

		/// <summary>
		/// Count of Selectables selecteds.
		/// </summary>
		public int SelectedCount { get { return selectedEntities.Count; } }
		/// <summary>
		/// There's at least one Selectable selected?
		/// </summary>
		public bool HasSelectedEntities { get { return selectedEntities.Count > 0; } }
		/// <summary>
		/// Returns a IEnumerable<Selectable> containing all the selecteds Selectables.
		/// </summary>
		public IEnumerable<Selectable> Selecteds { get { return selectedEntities; } }
		/// <summary>
		/// There's at least one Selectable hovered?
		/// </summary>
		public bool HasHoveredEntities { get { return hoveringEntities.Count > 0; } }
		/// <summary>
		/// Returns a IEnumerable<Selectable> containing all the hovered Selectables.
		/// </summary>
		public IEnumerable<Selectable> Hoverings { get { return hoveringEntities; } }
		/// <summary>
		/// Indicates the current amount of hovered entities.
		/// </summary>
		public int HoveringCount { get { return hoveringEntities.Count; } }
		/// <summary>
		/// Indicates the current amount of selected units.
		/// </summary>
		public int SelectablesCount { get { return colliders.Count; } }

		/// <summary>
		/// Returns a list of ordered Selectables. That order is given by the SelectionOrder wich is a value
		/// increased each time a hovered entity is selected. This allows us to known from all the currently
		/// hovered entities which one was already selected and which one not.
		/// </summary>
		public List<Selectable> OrderedHoveringEntities 
		{
			get
			{
				return hoveringEntities.OrderBy(m => m.SelectionOrder).ToList();
			}
		}

		#region Selectable Registration

		/// <summary>
		/// Registers the Selectable into the system.
		/// </summary>
		/// <param name="selectable"></param>
		internal void Register(Selectable selectable)
        {
			if (!colliders.ContainsKey(selectable.BoundCollider))
				colliders.Add(selectable.BoundCollider, selectable);
        }

		/// <summary>
		/// Unregisters the given collider from the system.
		/// </summary>
		/// <param name="collider">The collider to be unregistered.</param>
        internal void Unregister(Selectable selectable)
        {
			if( selectedEntities.Contains(selectable) )
				DoDeselect(selectable);
			RemoveHovering(selectable);
			SafelyRemoveColliderFor(selectable);
		}

		internal void SafelyRemoveColliderFor(Selectable target)
		{
			if (target.BoundCollider != null)
				colliders.Remove(target.BoundCollider);
			else
			{
				foreach (KeyValuePair<Collider, Selectable> kvp in colliders)
				{
					if (target == kvp.Value)
					{
						colliders.Remove(kvp.Key);
						break;
					}
				}
			}
		}

		#endregion Selectable Registration

		#region Helpers
		/// <summary>
		/// Collects all Selectables whose BoundCollider center is inside the given Camera viewport.
		/// </summary>
		/// <param name="cam">The camera that should be related when searching for the selectables in bounds.</param>
		/// <param name="viewportBounds">The viewport bounds where to collect the matching Selectables</param>
		/// <returns>A IEnumerable<Selectable> with each BoundCollider inside the viewport bounds.</returns>
		public IEnumerable<Selectable> CollectSelectablesInBounds(Camera cam, Bounds viewportBounds)
		{
			foreach(KeyValuePair<Collider,Selectable> pair in colliders)
			{
				if (viewportBounds.Contains(cam.WorldToViewportPoint(pair.Key.transform.position)))
					yield return pair.Value;
			}
			yield break;
		}

		/// <summary>
		/// Given a collider find its matching Selectable.
		/// </summary>
		/// <param name="colliderToCheck">The collider matching the requested selectable.</param>
		/// <returns>A Selectable matching their given BoundCollider.</returns>
		public Selectable GetSelectable(Collider colliderToCheck)
		{
			Selectable result = null;
			colliders.TryGetValue(colliderToCheck, out result);
			return result;
		}

		/// <summary>
		/// Collects each selectable matching their given BoundColliders.
		/// </summary>
		/// <param name="collidersToCheck"></param>
		/// <returns>Returns an IEnumerable<Selectable> which has selectables matching their given BoundCollider.</returns>
		public IEnumerable<Selectable> CollectSelectables(IEnumerable<Collider> collidersToCheck)
		{
			Selectable selectable = null;
			foreach (Collider collider in collidersToCheck)
			{
				if (colliders.TryGetValue(collider, out selectable))
					yield return selectable;
			}
			yield break;
		}
		#endregion Helpers

		#region Selection
		/// <summary>
		/// Marks as Selected a given selectable. Can be indicated if should be added or toggled with the current selection.
		/// </summary>
		/// <param name="toSelect">The selectable to be selected.</param>
		/// <param name="additive">Indicates if the selectable should be added to the current selection.</param>
		public void Select(Selectable toSelect, bool additive=false)
		{
			List<Selectable> listToDeselect = new List<Selectable>(selectedEntities);
			foreach(Selectable toDeselect in listToDeselect)
			{
				if(toDeselect != toSelect )
					DoDeselect(toDeselect);
			}
			DoSelect(toSelect);
		}

		/// <summary>
		/// Deselects the given selectable.
		/// </summary>
		/// <param name="toDeselect">The selectable to be deselected.</param>
		public void Deselect(Selectable toDeselect)
		{
			DoDeselect(toDeselect);
		}

		/// <summary>
		/// Marks as selected an entire collection of selectables. Can be inidcated if should be added or toggled with the current selection.
		/// </summary>
		/// <param name="toSelect">The collection of selectables to be selected.</param>
		/// <param name="additive">Indicates if the selectable should be added to the current selection.</param>
		public void Select(IEnumerable<Selectable> toSelect, bool additive = false)
		{
			List<Selectable> listToDeselect = new List<Selectable>(selectedEntities);
			foreach (Selectable toDeselect in listToDeselect)
			{
				if ( !toSelect.Contains(toDeselect) )
					DoDeselect(toDeselect);
			}
			foreach (Selectable sel in toSelect)
				DoSelect(sel);
		}

		/// <summary>
		/// Deselects all units from the current selection.
		/// </summary>
		public void DeselectAll()
		{
			RemoveAllSelected();
			RemoveAllHovering();
		}

		/// <summary>
		/// Marks as Selected the given Selectable.
		/// </summary>
		/// <param name="toSelect">The Selectable to be marked as selected.</param>
		void DoSelect(Selectable toSelect)
		{
			if (!toSelect.IsSelected)
			{
				toSelect.ReportSelection(true);
				if (OnSelected != null)
					OnSelected.Invoke(toSelect);
			}
			if(!selectedEntities.Contains(toSelect))
				selectedEntities.Add(toSelect);

		}

		/// <summary>
		/// Removes selected mark of the given Selectable.
		/// </summary>
		/// <param name="toSelect">The Selectable to be Unselected.</param>
		void DoDeselect(Selectable toDeselect)
		{
			if (toDeselect.IsSelected)
			{
				toDeselect.ReportSelection(false);
				if (OnDeselected != null)
					OnDeselected.Invoke(toDeselect);
			}
			if(selectedEntities.Contains(toDeselect))
				selectedEntities.Remove(toDeselect);
		}

		/// <summary>
		/// Removes marks of each Selectable already selected.
		/// </summary>
		internal void RemoveAllSelected()
		{
			List<Selectable> listToDeselect = new List<Selectable>(selectedEntities);
			foreach ( Selectable selected in listToDeselect)
				DoDeselect(selected);
		}

		#endregion Selection

		#region Hovering


		/// <summary>
		/// Marks each selectable as Hovered and remove hovered mark for each already hovered selectables
		/// that are not in selectablesToHover. 
		/// </summary>
		/// <param name="selectablesToHover"></param>
		public void SetAsHovering(IEnumerable<Selectable> selectablesToHover)
		{
			List<Selectable> currentHovering = new List<Selectable>();
			foreach( Selectable selectable in selectablesToHover)
			{
				currentHovering.Add(selectable);
				if (hoveringEntities.Contains(selectable)) // Is already in the list of hovering elements
					hoveringEntities.Remove(selectable); // We remove the element because we are creating a new list
				else
					selectable.ReportCursorHovering(true);
			}
			// The remaining elements in the hoveringEntities list are no longer been hovered
			foreach (Selectable selectable in hoveringEntities)
			{
				if(selectable.IsHighlighted)
					selectable.ReportHighlight(false);
				selectable.ReportCursorHovering(false);
			}
			// now the currentHovering list contains the valid hovering entities.
			hoveringEntities = currentHovering;
		}

		/// <summary>
		/// Marks all selectables currently highlighted as selected.
		/// </summary>
		public void ConfirmAllHighlightsAsSelected(bool additive=false)
		{
			foreach (Selectable sel in hoveringEntities)
			{
				if(sel.IsHighlighted)
					DoSelect(sel);
			}
			if (!additive || multipleSelection)
			{
				List<Selectable> toDeselect = new List<Selectable>(Selecteds);
				foreach (Selectable sel in toDeselect)
				{
					if (!sel.IsHighlighted)
						DoDeselect(sel);
				}
			}
		}

		/// <summary>
		/// Removes hover mark of each already hovering selectables (removes also its highlight mark if they have)
		/// </summary>
		public void RemoveAllHovering()
		{
			foreach (Selectable hover in hoveringEntities)
			{
				if(hover.IsHighlighted)
					hover.ReportHighlight(false);
				hover.ReportCursorHovering(false);
			}
			if(hoveringEntities.Count>0)
				hoveringEntities.Clear();
		}

		/// <summary>
		/// Removes the hovering state for a specific Selectable.
		/// </summary>
		/// <param name="toUnhover">The selectable to be unhovered.</param>
		public void RemoveHovering(Selectable toUnhover)
		{
			if(hoveringEntities.Contains(toUnhover))
			{
				if(toUnhover.IsHighlighted)
					toUnhover.ReportHighlight(false);
				toUnhover.ReportCursorHovering(false);
				hoveringEntities.Remove(toUnhover);
			}
		}

		/// <summary>
		/// Marks as selected all the selectables currently marked as hovered.
		/// </summary>
		/// <param name="additive">Indicates if should be added to the current selection or toggled.</param>
		public void SelectHovered(bool additive)
		{
			Selectable toSelect = null;
			bool hasAdditiveSelection = additive && multipleSelection;
			if (HasHoveredEntities)
			{
				List<Selectable> orderedList = OrderedHoveringEntities;
				toSelect = orderedList[0];
			}

			// No additive selection was requested?
			if (!hasAdditiveSelection)
			{
				// Unselects the current selected entities.
				List<Selectable> toDeselect = new List<Selectable>(Selecteds);
				toDeselect.Remove(toSelect);
				foreach (Selectable s in toDeselect)
					DoDeselect(s);
			}
			// Something to select?
			if (toSelect != null)
			{
				// Is already selected?
				if (!toSelect.IsSelected)
					DoSelect(toSelect);
				else if (!hasAdditiveSelection)
					DoDeselect(toSelect);
			}
		}

		/// <summary>
		/// Marks as hovered the given collection of colliders, also marks it as highlighted after applying the corresponding filters.
		/// </summary>
		/// <param name="colliders">The collection of colliders to mark as hovered.</param>
		public void SetupHoveredSelectables(IEnumerable<Collider> colliders)
		{
			// From each collider collects it corresponding Selectable (if it was registered).
			IEnumerable<Selectable> toHover = CollectSelectables(colliders);
			// 3. Marks each selectable as hovered.
			SetupHoveredSelectables(toHover);
		}

		/// <summary>
		/// Marks as hovered the given collection of selectables, also marks it as highlighted after applying the corresponding filters.
		/// </summary>
		/// <param name="toHover">The collection of selectables to mark as hovered.</param>
		public void SetupHoveredSelectables(IEnumerable<Selectable> toHover)
		{
			// Marks each selectable as hovered.
			SetAsHovering(toHover);

			// Select witch Selectables are going to be selected according to the game custom criteria, 
			// or all of them if no criteria is defined.
			IEnumerable<Selectable> prefilter = toHover; // When no criteria defined, all hovered must be highlighted
			prefilter = FilterByPriority(toHover);
			// Finally, we need to highlight the GameEntities.
			SetAsHighlighted(prefilter);
		}

		#endregion Hovering

		#region Highlight
		/// <summary>
		/// Marks each Selectable as Highlighted. each Selectable in selectablesToHighligh must be
		/// already marked as hovered in order to also be highlighted.
		/// </summary>
		/// <param name="selectablesToHighligh">Enumerable with the group of hovered Selectables to be highlighted.</param>
		public void SetAsHighlighted(IEnumerable<Selectable> selectablesToHighligh)
		{
			if (!multipleSelection)
				selectablesToHighligh = selectablesToHighligh.Reverse().Take(1);

			HashSet<Selectable> highlightables = new HashSet<Selectable>(selectablesToHighligh);
			foreach (Selectable hover in hoveringEntities)
			{
				bool mustHighlight = highlightables.Contains(hover);
				if(hover.IsHighlighted && !mustHighlight )
					hover.ReportHighlight(false);
				else if(!hover.IsHighlighted && mustHighlight)
					hover.ReportHighlight(true);
			}
		}

		/// <summary>
		/// Filters the given collection of selectables returning only the ones with highest priority.
		/// </summary>
		/// <param name="source">The collection of selectables to be filtered.</param>
		/// <returns>A new collection of selectables but filtered with the highest values of priority.</returns>
		public static IEnumerable<Selectable> FilterByPriority(IEnumerable<Selectable> source)
		{
			int highestPriority = 0;
			List<Selectable> result = new List<Selectable>();
			foreach( Selectable sel in source )
			{
				if (sel == null)
					continue;
				if (sel.Priority == highestPriority)
					result.Add(sel);
				else if(sel.Priority > highestPriority)
				{
					result.Clear();
					highestPriority = sel.Priority;
					result.Add(sel);
				}
			}
			return result;
		}
		#endregion Highlight
	}
}