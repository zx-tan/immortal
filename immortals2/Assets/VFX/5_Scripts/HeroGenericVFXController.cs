using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is a controller for the VFX that any hero can use. It is deliberately generic, It will grab the animator of said character and respond to animation events that occur.
/// Written by Timothy Bermanseder
/// </summary>

public class HeroGenericVFXController : MonoBehaviour
{

	[Header("Setup")]
	public bool useTravelSocket;
	public Transform travelSocket;

	[Header("VFX Prefabs")]
	public GameObject vfx_BasicAttack;
	public GameObject vfx_SpecialTravel;
	public GameObject vfx_SpecialImpact;

	GameObject travelFX;

	// This function will be called via anim event and will handle the spawning of the FX
	public void FX_BasicAttack()
	{
		// Spawn the effect
		Instantiate(vfx_BasicAttack, transform.position, transform.rotation);
	}

	public void FX_SpecialTravel()
	{
		if (useTravelSocket)
		{
			travelFX = Instantiate(vfx_SpecialTravel, travelSocket.position, travelSocket.rotation);
			travelFX.transform.parent = travelSocket;
		}
		else
		{
			Instantiate(vfx_SpecialTravel, transform.position, transform.rotation);
		}
	}

	public void FX_SpecialImpact()
	{

		if (useTravelSocket)
		{
			if(travelFX !=null)
				Destroy(travelFX);
		}

		// Spawn the effect
		Instantiate(vfx_SpecialImpact, transform.position, transform.rotation);
	}
}
