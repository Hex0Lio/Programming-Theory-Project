using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    protected NavMeshAgent agent;

    protected float radius;
    protected bool attacking = false;
    protected Vector3 enemyToPlayerDir;
    protected Vector3 enemyToPlayerDirFlat;
    protected float enemyToPlayerDistFlat;

    public Transform playerObj;
    public Rigidbody playerRb;

    LayerMask blockMask;

    [Header("Basic Variables")]
    public float speed;
    public int damage;
    public int hp;

    [Header("Attack Variables")]
    public float attackDelay;
    public float attackCooldown;
    public float maxAttackDistance;
    public float maxAttackLookAngle;

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        radius = GetComponent<CapsuleCollider>().radius;
        agent.speed = speed;

        blockMask = LayerMask.GetMask("Ground") | LayerMask.GetMask("Wall");
    }

    protected void Update()
    {
        CalculateDirectionAndDistance();

        if (!attacking) Move();
        else {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    protected abstract IEnumerator Attack();

    protected void CalculateDirectionAndDistance()
    {
        enemyToPlayerDir = (playerObj.position - transform.position).normalized;

        Vector3 enemyFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerFlat = new Vector3(playerObj.position.x, 0, playerObj.position.z);
        enemyToPlayerDirFlat = (playerFlat - enemyFlat).normalized;
        enemyToPlayerDistFlat = Vector3.Distance(playerFlat, enemyFlat) - radius - playerObj.GetComponent<CapsuleCollider>().radius;
    }

    protected void Move()
    {
        if (IsWithinAttackDistance() && CanSeeThePlayer()) {
            Stop();

            LookAtPlayer();

            if (IsLookingAtPlayer()) {
                // Attack
                StartCoroutine(Attack());

            }

            return;
        }
        MoveToPlayer();
    } 
    /* Enemy can only attack if:
        - Enemy is within the attack distance
        - Enemy can see the player
        - Enemy is facing the player
    */

    protected bool CanAttack()
    {
        return IsWithinAttackDistance() && CanSeeThePlayer() && IsLookingAtPlayer();
    }

    bool IsWithinAttackDistance()
    {
        return enemyToPlayerDistFlat <= maxAttackDistance;
    }
    bool CanSeeThePlayer()
    {
        return !Physics.SphereCast(new Ray(transform.position, enemyToPlayerDir), 0.5f, enemyToPlayerDistFlat, blockMask);
    }
    bool IsLookingAtPlayer()
    {
        return Vector3.Dot(enemyToPlayerDirFlat, transform.forward) >= Mathf.Cos(Mathf.Deg2Rad * maxAttackLookAngle);
    }

    void LookAtPlayer()
    {
        float time = Vector3.Angle(enemyToPlayerDirFlat, transform.forward) / 90f;
        iTween.LookTo(gameObject, new Vector3(playerObj.position.x, transform.position.y, playerObj.position.z), time);
    }

    void MoveToPlayer()
    {
        agent.isStopped = false;
        agent.destination = playerObj.position;
    }

    void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }
}
