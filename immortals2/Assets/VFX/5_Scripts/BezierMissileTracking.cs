using UnityEngine;

namespace VFX
{
	public class BezierMissileTracking : BezierTracking
	{
		public float control1Dist = 5.0f;
		public float control2Dist = 5.0f;
		public float control1Height = 5.0f;
		public float control2Height = 5.0f;

		private Vector3 startingDir;
		private Vector3 endingDir;

		public override Vector3 StartControlOffset { get { return startingDir * control1Dist + Vector3.up * control1Height; } }
		public override Vector3 EndControlOffset { get { return - endingDir * control2Dist + Vector3.up * control2Height; } }

		protected override void Start()
		{
			startingDir = transform.forward;
			Vector3 planeUpDir = Vector3.Cross(startingDir, NormalizedTargetDir);
			endingDir = Vector3.Cross(planeUpDir, NormalizedTargetDir);

			startingDir.y = startingDir.y * 0.2f;
			endingDir.y = endingDir.y * 0.2f;

			startingDir.Normalize();
			endingDir.Normalize();

			transform.forward = startingDir;
			base.Start();
		}
	}
}
