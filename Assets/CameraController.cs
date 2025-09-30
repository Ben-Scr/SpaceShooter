using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public enum CamMode { None, Follow, Manual }

    [SerializeField] private CamMode camMode;

    [Space(5)]
    [Header("Follow Properties")]
    public Transform followTarget;
    [SerializeField] private float smoothTime;
    [SerializeField] private float followOrthoSize = 11;
    [SerializeField] private float followOrthoSizeLerpSpeed = 0.1f;
    private Vector3 velocity;

    [Space(5)]
    [Header("Manual Properties")]

    [SerializeField] private float controlSpeed;
    [SerializeField] private float zoomSpeed;

    private float lastManualOrthoSize;
    private Vector3 lastManualPosition;

    internal static Camera cam;

    private Vector2 startMousePos;

    private void Awake()
    {
        cam = Camera.main;
        startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastManualOrthoSize = followOrthoSize;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            camMode = camMode == CamMode.Follow ? CamMode.Manual : CamMode.Follow;
            if (camMode == CamMode.Manual)
            {
                cam.orthographicSize = lastManualOrthoSize;
                transform.position = lastManualPosition == Vector3.zero ? transform.position : lastManualPosition;
                //GameController.Freeze(FreezeReason.ManualCamera);
            }
            else
            {
              //  GameController.Unfreeze(FreezeReason.ManualCamera);
            }
        }

        if (camMode == CamMode.None)
            return;

        else if (camMode == CamMode.Manual)
            ManualMode();
    }

    private void LateUpdate()
    {
    //    if (GameController.IsFrozen) return;

        if (camMode == CamMode.None)
            return;

        else if (camMode == CamMode.Follow)
            FollowTarget();
    }

    private void FollowTarget()
    {
        Vector3 targetPos = new Vector3(
            followTarget.position.x,
            followTarget.position.y,
            transform.position.z
        );

        // KEIN fixedDeltaTime hier multiplizieren!
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, followOrthoSize, Time.deltaTime * followOrthoSizeLerpSpeed);
    }

    private void ManualMode()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
            startMousePos = mousePos;

        if (Input.GetMouseButton(0))
        {
            transform.position += (Vector3)(startMousePos - mousePos) * Time.deltaTime * 50 * controlSpeed;
        }

        cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 100 * zoomSpeed;
        lastManualOrthoSize = cam.orthographicSize;
        lastManualPosition = transform.position;
    }
}
