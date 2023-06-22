using System;

namespace UnityEngine
{
	/// <summary>
	/// Contains some usefull Camera extension methods.
	/// </summary>
	public static class CameraExtensions
	{

		/// <summary>
		/// Converts two screen positions into a bounds rectangle in viewport coordinates and returns it.
		/// </summary>
		/// <param name="cam">The context camera of this call.</param>
		/// <param name="screenPosition1">first position in screen coords.</param>
		/// <param name="screenPosition2">second position in screen coords.</param>
		/// <returns>Bounds rectangle in viewport coordinates.</returns>
		public static Bounds GetViewportBounds(this Camera cam, Vector3 screenPosition1, Vector3 screenPosition2)
		{
			if (cam == null)
				throw new ArgumentNullException("cam", "GetViewportBounds Exception. The camera shouldn't be null.");

			Vector3 v1 = cam.ScreenToViewportPoint(screenPosition1);
			Vector3 v2 = cam.ScreenToViewportPoint(screenPosition2);
			Vector3 min = Vector3.Min(v1, v2);
			Vector3 max = Vector3.Max(v1, v2);
			min.z = cam.nearClipPlane;
			max.z = cam.farClipPlane;

			var bounds = new Bounds();
			bounds.SetMinMax(min, max);
			return bounds;
		}
	}
}