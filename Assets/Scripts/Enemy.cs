using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    protected NavMeshAgent agent;

    protected float radius;
    protected bool attacking = false;
    protected Vector3 enemyToPlayerDir;
    protected float enemyToPlayerDist;

    public Transform player;
    public LayerMask wallMask;

    [Header("Basic Variables")]
    public float speed;
    public int damage;
    public int hp;

    [Header("Attack Variables")]
    public float attackDelay;
    public float attackCooldown;
    public float maxAttackDistance;
    public float maxAttackLookAngle;

    protected abstract IEnumerator Attack();

    protected void CalculateDirectionAndDistance()
    {
        Vector3 enemyFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerFlat = new Vector3(player.position.x, 0, player.position.z);
        enemyToPlayerDir = (playerFlat - enemyFlat).normalized;
        enemyToPlayerDist = Vector3.Distance(playerFlat, enemyFlat) - radius - player.GetComponent<CharacterController>().radius;
    }

    protected void Move()
    {
        if (IsWithinAttackDistance()) {
            if (CanSeeThePlayer()) {
                Stop();

                LookAtPlayer();

                if (IsLookingAtPlayer()) {
                    // Attack
                    StartCoroutine(Attack());
                }

                return;
            }
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
        return enemyToPlayerDist <= maxAttackDistance;
    }
    bool CanSeeThePlayer()
    {
        return !Physics.SphereCast(new Ray(transform.position, enemyToPlayerDir), 0.75f, enemyToPlayerDist, wallMask);
    }
    bool IsLookingAtPlayer()
    {
        return Vector3.Dot(enemyToPlayerDir, transform.forward) >= Mathf.Cos(Mathf.Deg2Rad * maxAttackLookAngle);
    }

    void LookAtPlayer()
    {
        float time = Vector3.Angle(enemyToPlayerDir, transform.forward) / 90f;
        iTween.LookTo(gameObject, new Vector3(player.position.x, transform.position.y, player.position.z), time);
    }

    void MoveToPlayer()
    {
        agent.isStopped = false;
        agent.destination = player.position;
    }

    void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }
}
