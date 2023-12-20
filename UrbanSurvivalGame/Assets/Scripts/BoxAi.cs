using UnityEngine;

public class BoxAI : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    public float stopDistance = 1f;
   [SerializeField] private float runAwayTimer;
    private bool isRunningAway = false;
    private Vector3 runAwayDirection;
    private Rigidbody rb;


    
    public Transform[] waypoints;
    [SerializeField] public float chaseRadius = 10f;
    [SerializeField] public float stopChaseTime = 10f;

    private Transform currentWaypoint;
    [SerializeField] private float chaseTimer;
    private bool isChasing;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SelectNewWaypoint();
    }

    void Update()
    {
        if (isRunningAway)
        {
            RunAway();
            if (runAwayTimer <= 0)
            {
                isRunningAway = false;
                chaseTimer = stopChaseTime; // Reset chase timer to immediately resume chase
            }
        }
        else if (Vector3.Distance(transform.position, player.position) <= chaseRadius)
        {
            ChasePlayer();
        }
        else
        {
            Wander();
        }

        CheckPlayerDistance();
    }


    void FollowPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero && distance > stopDistance)
        {
            rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
    }

    void RunAway()
{
    if (runAwayTimer > 0)
    {
        rb.MovePosition(rb.position + runAwayDirection * speed * Time.deltaTime);
        runAwayTimer -= Time.deltaTime;
        // Update wolf's rotation to face the run away direction
        transform.rotation = Quaternion.LookRotation(runAwayDirection);
    }
    else
    {
        isRunningAway = false;
    }
}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isRunningAway)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(20);
                isRunningAway = true;
                runAwayTimer = 3f; // Run away for 3 seconds after biting
                runAwayDirection = Vector3.Normalize(transform.position - player.position);
                runAwayDirection.y = 0;
            }
        }
    }


    void Wander()
    {
        if (Vector3.Distance(transform.position, currentWaypoint.position) < 1f)
        {
            SelectNewWaypoint();
        }
        else
        {
            MoveTowards(currentWaypoint.position);
        }
    }

    void ChasePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= chaseRadius)
        {
            chaseTimer = 0;
            MoveTowards(player.position);
        }
        else
        {
            chaseTimer += Time.deltaTime;
            if (chaseTimer >= stopChaseTime)
            {
                isChasing = false;
                SelectNewWaypoint();
            }
        }


        if (chaseTimer >= stopChaseTime)
        {
            isChasing = false;
            isRunningAway = false; // Reset running away flag
            SelectNewWaypoint();
        }
    }

    void CheckPlayerDistance()
    {
        if (Vector3.Distance(transform.position, player.position) <= chaseRadius && !isRunningAway)
        {
            isChasing = true;
        }
    }

    void SelectNewWaypoint()
    {
        if (waypoints.Length == 0) return;
        currentWaypoint = waypoints[Random.Range(0, waypoints.Length)];
        MoveTowards(currentWaypoint.position);
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
    }



}







