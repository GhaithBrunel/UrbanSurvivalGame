using UnityEngine;

public class RoadAgent : MonoBehaviour
{
    public GameObject roadPrefab;
    public float moveSpeed = 5f;
    public Vector3 direction = Vector3.forward;
    private bool isOnRoad = false;

    private void Update()
    {
        Move();
        CheckIfOnRoad();
    }

    void Move()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    void LayRoad()
    {
        Instantiate(roadPrefab, transform.position, Quaternion.identity);
    }

    void CheckIfOnRoad()
    {
        RaycastHit hit;
        Vector3 raycastStartPosition = transform.position + Vector3.up * 0.5f; // Adjust the height as needed

        Debug.DrawRay(raycastStartPosition, -Vector3.up * 2, Color.red); // Debugging raycast

        if (Physics.Raycast(raycastStartPosition, -Vector3.up, out hit, 2))
        {
            if (hit.collider.CompareTag("Road"))
            {
                isOnRoad = true;
            }
            else
            {
                if (isOnRoad)
                {
                    isOnRoad = false;
                    LayRoad(); // Lay road when we exit the road
                }
            }
        }
        else if (isOnRoad)
        {
            isOnRoad = false;
            LayRoad(); // Lay road if not hitting anything and was previously on road
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Road"))
        {
            direction = Vector3.Reflect(direction, collision.contacts[0].normal);
        }
    }
}




