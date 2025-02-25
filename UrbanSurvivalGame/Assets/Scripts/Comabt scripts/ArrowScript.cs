using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasHit = false;
    public int damage = 20; // Damage the arrow will deal

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        hasHit = true;
        rb.isKinematic = true; // Stops the arrow

        // Check if the arrow hits an enemy
        if (collision.collider.CompareTag("enemy"))
        {
            // Calls a method to apply damage to the enemy
            ApplyDamage(collision.gameObject);
        }

        // Adjust the position and rotation to make it look like it's stuck to the surface adds realism
        transform.position = collision.contacts[0].point;
        transform.rotation = Quaternion.LookRotation(collision.contacts[0].normal);

        // Attach the arrow to the hit object
        transform.parent = collision.transform;
    }

    void ApplyDamage(GameObject enemy)
    {
       
        BoxAI enemyHealth = enemy.GetComponent<BoxAI>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
    }
}


