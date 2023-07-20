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

    [Range(0, 65)]
    public float damping;

    void Awake()
    {
        //if (targetCamera == null)
        //    targetCamera = Camera.main;

        //targetFollowCamera = targetCamera.gameObject.GetComponent<FollowCamera>();
        //if (targetFollowCamera == null)
        //    targetFollowCamera = targetCamera.gameObject.AddComponent<FollowCamera>();

        targetFollowCamera.xRotation = startXRotation;
        targetFollowCamera.yRotation = startYRotation;
        targetFollowCamera.zoomDistance = startZoomDistance;
        targetFollowCamera.damping = damping;
    }

    void LateUpdate()
    {
        targetFollowCamera.target = target;
        targetFollowCamera.targetOffset = targetOffset;
    }
}
