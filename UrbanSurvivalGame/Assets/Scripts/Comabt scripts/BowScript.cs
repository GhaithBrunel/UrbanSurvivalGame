using UnityEngine;

public class BowScript : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint; 
    public Camera playerCamera;
    public float arrowSpeed = 30f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            ShootArrow();
        }
    }

    void ShootArrow() // shoots the arrow 
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, playerCamera.transform.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = playerCamera.transform.forward * arrowSpeed;
    }
}

