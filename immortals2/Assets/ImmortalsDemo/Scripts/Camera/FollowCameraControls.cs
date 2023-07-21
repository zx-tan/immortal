using UnityEngine;

public class FollowCameraControls : MonoBehaviour
{
    private Camera targetCamera;
    private FollowCamera targetFollowCamera;

    [SerializeField]
    private float startXRotation;
    [SerializeField]
    private float startYRotation;
    [SerializeField]
    private float startZoomDistance;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float minXRotation;
    [SerializeField]
    private float maxXRotation;
    [SerializeField]
    private bool updateRotation = true;
    [SerializeField]
    private bool updateZoom = true;
    [SerializeField]
    private float minZoomDistance;
    [SerializeField]
    private float maxZoomDistance;

    [Range(0, 65)]
    public float damping;

    void Awake()
    {
        targetFollowCamera.xRotation = startXRotation;
        targetFollowCamera.yRotation = startYRotation;
        targetFollowCamera.zoomDistance = startZoomDistance;
        targetFollowCamera.damping = damping;
    }

    void LateUpdate()
    {
        targetFollowCamera.target = target;
        targetFollowCamera.targetOffset = targetOffset;

        if (updateRotation)
        {
            var mX = InputManager.GetAxis("Mouse X", false);
            var mY = InputManager.GetAxis("Mouse Y", false);
            UpdateFollowCameraRotation(mX, mY);
        }

        if (updateZoom)
        {
            var mZ = InputManager.GetAxis("Mouse ScrollWheel", false);
            UpdateFollowCameraZoom(mZ);
        }
    }

    private void UpdateFollowCameraRotation(float x, float y)
    {
        targetFollowCamera.xRotation -= y * rotationSpeed;
        targetFollowCamera.xRotation = Mathf.Clamp(targetFollowCamera.xRotation, minXRotation, maxXRotation);
        targetFollowCamera.yRotation += x * rotationSpeed;
    }

    private void UpdateFollowCameraZoom(float z)
    {
        targetFollowCamera.zoomDistance += mZ * zoomSpeed;
        targetFollowCamera.zoomDistance = Mathf.Clamp(targetFollowCamera.zoomDistance, minZoomDistance, maxZoomDistance);
    }
}
