using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_SpawnAfterDelay : MonoBehaviour {

    public float delay = 1;
    public GameObject toSpawn;

    GameObject tempObj;

	// Use this for initialization
	IEnumerator Start () {


        yield return new WaitForSeconds(delay);

        if (toSpawn != null)
        {
            tempObj = Instantiate(toSpawn, transform.position, transform.rotation);
            tempObj.SetActive(true);
        }

        yield return null;
	}
	
}
