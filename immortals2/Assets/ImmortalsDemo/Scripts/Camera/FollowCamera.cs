using UnityEngine;

public class FollowCamera : MonoBehaviour, IFollow
{
    [SerializeField]
    private Transform target;
    public Transform Target { get { return target; } }

    private void Update()
    {
        // Early out if we don't have a target
        if (!target) return;

        // implement follow
    }
}
