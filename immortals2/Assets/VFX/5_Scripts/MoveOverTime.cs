using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveOverTime : MonoBehaviour {

    bool moving = true;

	[FormerlySerializedAs("moveZ")]
    public float speed;
	public bool absolute = false;
	public Vector3 dir = Vector3.forward;

	// Use this for initialization
	IEnumerator Start () {

        while(moving)
        {
			if(absolute)
				transform.position += dir * speed * Time.deltaTime;
			else
				transform.position += transform.forward * speed * Time.deltaTime;

            yield return null;
        }


        //yield return null;

	}
	
}
