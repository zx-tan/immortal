using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a simple script that will spawn a prefab on the mouse position 
/// This is used to test FX
/// </summary>
namespace VFX
{
    public class ObjectSpawnerOnInput : MonoBehaviour
    {

        public int instanceCount = 1;
        public GameObject instanceToSpawn;
        public bool randomRotationOnY;

        public bool useRandomScale;
        public float minScale = 1, maxScale = 1;

        public Vector3 spawnOffset;

        public bool useObjectPositionInstead;   // Used to spawn things on this object

        GameObject instance;

        Ray ray;
        RaycastHit hit;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (useObjectPositionInstead)
                {
                    //Instantiate
                    SpawnObject(transform.position);
                }
                else
                {
                    //Raycast to the ground
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, 10000f))
                    {

                    }
                    //Instantiate
                    SpawnObject(hit.point);
                }
                
            }
        }

        void SpawnObject(Vector3 pos)
        {
            for (int i = 0; i < instanceCount; i++)
            {
                instance = Instantiate(instanceToSpawn, pos + spawnOffset, Quaternion.identity);
                instance.SetActive(true);   // Ensures the object is turned on when it spawns

                if (randomRotationOnY)
                {
                    Quaternion newRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    instance.transform.rotation = newRot;
                }

                if (useRandomScale)
                {
                    instance.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);
                }
            }
        }

    }
}