using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviour
{
    public Transform enemyHeadPos;
    Transform playerHeadPos;
    Transform playerPos;
    protected Rigidbody playerRb;

    protected NavMeshAgent agent;
    protected Rigidbody rb;
    
    protected float radius;
    protected float playerRadius = 0.5f;
    protected float attackRadius;

    protected Vector3 enemyToPlayerDir;
    protected Vector3 enemyToPlayerDirFlat;
    protected float enemyToPlayerDist;
    protected float enemyToPlayerDistFlat;

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

    protected enum State
    {
        Moving,
        Idle,
        Attacking
    }
    protected State state;

    protected void Start()
    {
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody>();
        playerPos = GameObject.Find("PlayerPos").transform;
        playerHeadPos = GameObject.Find("PlayerHeadPos").transform;

        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        radius = GetComponent<CapsuleCollider>().radius;
        agent.speed = speed;

        blockMask = LayerMask.GetMask("Ground") | LayerMask.GetMask("Wall");

        state = State.Idle;
    }

    protected void Update()
    {
        CalculateDirectionAndDistance();

        if (state != State.Attacking) {
            UpdateState();
        }
    }

    protected void UpdateState()
    {
        if (IsWithinAttackDistance() && CanSeeThePlayer() && IsLookingAtPlayer()) {
            state = State.Attacking;
            Stop();
            StartCoroutine(Attack());
        } else if (IsWithinAttackDistance() && CanSeeThePlayer()) {
            state = State.Idle;
            Stop();
            LookAtPlayer();
        } else {
            state = State.Moving;
            MoveToPlayer();
        }
    }

    public void TakDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0) Destroy(gameObject);
    }

    protected abstract IEnumerator Attack();

    protected void CalculateDirectionAndDistance()
    {
        enemyToPlayerDir = (playerHeadPos.position - enemyHeadPos.position).normalized;
        enemyToPlayerDist = Vector3.Distance(playerHeadPos.position, enemyHeadPos.position) - radius - playerRadius;

        Vector3 enemyFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerFlat = new Vector3(playerPos.position.x, 0, playerPos.position.z);
        enemyToPlayerDirFlat = (playerFlat - enemyFlat).normalized;
        enemyToPlayerDistFlat = Vector3.Distance(playerFlat, enemyFlat) - radius - playerRadius;
    }

    bool IsWithinAttackDistance()
    {
        return enemyToPlayerDistFlat <= maxAttackDistance;
    }
    bool CanSeeThePlayer()
    {
        return !Physics.SphereCast(new Ray(enemyHeadPos.position, enemyToPlayerDir), attackRadius, enemyToPlayerDist, blockMask);
    }
    bool IsLookingAtPlayer()
    {
        return Vector3.Dot(enemyToPlayerDirFlat, transform.forward) >= Mathf.Cos(Mathf.Deg2Rad * maxAttackLookAngle);
    }

    protected void LookAtPlayer()
    {
        float time = Vector3.Angle(enemyToPlayerDirFlat, transform.forward) / 90f;
        iTween.LookTo(gameObject, new Vector3(playerPos.position.x, transform.position.y, playerPos.position.z), time);
    }

    void MoveToPlayer()
    {
        agent.isStopped = false;
        agent.destination = playerPos.position;
    }
    void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }
}
