using UnityEngine;

namespace Tenko
{
    public class CameraFollowController : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private float followSpeed = 2f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

        [Header("Smoothing")]
        [SerializeField] private bool useSmoothDamp = true;
        [SerializeField] private float smoothTime = 0.1f;
        private Vector3 velocity = Vector3.zero;

        [Header("Bounds (Optional)")]
        [SerializeField] private bool useBounds = false;
        [SerializeField] private Vector2 minBounds = new Vector2(-10, -10);
        [SerializeField] private Vector2 maxBounds = new Vector2(10, 10);

        [Header("Dead Zone")]
        [SerializeField] private bool useDeadZone = false;
        [SerializeField] private float deadZoneRadius = 1f;
        private Vector3 lastTargetPosition;

        private Camera cam;
        private Vector3 targetPosition;

        private void Start()
        {
            cam = GetComponent<Camera>();

            if (target == null)
            {
                var player = PlayerLocomotionManager.Instance;
                if (player != null)
                {
                    target = player.transform;
                }
                else
                {
                    Debug.LogWarning("CameraFollowController: No target set and PlayerLocomotionManager.Instance not found!");
                }
            }

            if (target != null)
            {
                lastTargetPosition = target.position;
                transform.position = target.position + offset;
                if (useBounds)
                {
                    ApplyBounds();
                }
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            Vector3 desiredPosition = target.position + offset;

            if (useDeadZone)
            {
                float distanceFromLastPosition = Vector3.Distance(target.position, lastTargetPosition);
                if (distanceFromLastPosition < deadZoneRadius)
                {
                    return;
                }
                lastTargetPosition = target.position;
            }

            if (useSmoothDamp)
            {
                targetPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
            }
            else
            {
                targetPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            }

            // maybe use later when we have walls and shit
            if (useBounds)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
            }

            transform.position = targetPosition;
        }

        private void ApplyBounds()
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
            pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
            transform.position = pos;
        }

        // we will use this to focus camera on enemy
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                lastTargetPosition = target.position;
            }
        }

        public void SetFollowSpeed(float speed)
        {
            followSpeed = speed;
        }

        public void SetSmoothTime(float time)
        {
            smoothTime = time;
        }

        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }

        public void SetBounds(Vector2 min, Vector2 max)
        {
            minBounds = min;
            maxBounds = max;
            useBounds = true;
        }

        public void DisableBounds()
        {
            useBounds = false;
        }

        public void SnapToTarget()
        {
            if (target == null) return;

            transform.position = target.position + offset;
            if (useBounds)
            {
                ApplyBounds();
            }
            velocity = Vector3.zero;
        }

        private void OnDrawGizmosSelected()
        {
            if (useBounds)
            {
                Gizmos.color = Color.yellow;
                Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, 0);
                Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0);
                Gizmos.DrawWireCube(center, size);
            }

            if (useDeadZone && target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(target.position, deadZoneRadius);
            }

            if (target != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, target.position + offset);
            }
        }
    }
}
