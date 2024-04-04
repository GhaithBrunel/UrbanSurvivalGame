using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
    public int damage = 15;
    public Camera playerCamera;
    private Animator animator;
    private bool isSwinging = false;
    private Collider swordCollider; 

    // orginal  transform values for the sword when not in use
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {

      
        animator = GetComponent<Animator>();
        swordCollider = GetComponent<Collider>(); // Gets teh swords collider 

        // Stores the orginal pos
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
            swordCollider.enabled = true; // enables the collider when swinging 
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
        yield return new WaitForSeconds(2.0f); // Duration of the attacking animation of 'Attacking' animation
        isSwinging = false;
        swordCollider.enabled = false; 
        animator.ResetTrigger("Attacking");
    }

    void OnTriggerEnter(Collider other)
    {
        // damages wolf only when the is swinging is true and there's an enemey 
        if (isSwinging && other.CompareTag("enemy"))
        {
            BoxAI enemyAI = other.GetComponent<BoxAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(damage);
                swordCollider.enabled = false; // Disable the collider  after hitting the wolf so that it doenst spam the wolf with damage 
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










