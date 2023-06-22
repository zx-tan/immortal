using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX
{
    public class FX_ShotgunPellet : MonoBehaviour
    {
        public GameObject onDisableFX;
        public float maxLifetime;
        public float minLifetime;
        float lifetime;

        public float travelSpeed = 4f;
        public float spread = 15f;

        IEnumerator currentRoutine;

        private void OnEnable()
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);
            currentRoutine = PelletRoutine();
            StartCoroutine(currentRoutine);
        }

        private void OnDisable()
        {
            if(onDisableFX != null)
                Instantiate(onDisableFX, transform.position, Quaternion.identity);
        }

        IEnumerator PelletRoutine()
        {
            float currentLife = 0;

            lifetime = Random.Range(minLifetime, maxLifetime);

            Vector3 startFacing = transform.localEulerAngles;
            startFacing.y += Random.Range(-spread, spread);

            transform.localEulerAngles = startFacing;

            while(currentLife <= lifetime)
            {
                transform.position += transform.forward * travelSpeed * Time.deltaTime;

                currentLife += Time.deltaTime;
                yield return null;
            }

            this.gameObject.SetActive(false);
            Destroy(this.gameObject, 1);
        }
    }
}