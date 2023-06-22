using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// Handles the scene borders and clickeable area into the scene.
	/// </summary>
	public class SceneBounds : GameSystem
	{
		private static Vector3 defaultPlaneNormal = Vector3.up;
		private static Vector3 defaultPlanePoint = Vector3.zero;

		/// <summary>
		/// Collider that defines the available dragging area for the camera.
		/// If nothing is defined then an infinity plane at the origin is used. 
		/// </summary>
		[Tooltip("Collider that defines drag area. If nothing is defined then an infinity plane at the origin is used.")]
		public Collider draggingCollider;
		/// <summary>
		/// Collider that defines the available scene area to move with the camera.
		/// If nothing is defined then an infinity plane at the origin is used.
		/// </summary>
		[Tooltip("Collider that defines the bounding area of the game scene (How far the camera can scroll). If nothing is defined then an infinity plane at the origin is used.")]
		public Collider boundsCollider;
		/// <summary>
		/// Raycast max distance.
		/// </summary>
		[Tooltip("Raycast max distance.")]
		public float maxDistance = 1000.0f;

		private Vector3 cursorLookPoint = Vector3.zero;
		private bool focusWasRestored = false;

		/// <summary>
		/// The world coordinates of the mouse cusor hitting with the bounds surface.
		/// </summary>
		public Vector3 CursorLookPoint 
		{
			get
			{
				// Fix for unity bug not updating the mouse position correctly
				if(focusWasRestored) // after a windows focus lost.
				{
					ScreenPosToWorldPlane(Input.mousePosition, ref cursorLookPoint);
					focusWasRestored = false;
				}
				return cursorLookPoint;
			}
		}

		private void Update()
		{
			ScreenPosToWorldPlane(Input.mousePosition, ref cursorLookPoint);
		}

		private void OnApplicationFocus(bool focus)
		{
			if(focus) // Fix for unity bug not updating the mouse position correctly
				focusWasRestored = true; // after a windows focus lost.
		}

		[ContextMenu("Create Default Colliders")]
		public void CreateDefaultColliders()
		{
			//GameScene.
		}

		/// <summary>
		/// Returns as a result the point hitting over the boundsCollider
		/// surface and traced from the camera to the given screen coordinates
		/// </summary>
		/// <param name="screenPos">Screen coordinates to trace over the bounds surface.</param>
		/// <param name="result">A world coordinates vector as a result of the trace.</param>
		/// <returns>False if the ray doesn't hit the bounds surface or true on succeed.</returns>
		public bool ScreenPosToWorldPlane(Vector3 screenPos, ref Vector3 result)
		{
			return Raycast(Camera.main.ScreenPointToRay(screenPos), ref result);
		}

		/// <summary>
		/// Returns as a result the point hitting over the boundsCollider
		/// surface and traced from the camera to the given screen coordinates
		/// </summary>
		/// <param name="ray">The starting point and direction of the ray.</param>
		/// <param name="result">A world coordinates vector as a result of the trace.</param>
		/// <returns>False if the ray doesn't hit the bounds surface or true on succeed.</returns>
		public bool Raycast(Ray ray, ref Vector3 result)
		{
			if(draggingCollider!=null)
			{
				RaycastHit hitInfo;
				if (draggingCollider.Raycast(ray, out hitInfo, maxDistance))
				{
					result = ray.GetPoint(hitInfo.distance);
					return true;
				}
			}
			else if( PlaneRaycast(ray, defaultPlaneNormal, defaultPlanePoint, out result))
				return true;
			return false;
		}
		
		/// <summary>
		/// Restrains the point to a position inside the terrainCollider.
		/// The same position will be returned in case that is correctly inside the bounds of
		/// position and height.
		/// </summary>
		/// <param name="point">Source position agains that you want to check.</param>
		/// <param name="result">The resulting fixed position.</param>
		/// <returns>true if the point it's inside the colliders bounds.</returns>
		public bool PointInsideWorldBounds(Vector3 point, out Vector3 result)
		{
			RaycastHit hitInfo;
			Ray cursorRay = new Ray(point + Vector3.up * maxDistance * 0.5f, -Vector3.up);
			if(boundsCollider!=null)
			{
				if (boundsCollider.Raycast(cursorRay, out hitInfo, maxDistance))
				{
					result = cursorRay.GetPoint(hitInfo.distance);
					return true;
				}
				result = boundsCollider.ClosestPoint(point);
				return false;
			}
			result = ClosestPointOnPlane(point, defaultPlaneNormal, defaultPlanePoint);
			return true;
		}

		/// <summary>
		/// Returns a point in the scene bounds that is closest to a given location.
		/// </summary>
		/// <param name="point">Location you want to find the closest point to.</param>
		/// <param name="planeNormal">The plane normal.</param>
		/// <param name="planePoint">Any point in the plane.</param>
		/// <returns>The point on the collider that is closest to the specified location.</returns>
		public Vector3 ClosestPointOnPlane(Vector3 point, Vector3 planeNormal, Vector3 planePoint)
		{
			Vector3 normal = Vector3.Normalize(planeNormal);
			float d = Vector3.Dot(normal, point) - Vector3.Dot(normal, planePoint);
			return point - normal * d;
		}

		/// <summary>
		/// Casts a Ray that intersects with the given plane.
		/// </summary>
		/// <param name="ray">The starting point and direction of the ray.</param>
		/// <param name="planeNormal">The plane normal.</param>
		/// <param name="planePoint">Any point in the plane.</param>
		/// <param name="resultPoint">The intersection point between the ray and the plane.</param>
		/// <returns>True when the ray intersects the collider, otherwise false.</returns>
		private bool PlaneRaycast(Ray ray, Vector3 planeNormal, Vector3 planePoint, out Vector3 resultPoint)
		{
			Vector3 normal = Vector3.Normalize(planeNormal);
			float dist = -Vector3.Dot(normal, planePoint);
			float num = Vector3.Dot(ray.direction, normal);
			float num2 = -Vector3.Dot(ray.origin, normal) - dist;
			float enter;
			bool result;
			if (Mathf.Approximately(num, 0f))
			{
				enter = 0f;
				result = false;
			}
			else
			{
				enter = num2 / num;
				result = (enter > 0f);
			}
			resultPoint = ray.GetPoint(enter);
			return result;
		}
	}
}