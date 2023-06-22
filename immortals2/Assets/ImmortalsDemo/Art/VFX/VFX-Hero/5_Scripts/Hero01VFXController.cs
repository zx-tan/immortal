using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is a controller for the VFX that the Hero01 uses. It will grab the animator of said character and respond to animation events that occur.
/// Written by Timothy Bermanseder
/// </summary>

public class Hero01VFXController : MonoBehaviour
{

	[Header("Setup")]
	[Tooltip("The animator we will be looking at and interfacing with.")]
	public Animator targetAnim;

	[Tooltip("If there is a weapon, this is the bone/transform for the hand holding it")]
	public Transform weaponSocket;

	[Tooltip("The length from the weapon socket to the head of the weapon. This is used for impacts.")]
	public float weaponLength = 1.5f;

	[Header("VFX Prefabs")]
	public HeroVFX vfx_hero01_C_Attack;
	public HeroVFX vfx_hero01_C_SpecialAttack_00;
	public HeroVFX vfx_hero01_C_SpecialAttack_01;
	public HeroVFX vfx_hero01_C_SpecialAttack_02;

	public GameObject vfx_Shockwave_Small;
	public GameObject vfx_Shockwave_Med;
	public GameObject vfx_Shockwave_Big;

	HeroVFX tempFX;
	Vector3 impactPoint;

	// Use this for initialization
	void Start () {
	
		// Setup
		if(targetAnim == null)
		{
			targetAnim = transform.GetComponent<Animator>();
		}

	}
	

	// This function will be called via anim event and will handle the spawning of the FX
	public void FX_C_AttackStart()
	{
		// Spawn the effect
		tempFX = Instantiate(vfx_hero01_C_Attack, transform.position, transform.rotation, EffectEntity.Root);
		tempFX.heroAnimator = targetAnim;
		tempFX.weaponSocket = weaponSocket;
		tempFX.animSpeed = targetAnim.GetCurrentAnimatorStateInfo(0).speed;
	}
	
	// This function will be called via anim event and will handle the spawning of the FX
	public void FX_C_SpecialAttack_00_Start()
	{
		// Spawn the effect
		tempFX = Instantiate(vfx_hero01_C_SpecialAttack_00, transform.position, transform.rotation, EffectEntity.Root);
		tempFX.heroAnimator = targetAnim;
		tempFX.weaponSocket = weaponSocket;
		tempFX.animSpeed = targetAnim.GetCurrentAnimatorStateInfo(0).speed;
	}

	// This function will be called via anim event and will handle the spawning of the FX
	public void FX_C_SpecialAttack_01_Start()
	{
		// Spawn the effect
		tempFX = Instantiate(vfx_hero01_C_SpecialAttack_01, transform.position, transform.rotation, EffectEntity.Root);
		tempFX.heroAnimator = targetAnim;
		tempFX.weaponSocket = weaponSocket;
		tempFX.animSpeed = targetAnim.GetCurrentAnimatorStateInfo(0).speed;
	}

	// This function will be called via anim event and will handle the spawning of the FX
	public void FX_C_SpecialAttack_02_Start()
	{
		// Spawn the effect
		tempFX = Instantiate(vfx_hero01_C_SpecialAttack_02, transform.position, transform.rotation, EffectEntity.Root);
		tempFX.heroAnimator = targetAnim;
		tempFX.weaponSocket = weaponSocket;
		tempFX.animSpeed = targetAnim.GetCurrentAnimatorStateInfo(0).speed;
	}

	// This function will be called via anim event and will handle the spawning of the FX
	public void FX_SmallShockwave()
	{
		impactPoint = weaponSocket.position;	// Line it up with the weapon socket
		impactPoint += weaponSocket.up * weaponLength; // Move it up to the head of the weapon
		impactPoint.y = transform.position.y; // Flatten it to the ground

		Instantiate(vfx_Shockwave_Small, impactPoint, transform.rotation, EffectEntity.Root);
	}

	// This function will be called via anim event and will handle the spawning of the FX
	public void FX_BigShockwave()
	{
		impactPoint = weaponSocket.position;    // Line it up with the weapon socket
		impactPoint += weaponSocket.up * weaponLength; // Move it up to the head of the weapon
		impactPoint.y = transform.position.y; // Flatten it to the ground

		Instantiate(vfx_Shockwave_Big, impactPoint, transform.rotation, EffectEntity.Root);
	}
	
	// This function will be called via anim event and will handle the spawning of the FX
	public void FX_MedShockwave()
	{
		impactPoint = weaponSocket.position;    // Line it up with the weapon socket
		impactPoint += weaponSocket.up * weaponLength; // Move it up to the head of the weapon
		impactPoint.y = transform.position.y; // Flatten it to the ground

		Instantiate(vfx_Shockwave_Med, impactPoint, transform.rotation, EffectEntity.Root);
	}
}
