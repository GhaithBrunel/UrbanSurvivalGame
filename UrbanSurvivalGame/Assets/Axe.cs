using UnityEngine;
using System.Collections;

public class Axe : MonoBehaviour
{
    public int damage = 15; // You can adjust the damage value specific to the axe
    public Camera playerCamera;
    private Animator animator;
    private bool isSwinging = false;
    private Collider axeCollider; // Reference to the axe's collider

    void Start()
    {
        animator = GetComponent<Animator>();
        axeCollider = GetComponent<Collider>(); // Get the axe's collider
    }

    void Update()
    {
        AlignWithCamera();

        if (Input.GetMouseButtonDown(0) && !isSwinging) // Assuming left mouse button for swinging axe
        {
            animator.SetTrigger("Attacking"); // Make sure the trigger name matches the one in your Animator
            isSwinging = true;
            axeCollider.enabled = true; // Enable the collider when starting the swing
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
        yield return new WaitForSeconds(1.0f); // Adjust this to match the axe's attacking animation duration
        isSwinging = false;
        axeCollider.enabled = false; // Disable the collider after the swing is complete
        animator.ResetTrigger("Attacking");
    }

    void OnTriggerEnter(Collider other)
    {
        // Apply damage only if the axe is swinging and hits an enemy
        if (isSwinging && other.CompareTag("enemy")) // Make sure the tag matches your enemy tag
        {
            BoxAI enemyAI = other.GetComponent<BoxAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(damage);
                axeCollider.enabled = false; // Disable the collider immediately after dealing damage
            }
        }
    }
}
