using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform playerCamera;

    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float Speed = 6.0f;

    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] float gravity = -30f;
    [SerializeField] Transform groundCheck;

    [SerializeField] LayerMask ground;

    /// <summary> Stamina implementation 
    [SerializeField] private float maxSprintStamina = 100f;
    [SerializeField] float sprintDrainRate = 10f; // Stamina drained per second while sprinting
    [SerializeField] float sprintRecoveryRate = 5f; // Stamina recovered per second when not sprinting
    private float currentSprintStamina;

    /// </summary>
    /// 


    /// fall damage health 
    private float fallVelocityThreshold = -1f; // Velocity threshold for taking fall damage
    private float lastYVelocity; // Track the last Y velocity
    ///

    public float jumpHeight = 6f;
    float velocityY;
    bool isGrounded;

    float cameraCap;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;

    CharacterController controller;
    Vector2 currentDir;
    Vector2 currentDirVelocity;
    Vector3 velocity;

    /// <summary> Hunger

    [SerializeField] private TextMeshProUGUI hungerText; // Reference to the TextMeshProUGUI element

    [SerializeField] private float maxHunger = 100f;
    private float currentHunger;
    [SerializeField] private float hungerDrainRate = 1f; // The rate at which hunger decreases
    [SerializeField] private float healthDamageRate = 5f; // Health damage per second when hunger is zero
    private bool canRun = true;
    /// </summary>
    void Start()
    {
        currentHunger = maxHunger; // Initialize hunger

        currentSprintStamina = maxSprintStamina;
        controller = GetComponent<CharacterController>();

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }

    void Update() //update per frame 
    {
        UpdateMouse();
        UpdateMove();
        UpdateHunger(); // Hunger


        ///Health fall damage
        if (isGrounded && lastYVelocity < fallVelocityThreshold)
        {
            OnLanding(lastYVelocity);
        }

        lastYVelocity = controller.velocity.y; // Update last Y velocity


    }

    void UpdateMouse()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraCap -= currentMouseDelta.y * mouseSensitivity;

        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraCap;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMove()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        float adjustedSpeed = Speed;
        bool isWalkingSlowly = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool isTryingToSprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool hasStamina = currentSprintStamina > 0;

        if (canRun && isTryingToSprint && hasStamina)
        {
            adjustedSpeed *= 2f; // Increase speed for sprinting
            currentSprintStamina -= sprintDrainRate * Time.deltaTime;
        }
        else
        {
            if (isWalkingSlowly)
            {
                adjustedSpeed *= 0.5f; // Halve the speed when Ctrl is pressed
            }
            // Reset speed to normal if trying to sprint without stamina
            else if (isTryingToSprint && !hasStamina)
            {
                adjustedSpeed = Speed;
            }
            currentSprintStamina += sprintRecoveryRate * Time.deltaTime; // Recover sprint stamina
        }
        currentSprintStamina = Mathf.Clamp(currentSprintStamina, 0, maxSprintStamina);

        velocityY += gravity * 2f * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * adjustedSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (!isGrounded && controller.velocity.y < -1f)
        {
            velocityY = -8f;
        }

        if (isGrounded && lastYVelocity < fallVelocityThreshold)
        {
            OnLanding(lastYVelocity);
        }
    }



    public float GetCurrentSprintStamina()
    {
        return currentSprintStamina;
    }

    public float GetMaxSprintStamina()
    {
        return maxSprintStamina;
    }


    private void OnLanding(float velocityY)
    {
        float fallDistance = Mathf.Abs(velocityY);
        // Call a method on PlayerHealth to apply damage
        GetComponent<PlayerHealth>().TakeFallDamage(fallDistance);
    }




    private void UpdateHunger()
    {
        if (currentHunger > 0)
        {
            currentHunger -= hungerDrainRate * Time.deltaTime;

            // Check if hunger is at 50%
            if (currentHunger <= maxHunger * 0.5f && currentHunger > maxHunger * 0.5f - hungerDrainRate * Time.deltaTime)
            {
                hungerText.text = "Hunger is at 50%!"; // Display the message
            }
        }
        else
        {
            // Apply health damage when hunger is zero
            GetComponent<PlayerHealth>().TakeDamage(healthDamageRate * Time.deltaTime);

            // Disable running
            // You can do this by setting a flag that is checked in your UpdateMove method
            canRun = false;
        }
    }

    public float GetCurrentHunger()
    {
        return currentHunger;
    }

    public float GetMaxHunger()
    {
        return maxHunger;
    }




}

