using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a base script for interacting with FX
/// Written by Timothy Bermanseder
/// </summary>

public class HeroVFX : MonoBehaviour {

	public Animator heroAnimator;

	//[HideInInspector]
	public float animSpeed = 1;

	[HideInInspector]
	public Transform weaponSocket;

	[HideInInspector]
	public float VFXSequenceTimer = 0;
}
