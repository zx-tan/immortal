using UnityEngine;

namespace VFX
{
	public class BezierCurvedTracking : BezierTracking
	{
		public float startControlDistFactor = 0.2f;
		public float endControlDistFactor = 0.2f;

		private float totalDist;
		private Vector3 startingDir;
		private Vector3 endingDir;

		protected float StartControlDist { get { return StartingTargetDistance * startControlDistFactor; } }
		protected float EndControlDist { get { return StartingTargetDistance * endControlDistFactor; } }

		public override Vector3 StartControlOffset { get { return startingDir * StartControlDist; } }
		public override Vector3 EndControlOffset { get { return endingDir * EndControlDist; } }


		protected override void Start()
		{
			startingDir = transform.forward;
			endingDir = startingDir;
			endingDir.y = -endingDir.y;
			base.Start();
		}
	}
}
