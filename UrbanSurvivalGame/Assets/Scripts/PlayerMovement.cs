using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 6.0f;
    [SerializeField] private float jumpHeight = 6f;
    [SerializeField] private float gravity = -30f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;
    [SerializeField][Range(0.0f, 0.5f)] private float moveSmoothTime = 0.3f;

    private Vector3 frozenPosition;
    private Quaternion frozenRotation;
    private bool isFrozen = false;

    [Header("Mouse Look Settings")]
    [SerializeField] private Transform playerCamera;
    [SerializeField][Range(0.0f, 0.5f)] private float mouseSmoothTime = 0.03f;
    [SerializeField] private float mouseSensitivity = 3.5f;
    [SerializeField] private bool cursorLock = true;

    [Header("Stamina Settings")]
    [SerializeField] private float maxSprintStamina = 100f;
    [SerializeField] private float sprintDrainRate = 10f;
    [SerializeField] private float sprintRecoveryRate = 5f;
    private float currentSprintStamina;

    [Header("Hunger Settings")]
    [SerializeField] private TextMeshProUGUI hungerText;
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float hungerDrainRate = 1f;
    [SerializeField] private float healthDamageRate = 5f;
    private float currentHunger;

    private CharacterController controller;
    private Vector2 currentDir;
    private Vector2 currentDirVelocity;
    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;
    private float cameraCap;
    private float velocityY;
    private bool isGrounded;
    private bool canRun = true;
    private bool canMove = true;
    private bool cameraCanMove = true;

    private void Start()
    {
        InitializePlayer();
    }

    private void Update()
    {
        if (isFrozen)
        {
            transform.position = frozenPosition;
            transform.rotation = frozenRotation;
            return;
        }

        if (!canMove) return;

        UpdateMouseLook();
        UpdateMovement();
        UpdateHunger();
    }


    private void InitializePlayer()
    {
        controller = GetComponent<CharacterController>();
        currentHunger = maxHunger;
        currentSprintStamina = maxSprintStamina;
        LockCursor(cursorLock);
    }

    private void UpdateMouseLook()
    {
        if (!cameraCanMove) return;

        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraCap -= currentMouseDelta.y * mouseSensitivity;
        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);
        playerCamera.localEulerAngles = Vector3.right * cameraCap;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }
  
    private void UpdateMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        float adjustedSpeed = CalculateSpeed();
        velocityY += gravity * 2f * Time.deltaTime;
        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * adjustedSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump"))
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);

        if (!isGrounded && controller.velocity.y < -1f)
            velocityY = -8f;
    }

    private float CalculateSpeed()
    {
        float currentSpeed = speed;
        if (canRun && IsSprinting() && currentSprintStamina > 0)
        {
            currentSpeed *= 2f;
            currentSprintStamina -= sprintDrainRate * Time.deltaTime;
        }
        else
        {
            if (IsWalkingSlowly())
                currentSpeed *= 0.5f;

            currentSprintStamina += sprintRecoveryRate * Time.deltaTime;
        }
        currentSprintStamina = Mathf.Clamp(currentSprintStamina, 0, maxSprintStamina);
        return currentSpeed;
    }

    private bool IsSprinting()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private bool IsWalkingSlowly()
    {
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }

    private void UpdateHunger()
    {
        currentHunger -= hungerDrainRate * Time.deltaTime;
        if (currentHunger <= 0)
        {
            GetComponent<PlayerHealth>().TakeDamage(healthDamageRate * Time.deltaTime);
            canRun = false;
            currentHunger = 0;
        }

        if (currentHunger <= maxHunger * 0.5f)
            hungerText.text = "Hunger is at 50%!";
    }

    public void SetPlayerMovement(bool enable)
    {
        canMove = enable;
        cameraCanMove = enable;

        if (!enable)
        {
            // Freeze player
            FreezePlayer();
        }
        else if (isFrozen)
        {
            // Unfreeze player
            UnfreezePlayer();
        }
    }

    private void FreezePlayer()
    {
        frozenPosition = transform.position;
        frozenRotation = transform.rotation;
        isFrozen = true;
    }

    private void UnfreezePlayer()
    {
        isFrozen = false;
    }


    public void SetCameraMovement(bool enable)
    {
        cameraCanMove = enable;
    }

    public void SetCursorState(bool isInventoryOpen)
    {
        LockCursor(!isInventoryOpen);
    }

    private void LockCursor(bool shouldLock)
    {
        Cursor.lockState = shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !shouldLock;
    }

    // Public methods for accessing current stamina and hunger...
    public float GetCurrentSprintStamina()
    {
        return currentSprintStamina;
    }

    public float GetMaxSprintStamina()
    {
        return maxSprintStamina;
    }

    public float GetCurrentHunger()
    {
        return currentHunger;
    }

    public float GetMaxHunger()
    {
        return maxHunger;
    }

    public void ResetHungerAndStamina()
    {
        currentHunger = maxHunger;
        currentSprintStamina = maxSprintStamina;
        canRun = true;
    }
}



