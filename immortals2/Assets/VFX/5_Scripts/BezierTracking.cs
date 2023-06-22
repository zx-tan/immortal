using NullPointerCore;
using System.Collections;
using UnityEngine;

namespace VFX
{
	public class BezierTracking : MonoBehaviour
	{
		IEnumerator currentRoutine;

		public enum BezierType
		{
			Cubic,
			Quad,
		}

		public BezierType bezierType = BezierType.Cubic;
		public Transform artContainer;
		public bool explodeOnTrajectoryEnds = true;
		public float destroyDelay = 2f;

		private bool traveling = false;
		private Vector3 lastPos = Vector3.zero;

		public virtual Vector3 StartPoint { get; set; }
		public virtual Vector3 StartControlOffset { get; set; }
		public virtual Vector3 EndControlOffset { get; set; }
		public virtual Vector3 EndPoint { get; set; }
		public virtual float TrajectoryProgress { get; set; }
		public virtual float StartingTrajectoryProgress { get; set; }

		public virtual Vector3 StartControlPoint { get { return StartPoint + StartControlOffset; } }
		public virtual Vector3 EndControlPoint { get { return EndPoint + EndControlOffset; } }

		public Vector3 TargetDir { get { return EndPoint - StartPoint; } }
		public Vector3 NormalizedTargetDir { get { return TargetDir.normalized; } }
		public float StartingTargetDistance { get { return TargetDir.magnitude; } }

		private float currProgress = 0.0f;

		protected virtual void Start()
		{
			lastPos = transform.position;
			traveling = true;
		}

		// Use this as a cheaper update loop
		public void LateUpdate()
		{
			
			if (traveling)
			{
				if (StartingTrajectoryProgress == 1.0f)
					currProgress = 1.0f;
				else
					currProgress = (TrajectoryProgress - StartingTrajectoryProgress) / (1- StartingTrajectoryProgress);

				if( bezierType == BezierType.Cubic)
					transform.position = CubicBezier(StartPoint, StartControlPoint, EndControlPoint, EndPoint, currProgress);
				else
					transform.position = QuadBezier(StartPoint, StartControlPoint, EndPoint, currProgress);

				if(transform.position != lastPos)
					transform.forward = transform.position - lastPos;

				lastPos = transform.position;
				if (explodeOnTrajectoryEnds && TrajectoryProgress >= 1)
					TriggerExplotion();
			}
			
		}

		public Vector3 QuadBezier(Vector3 start, Vector3 control, Vector3 end, float t)
		{
			return (((1 - t) * (1 - t)) * start) + (2 * t * (1 - t) * control) + ((t * t) * end);
		}

		public Vector3 CubicBezier(Vector3 start, Vector3 sc, Vector3 ec, Vector3 end, float t)
		{
			return (((-start + 3 * (sc - ec) + end) * t + (3 * (start + ec) - 6 * sc)) * t + 3 * (sc - start)) * t + start;
		}

		public void TriggerExplotion()
		{
			if (traveling)
			{
				traveling = false;
				Explode();
			}
		}

		private void Explode()
		{
			if (artContainer)
				artContainer.gameObject.SetActive(false);

			// Tell all child particle systems to STOP emitting
			foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
			{
				ps.Stop(true);
			}
			Destroy(this.gameObject, destroyDelay);
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(StartPoint, 0.4f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(EndPoint, 0.4f);
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(StartControlPoint, 0.4f);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(StartPoint, StartControlPoint);
			if (bezierType == BezierType.Cubic)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(EndControlPoint, 0.4f);
				Gizmos.DrawLine(StartControlPoint, EndControlPoint);
				Gizmos.DrawLine(EndControlPoint, EndPoint);
			}
			else
				Gizmos.DrawLine(StartControlPoint, EndPoint);

#if UNITY_EDITOR
			GizmosExt.Label(StartPoint, "StartPoint");
			GizmosExt.Label(EndPoint, "EndPoint");
			GizmosExt.Label(StartControlPoint, "StartControlPoint");
			if (bezierType == BezierType.Cubic)
				GizmosExt.Label(EndControlPoint, "EndControlPoint");
#endif
		}
	}
}
