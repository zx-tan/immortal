using System.Collections;
using UnityEngine;

namespace VFX
{
	/// <summary>
	/// This script handles flying a projectile out. You will need to spawn it at the starting point and load the "targetPosition" variable with the position you want it to land
	/// </summary>
	public class FX_ProjectileTravel : MonoBehaviour
    {

        [SerializeField]
        GameObject projectileArt;

        [SerializeField]
        float effectDuration = 1f;
        float normalizedTime, actualEffectDuration;

        [SerializeField]
        AnimationCurve projectileHeightCurve;
        [SerializeField]
        float heightMultiplier = 2f;

        public bool faceTravelDir;

        // Load this with wherever the object will be landing
        public Vector3 targetPostion;

        public GameObject impactFX;

        Vector3 tempPosition, facingDir, startPosition;
        IEnumerator currentProjectileRoutine;

        private void OnEnable()
        {
            if (currentProjectileRoutine != null)
                StopCoroutine(currentProjectileRoutine);
            currentProjectileRoutine = ProjectileRoutine();
            StartCoroutine(currentProjectileRoutine);

        }

        IEnumerator ProjectileRoutine()
        {
            float timer = 0;
            tempPosition = transform.position;
            startPosition = transform.position;

            actualEffectDuration = effectDuration + (Vector3.Distance(startPosition, targetPostion) * 0.01f); // This allows further distance shots to take longer amount of time, magic number to help scope it into a reasonable timeframe

            while (timer <= actualEffectDuration)
            {
                normalizedTime = timer / actualEffectDuration;
                //Debug.Log("Norm time: " + normalizedTime);

                // Movement
                tempPosition = transform.position;
                tempPosition = Vector3.Lerp(startPosition, targetPostion, normalizedTime);
                tempPosition.y += projectileHeightCurve.Evaluate(normalizedTime) * heightMultiplier;

                transform.position = tempPosition;

                //Face arrow along path
                if (faceTravelDir)
                {
                    tempPosition = transform.position;
                    tempPosition = Vector3.Lerp(startPosition, targetPostion, normalizedTime + 0.1f);
                    tempPosition.y += projectileHeightCurve.Evaluate(normalizedTime + 0.1f) * heightMultiplier;

                    facingDir = (tempPosition - transform.position).normalized;
                    transform.forward = facingDir;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            // Ensure we hit the end position
            transform.position = targetPostion;

            if (impactFX != null)
            {
                Instantiate(impactFX, transform.position, Quaternion.identity);
            }
            projectileArt.SetActive(false);
            // Need to wait a short moment for the trail renders to close
            yield return new WaitForSeconds(0.1f);

            // Impact!
            Destroy(this.gameObject);

            yield return null;
        }
    }
}