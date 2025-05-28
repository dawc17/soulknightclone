using UnityEngine;
using UnityEngine.InputSystem;

namespace Tenko
{
    public class PlayerLocomotionManager : MonoBehaviour
    {
        public static PlayerLocomotionManager Instance; [SerializeField] private Rigidbody2D rb;

        [Header("Movement Input")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Vector2 moveInput = Vector2.zero;

        public Vector2 moveDir => moveInput; // Public property to access moveInput
        public float LastHorizontalVector { get; private set; } = 1f; // Default facing right

        PlayerControls playerControls;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("PlayerLocomotionManager requires a Rigidbody2D component!");
                return;
            }

            playerControls = new PlayerControls();
            playerControls.PlayerInput.Movement.performed += OnMovementPerformed;
            playerControls.PlayerInput.Movement.canceled += OnMovementCanceled;
            playerControls.Enable();
        }

        private void OnMovementPerformed(InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>();

            // Update LastHorizontalVector when horizontal input changes
            if (moveInput.x != 0)
            {
                LastHorizontalVector = moveInput.x;
            }
        }

        private void OnMovementCanceled(InputAction.CallbackContext ctx)
        {
            moveInput = Vector2.zero;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (enabled && playerControls != null)
            {
                if (hasFocus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void FixedUpdate()
        {
            HandleMovementInput();
        }
        public void HandleMovementInput()
        {
            if (rb != null)
            {
                Vector2 targetVelocity = moveInput * moveSpeed;
                rb.linearVelocity = targetVelocity;
            }
        }

        private void OnDestroy()
        {
            if (playerControls != null)
            {
                playerControls.PlayerInput.Movement.performed -= OnMovementPerformed;
                playerControls.PlayerInput.Movement.canceled -= OnMovementCanceled;
                playerControls.Disable();
                playerControls.Dispose();
            }
        }
    }
}