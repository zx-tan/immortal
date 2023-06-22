using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is a controller for the VFX that the Opera Hero uses. It will grab the animator of said character and respond to animation events that occur.
/// Written by Timothy Bermanseder
/// </summary>

public class HeroOperaVFXController : MonoBehaviour
{

	[Header("Setup")]
	public Transform parentObject;	// This is used to ensure the traveling effects are spawned on an object that travels with the animation

	[Header("VFX Prefabs")]
	public GameObject vfx_OperaBasicAttack;
	public GameObject vfx_OperaSpecialTravel;
	public GameObject vfx_OperaSpecialImpact;

	// This function will be called via anim event and will handle the spawning of the FX
	public void FX_BasicAttack()
	{
		// Spawn the effect
		Instantiate(vfx_OperaBasicAttack, transform.position, transform.rotation, parentObject);
	}

	public void FX_SpecialTravel()
	{
		// Spawn the effect
		Instantiate(vfx_OperaSpecialTravel, parentObject.position, transform.rotation, parentObject);
	}

	public void FX_SpecialImpact()
	{
		// Spawn the effect
		Instantiate(vfx_OperaSpecialImpact, transform.position, transform.rotation);
	}
}
