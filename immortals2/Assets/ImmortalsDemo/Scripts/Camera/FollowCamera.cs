using UnityEngine;

public class FollowCamera : MonoBehaviour, IFollow
{
    [SerializeField]
    private Transform target;
    public Transform Target { get { return target; } }

    private float xRotation;
    private float yRotation;
    private bool useTargetYRotation;
    private Vector3 targetOffset;
    private float zoomDistance = 10.0f;

    //smoothing
    [Range(0, 65)]
    private float damping = 2.0f;

    private void Update()
    {
        // Early out if we don't have a target
        if (!Target) return;

        // implement follow
        var wantedPosition = Target.position + targetOffset;
        var wantedYRotation = useTargetYRotation ? Target.eulerAngles.y : yRotation;

        // Convert the angle into a rotation
        var currentRotation = Quaternion.Euler(xRotation, wantedYRotation, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        wantedPosition -= currentRotation * Vector3.forward * zoomDistance;

        // Update position
        transform.position = Vector3.Lerp(transform.position, wantedPosition, damping * Time.deltaTime);

        // Always look at the target
        transform.LookAt(target.transform.position + targetOffset);
    }
}
