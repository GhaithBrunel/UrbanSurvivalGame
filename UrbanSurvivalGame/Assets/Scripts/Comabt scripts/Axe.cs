using UnityEngine;
using System.Collections;

public class Axe : MonoBehaviour
{
    public int damage = 15; 
    public Camera playerCamera;
    private Animator animator;
    private bool isSwinging = false;
    private Collider axeCollider; 

    private Vector3 originalPosition;
    private Quaternion originalRotation;


    void Start()
    {
        animator = GetComponent<Animator>();
        axeCollider = GetComponent<Collider>(); // find the axe's collider

        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        AlignWithCamera();

        if (Input.GetMouseButtonDown(0) && !isSwinging) 
        {
            animator.SetTrigger("Attacking"); // uses the animation i made for the axe 
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
        yield return new WaitForSeconds(1.0f); //  Duration of the attacking animation of 'Attacking' animation
        isSwinging = false;
        axeCollider.enabled = false; // Disable the collider after the swing is complete
        animator.ResetTrigger("Attacking");
    }

    void OnTriggerEnter(Collider other)
    {
        // Apply damage only if the axe is swinging and hits an enemy
        if (isSwinging && other.CompareTag("enemy")) // enemy tag ie the wolf
        {
            BoxAI enemyAI = other.GetComponent<BoxAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(damage);
                axeCollider.enabled = false; 
            }
        }
    }


    public void ResetWeapon()
    {
        StopAllCoroutines();
        isSwinging = false;
        axeCollider.enabled = false;
        if (animator != null)
        {
            animator.ResetTrigger("Attacking");
            
        }
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }



}
