using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a generic script for panning effects that rely on timed panning
/// Written by Timothy Bermanseder
/// </summary>

public class FXPanOffset : HeroVFX
{
	[Tooltip("The renderes we will influence to pan their materials.")]
	public Renderer[] panRenderers;

	[Tooltip("The material(shader) property we will be influencing.")]
	public string materialPropertyName;
	[Tooltip("The values that will be panned across and how fast it will happen.")]
	public float offsetStartPoint, offsetPanSpeed;
	[Tooltip("For each renderer, pull back the offset point by this much.")]
	public float offsetRendererAdjustment;
	[Tooltip("Will the offset pan positively, or negatively. (growing = positive)")]
	public bool offsetGrowing;
	float offsetCurrentPoint;
	Vector2 offsetVec;

	[Tooltip("How long until the effect is done and will clean itself up.")]
	public float fxLifetime;

	[Tooltip("How quickly the effect will fade out")]
	public float fadeSpeed;
	Color fadeCol;

	float lifetimer;
	// Use this for initialization
	IEnumerator Start () {

		lifetimer = fxLifetime;
		fadeCol = panRenderers[0].material.GetColor("_Color");

		offsetCurrentPoint = offsetStartPoint;

		while (lifetimer > 0)
		{
			// Moving the offset point
			if(offsetGrowing)
			{
				offsetCurrentPoint += Time.deltaTime * offsetPanSpeed;
			}
			else
			{
				offsetCurrentPoint -= Time.deltaTime * offsetPanSpeed;
			}

			// Applying the offset point
			for(int i=0; i < panRenderers.Length; i++)
			{
				//Debug.Log("Loop " + i);
				offsetVec.x = offsetCurrentPoint - (offsetRendererAdjustment * i);

				panRenderers[i].material.SetTextureOffset(materialPropertyName, offsetVec);
			}

			// Ending the effect
			lifetimer -= Time.deltaTime;

			yield return null;
		}

		// Quickly fade out the alpha
		lifetimer = 2;
		while(lifetimer > 0)
		{
			if (fadeCol.a > 0)
			{
				fadeCol.a -= Time.deltaTime * fadeSpeed;


				foreach (Renderer ren in panRenderers)
				{
					ren.material.SetColor("_Color", fadeCol);
				}
			}
			else
			{
				fadeCol.a = 0;
				foreach (Renderer ren in panRenderers)
				{
					ren.material.SetColor("_Color", fadeCol);
				}
			}
			lifetimer -= Time.deltaTime;

			yield return null;
		}

		// All done, clean up.
		Destroy(this.gameObject);

		yield return null;
	}
	
}
