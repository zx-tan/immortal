using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutTrail : MonoBehaviour {

    public TrailRenderer tr;
    public float lifetime = 2f;
    public Gradient colourOverLife;

    float time = 0;
	// Use this for initialization
	IEnumerator Start () {
        float normalizedTime;

        while (time <= lifetime)
        {

            normalizedTime = time / lifetime;

            tr.material.SetColor("_TintColor", colourOverLife.Evaluate(normalizedTime));


            time += Time.deltaTime;
            yield return null;
        }

        tr.material.SetColor("_TintColor", colourOverLife.Evaluate(1));

        yield return null;
	}
	
}
