using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlaceOnGround : MonoBehaviour
{
	public LayerMask navMeshMask = -1;
	
	public float maxDistance = 5.0f;
	public Transform target;

	public Transform Target { get { return target != null ? target : transform; } }
	// Use this for initialization
	void Awake ()
	{
		NavMeshHit hit;
		if( NavMesh.SamplePosition(Target.position, out hit, maxDistance, -1) )
		{
			Target.position = hit.position;
		}
	}
}
