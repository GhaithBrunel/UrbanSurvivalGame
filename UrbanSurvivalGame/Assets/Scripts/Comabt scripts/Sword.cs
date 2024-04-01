using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
    public int damage = 15;
    public Camera playerCamera;
    private Animator animator;
    private bool isSwinging = false;
    private Collider swordCollider; // Reference to the sword's collider

    // Original transform values for the sword when not in use
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {

      
        animator = GetComponent<Animator>();
        swordCollider = GetComponent<Collider>(); // Get the sword's collider

        // Store the original position and rotation
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        AlignWithCamera();

        if (Input.GetMouseButtonDown(0) && !isSwinging)
        {
            animator.SetTrigger("Attacking");
            isSwinging = true;
            swordCollider.enabled = true; // Enable the collider when starting the swing
            StartCoroutine(ResetSwing());
        }
    }

    private void AlignWithCamera()
    {
        if (playerCamera != null)
        {
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0;
            transform.rotation = Quaternion.LookRotation(cameraForward);
        }
    }

    private IEnumerator ResetSwing()
    {
        yield return new WaitForSeconds(2.0f); // Duration of 'Attacking' animation
        isSwinging = false;
        swordCollider.enabled = false; // Disable the collider after the swing is complete
        animator.ResetTrigger("Attacking");
    }

    void OnTriggerEnter(Collider other)
    {
        // Apply damage only if the sword is swinging and hits an enemy
        if (isSwinging && other.CompareTag("enemy"))
        {
            BoxAI enemyAI = other.GetComponent<BoxAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(damage);
                swordCollider.enabled = false; // Disable the collider immediately after dealing damage
            }
        }
    }

    public void ResetWeapon()
    {
       
        StopAllCoroutines(); // Stop any ongoing actions
        isSwinging = false;
        swordCollider.enabled = false;
        if (animator != null)
        {
            animator.ResetTrigger("Attacking");
       
        }

        // Reset the position and rotation of the sword
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
}










