using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script handles the timing of the named effect
/// Written by Timothy Bermanseder
/// </summary>

public class Hero01CBasicAttackSequence : HeroVFX {

	[SerializeField]
	Renderer[] strikeRenderers;

	[SerializeField]
	float strikeStartingPoint = 1, strikeEndPoint = -1, strikePanSpeed = 1;

	float strikeCurrentPoint;
	Vector2 tempOffset;

	[SerializeField]
	Renderer HammerGlow;

	[SerializeField]
	AnimationCurve hammerGlowAlphaCurve;
	Color hammerGlowCol;
	// ref
	//strikeRenderer.material.GetTextureOffset("_MainTex").x;

	// Use this for initialization
	IEnumerator Start () {
		// Setup
		if (weaponSocket != null)
		{
			// Position and parent the hammer glow
			HammerGlow.transform.parent = weaponSocket;
			HammerGlow.transform.position = weaponSocket.position;
			HammerGlow.transform.rotation = weaponSocket.rotation;
			hammerGlowCol = HammerGlow.material.GetColor("_Color");
			hammerGlowCol.a = 0;
		}
		else
		{
			HammerGlow.gameObject.SetActive(false);
		}
		// Setup the strike materials
		strikeCurrentPoint = strikeStartingPoint;
		tempOffset.x = strikeCurrentPoint;

		// Apply the starting point to the material
		SetMaterialOffsets(strikeRenderers, tempOffset);

		// The main loop
		while (strikeCurrentPoint > strikeEndPoint)
		{
			strikeCurrentPoint -= Time.deltaTime * strikePanSpeed * animSpeed;
			VFXSequenceTimer += Time.deltaTime * strikePanSpeed * animSpeed; // Tick up the sequence

			// The strike meshes
			if (strikeCurrentPoint <= strikeEndPoint)
				strikeCurrentPoint = strikeEndPoint;

			tempOffset.x = strikeCurrentPoint;
			SetMaterialOffsets(strikeRenderers, tempOffset);

			// The hammer glow
			hammerGlowCol.a = hammerGlowAlphaCurve.Evaluate(VFXSequenceTimer);
			HammerGlow.material.SetColor("_Color", hammerGlowCol);

			yield return null;
		}

		// All finished, clean itself up.
		Destroy(HammerGlow.gameObject);
		Destroy(transform.gameObject);

		yield return null;
	}
	
	void SetMaterialOffsets(Renderer[] Rens, Vector2 newValue)
	{
		foreach(Renderer ren in Rens)
		{
			ren.material.SetTextureOffset("_MainTex", newValue);
		}
	}

}
