using UnityEngine;

public class FollowCamera : MonoBehaviour, IFollow
{
    [SerializeField]
    private Transform target;
    public Transform Target { get { return target; } }
}
