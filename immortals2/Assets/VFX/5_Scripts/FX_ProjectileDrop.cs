using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX
{
    public class FX_ProjectileDrop : MonoBehaviour
    {
        public float groundHeight = 0;
        public GameObject impactFX;
        public float travelSpeed = 5f;
        public ParticleSystem ps;

        IEnumerator currentRoutine;
        private void OnEnable()
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);
            currentRoutine = DropRoutine();
            StartCoroutine(currentRoutine);

        }

        IEnumerator DropRoutine()
        {
            Vector3 targetPos = transform.position;
            targetPos.y = groundHeight;
            bool falling = true;

            while (falling)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * travelSpeed);

                if(Vector3.Distance(transform.position, targetPos) <= 0.05f)
                {
                    falling = false;
                }

                yield return null;
            }

            ps.Stop(true);
            Instantiate(impactFX, transform.position, Quaternion.identity);

            yield return new WaitForSeconds (2f);

            Destroy(this.gameObject);

            yield return null;
        }
    }
}