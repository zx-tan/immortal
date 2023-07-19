using UnityEngine;

public class FollowCameraControls : MonoBehaviour
{
    private Camera targetCamera;
    private FollowCamera targetFollowCamera;

    void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        targetFollowCamera = targetCamera.gameObject.GetComponent<FollowCamera>();
        if (targetFollowCamera == null)
            targetFollowCamera = targetCamera.gameObject.AddComponent<FollowCamera>();
    }

    void LateUpdate()
    {
        targetFollowCamera.target = target;
        targetFollowCamera.targetOffset = targetOffset;
    }
}
