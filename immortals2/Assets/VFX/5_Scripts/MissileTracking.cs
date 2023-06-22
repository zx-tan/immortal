using System;
using System.Collections;
using UnityEngine;

namespace VFX
{

	public class MissileTracking : MonoBehaviour
    {
        IEnumerator currentRoutine;


        // Pass to this variable the location you want the missile to home in towards
        public Vector3 target;
        public float travelSpeed = 5;
        public float turnSpeed = 5;
        public float turnSpeedLifetimeBonus = 2;
        Vector3 desiredFacing;

        float minDistance = 0.25f;
        float lifetime = 0;
        bool traveling = true;

        public GameObject impactFX, missileArtContainer;
        public float destroyDelay = 1f;

		public Action destroyed;

        private void OnEnable()
        {
            // When enabled, start the routine.

            if (currentRoutine != null)
                StopCoroutine(currentRoutine);
            currentRoutine = VFXRoutine();
            StartCoroutine(currentRoutine);

        }

        // Use this as a cheaper update loop
        IEnumerator VFXRoutine()
        {
            traveling = true;
            while(traveling)
            {
                // Move towards target
                //transform.position = Vector3.MoveTowards(transform.position, target, travelSpeed * Time.deltaTime);
                
                // Move forward towards facing!
                desiredFacing = Vector3.Normalize(target - transform.position);
                transform.forward = Vector3.Lerp(transform.forward, desiredFacing, (turnSpeed + lifetime * turnSpeedLifetimeBonus) * Time.deltaTime);

                // Move Forward
                transform.position += transform.forward * travelSpeed * Time.deltaTime;

                // Check if close enough to explode
                if (Vector3.Distance(transform.position, target) <= minDistance + (lifetime * 0.5f))
                {
                    Explode();
                    traveling = false;
                }

                lifetime += Time.deltaTime;
                yield return null;
            }
        }

		void Explode()
        {

            // Spawn the explosion
            if (impactFX != null)
            {
                Instantiate(impactFX, transform.position, Quaternion.identity);
            }

            // Do one last tick moving forward
            transform.position += transform.forward * travelSpeed * Time.deltaTime;

            // Tell all child particle systems to STOP emitting
            foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
            {
                ps.Stop(true);
            }
			if (destroyed!=null)
				destroyed.Invoke();
			// Destroy this object after short delay for particles
			missileArtContainer.SetActive(false);
            Destroy(this.gameObject, destroyDelay);
        }
    }
}