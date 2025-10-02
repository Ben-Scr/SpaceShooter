using UnityEngine;
using UnityEngine.UI;

namespace SpaceShooter
{
    public class CameraController : MonoBehaviour
    {
        public enum CamMode { None, Follow, Manual }

        [Header("General")]
        public CamMode camMode;

        [Space(5)]
        [Header("Follow Properties")]
        [SerializeField] private Transform followTarget;
        [SerializeField] private float smoothTime;
        public float followOrthoSize = 11;
        [SerializeField] private float followOrthoSizeLerpSpeed = 0.1f;
        private Vector3 velocity;

        [Space(5)]
        [Header("Manual Properties")]
        [SerializeField] private float controlSpeed;
        [SerializeField] private float zoomSpeed;
        [SerializeField] private Range zoomRange;

        private float lastManualOrthoSize;
        private Vector3 lastManualPosition;
        private Vector2 startMousePos;

        private Camera camera;
        public static CameraController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            camera = Camera.main;
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
                    camera.orthographicSize = lastManualOrthoSize;
                    transform.position = lastManualPosition == Vector3.zero ? transform.position : lastManualPosition;
                }
            }

            if (camMode == CamMode.None)
                return;

            else if (camMode == CamMode.Manual)
                ManualMode();
        }

        private void LateUpdate()
        {
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


            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref velocity,
                smoothTime
            );

            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, followOrthoSize, Time.deltaTime * followOrthoSizeLerpSpeed);
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

            float targetOrthoSize = camera.orthographicSize - (Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 100 * zoomSpeed);
            camera.orthographicSize = Mathf.Clamp(targetOrthoSize, zoomRange.Min, zoomRange.Max);
            lastManualOrthoSize = camera.orthographicSize;
            lastManualPosition = transform.position;
        }
    }
}

