using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
	/// <summary>
	/// Contains some usefull Rect extension methods.
	/// </summary>
	public static class RectExtensions
	{
		/// <summary>
		/// Returns a similar rect but expanded in all directions by the given borders.
		/// @note negative values will shrink the rect. make sure that the shrink values doesn't give negative rect.
		/// </summary>
		/// <param name="source">The source rect to be changed.</param>
		/// <param name="left">The space to add to the left side of the rect.</param>
		/// <param name="right">The space to add to the right side of the rect.</param>
		/// <param name="top">The space to add to the top side of the rect.</param>
		/// <param name="bottom">The space to add to the bottom side of the rect.</param>
		/// <returns>The new rect with the expanded borders.</returns>
		public static Rect MakeBorders(this Rect source, float left, int right, float top, float bottom)
		{
			return new Rect(source.x + left, source.y + top, source.width - left - right, source.height - top - bottom);
		}

		/// <summary>
		/// Returns a new rect similar to this one but changing its height value.
		/// </summary>
		/// <param name="source">The source rect.</param>
		/// <param name="height">The height value of the new rect.</param>
		/// <returns>The new rect with the modified height.</returns>
		public static Rect Height(this Rect source, float height)
		{
			return new Rect(source.x, source.y, source.width, height);
		}

		/// <summary>
		/// Creates a new rect with an identation relative to the old one.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="indent">The identation to add to the new rect.</param>
		/// <returns></returns>
		public static Rect Indent(this Rect source, float indent)
		{
			return new Rect(source.x + indent, source.y, source.width - indent, source.height);
		}

		/// <summary>
		/// Returns a new rect with its position offseted by the given delta parameters.
		/// </summary>
		/// <param name="source">The source rect.</param>
		/// <param name="dx">The offset x cooord to move the new rect.</param>
		/// <param name="dy">The offset y cooord to move the new rect.</param>
		/// <returns>The new rect with the modified coords.</returns>
		public static Rect Move(this Rect source, float dx, float dy)
		{
			return new Rect(source.x + dx, source.y + dy, source.width, source.height);
		}

		/// <summary>
		/// Returns a new rectangle with cutted horizontally by the given percentual parameters.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="percent_x">Specifies at with percent the rect should be cutted from the left.</param>
		/// <param name="percent_width">Specifies at with percent the rect should be cutted from the right.</param>
		/// <returns>The new rect with the modified horizontal values.</returns>
		public static Rect HorizontalPercent(this Rect source, float percent_x, float percent_width)
		{
			return new Rect(source.x + source.width * percent_x, source.y, source.width - source.width * percent_x - source.width * percent_width, source.height);
		}

		public static Rect HorizontalCut(this Rect source, float offset, float width)
		{
			return new Rect(source.x + offset, source.y, width, source.height);
		}

		public static Rect HorizontalCut(this Rect source, float offset)
		{
			return new Rect(source.x + offset, source.y, source.width-offset, source.height);
		}
	}
}