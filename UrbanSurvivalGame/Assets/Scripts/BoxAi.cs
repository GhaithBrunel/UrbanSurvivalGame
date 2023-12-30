using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class BoxAI : MonoBehaviour
{
    public Transform player;
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackMoveRadius = 5f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float waypointDistance = 20f;
    [SerializeField] private float chaseRadius = 10f;

    private float lastAttackTime;
    private bool isAttacking = false;

    private NavMeshAgent agent;

    public int maxHealth = 100;
    public Slider healthBar; // Assign this in the inspector
    private int currentHealth;

    public GameObject itemPrefab; // Reference to the item prefab, assign this in the inspector
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        agent.stoppingDistance = attackDistance;
        lastAttackTime = -attackCooldown;
        SetNewDestination();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRadius)
        {
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            Patrol();
        }

        if (isAttacking && distanceToPlayer > attackDistance)
        {
            isAttacking = false;
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            SetNewDestination();
        }
    }

    void ChasePlayer(float distanceToPlayer)
    {
        agent.speed = chaseSpeed;

        if (distanceToPlayer <= attackDistance && Time.time - lastAttackTime > attackCooldown)
        {
            AttackPlayer();
        }
        else if (!isAttacking)
        {
            agent.SetDestination(player.position);
        }
    }

    void AttackPlayer()
    {
        FacePlayer();
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(20);
            lastAttackTime = Time.time;
            isAttacking = true;
            MoveRandomlyAroundPlayer();
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void MoveRandomlyAroundPlayer()
    {
        Vector3 randomDirection = Random.insideUnitSphere * attackMoveRadius;
        randomDirection += player.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, attackMoveRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void SetNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * waypointDistance;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, waypointDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }



    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
        UpdateHealthUI();
    }


    private void Die()
    {
        Debug.Log("Enemy Died");

        // Disable the NavMeshAgent and other components that should not be active after death
        if (agent != null) agent.enabled = false;

        // Determine the number of items to drop (between 1 and 3)
        int itemsToDrop = Random.Range(1, 4); // Random.Range is inclusive for the first parameter and exclusive for the second

        // Spawn the items at the wolf's position
        for (int i = 0; i < itemsToDrop; i++)
        {
            if (itemPrefab != null)
            {
                Instantiate(itemPrefab, transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject);

        // You can also add a delay or invoke other methods to handle the enemy's death
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }

}












