using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This simple script is used to clean up VFX prefabs and the like.    TODO: Convert this to use a pooling system instead of just destroying.
/// </summary>

namespace VFX
{
    public class DestroyAfterDelay : MonoBehaviour
    {
        public float delayTime = 1;
        // Start is called before the first frame update
        void Start()
        {

            Destroy(this.gameObject, delayTime);

        }

    }
}